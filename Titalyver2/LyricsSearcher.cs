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
            SearchList = new List<string> { "file:%directoryname%/%filename%.kra",
                                            "file:%directoryname%/%filename%.lrc",
                                            "file:%directoryname%/%filename%.txt",
                                            "string:<lyrics>"};
        }

        private string command;
        private string path;
        private string text;

        public string Search(string filepath, Dictionary<string, string[]> metaData)
        {
            string directoryname = Path.GetDirectoryName(filepath) ?? "";
            string filename = Path.GetFileNameWithoutExtension(filepath);
            string filename_ext = Path.GetFileName(filepath);

            string default_separator = ",";

            for (int i = 0; i < SearchList.Count;i++)
            {
                string s = SearchList[i];

                s = Regex.Replace(s, "%directoryname%", directoryname);
                s = Regex.Replace(s, "%filename%", filename);
                s = Regex.Replace(s, "%filename_ext%", filename_ext);
                s = Regex.Replace(s, "%path%", filepath);

                string r = "";


                Regex regex = new(@"<(.+?)(\|([^>]*))?>");
                Match m;
                for (m = regex.Match(s); m.Success; m = regex.Match(s))
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
                r += s;

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
                    return r[7..];
                }
            }

            return "";
        }


        private readonly List<string> SearchList;
    }
}
