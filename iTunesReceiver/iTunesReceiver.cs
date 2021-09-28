using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Titalyver2;
namespace iTunesReceiver
{
    public class iTunesReceiver : ITitalyverReceiver
    {
        public event PlaybackEventHandler OnPlaybackEventChanged;

        void ITitalyverReceiver.Terminalize()
        {
            iTunes.Dispose();
        }

        ReceiverData ITitalyverReceiver.GetData() { return data; }

        private readonly iTunes iTunes;
        private ReceiverData data;

        private int LastUpdateTrackDatabaseID = -1;



        public iTunesReceiver()
        {
            iTunes = new iTunes();
            iTunes.SetOnPlayerPlayEvent(OnPlayerPlayEvent);
            iTunes.SetOnPlayerStopEvent(OnPlayerStopEvent);
            iTunes.SetOnAboutToPromptUserToQuitEvent(OnAboutToPromptUserToQuitEvent);
            iTunes.SetOnQuittingEvent(OnQuittingEvent);
        }

        private static int GetTimeOfDay()
        {
            DateTime now = DateTime.Now;
            return ((now.Hour * 60 + now.Minute) * 60 + now.Second) * 1000 + now.Millisecond;
        }

        private void OnPlayerEvent(object iTrack, EnumPlaybackEvent playbackEvent)
        {
            if (iTunes.App == null)
                return;
            if (iTrack is not iTunesLib.IITTrack track)
                return;

            EnumPlaybackEvent pbevent = playbackEvent;
            double seekTime = iTunes.App.PlayerPositionMS / 1000.0;
            int timeOfDay = GetTimeOfDay();

            if (track.TrackDatabaseID == LastUpdateTrackDatabaseID)
            {
                data = new ReceiverData(pbevent, seekTime, timeOfDay, data.UpdateTimeOfDay,
                    data.FilePath, data.Title, data.Artists, data.Album, data.Duration,
                    data.MetaData, false);
                OnPlaybackEventChanged?.Invoke(data);
                return;
            }
            Dictionary<string, string[]> meta = new();

            LastUpdateTrackDatabaseID = track.TrackDatabaseID;

            string filePath = "";

            if (track is iTunesLib.IITFileOrCDTrack file)
            {
                filePath = file.Location;
                if (!string.IsNullOrEmpty(file.Lyrics))
                {
                   meta.Add("lyrics", new string[] { file.Lyrics });
                }
            }
            meta.Add("album", new string[] { track.Album });
            meta.Add("artist", new string[] { track.Artist });
            meta.Add("bitrate", new string[] { track.BitRate.ToString() });
            meta.Add("bpm", new string[] { track.BPM.ToString() });
            meta.Add("comment", new string[] { track.Comment });
            meta.Add("composer", new string[] { track.Composer });
            meta.Add("disccount", new string[] { track.DiscCount.ToString() });
            meta.Add("discnumber", new string[] { track.DiscNumber.ToString() });
            meta.Add("duration", new string[] { track.Duration.ToString() });
            meta.Add("genre", new string[] { track.Genre });
            meta.Add("grouping", new string[] { track.Grouping });
            meta.Add("kindasstring", new string[] { track.KindAsString });
            meta.Add("name", new string[] { track.Name });
            meta.Add("playedcount", new string[] { track.PlayedCount.ToString() });
            meta.Add("rating", new string[] { track.Rating.ToString() });
            meta.Add("samplerate", new string[] { track.SampleRate.ToString() });
            meta.Add("time", new string[] { track.Time });
            meta.Add("trackcount", new string[] { track.TrackCount.ToString() });
            meta.Add("tracknumber", new string[] { track.TrackNumber.ToString() });
            meta.Add("year", new string[] { track.Year.ToString() });


            data = new(pbevent, seekTime, timeOfDay, timeOfDay,
                filePath, track.Name, new string[] { track.Artist }, track.Album, track.Duration,
                meta, true);

            OnPlaybackEventChanged?.Invoke(data);
        }

        private void OnPlayerPlayEvent(object iTrack)
        {
            OnPlayerEvent(iTrack, EnumPlaybackEvent.SeekPlay);
        }
        private void OnPlayerStopEvent(object iTrack)
        {
            OnPlayerEvent(iTrack, EnumPlaybackEvent.SeekStop);
        }


//COM参照がある状態でiTunesが終了しようとすると、確認ダイアログが出る前に飛んで来る
//５秒以内に参照を解放するとダイアログの表示を防げるらしい
        private void OnAboutToPromptUserToQuitEvent()
        {
            iTunes.Dispose();

//めちゃくちゃ強引だけどいいのか？
            Environment.Exit(0);

        }

        //OnAboutToPromptUserToQuitEventで解放処理しなかった場合に来る最終報告
        private void OnQuittingEvent()
        {
            iTunes.Dispose();
        }


    }
}
