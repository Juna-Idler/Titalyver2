using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iTunesLib;
using System.Runtime.InteropServices;


namespace iTunesReceiver
{
    public class iTunes : IDisposable
    {
        public iTunes()
        {
            App = new iTunesApp();
        }

        ~iTunes()
        {
            Dispose(disposing: false);
        }

        public iTunesApp App { get; private set; }


        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // マネージリソースの解放をここに書く
            }

            if (App != null)
            {
                SetOnPlayerPlayEvent(null);
                SetOnPlayerStopEvent(null);
                SetOnAboutToPromptUserToQuitEvent(null);
                SetOnQuittingEvent(null);
                _ = Marshal.ReleaseComObject(App);
                App = null;
            }
        }


        private _IiTunesEvents_OnPlayerPlayEventEventHandler OnPlayerPlayEvent;
        public void SetOnPlayerPlayEvent(_IiTunesEvents_OnPlayerPlayEventEventHandler onPlayerPlayEvent)
        {
            if (OnPlayerPlayEvent != null)
                App.OnPlayerPlayEvent -= OnPlayerPlayEvent;
            if (onPlayerPlayEvent != null)
                App.OnPlayerPlayEvent += onPlayerPlayEvent;
            OnPlayerPlayEvent = onPlayerPlayEvent;
        }

        private _IiTunesEvents_OnPlayerStopEventEventHandler OnPlayerStopEvent;
        public void SetOnPlayerStopEvent(_IiTunesEvents_OnPlayerStopEventEventHandler onPlayerStopEvent)
        {
            if (OnPlayerStopEvent != null)
                App.OnPlayerStopEvent -= OnPlayerStopEvent;
            if (onPlayerStopEvent != null)
                App.OnPlayerStopEvent += onPlayerStopEvent;
            OnPlayerStopEvent = onPlayerStopEvent;
        }

        private _IiTunesEvents_OnAboutToPromptUserToQuitEventEventHandler OnAboutToPromptUserToQuitEvent;
        public void SetOnAboutToPromptUserToQuitEvent(_IiTunesEvents_OnAboutToPromptUserToQuitEventEventHandler onAboutToPromptUserToQuitEvent)
        {
            if (OnAboutToPromptUserToQuitEvent != null)
                App.OnAboutToPromptUserToQuitEvent -= OnAboutToPromptUserToQuitEvent;
            if (onAboutToPromptUserToQuitEvent != null)
                App.OnAboutToPromptUserToQuitEvent += onAboutToPromptUserToQuitEvent;
            OnAboutToPromptUserToQuitEvent = onAboutToPromptUserToQuitEvent;
        }


        private _IiTunesEvents_OnQuittingEventEventHandler OnQuittingEvent;
        public void SetOnQuittingEvent(_IiTunesEvents_OnQuittingEventEventHandler onQuittingEvent)
        {
            if (OnQuittingEvent != null)
                App.OnQuittingEvent -= OnQuittingEvent;
            if (onQuittingEvent != null)
                App.OnQuittingEvent += onQuittingEvent;
            OnQuittingEvent = onQuittingEvent;
        }
    }
}
