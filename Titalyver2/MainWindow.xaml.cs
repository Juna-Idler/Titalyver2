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
            Message.Data data = Receiver.GetData();
            if (data.PlaybackEvent == Message.EnumPlaybackEvent.NULL)
                return;
            string lyrics =  GetLyrics(data);
            KaraokeDisplay.SetLyrics(lyrics);
            if (lyrics != "")
            {
                double delay = Message.GetTimeOfDay() - data.TimeOfDay / 1000.0;
                KaraokeDisplay.ForceMove(delay + data.SeekTime, 0.5);
                KaraokeDisplay.Time = delay + data.SeekTime;
                if ((data.PlaybackEvent & Message.EnumPlaybackEvent.PlayNew) == Message.EnumPlaybackEvent.PlayNew )
                    KaraokeDisplay.Start();
            }
        }

        private void RestoreSettings()
        {
            FontFamily ff = new FontFamily(Properties.Settings.Default.FontFamily);
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

            double delay = (Receiver.GetTimeOfDay() - data.TimeOfDay) / 1000.0;

            switch (data.PlaybackEvent)
            {
                case Message.EnumPlaybackEvent.PlayNew:
                    {
                        Uri uri = new(data.FilePath);
                        string lp = uri.LocalPath;

                        string text = LyricsSearcher.Search(lp, data.MetaData);
                        if (text == "")
                            break;
                        _ = Dispatcher.InvokeAsync(() =>
                        {
                            KaraokeDisplay.SetLyrics(text);
                            KaraokeDisplay.Time = (Message.GetTimeOfDay() - data.TimeOfDay) / 1000.0;
                            KaraokeDisplay.Start();
                            KaraokeDisplay.ForceMove(delay, 0.5);
                        });
                        break;
                    }
                case Message.EnumPlaybackEvent.Stop:
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        KaraokeDisplay.Stop();
                    });
                    break;
                case Message.EnumPlaybackEvent.PauseCancel:
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        KaraokeDisplay.Time = data.SeekTime + delay;
                        KaraokeDisplay.Start();
                    });
                    break;
                case Message.EnumPlaybackEvent.Pause:
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        KaraokeDisplay.Stop();
                        KaraokeDisplay.Time = data.SeekTime;
                    });
                    break;
                case Message.EnumPlaybackEvent.SeekPlaying:
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        KaraokeDisplay.Time = data.SeekTime + delay;
                        KaraokeDisplay.Start();
                        KaraokeDisplay.ForceMove(data.SeekTime + delay, 0.5);
                    });
                    break;
                case Message.EnumPlaybackEvent.SeekPause:
                    _ = Dispatcher.InvokeAsync(() =>

                    {
                        KaraokeDisplay.Stop();
                        KaraokeDisplay.Time = data.SeekTime;
                        KaraokeDisplay.ForceMove(data.SeekTime, 0.5);
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

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {

            SettingsWindow SettingWindow = new SettingsWindow(this);
            SettingWindow.Show();
        }
    }
}
