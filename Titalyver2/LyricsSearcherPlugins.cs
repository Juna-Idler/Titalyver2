using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

using System.Reflection;

namespace Titalyver2
{

    //歌詞読み込みプラグイン読み込み
    //interface DLLを別に作るのが面倒なのでdynamicで
    public class LyricsSearcherPlugins
    {
        private static readonly string Directory = Path.Combine(AppContext.BaseDirectory, "plugin");

        private readonly Dictionary<string, dynamic> Plugins = new();

        public async Task<string[]> GetLyrics(string dll,
            string title, string[] artists, string album, string path, string param,
            int millisecondsTimeout)
        {
            return await Task.Run(() =>
            {
                var task = Task.Run(() =>
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
                });
                if (task.Wait(millisecondsTimeout))
                {
                    return task.Result;
                }
                return null;
            });
        }
    }
}
