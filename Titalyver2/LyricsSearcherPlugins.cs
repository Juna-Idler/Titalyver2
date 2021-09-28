using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;

using System.Reflection;

namespace Titalyver2
{

    //歌詞読み込みプラグイン読み込み
    //interface DLLを別に作るのが面倒なのでdynamicで
    public class LyricsSearcherPlugins
    {
        private static readonly string Directory = Path.Combine(AppContext.BaseDirectory, "plugin");

        private readonly Dictionary<string, dynamic> Plugins = new();
        public class TimedResult
        {
            public string [] Result { get; set; }
            public TimeSpan Time { get; set; }
        }

        public struct TimeoutReport
        {
            public string dll { get; set; }
            public string title { get; set; }
            public string[] artists { get; set; }
            public string artist => string.Join('.', artists);
            public string album { get; set; }
            public string path { get; set; }
            public string param { get; set; }
            public int millisecondsTimeout { get; set; }
            public Task<TimedResult> task { get; set; }
            public string task_status => task.Status.ToString();
            public TimeSpan Elapsed => task.Status == TaskStatus.RanToCompletion ? task.Result.Time : TimeSpan.Zero;
        }
        public List<TimeoutReport> TimeoutList = new();

        public async Task<string[]> GetLyrics(string dll,
            string title, string[] artists, string album, string path, string param,
            int millisecondsTimeout)
        {
            return await Task.Run(() =>
            {
                Task<TimedResult> task = Task.Run(() =>
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
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
                        string[] r = searcher.Search(title, artists, album, path, param);
                        return new TimedResult() { Result = r, Time = stopwatch.Elapsed };
                    }
                    catch (Exception e)
                    {
                        return new TimedResult() { Result = null, Time = stopwatch.Elapsed };
                    }
                });
                if (task.Wait(millisecondsTimeout))
                {
                    return task.Result.Result;
                }
                TimeoutList.Add(new TimeoutReport()
                {
                    dll = dll,
                    title = title,
                    artists = artists,
                    album = album,
                    path = path,
                    param = param,
                    millisecondsTimeout = millisecondsTimeout,
                    task = task
                });
                return null;
            });
        }
    }
}
