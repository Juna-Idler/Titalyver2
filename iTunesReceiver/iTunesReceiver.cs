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

        private int LastUpdateTrackID = 0;



        public iTunesReceiver()
        {
            iTunes = new iTunes();
            iTunes.SetOnPlayerPlayEvent(OnPlayerPlayEvent);
            iTunes.SetOnPlayerStopEvent(OnPlayerStopEvent);
            iTunes.SetOnQuittingEvent(OnQuittingEvent);
        }

        private static int GetTimeOfDay()
        {
            DateTime now = DateTime.Now;
            return ((now.Hour * 60 + now.Minute) * 60 + now.Second) * 1000 + now.Millisecond;
        }


        private void OnPlayerPlayEvent(object iTrack)
        {
            if (iTunes.App == null)
                return;
            iTunesLib.IITTrack track = iTrack as iTunesLib.IITTrack;
            if (track == null)
                return;

            data.PlaybackEvent = ITitalyverReceiver.EnumPlaybackEvent.SeekPlay;
            data.SeekTime = iTunes.App.PlayerPositionMS / 1000.0;
            data.TimeOfDay = GetTimeOfDay();

            if (data.MetaData == null)
                data.MetaData = new Dictionary<string, string[]>();
            else if (track.trackID == LastUpdateTrackID)
            {
                data.MetaDataUpdated = false;

                OnPlaybackEventChanged?.Invoke(data);
                return;
            }
            else
                data.MetaData.Clear();

            LastUpdateTrackID = track.trackID;

            data.UpdateTimeOfDay = data.TimeOfDay;
            data.MetaDataUpdated = true;

            iTunesLib.IITFileOrCDTrack file = track as iTunesLib.IITFileOrCDTrack;
            if (file != null)
            {
                data.FilePath = file.Location;
                if (!string.IsNullOrEmpty(file.Lyrics))
                {
                    data.MetaData.Add("lyrics", new string[] { file.Lyrics });
                }
            }
            data.MetaData.Add("album", new string[] { track.Album});
            data.MetaData.Add("artist", new string[] { track.Artist});
            data.MetaData.Add("comment", new string[] { track.Comment});
            data.MetaData.Add("composer", new string[] { track.Composer});
            data.MetaData.Add("disccount", new string[] { track.DiscCount.ToString()});
            data.MetaData.Add("discnumber", new string[] { track.DiscNumber.ToString() });
            data.MetaData.Add("duration", new string[] { track.Duration.ToString() });
            data.MetaData.Add("genre", new string[] { track.Genre});
            data.MetaData.Add("kindasstring", new string[] { track.KindAsString});
            data.MetaData.Add("name", new string[] { track.Name});
            data.MetaData.Add("playedcount", new string[] { track.PlayedCount.ToString() });
            data.MetaData.Add("rating", new string[] { track.Rating.ToString() });
            data.MetaData.Add("time", new string[] { track.Time});
            data.MetaData.Add("trackcount", new string[] { track.TrackCount.ToString() });
            data.MetaData.Add("tracknumber", new string[] { track.TrackNumber.ToString() });
            data.MetaData.Add("year", new string[] { track.Year.ToString() });

            OnPlaybackEventChanged?.Invoke(data);
        }
        private void OnPlayerStopEvent(object iTrack)
        {
            if (iTunes.App == null)
                return;
            iTunesLib.IITTrack track = iTrack as iTunesLib.IITTrack;
            if (track == null)
                return;

            data.PlaybackEvent = ITitalyverReceiver.EnumPlaybackEvent.SeekStop;
            data.SeekTime = iTunes.App.PlayerPositionMS / 1000.0;
            data.TimeOfDay = GetTimeOfDay();

            if (data.MetaData == null)
                data.MetaData = new Dictionary<string, string[]>();
            else if (track.trackID == LastUpdateTrackID)
            {
                data.MetaDataUpdated = false;

                OnPlaybackEventChanged?.Invoke(data);
                return;
            }
            else
                data.MetaData.Clear();

            data.UpdateTimeOfDay = data.TimeOfDay;
            data.MetaDataUpdated = true;

            iTunesLib.IITFileOrCDTrack file = track as iTunesLib.IITFileOrCDTrack;
            if (file != null)
            {
                data.FilePath = file.Location;
                if (!string.IsNullOrEmpty(file.Lyrics))
                {
                    data.MetaData.Add("lyrics", new string[] { file.Lyrics });
                }
            }
            data.MetaData.Add("album", new string[] { track.Album });
            data.MetaData.Add("artist", new string[] { track.Artist });
            data.MetaData.Add("comment", new string[] { track.Comment });
            data.MetaData.Add("composer", new string[] { track.Composer });
            data.MetaData.Add("disccount", new string[] { track.DiscCount.ToString() });
            data.MetaData.Add("discnumber", new string[] { track.DiscNumber.ToString() });
            data.MetaData.Add("duration", new string[] { track.Duration.ToString() });
            data.MetaData.Add("genre", new string[] { track.Genre });
            data.MetaData.Add("kindasstring", new string[] { track.KindAsString });
            data.MetaData.Add("name", new string[] { track.Name });
            data.MetaData.Add("playedcount", new string[] { track.PlayedCount.ToString() });
            data.MetaData.Add("rating", new string[] { track.Rating.ToString() });
            data.MetaData.Add("time", new string[] { track.Time });
            data.MetaData.Add("trackcount", new string[] { track.TrackCount.ToString() });
            data.MetaData.Add("tracknumber", new string[] { track.TrackNumber.ToString() });
            data.MetaData.Add("year", new string[] { track.Year.ToString() });

            OnPlaybackEventChanged?.Invoke(data);
        }
        private void OnQuittingEvent()
        {
            iTunes.Dispose();
        }


    }
}
