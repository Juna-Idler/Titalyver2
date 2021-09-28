using System;
using System.Collections.Generic;
using System.Text.Json;


namespace Titalyver2
{
    public enum EnumPlaybackEvent
    {
        Bit_Play = 1,
        Bit_Stop = 2,
        Bit_Seek = 4,

        NULL = 0,
        Play = 1,
        Stop = 2,

        Seek = 4,
        SeekPlay = 5,
        SeekStop = 6,
    };

    public class ReceiverData
    {
        public EnumPlaybackEvent PlaybackEvent { get; private set; } //イベント内容
        public double SeekTime { get; private set; }  //イベントが発生した時の再生位置
        public Int32 TimeOfDay { get; private set; } //イベントが発生した24時間周期のミリ秒単位の時刻
        public Int32 UpdateTimeOfDay { get; private set; } //メタデータ更新時刻


        //おそらく音楽ファイルの多分フルパス
        public string FilePath { get; private set; }
        public string Title { get; private set; }
        public string[] Artists { get; private set; }
        public string Album { get; private set; }
        public double Duration { get; private set; }

        public Dictionary<string, string[]> MetaData { get; private set; }    //メタデータ 中身はUTF-8のJSON
        public bool MetaDataUpdated { get; private set; }//Metadataに変更があるか？

        public ReceiverData(EnumPlaybackEvent playbackEvent, double seekTime, Int32 timeOfDay, Int32 updateTimeOfDay,
            string filePath, string title, string[] artists, string album, double duration, Dictionary<string, string[]> metaData, bool metaDataUpdated)
        {
            PlaybackEvent = playbackEvent;
            SeekTime = seekTime;
            TimeOfDay = timeOfDay;
            UpdateTimeOfDay = updateTimeOfDay;
            FilePath = filePath;
            Title = title;
            Artists = artists;
            Album = album;
            Duration = duration;
            MetaData = metaData;
            MetaDataUpdated = metaDataUpdated;
        }
    }

    public delegate void PlaybackEventHandler(ReceiverData data);

    //音楽プレイヤーの再生状態等の変化を受信するインターフェイス
    public interface ITitalyverReceiver
    {
        event PlaybackEventHandler OnPlaybackEventChanged;

        ReceiverData GetData();

        void Terminalize();
    }
}
