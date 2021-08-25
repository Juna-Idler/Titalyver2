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

        public string Command { get; private set; } = "";
        public string Parameter { get; private set; } = "";
        public string ReplacedParameter { get; private set; } = "";

        public string FilePath { get; private set; } = "";
        public string Text { get; private set; } = "";

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

        public string Search(string filepath, Dictionary<string, string[]> metaData)
        {
            string directoryname = Path.GetDirectoryName(filepath) ?? "";
            string filename = Path.GetFileNameWithoutExtension(filepath);
            string filename_ext = Path.GetFileName(filepath);

            string title = null;
            string[] artists = null;
            string album = null;
            for (int i = 0; i < SearchList.Count;i++)
            {
                int index = SearchList[i].IndexOf(":");
                if (index <= 0)
                    continue;
                Command = SearchList[i][..index].ToLower(System.Globalization.CultureInfo.InvariantCulture);
                Parameter = SearchList[i][(index+1)..];
                ReplacedParameter = Replace(Parameter, directoryname, filename, filename_ext, filepath, metaData);

                switch (Command)
                {
                    case "file":
                        {
                            if (!File.Exists(ReplacedParameter))
                                continue;
                            try
                            {
                                FilePath = ReplacedParameter;
                                Text = File.ReadAllText(ReplacedParameter);
                                return Text;
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }
                        }
                        break;
                    case "string":
                        Text = ReplacedParameter;
                        if (Text != "")
                        {
                            FilePath = "";
                            return Text;
                        }
                        break;
                    case "plugin":
                        {
                            string dll = ReplacedParameter;
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
                            string[] lyrics = Plugins.GetLyrics(dll, title, artists, album, filepath);
                            if (lyrics != null && lyrics.Length > 0 && !string.IsNullOrEmpty(lyrics[0]))
                            {
                                return lyrics[0];
                            }
                        }

                        break;
                }
            }

            Command = "";
            Parameter = ReplacedParameter = "";
            FilePath = "";
            Text = Replace(NoLyricsFormatText, directoryname, filename, filename_ext, filepath, metaData);
            return Text;
        }


    }
}
