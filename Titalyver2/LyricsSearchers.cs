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

        public bool InstantReply;

        public string NoLyricsFormatText { get; set; }

        private string default_separator = ",";


        private readonly Regex MetaTagNameRegex = new(@"<(.+?)(\|([^>]*))?>");
        private string Replace(string s, string directoryname, string filename, string filename_ext, string filepath, Dictionary<string, string[]> metaData)
        {
            s = Regex.Replace(s, "%directoryname%", directoryname);
            s = Regex.Replace(s, "%filename%", filename);
            s = Regex.Replace(s, "%filename_ext%", filename_ext);
            s = Regex.Replace(s, "%path%", filepath);

            string r = "";

            Match m;
            for (m = MetaTagNameRegex.Match(s); m.Success; m = MetaTagNameRegex.Match(s))
            {
                r += s[..m.Index];
                s = s[(m.Index + m.Value.Length)..];

                foreach (KeyValuePair<string, string[]> d in metaData)
                {
                    if (d.Key == m.Groups[1].Value)
                    {
                        if (d.Value.Length == 1)
                            r += d.Value[0];
                        else
                        {
                            string separator = m.Groups[3].Success ? m.Groups[3].Value : default_separator;
                            r += string.Join(separator, d.Value);

                        }
                    }
                }
            }
            return r + s;
        }

        public ReturnValue[] Search(string filepath, Dictionary<string, string[]> metaData)
        {
            string directoryname = Path.GetDirectoryName(filepath) ?? "";
            string filename = Path.GetFileNameWithoutExtension(filepath);
            string filename_ext = Path.GetFileName(filepath);

            string title = null;
            string[] artists = null;
            string album = null;
            List<ReturnValue> Return = new();
            foreach (string searchText in SearchList)
            {
                int index = searchText.IndexOf(":");
                if (index <= 0)
                    continue;
                string command = searchText[..index].ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim();
                string parameter = searchText[(index+1)..].Trim();
                string replacedParameter = Replace(parameter, directoryname, filename, filename_ext, filepath, metaData);

                //制御コマンド
                if (command == "shortcut")  //ここまでに一つ以上の有効な歌詞があるならこの時点で切り上げる
                {
                    if (Return.Count> 0)
                    {
                        return Return.ToArray();
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
                                ReturnValue value = new ReturnValue(command, parameter, replacedParameter, replacedParameter, File.ReadAllText(replacedParameter));
                                if (InstantReply)
                                    return new[] { value };
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
                            ReturnValue value = new ReturnValue(command, parameter, replacedParameter, "", replacedParameter);
                            if (InstantReply)
                                return new[] { value };
                            Return.Add(value);
                        }
                        break;
                    case "plugin":
                        {
                            if (title == null)
                            {
                                string[] t;
                                if (metaData != null &&
                                    (metaData.TryGetValue("title", out t) ||         //foobar2000
                                     metaData.TryGetValue("tracktitle", out t) ||    //MusicBee
                                     metaData.TryGetValue("name", out t)))            //iTunes
                                {
                                    title = t[0];
                                }
                            }
                            if (artists == null)
                            {
                                if (metaData != null && !metaData.TryGetValue("artist", out artists))
                                {
                                    artists = null;
                                }
                            }
                            if (album == null)
                            {
                                if (metaData != null && metaData.TryGetValue("album", out string[] a))
                                {
                                    album = a[0];
                                }
                            }
                            string[] dll = replacedParameter.Split(" ", 2);
                            string[] lyrics = Plugins.GetLyrics(dll[0], title, artists, album, filepath, dll.Length >= 2 ? dll[1] : "");
                            if (lyrics != null && lyrics.Length > 0)
                            {
                                foreach (string l in lyrics)
                                {
                                    if (!string.IsNullOrEmpty(l))
                                    {
                                        Return.Add(new ReturnValue(command, parameter, replacedParameter, "", l));
                                    }
                                }
                                if (InstantReply)
                                   return Return.ToArray();
                            }
                        }
                        break;
                }
            }
            if (Return.Count > 0)
            {
                return Return.ToArray();
            }

            return new[] { new ReturnValue("No Lyrics", "", "", "", Replace(NoLyricsFormatText, directoryname, filename, filename_ext, filepath, metaData)) };
        }


    }
}
