using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using System.IO;

using System.Windows.Data;
using System.Globalization;

using System.Text.Json;


namespace Titalyver2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Juna.WindowMoveSize windowMoveSize;

        private ITitalyverReceiver Receiver;

        public LyricsSearchers LyricsSearcher { get; private set; } = new();
        public LyricsSaver LyricsSaver { get; private set; } = new();

        public bool AutoSave { get; set; }
        public string LastSaveFile { get; set; }

        private LyricsSearchers.ReturnValue[] Lyrics;
        private int CurrentLyrics;

        public bool SpecifyWheelDelta { get; set; }
        public int WheelDelta { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            _ = WindowPositionStrage.Load(this);

            KaraokeDisplay.SetLyrics("");

            RestoreSettings();

        }
        private async void window_Loaded(object sender, RoutedEventArgs e)
        {
            windowMoveSize = new(this, 12, 16);

            iTunesReceiverDll iTunesDll = new();
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
                MMFReceiver mmfr = new(PlaybackEvent);
                Receiver = mmfr;


                ReceiverData data = mmfr.ReadData();
                if (data == null || data.MetaDataUpdated)
                    return;
                await SearchLyrics(data);

                MultiLyricsNumber.Text = (CurrentLyrics + 1).ToString() + "/" + Lyrics.Length;
                double delay = (MMF_Base.GetTimeOfDay() - data.TimeOfDay) / 1000.0;
                KaraokeDisplay.Time = delay + data.SeekTime;
                KaraokeDisplay.SetAutoScrollY(KaraokeDisplay.Time);
                if ((data.PlaybackEvent & EnumPlaybackEvent.Play) == EnumPlaybackEvent.Play)
                    KaraokeDisplay.Start();

            }

        }
        private void window_Closing(object sender, CancelEventArgs e)
        {
            WindowState = WindowState.Normal;
            _ = WindowPositionStrage.Save(this);
        }

        private void window_Closed(object sender, EventArgs e)
        {
            Receiver.Terminalize();
        }



        private void RestoreSettings()
        {
            SettingsStorage set = SettingsStorage.Default = SettingsStorage.Load() ?? new();

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


            LyricsSearcher.SearchList = set.LyricsSearchList;
            LyricsSearcher.NoLyricsFormatText = set.NoLyricsFormat;

            KaraokeDisplay.IgnoreKaraokeTag = set.IgnoreKaraoke;

            LyricsSearcher.MillisecondsTimeout = set.PluginTimeout;


            ff = new(set.UnsyncFontFamily);
            try { fsy = (FontStyle)TypeDescriptor.GetConverter(typeof(FontStyle)).ConvertFromString(set.UnsyncFontStyle); }
            catch (Exception) { fsy = FontStyles.Normal; }
            try { fw = (FontWeight)TypeDescriptor.GetConverter(typeof(FontWeight)).ConvertFromString(set.UnsyncFontWeight); }
            catch (Exception) { fw = FontWeights.Normal; }
            try { fsr = (FontStretch)TypeDescriptor.GetConverter(typeof(FontStretch)).ConvertFromString(set.UnsyncFontStretch); }
            catch (Exception) { fsr = FontStretches.Normal; }
            KaraokeDisplay.SetUnsyncFont(new(ff, fsy, fw, fsr), set.UnsyncFontSize);


            KaraokeDisplay.UnsyncFillColor = (SolidColorBrush)bc.ConvertFromString(set.UnsyncFill);
            KaraokeDisplay.UnsyncStrokeColor = (SolidColorBrush)bc.ConvertFromString(set.UnsyncStroke);

            KaraokeDisplay.UnsyncTextAlignment = (TextAlignment)TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertFromString(set.UnsyncTextAlignment);

            KaraokeDisplay.UnsyncLinePadding = new Thickness(set.UnsyncOffsetLeft, set.UnsyncLineTopSpace, set.UnsyncOffsetRight, set.UnsyncLineBottomSpace);
            KaraokeDisplay.UnsyncVerticalOffsetY = set.UnsyncOffsetVertical;

            KaraokeDisplay.UnsyncRubyBottomSpace = set.UnsyncRubyBottomSpace;
            KaraokeDisplay.UnsyncNoRubyTopSpace = set.UnsyncNoRubySpace;

            KaraokeDisplay.UnsyncAutoScroll = set.UnsyncAutoScroll;
            KaraokeDisplay.UnsyncIntro = set.UnsyncIntro;
            KaraokeDisplay.UnsyncOutro = set.UnsyncOutro;

            LyricsSaver.SaveList = set.SavePathList;
            LyricsSaver.Extension = (LyricsSaver.EnumExtension)set.SaveExtension;
            LyricsSaver.Overwrite = (LyricsSaver.EnumOverwrite)set.SaveOverwrite;
            AutoSave = set.AutoSave;

            SpecifyWheelDelta = set.SpecifyWheelDelta;
            WheelDelta = set.WheelDelta;

            LyricsSearcher.ManualSearchList = set.ManualSearchList;
        }

        private async Task SearchLyrics(ReceiverData data)
        {
            KaraokeDisplay.SetLyrics("Searching...");
            Lyrics = null;
            MultiLyricsSwitchPanel.Visibility = Visibility.Hidden;

            Lyrics = await LyricsSearcher.Search(data);

            CurrentLyrics = 0;
            if (Lyrics.Length > 1)
            {
                MultiLyricsSwitchPanel.Visibility = Visibility.Visible;
                MultiLyricsNumber.Text = 1 + "/" + Lyrics.Length;
            }
            else
            {
                MultiLyricsSwitchPanel.Visibility = Visibility.Hidden;
            }
            KaraokeDisplay.SetLyrics(Lyrics[0].Text);
            if (KaraokeDisplay.Lyrics.Sync == LyricsContainer.SyncMode.Unsync)
            {
                KaraokeDisplay.UnsyncDuration = data.Duration;
            }

            if (AutoSave && Lyrics[0].Command == "plugin")
            {
                if (LyricsSaver.Save(Lyrics[0].Text, KaraokeDisplay.Lyrics.Sync,
                                     data, out string saved_path))
                {
                    LastSaveFile = saved_path;
                }
            }
        }
        public async Task ManualSearchLyrics(int plugin_index, string title, string[] artists, string album, string path, string param, int timeout,
                                             ReceiverData data = null)
        {
            KaraokeDisplay.SetLyrics("Searching...");
            Lyrics = null;
            MultiLyricsSwitchPanel.Visibility = Visibility.Hidden;

            Lyrics = await LyricsSearcher.ManualSearch(plugin_index, title, artists, album, path, param, timeout);
            CurrentLyrics = 0;
            if (Lyrics.Length > 1)
            {
                MultiLyricsSwitchPanel.Visibility = Visibility.Visible;
                MultiLyricsNumber.Text = 1 + "/" + Lyrics.Length;
            }
            else
            {
                MultiLyricsSwitchPanel.Visibility = Visibility.Hidden;
            }

            KaraokeDisplay.SetLyrics(Lyrics[0].Text);
            if (KaraokeDisplay.Lyrics.Sync == LyricsContainer.SyncMode.Unsync)
            {
                KaraokeDisplay.UnsyncDuration = data == null ? 0 : data.Duration;
            }
            if (data != null)
            {
                if (LyricsSaver.Save(Lyrics[0].Text, KaraokeDisplay.Lyrics.Sync,
                                     data, out string saved_path))
                {
                    LastSaveFile = saved_path;
                }
            }
        }


        private void PlaybackEvent(ReceiverData data)
        {
            if (data.MetaDataUpdated)
            {
                _ = Dispatcher.InvokeAsync(async () =>
                {
                    await SearchLyrics(data);
                });
            }
            double time = -1;
            if ((data.PlaybackEvent & EnumPlaybackEvent.Bit_Seek) == EnumPlaybackEvent.Bit_Seek)
            {
                time = data.SeekTime;
            }
            switch (data.PlaybackEvent & ~EnumPlaybackEvent.Bit_Seek)
            {
                case EnumPlaybackEvent.Play:
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        if (time >= 0)
                        {
                            double delay = (MMF_Base.GetTimeOfDay() - data.TimeOfDay) / 1000.0;
                            KaraokeDisplay.Time = time + delay;
                            KaraokeDisplay.SetAutoScrollY(KaraokeDisplay.Time);
                        }
                        KaraokeDisplay.Start();
                    });
                    break;
                case EnumPlaybackEvent.Stop:
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
            KaraokeDisplay.ManualScrollY += SpecifyWheelDelta ? e.Delta / Mouse.MouseWheelDeltaForOneLine * WheelDelta : e.Delta;
        }

//        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
//        {
//            DragMove();
//        }

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

        private async void MenuItemReload_Click(object sender, RoutedEventArgs e)
        {
            await SearchLyrics(Receiver.GetData());
        }
        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            if (LyricsSaver.Save(KaraokeDisplay.LyricsText, KaraokeDisplay.Lyrics.Sync,
                                 Receiver.GetData(), out string saved_path))
            {
                LastSaveFile = saved_path;
            }
        }
        private void ManualSearch_Click(object sender, RoutedEventArgs e)
        {
            double h = System.Windows.SystemParameters.WorkArea.Height;
            ManualSearchWindow msw = new(this, Receiver.GetData());
            msw.Owner = this;
            msw.Show();
        }

        private void OpenSaveFolder_Click(object sender, RoutedEventArgs e)
        {
//            string directoryname = Path.GetDirectoryName(LastSaveFile);
//            string filename = Path.GetFileName(LastSaveFile);
//            string filepath = Path.Combine(directoryname, filename);

            string filepath = LastSaveFile.Replace('/', '\\');
            _ = System.Diagnostics.Process.Start("EXPLORER.EXE", $"/select,\"{filepath}\"");
        }

        private void window_ContextMenuOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                e.Handled = true;
                return;
            }

            Maximize.IsChecked = WindowState == WindowState.Maximized;

            if (Lyrics == null)
            {
                OpenFolder.IsEnabled = false;
                Save.IsEnabled = false;
                ViewText.IsEnabled = false;
                ReSearch.IsEnabled = false;
                MusicData.IsEnabled = false;
            }
            else
            {
                Save.IsEnabled = true;
                ViewText.IsEnabled = true;
                ReSearch.IsEnabled = true;
                MusicData.IsEnabled = true;

                OpenFolder.IsEnabled = !string.IsNullOrEmpty(Lyrics[CurrentLyrics].FilePath);
                SearchListCommand.Header = Lyrics[CurrentLyrics].Command + ":" + Lyrics[CurrentLyrics].Parameter;
            }
            OpenLastSaveFolder.IsEnabled = !string.IsNullOrEmpty(LastSaveFile);

            ManualSearch.IsEnabled = LyricsSearcher.ManualSearchList.Length > 0;
        }



        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
//            string directoryname = Path.GetDirectoryName(Lyrics[CurrentLyrics].FilePath);
//            string filename = Path.GetFileName(Lyrics[CurrentLyrics].FilePath);
//            string filepath = Path.Combine(directoryname, filename);

            string filepath = Lyrics[CurrentLyrics].FilePath.Replace('/', '\\');
            _ = System.Diagnostics.Process.Start("EXPLORER.EXE", $"/select,\"{filepath}\"");
        }

        private void MenuItemText_Click(object sender, RoutedEventArgs e)
        {
            double h = System.Windows.SystemParameters.WorkArea.Height;
            TextViewWindow tvw = new();
            tvw.MaxHeight = h;
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)tvw.Content;
            tb.Text = Lyrics[CurrentLyrics].Text;
            tvw.Owner = this;
            tvw.Show();
        }
        private void MusicData_Click(object sender, RoutedEventArgs e)
        {
            double h = System.Windows.SystemParameters.WorkArea.Height;
            TextViewWindow tvw = new();
            tvw.MaxHeight = h;
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)tvw.Content;
            var data = Receiver.GetData();
            tb.Text = $"Title={data.Title}\nArtists={string.Join(',', data.Artists)}\nAlbum={data.Album}\nPath={data.FilePath}\nDuration={data.Duration}\n";

            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create( System.Text.Unicode.UnicodeRanges.All)
            };
            tb.Text += System.Text.RegularExpressions.Regex.Unescape(JsonSerializer.Serialize(data.MetaData, options));
            tvw.Owner = this;
            tvw.Show();
        }

        private void PrevLyricsButton_Click(object sender, RoutedEventArgs e)
        {
            if (--CurrentLyrics < 0)
                CurrentLyrics = Lyrics.Length - 1;
            KaraokeDisplay.SetLyrics(Lyrics[CurrentLyrics].Text); ;

            MultiLyricsNumber.Text = (CurrentLyrics + 1).ToString() + "/" + Lyrics.Length; 
        }

        private void NextLyricsButton_Click(object sender, RoutedEventArgs e)
        {
            if (++CurrentLyrics >= Lyrics.Length)
                CurrentLyrics = 0;
            KaraokeDisplay.SetLyrics(Lyrics[CurrentLyrics].Text); ;

            MultiLyricsNumber.Text = (CurrentLyrics + 1).ToString() + "/" + Lyrics.Length;
        }

    }
}
