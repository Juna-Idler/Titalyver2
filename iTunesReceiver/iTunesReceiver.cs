using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Titalyver2;
namespace iTunesReceiver
{
    public class iTunesReceiver : Titalyver2.ITitalyverReceiver
    {
        public event ITitalyverReceiver.PlaybackEventHandler OnPlaybackEventChanged;

        void ITitalyverReceiver.Terminalize()
        {
            iTunes.Dispose();
        }

        ITitalyverReceiver.Data ITitalyverReceiver.GetData() { return data; }

        private readonly iTunes iTunes;
        private ITitalyverReceiver.Data data;

        private int LastUpdateTrackDatabaseID = 0;



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

        private void OnPlayerEvent(object iTrack, ITitalyverReceiver.EnumPlaybackEvent playbackEvent)
        {
            if (iTunes.App == null)
                return;
            if (iTrack is not iTunesLib.IITTrack track)
                return;

            data.PlaybackEvent = playbackEvent;
            data.SeekTime = iTunes.App.PlayerPositionMS / 1000.0;
            data.TimeOfDay = GetTimeOfDay();

            if (data.MetaData == null)
                data.MetaData = new Dictionary<string, string[]>();
            else if (track.TrackDatabaseID == LastUpdateTrackDatabaseID)
            {
                data.MetaDataUpdated = false;

                OnPlaybackEventChanged?.Invoke(data);
                return;
            }
            else
                data.MetaData.Clear();

            LastUpdateTrackDatabaseID = track.TrackDatabaseID;

            data.UpdateTimeOfDay = data.TimeOfDay;
            data.MetaDataUpdated = true;

            if (track is iTunesLib.IITFileOrCDTrack file)
            {
                data.FilePath = file.Location;
                if (!string.IsNullOrEmpty(file.Lyrics))
                {
                    data.MetaData.Add("lyrics", new string[] { file.Lyrics });
                }
            }
            data.MetaData.Add("album", new string[] { track.Album });
            data.MetaData.Add("artist", new string[] { track.Artist });
            data.MetaData.Add("bitrate", new string[] { track.BitRate.ToString() });
            data.MetaData.Add("bpm", new string[] { track.BPM.ToString() });
            data.MetaData.Add("comment", new string[] { track.Comment });
            data.MetaData.Add("composer", new string[] { track.Composer });
            data.MetaData.Add("disccount", new string[] { track.DiscCount.ToString() });
            data.MetaData.Add("discnumber", new string[] { track.DiscNumber.ToString() });
            data.MetaData.Add("duration", new string[] { track.Duration.ToString() });
            data.MetaData.Add("genre", new string[] { track.Genre });
            data.MetaData.Add("grouping", new string[] { track.Grouping });
            data.MetaData.Add("kindasstring", new string[] { track.KindAsString });
            data.MetaData.Add("name", new string[] { track.Name });
            data.MetaData.Add("playedcount", new string[] { track.PlayedCount.ToString() });
            data.MetaData.Add("rating", new string[] { track.Rating.ToString() });
            data.MetaData.Add("samplerate", new string[] { track.SampleRate.ToString() });
            data.MetaData.Add("time", new string[] { track.Time });
            data.MetaData.Add("trackcount", new string[] { track.TrackCount.ToString() });
            data.MetaData.Add("tracknumber", new string[] { track.TrackNumber.ToString() });
            data.MetaData.Add("year", new string[] { track.Year.ToString() });

            data.Title = track.Name;
            data.Artists = new string[] { track.Artist };
            data.Album = track.Album;
            data.Duration = track.Duration;

            OnPlaybackEventChanged?.Invoke(data);
        }

        private void OnPlayerPlayEvent(object iTrack)
        {
            OnPlayerEvent(iTrack, ITitalyverReceiver.EnumPlaybackEvent.SeekPlay);
        }
        private void OnPlayerStopEvent(object iTrack)
        {
            OnPlayerEvent(iTrack, ITitalyverReceiver.EnumPlaybackEvent.SeekStop);
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
