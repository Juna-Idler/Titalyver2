using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Reflection;

namespace Titalyver2
{

    //歌詞読み込みプラグイン読み込み
    //interface DLLを別に作るのが面倒なのでdynamicで
    public class LyricsSearcherPlugins
    {
        private static readonly string Directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "plugin");

        private readonly Dictionary<string, dynamic> Plugins = new();

        public string[] GetLyrics(string dll, string title, string[] artists, string album, string path, string param)
        {
            try
            {
                if (!Plugins.TryGetValue(dll, out dynamic searcher))
                {
                    string dllpath = Path.Combine(Directory, dll);
                    Assembly assembly = Assembly.LoadFrom(dllpath);
                    Type type = assembly.GetType("Titalyver2.LyricsSearcher", true);
                    searcher = Activator.CreateInstance(type);
                    if (searcher == null)
                        return null;
                    Plugins.Add(dll, searcher);
                }
                return searcher.Search(title, artists, album, path, param);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
