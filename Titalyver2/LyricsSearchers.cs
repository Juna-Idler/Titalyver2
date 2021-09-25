using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Titalyver2
{
    public class LyricsSearchers
    {

        public LyricsSearchers()
        {
        }

        public struct ReturnValue
        {
            public string Command { get; private set; }
            public string Parameter { get; private set; }
            public string ReplacedParameter { get; private set; }
            public string FilePath { get; private set; }
            public string Text { get; private set; }

            public ReturnValue(string command, string parameter, string replacedParameter, string filePath, string text)
            {
                Command = command;
                Parameter = parameter;
                ReplacedParameter = replacedParameter;
                FilePath = filePath;
                Text = text;
            }
        }

        public List<string> SearchList { get; private set; } = new();

        public void SetSearchList(string list)
        {
            SearchList.Clear();
            using StringReader sr = new(list);
            for (string line = sr.ReadLine(); line != null; line = sr.ReadLine())
            {
                SearchList.Add(line);
            }
        }

        private readonly LyricsSearcherPlugins Plugins = new();

        public int MillisecondsTimeout { get; set; } = 10000;

        public LyricsSearcherPlugins.TimeoutReport[] GetTimeoutList() { return Plugins.TimeoutList.ToArray(); }


        private readonly HashSet<string> EmptyFlag = new();


        public string NoLyricsFormatText { get; set; }

        private static string default_separator = ",";


        private static readonly Regex MetaTagNameRegex = new(@"<(.+?)(\|([^>]*))?>");
        public static string Replace(string s, string filepath,
                                     string title, string[] artists, string artists_separator, string album,
                                     Dictionary<string, string[]> metaData)
        {
            string directoryname = Path.GetDirectoryName(filepath) ?? "";
            string filename = Path.GetFileNameWithoutExtension(filepath);
            string filename_ext = Path.GetFileName(filepath);

            StringBuilder sb = new(s);
            _ = sb.Replace("%directoryname%", directoryname);
            _ = sb.Replace("%filename%", filename).Replace("%filename_ext%", filename_ext).Replace("%path%", filepath);
            _ = sb.Replace("%mydocuments%", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            _ = sb.Replace("%title%", title).Replace("%artist%", string.Join(artists_separator, artists)).Replace("%album%", album);

            s = sb.ToString();
            string r = "";

            Match m;
            for (m = MetaTagNameRegex.Match(s); m.Success; m = MetaTagNameRegex.Match(s))
            {
                r += s[..m.Index];
                s = s[(m.Index + m.Value.Length)..];

                if (metaData != null)
                {
                    foreach (KeyValuePair<string, string[]> d in metaData)
                    {
                        if (d.Key == m.Groups[1].Value)
                        {
                            if (d.Value.Length == 1)
                                r += ReplaceInvalidFileNameChars(d.Value[0]);
                            else
                            {
                                string separator = m.Groups[3].Success ? m.Groups[3].Value : default_separator;
                                r += ReplaceInvalidFileNameChars(string.Join(separator, d.Value));
                            }
                        }
                    }
                }
            }
            return r + s;
        }
        private static string ReplaceInvalidFileNameChars(string filename)
        {
            filename = filename.Replace("\\", "");
            filename = filename.Replace('/', '／');

            filename = filename.Replace(':', '：');
            filename = filename.Replace('*', '＊');
            filename = filename.Replace('?', '？');
            filename = filename.Replace('"', '”');
            filename = filename.Replace('<', '＜');
            filename = filename.Replace('>', '＞');
            filename = filename.Replace('|', '｜');
            filename = filename.Replace('.', '．');
            return filename;
        }

        public async Task<ReturnValue[]> Search(ITitalyverReceiver.Data data)
        {
            string filepath = "";
            if (!string.IsNullOrEmpty(data.FilePath))
            {
                Uri u = new(data.FilePath);
                filepath = u.LocalPath + Uri.UnescapeDataString(u.Fragment);
            }


            List<ReturnValue> Return = new();

            List<Task<string[]>> pluginTasks = new();

            foreach (string searchText in SearchList)
            {
                int index = searchText.IndexOf(":");
                if (index <= 0)
                    continue;
                string command = searchText[..index].ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim();
                string parameter = searchText[(index+1)..].Trim();
                string replacedParameter = Replace(parameter, filepath,
                                                   data.Title, data.Artists, ",", data.Album, data.MetaData);

                //制御コマンド
                if (command == "shortcut" || command == "set_empty")
                {
                    if (pluginTasks.Count > 0)
                    {
                        _ = await Task.WhenAll(pluginTasks.ToArray());
                        foreach (var t in pluginTasks)
                        {
                            int i = Return.FindIndex(r => r.Command == "plugin Reserved");
                            ReturnValue r = Return[i];
                            Return.RemoveAt(i);
                            string[] lyrics = t.Result;
                            if (lyrics != null && lyrics.Length > 0)
                            {
                                List<ReturnValue> ins = new(lyrics.Length);
                                foreach (string l in lyrics)
                                {
                                    if (!string.IsNullOrEmpty(l))
                                    {
                                        ins.Add(new ReturnValue("plugin", r.Parameter, r.ReplacedParameter, "", l));
                                    }
                                }
                                Return.InsertRange(i, ins);
                            }
                        }
                        pluginTasks.Clear();
                    }
                    if (command == "shortcut")  //ここまでに一つ以上の有効な歌詞があるならこの時点で切り上げる
                    {
                        if (Return.Count> 0)
                        {
                            return Return.ToArray();
                        }
                        //パラメータの文字列のEmptyフラグが設定されていても切り上げる
                        if (!string.IsNullOrEmpty(replacedParameter) && EmptyFlag.Contains(replacedParameter))
                        {
                            string text = Replace(NoLyricsFormatText, filepath,
                                                  data.Title, data.Artists, ",", data.Album, data.MetaData);
                            return new[] { new ReturnValue(command, parameter, replacedParameter, "", text) };
                        }
                    }
                    else if (command == "set_empty")
                    {
                        //この時点で探索歌詞が空ならパラメータの文字列にEmptyフラグを設定する
                        if (Return.Count == 0)
                        {
                            if (!string.IsNullOrEmpty(replacedParameter))
                                _ = EmptyFlag.Add(replacedParameter);
                        }
                    }
                }

                switch (command)
                {
                    case "file":
                        {
                            if (!File.Exists(replacedParameter))
                                continue;
                            try
                            {
                                ReturnValue value = new(command, parameter, replacedParameter, replacedParameter, File.ReadAllText(replacedParameter));
                                Return.Add(value);

                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }
                        }
                        break;
                    case "string":

                        if (replacedParameter != "")
                        {
                            ReturnValue value = new(command, parameter, replacedParameter, "", replacedParameter);
                            Return.Add(value);
                        }
                        break;
                    case "plugin":
                        {
                            string[] dll = replacedParameter.Split(" ", 2);

                            pluginTasks.Add(Plugins.GetLyrics(dll[0],data.Title, data.Artists, data.Album, filepath, dll.Length >= 2 ? dll[1] : "", MillisecondsTimeout));
                            Return.Add(new ReturnValue("plugin Reserved", parameter, replacedParameter, "", null));
                        }
                        break;
                }
            }
            if (pluginTasks.Count > 0)
            {
                _ = await Task.WhenAll(pluginTasks.ToArray());
                foreach (var t in pluginTasks)
                {
                    int i = Return.FindIndex(r => r.Command == "plugin Reserved");
                    ReturnValue r = Return[i];
                    Return.RemoveAt(i);
                    string[] lyrics = t.Result;
                    if (lyrics != null && lyrics.Length > 0)
                    {
                        List<ReturnValue> ins = new(lyrics.Length);
                        foreach (string l in lyrics)
                        {
                            if (!string.IsNullOrEmpty(l))
                            {
                                ins.Add(new ReturnValue("plugin", r.Parameter, r.ReplacedParameter, "", l));
                            }
                        }
                        Return.InsertRange(i, ins);
                    }
                }
                pluginTasks.Clear();
            }

            if (Return.Count > 0)
            {
                return Return.ToArray();
            }
            {
                string text = Replace(NoLyricsFormatText, filepath,
                          data.Title, data.Artists, ",", data.Album, data.MetaData);
                return new[] { new ReturnValue("No Lyrics", "", "", "", text) };
            }
        }


    }
}
