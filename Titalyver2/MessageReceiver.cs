using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Diagnostics;

using System.Text.Json;


namespace Titalyver2
{
    public class MessageReceiver
    {
        public enum EnumPlaybackEvent
        {
            NULL = 0,
            PlayNew = 1,
            Stop = 2,
            PauseCancel = 3,
            Pause = 4,
            SeekPlaying = 5,
            SeekPause = 6,
        };

        public struct Data
        {
            public EnumPlaybackEvent PlaybackEvent; //イベント内容
            public double SeekTime;  //イベントが発生した時の再生位置
            public Int32 TimeOfDay; //イベントが発生した24時間周期のミリ秒単位の時刻

            //メタデータ keyは小文字 複数の同一keyの可能性あり（なのでList<Pair>） Dic<string,string[]>とどっちがいいのか？
            //文字列はstring それ以外はRawTextなstring
            public List<KeyValuePair<string, string>> MetaData;

            //おそらく音楽ファイルの多分フルパス
            public string FilePath;
        }

        public delegate void PlaybackEventHandler(Data data);

        public event PlaybackEventHandler OnPlaybackEventChanged;


        public Data GetData() { return data; }

        private Data data;
       private bool ReadData()
        {
            byte[] buffer;
            try
            {
                using MutexLock ml = new(Mutex, 100);
                if (!ml.Result)
                    return false;
                using MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(MMF_Name, MemoryMappedFileRights.Read);
                using MemoryMappedViewStream stream = mmf.CreateViewStream(0, 0, MemoryMappedFileAccess.Read);
                byte[] bytes = new byte[20];
                int read = stream.Read(bytes, 0, 20);
                data.PlaybackEvent = (EnumPlaybackEvent)BitConverter.ToUInt32(bytes, 0);
                data.SeekTime = BitConverter.ToDouble(bytes, 4);
                data.TimeOfDay = BitConverter.ToInt32(bytes, 12);
                Int32 json_size = BitConverter.ToInt32(bytes, 16);

                if (data.MetaData != null && data.PlaybackEvent != EnumPlaybackEvent.PlayNew)
                {
                    return true;
                }
                buffer = new byte[json_size];
                _ = stream.Read(buffer, 0, (int)json_size);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            if (data.MetaData == null)
                data.MetaData = new List<KeyValuePair<string, string>>();
            else
                data.MetaData.Clear();
            try
            {
                using JsonDocument document = JsonDocument.Parse(buffer);
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                    return false;
                data.FilePath = document.RootElement.GetProperty("path").GetString();
                JsonElement meta = document.RootElement.GetProperty("meta");
                foreach (JsonProperty e in meta.EnumerateObject())
                {
                    switch (e.Value.ValueKind)
                    {
                        case JsonValueKind.String:
                            data.MetaData.Add(new KeyValuePair<string, string>(e.Name.ToLower(null), e.Value.GetString()));
                            break;
                        case JsonValueKind.Array:
                            foreach (JsonElement a in e.Value.EnumerateArray())
                            {
                                if (a.ValueKind == JsonValueKind.String)
                                    data.MetaData.Add(new KeyValuePair<string, string>(e.Name.ToLower(null), a.GetString()));
                                else
                                    data.MetaData.Add(new KeyValuePair<string, string>(e.Name.ToLower(null), a.GetRawText()));
                            }
                            break;
                        default:
                            data.MetaData.Add(new KeyValuePair<string, string>(e.Name.ToLower(null), e.Value.GetRawText()));
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }


        //read イベントの発生を待たずにとりあえず一回MMFを読みに行く
        public MessageReceiver(bool read = false)
        {
            Mutex = new Mutex(false, Mutex_Name);
            EventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, WriteEvent_Name);
            RegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(EventWaitHandle, WaitOrTimerCallback, null, -1, false);
            if (read)
                _ = ReadData();
        }
        ~MessageReceiver()
        {
            _ = RegisteredWaitHandle?.Unregister(EventWaitHandle);
            RegisteredWaitHandle = null;

            EventWaitHandle?.Dispose();
            EventWaitHandle = null;

            Mutex?.Dispose();
            Mutex = null;
        }
        private void WaitOrTimerCallback(object state, bool timedOut)
        {
            if (!timedOut)
            {
                if (ReadData())
                    OnPlaybackEventChanged?.Invoke(data);
            }
        }



        private class MutexLock : IDisposable
        {
            private readonly Mutex Mutex;
            public bool Result { get; private set; }
            public MutexLock(Mutex mutex,int timeout_millisec)
            {
                Mutex = mutex;
                Result = mutex.WaitOne(timeout_millisec);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                }
                if (Result)
                {
                    Mutex.ReleaseMutex();
                    Result = false;
                }
            }

            // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
            ~MutexLock()
            {
                // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
                Dispose(disposing: false);
            }

            public void Dispose()
            {
                // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }


        private const string MMF_Name = "Titalyver Message Data MMF";
        private const string WriteEvent_Name = "Titalyver Message Write Event";
        private const string Mutex_Name = "Titalyver Message Mutex";

        private Mutex Mutex;
        private EventWaitHandle EventWaitHandle;
        private RegisteredWaitHandle RegisteredWaitHandle;


    }
}
