using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

using System.IO;

using System.Windows.Data;
using System.Globalization;

namespace Titalyver2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private ITitalyverReceiver Receiver;

        public readonly LyricsSearcher LyricsSearcher;


        public MainWindow()
        {
            InitializeComponent();

            KaraokeDisplay.SetLyrics("");
            LyricsSearcher = new LyricsSearcher();

            RestoreSettings();

        }
        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            iTunesReceiverDll iTunesDll = new iTunesReceiverDll();
            if (iTunesDll.Load())
            {
                Receiver = iTunesDll.GetReceiver();
                if (Receiver != null)
                {
                    Receiver.OnPlaybackEventChanged += PlaybackEvent;
                }
            }
            if (Receiver == null)
            {
                MMFReceiver mmfr = new MMFReceiver(PlaybackEvent);
                Receiver = mmfr;
                mmfr.ReadData();
                ITitalyverReceiver.Data data = mmfr.GetData();
                if (!data.IsValid())
                    return;
                string lyrics = GetLyrics(data);
                KaraokeDisplay.SetLyrics(lyrics);
                if (lyrics != "")
                {
                    double delay = (MMFMessage.GetTimeOfDay() - data.TimeOfDay) / 1000.0;
                    KaraokeDisplay.Time = delay + data.SeekTime;
                    KaraokeDisplay.SetAutoScrollY(KaraokeDisplay.Time);
                    if ((data.PlaybackEvent & ITitalyverReceiver.EnumPlaybackEvent.Play) == ITitalyverReceiver.EnumPlaybackEvent.Play)
                        KaraokeDisplay.Start();
                }
            }

        }
        private void window_Closed(object sender, EventArgs e)
        {
            Receiver.Terminalize();

        }



        private void RestoreSettings()
        {
            Properties.Settings set = Properties.Settings.Default;
            FontFamily ff = new(set.FontFamily);
            FontStyle fsy;
            try { fsy = (FontStyle)TypeDescriptor.GetConverter(typeof(FontStyle)).ConvertFromString(set.FontStyle); }
            catch ( Exception) { fsy = FontStyles.Normal; }
            FontWeight fw;
            try { fw = (FontWeight)TypeDescriptor.GetConverter(typeof(FontWeight)).ConvertFromString(set.FontWeight); }
            catch (Exception) { fw = FontWeights.Normal; }
            FontStretch fsr;
            try { fsr = (FontStretch)TypeDescriptor.GetConverter(typeof(FontStretch)).ConvertFromString(set.FontStretch); }
            catch (Exception) { fsr = FontStretches.Normal; }
            KaraokeDisplay.SetFont(new(ff, fsy, fw, fsr), set.FontSize);

            BrushConverter bc = new();
            KaraokeDisplay.ActiveFillColor = (SolidColorBrush)bc.ConvertFromString(set.ActiveFill);
            KaraokeDisplay.StandbyFillColor = (SolidColorBrush)bc.ConvertFromString(set.StandbyFill);
            KaraokeDisplay.SleepFillColor = (SolidColorBrush)bc.ConvertFromString(set.SleepFill);
            KaraokeDisplay.ActiveStrokeColor = (SolidColorBrush)bc.ConvertFromString(set.ActiveStroke);
            KaraokeDisplay.StandbyStrokeColor = (SolidColorBrush)bc.ConvertFromString(set.StandbyStroke);
            KaraokeDisplay.SleepStrokeColor = (SolidColorBrush)bc.ConvertFromString(set.SleepStroke);
            KaraokeDisplay.ActiveBackColor = (SolidColorBrush)bc.ConvertFromString(set.ActiveBack);

            Background = (SolidColorBrush)bc.ConvertFromString(set.WindowBack);

            KaraokeDisplay.TextAlignment = (TextAlignment)TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertFromString(set.TextAlignment);
            KaraokeDisplay.KaraokeVerticalAlignment = (VerticalAlignment)TypeDescriptor.GetConverter(typeof(VerticalAlignment)).ConvertFromString(set.VerticalAlignment);

            KaraokeDisplay.LinePadding = new Thickness(set.OffsetLeft, set.LineTopSpace, set.OffsetRight, set.LineBottomSpace);
            KaraokeDisplay.VerticalOffsetY = set.OffsetVertical;

            KaraokeDisplay.RubyBottomSpace = set.RubyBottomSpace;
            KaraokeDisplay.NoRubyTopSpace = set.NoRubySpace;


            LyricsSearcher.SetSearchList(set.LyricsSearchList);
            LyricsSearcher.NoLyricsFormatText = set.NoLyricsFormat;

            KaraokeDisplay.IgnoreKaraokeTag = set.IgnoreKaraoke;
        }

        private string GetLyrics(ITitalyverReceiver.Data data)
        {
            string lp = "";
            if (!string.IsNullOrEmpty(data.FilePath))
            {
                Uri u = new Uri(data.FilePath);
                lp = u.LocalPath + Uri.UnescapeDataString(u.Fragment);
            }
            string text = LyricsSearcher.Search(lp, data.MetaData);
            if (text == "")
            {
                return "";
            }
            return text;
        }

        private void PlaybackEvent(ITitalyverReceiver.Data data)
        {

            if (data.MetaDataUpdated)
            {
                string text = GetLyrics(data);
                _ = Dispatcher.InvokeAsync(() =>
                {
                    KaraokeDisplay.SetLyrics(text);
                });
            }
            double time = -1;
            if ((data.PlaybackEvent & ITitalyverReceiver.EnumPlaybackEvent.Bit_Seek) == ITitalyverReceiver.EnumPlaybackEvent.Bit_Seek)
            {
                time = data.SeekTime;
            }
            switch (data.PlaybackEvent & ~ITitalyverReceiver.EnumPlaybackEvent.Bit_Seek)
            {
                case ITitalyverReceiver.EnumPlaybackEvent.Play:
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        if (time >= 0)
                        {
                            double delay = (MMFMessage.GetTimeOfDay() - data.TimeOfDay) / 1000.0;
                            KaraokeDisplay.Time = time + delay;
                            KaraokeDisplay.SetAutoScrollY(KaraokeDisplay.Time);
                        }
                        KaraokeDisplay.Start();
                    });
                    break;
                case ITitalyverReceiver.EnumPlaybackEvent.Stop:
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        if (time >= 0)
                        {
                            KaraokeDisplay.Time = time;
                            KaraokeDisplay.SetAutoScrollY(KaraokeDisplay.Time);
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

        private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            KaraokeDisplay.UserTimeOffset = TimeSlider.Value;
//            if (!KaraokeDisplay.Starting)
//                KaraokeDisplay
        }
        private void TimeSlider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                TimeSlider.Value = 0;
                e.Handled = true;
            }
        }

        private void TimeSlider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                TimeSlider.Value += TimeSlider.SmallChange;
            if (e.Delta < 0)
                TimeSlider.Value -= TimeSlider.SmallChange;
            e.Handled = true;
        }

        private void SliderButton_Click(object sender, RoutedEventArgs e)
        {
            TimeSlider.Visibility = (SliderButton.IsChecked ?? false) ? Visibility.Visible : Visibility.Hidden;
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TimeSlider.Width = ActualWidth - 64;
        }

        private void MenuItemTopmost_Click(object sender, RoutedEventArgs e)
        {
            var i = (System.Windows.Controls.MenuItem)sender;
            Topmost = i.IsChecked;
        }

        private void MenuItemReload_Click(object sender, RoutedEventArgs e)
        {
            KaraokeDisplay.SetLyrics(GetLyrics(Receiver.GetData()));
        }

        private void window_ContextMenuOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            Maximize.IsChecked = WindowState == WindowState.Maximized;
            OpenFolder.IsEnabled = !String.IsNullOrEmpty(LyricsSearcher.FilePath);
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        }

        private void window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            string directoryname = Path.GetDirectoryName(LyricsSearcher.FilePath);
            string filename = Path.GetFileName(LyricsSearcher.FilePath);
            string filepath = Path.Combine(directoryname, filename);
            _ = System.Diagnostics.Process.Start("EXPLORER.EXE", @"/select," + filepath);
        }

        private void MenuItemText_Click(object sender, RoutedEventArgs e)
        {
            double h = System.Windows.SystemParameters.WorkArea.Height;
            TextViewWindow tvw = new TextViewWindow();
            tvw.MaxHeight = h;
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)tvw.Content;
            tb.Text = LyricsSearcher.Text;
            tvw.Owner = this;
            tvw.Show();
        }

    }
}
