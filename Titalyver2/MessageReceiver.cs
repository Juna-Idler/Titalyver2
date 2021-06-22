using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;

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
            Pause = 3,
            PauseCancel = 4,
            SeekPlaying = 5,
            SeekPause = 6,
        };

        public EnumPlaybackEvent PlaybackEvent { get; private set; }
        public float SeekTime { get; private set; }
        public float TimeOfDay { get; private set; }

        public List<KeyValuePair<string, string>> Data { get; private set; } = new();

        private RegisteredWaitHandle WaitHandle;
        public void SetUpdateCallback(WaitOrTimerCallback callback)
        {
            if (callback == null)
            {
                _ = WaitHandle?.Unregister(EventWaitHandle);
                WaitHandle = null;
                return;
            }
            WaitHandle = ThreadPool.RegisterWaitForSingleObject(EventWaitHandle, callback, null, -1, false);
        }
        public bool GetData(bool all = false)
        {
            byte[] buffer;
            try
            {
                using MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(MMF_Name, MemoryMappedFileRights.Read);
                using MutexLock ml = new(Mutex, 100);
                if (!ml.Result)
                    return false;
                using MemoryMappedViewStream stream = mmf.CreateViewStream(0, 0, MemoryMappedFileAccess.Read);
                byte[] bytes = new byte[16];
                int read = stream.Read(bytes, 0, 16);
                PlaybackEvent = (EnumPlaybackEvent)BitConverter.ToUInt32(bytes, 0);
                SeekTime = BitConverter.ToSingle(bytes, 4);
                TimeOfDay = BitConverter.ToSingle(bytes, 8);
                Int32 json_size = BitConverter.ToInt32(bytes, 12);

                if (all == false && PlaybackEvent != EnumPlaybackEvent.PlayNew)
                {
                    return true;
                }
                buffer = new byte[json_size];
                stream.Read(buffer, 0, (int)json_size);
            }
            catch (Exception)
            { return false; }

            try
            {
                using JsonDocument document = JsonDocument.Parse(buffer);
                Data.Clear();
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                    return false;
                foreach (var e in document.RootElement.EnumerateObject())
                {
                    Data.Add(new KeyValuePair<string, string>(e.Name, e.Value.GetRawText()));
                }
            }
            catch (Exception)
            { return false; }

            return true;
        }

        public MessageReceiver()
        {
            EventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, WriteEvent_Name);
            Mutex = new Mutex(false, Mutex_Name);
        }
        ~MessageReceiver()
        {
            if (Mutex != null)
            {
                Mutex.Dispose();
                Mutex = null;
            }
            if (EventWaitHandle != null)
            {
                EventWaitHandle.Dispose();
                EventWaitHandle = null;
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


    }
}
