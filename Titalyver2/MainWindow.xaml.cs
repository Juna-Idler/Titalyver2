using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

namespace Titalyver2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private readonly Receiver Receiver;

        private readonly LyricsSearcher LyricsSearcher;


        public MainWindow()
        {
            InitializeComponent();

            KaraokeDisplay.SetLyrics("");
            LyricsSearcher = new LyricsSearcher();


            RestoreSettings();


            Receiver = new Receiver(PlaybackEvent);
            Receiver.ReadData();
            Message.Data data = Receiver.GetData();
            if (!data.IsValid())
                return;
            string lyrics =  GetLyrics(data);
            KaraokeDisplay.SetLyrics(lyrics);
            if (lyrics != "")
            {
                double delay = Message.GetTimeOfDay() - data.TimeOfDay / 1000.0;
                KaraokeDisplay.ForceMove(delay + data.SeekTime, 0.5);
                KaraokeDisplay.Time = delay + data.SeekTime;
                if ((data.PlaybackEvent & Message.EnumPlaybackEvent.Play) == Message.EnumPlaybackEvent.Play)
                    KaraokeDisplay.Start();
            }
        }

        private void RestoreSettings()
        {
            FontFamily ff = new(Properties.Settings.Default.FontFamily);
            FontStyle fsy;
            try { fsy = (FontStyle)TypeDescriptor.GetConverter(typeof(FontStyle)).ConvertFromString(Properties.Settings.Default.FontStyle); }
            catch ( Exception) { fsy = FontStyles.Normal; }
            FontWeight fw;
            try { fw = (FontWeight)TypeDescriptor.GetConverter(typeof(FontWeight)).ConvertFromString(Properties.Settings.Default.FontWeight); }
            catch (Exception) { fw = FontWeights.Normal; }
            FontStretch fsr;
            try { fsr = (FontStretch)TypeDescriptor.GetConverter(typeof(FontStretch)).ConvertFromString(Properties.Settings.Default.FontStretch); }
            catch (Exception) { fsr = FontStretches.Normal; }
            KaraokeDisplay.SetFont(new(ff, fsy, fw, fsr), Properties.Settings.Default.FontSize);

            BrushConverter bc = new();
            KaraokeDisplay.ActiveFillColor = (SolidColorBrush)bc.ConvertFromString(Properties.Settings.Default.ActiveFill);
            KaraokeDisplay.StandbyFillColor = (SolidColorBrush)bc.ConvertFromString(Properties.Settings.Default.StandbyFill);
            KaraokeDisplay.SleepFillColor = (SolidColorBrush)bc.ConvertFromString(Properties.Settings.Default.SleepFill);
            KaraokeDisplay.ActiveStrokeColor = (SolidColorBrush)bc.ConvertFromString(Properties.Settings.Default.ActiveStroke);
            KaraokeDisplay.StandbyStrokeColor = (SolidColorBrush)bc.ConvertFromString(Properties.Settings.Default.StandbyStroke);
            KaraokeDisplay.SleepStrokeColor = (SolidColorBrush)bc.ConvertFromString(Properties.Settings.Default.SleepStroke);
            KaraokeDisplay.ActiveBackColor = (SolidColorBrush)bc.ConvertFromString(Properties.Settings.Default.ActiveBack);

            Background = (SolidColorBrush)bc.ConvertFromString(Properties.Settings.Default.WindowBack);
        }

        private string GetLyrics(Receiver.Data data)
        {
            Uri uri = new(data.FilePath);
            string lp = uri.LocalPath;

            string text = LyricsSearcher.Search(lp, data.MetaData);
            if (text == "")
                return "";
            return text;

        }

        private void PlaybackEvent(Receiver.Data data)
        {

            if ((data.PlaybackEvent & Message.EnumPlaybackEvent.Bit_Update) == Message.EnumPlaybackEvent.Bit_Update)
            {
                Uri uri = new(data.FilePath);
                string lp = uri.LocalPath;

                string text = LyricsSearcher.Search(lp, data.MetaData);
                _ = Dispatcher.InvokeAsync(() =>
                {
                    KaraokeDisplay.SetLyrics(text);
                });
            }
            double time = -1;
            if ((data.PlaybackEvent & Message.EnumPlaybackEvent.Bit_Seek) == Message.EnumPlaybackEvent.Bit_Seek)
            {
                time = data.SeekTime;
            }
            switch (data.PlaybackEvent & ~Message.EnumPlaybackEvent.Bit_Update & ~Message.EnumPlaybackEvent.Bit_Seek)
            {
                case Message.EnumPlaybackEvent.Play:
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        if (time >= 0)
                        {
                            double delay = (Receiver.GetTimeOfDay() - data.TimeOfDay) / 1000.0;
                            KaraokeDisplay.Time = time + delay;
                            KaraokeDisplay.ForceMove(KaraokeDisplay.Time, 0.5);
                        }
                        KaraokeDisplay.Start();
                    });
                    break;
                case Message.EnumPlaybackEvent.Stop:
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        if (time >= 0)
                        {
                            KaraokeDisplay.Time = time;
                            KaraokeDisplay.ForceMove(KaraokeDisplay.Time, 0.5);
                        }
                        KaraokeDisplay.Stop();
                    });
                    break;
            }
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            KaraokeDisplay.ManualScrollY += e.Delta;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                KaraokeDisplay.ManualScrollY = 0;
            }
        }

        private SettingsWindow SettingsWindow;

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsWindow != null)
            {
                SettingsWindow.Activate();
                return;
            }
            SettingsWindow = new SettingsWindow(this);
            SettingsWindow.Closed += (s, e) => { SettingsWindow = null; };
            SettingsWindow.Show();
        }
    }
}
