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
                                            "file:%directoryname%/%filename%.txt" };
        }

        public string Search(string filepath, Dictionary<string, string[]> metaData)
        {
            string directoryname = Path.GetDirectoryName(filepath);
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

                foreach (KeyValuePair<string, string[]> d in metaData)
                {
                    string pattern = "<" + Regex.Escape(d.Key) + "(|([^>]*))?>";
                    s = Regex.Replace(s, pattern, (m) => {
                        return d.Value.Length == 1 ? d.Value[0]
                        : string.Join(m.Groups[2].Success ? m.Groups[2].Value : default_separator, d.Value);
                    });
                }
                if (s.IndexOf("file:") == 0)
                {
                    string path = s[5..];
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
            }

            return "";
        }


        private List<string> SearchList;
    }
}
