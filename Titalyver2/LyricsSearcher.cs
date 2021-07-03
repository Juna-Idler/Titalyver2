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
    public class LyricsSearcher
    {

        public LyricsSearcher()
        {
        }

        //        private string command;
        //        private string path;
        //        private string text;

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

            for (int i = 0; i < SearchList.Count;i++)
            {
                string r = Replace(SearchList[i], directoryname,filename,filename_ext,filepath,metaData);

                if (r.IndexOf("file:", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    string path = r[5..];
                    if (!File.Exists(path))
                        continue;
                    try
                    {
                        return File.ReadAllText(path);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
                else if (r.IndexOf("string:", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    r = r[7..];
                    if (r != "")
                        return r;
                }
            }

            return Replace(NoLyricsFormatText, directoryname, filename, filename_ext, filepath, metaData);
        }


    }
}
