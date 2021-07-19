using System;
using System.Collections.Generic;


namespace Titalyver2
{

    //音楽プレイヤーの再生状態等の変化を受信するインターフェイス
    public interface ITitalyverReceiver
    {
        enum EnumPlaybackEvent
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

        struct Data
        {
            public EnumPlaybackEvent PlaybackEvent; //イベント内容
            public double SeekTime;  //イベントが発生した時の再生位置
            public Int32 TimeOfDay; //イベントが発生した24時間周期のミリ秒単位の時刻
            public Int32 UpdateTimeOfDay; //メタデータ更新時刻

            //メタデータ keyは小文字 複数の同一keyの可能性あり
            //文字列はstring それ以外はRawTextなstring
            public Dictionary<string, string[]> MetaData;

            //おそらく音楽ファイルの多分フルパス
            public string FilePath;

            public bool MetaDataUpdated;//Metadataに変更があるか？
            public bool IsValid() => MetaData != null;
        }


        delegate void PlaybackEventHandler(Data data);
        event PlaybackEventHandler OnPlaybackEventChanged;

        Data GetData();

        void Terminalize();
    }
}
