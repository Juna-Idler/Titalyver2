using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.IO;

using System.Windows.Markup;
using System.Globalization;

using emanual.Wpf.Dialogs;

using System.ComponentModel;


namespace Titalyver2
{
    /// <summary>
    /// Settings.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private static readonly BrushConverter bc = new();

        private MainWindow MainWindow;


        public SettingsWindow(MainWindow mainWindow)
        {
            Owner = mainWindow;
            InitializeComponent();
            Language = Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);

            InitializeDisplay(mainWindow);
            InitializeLyrics(mainWindow);
            InitializeUnsync(mainWindow);

            MainWindow = mainWindow;


            TabItemSave.Content = new SaveSettings(mainWindow);

        }

        #region Display
        private void InitializeDisplay(MainWindow mainWindow)
        {
            FontSelect.Content = $"{TypefaceString(mainWindow.KaraokeDisplay.Typeface)} {mainWindow.KaraokeDisplay.FontSize}";
            Outline.Value = (decimal)mainWindow.KaraokeDisplay.StrokeThickness;

            ActiveFill.Background = mainWindow.KaraokeDisplay.ActiveFillColor;
            ActiveFill.Text = mainWindow.KaraokeDisplay.ActiveFillColor.Color.ToString();
            ActiveStroke.Background = mainWindow.KaraokeDisplay.ActiveStrokeColor;
            ActiveStroke.Text = mainWindow.KaraokeDisplay.ActiveStrokeColor.Color.ToString();
            StandbyFill.Background = mainWindow.KaraokeDisplay.StandbyFillColor;
            StandbyFill.Text = mainWindow.KaraokeDisplay.StandbyFillColor.Color.ToString();
            StandbyStroke.Background = mainWindow.KaraokeDisplay.StandbyStrokeColor;
            StandbyStroke.Text = mainWindow.KaraokeDisplay.StandbyStrokeColor.Color.ToString();
            SleepFill.Background = mainWindow.KaraokeDisplay.SleepFillColor;
            SleepFill.Text = mainWindow.KaraokeDisplay.SleepFillColor.Color.ToString();
            SleepStroke.Background = mainWindow.KaraokeDisplay.SleepStrokeColor;
            SleepStroke.Text = mainWindow.KaraokeDisplay.SleepStrokeColor.Color.ToString();

            ActiveBack.Background = mainWindow.KaraokeDisplay.ActiveBackColor;
            ActiveBack.Text = mainWindow.KaraokeDisplay.ActiveBackColor.Color.ToString();
            WindowBack.Background = mainWindow.Background;
            WindowBack.Text = ((SolidColorBrush)mainWindow.Background).Color.ToString();

            switch (mainWindow.KaraokeDisplay.TextAlignment)
            {
                case TextAlignment.Left:
                    HLeft.IsChecked = true;
                    break;
                case TextAlignment.Center:
                    HCenter.IsChecked = true;
                    break;
                case TextAlignment.Right:
                    HRight.IsChecked = true;
                    break;
            }
            switch (mainWindow.KaraokeDisplay.KaraokeVerticalAlignment)
            {
                case VerticalAlignment.Top:
                    VTop.IsChecked = true;
                    break;
                case VerticalAlignment.Center:
                    VMiddle.IsChecked = true;
                    break;
                case VerticalAlignment.Bottom:
                    VBottom.IsChecked = true;
                    break;
            }

            Thickness t = mainWindow.KaraokeDisplay.LinePadding;
            OffsetLeft.Value = (decimal)t.Left;
            OffsetRight.Value = (decimal)t.Right;
            OffsetVertical.Value = (decimal)mainWindow.KaraokeDisplay.VerticalOffsetY;

            LineTop.Value = (decimal)t.Top;
            LineBottom.Value = (decimal)t.Bottom;
            RubyBottom.Value = (decimal)mainWindow.KaraokeDisplay.RubyBottomSpace;
            NoRubyTop.Value = (decimal)mainWindow.KaraokeDisplay.NoRubyTopSpace;
        }

        private string TypefaceString(Typeface typeface)
        {
            string family = typeface.FontFamily.FamilyNames.ContainsKey(Language) ? typeface.FontFamily.FamilyNames[Language] : typeface.FontFamily.Source;
            return family + " " + typeface.Style.ToString() + " " + typeface.Weight.ToString() + " " + typeface.Stretch.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FontDialogEx
            {
                SelectedFontFamily = MainWindow.KaraokeDisplay.Typeface.FontFamily,
                SelectedFontStyle = MainWindow.KaraokeDisplay.Typeface.Style,
                SelectedFontWeight = MainWindow.KaraokeDisplay.Typeface.Weight,
                SelectedFontStretch = MainWindow.KaraokeDisplay.Typeface.Stretch,
                SelectedFontSize = MainWindow.KaraokeDisplay.FontSize,

                Owner = this
            };

            if (dlg.ShowDialog() == true)
            {
                MainWindow.KaraokeDisplay.SetFont(new Typeface(dlg.SelectedFontFamily, dlg.SelectedFontStyle, dlg.SelectedFontWeight, dlg.SelectedFontStretch), dlg.SelectedFontSize);

                Properties.Settings.Default.FontFamily = dlg.SelectedFontFamily.Source;
                Properties.Settings.Default.FontSize = dlg.SelectedFontSize;
                Properties.Settings.Default.FontStyle = TypeDescriptor.GetConverter(typeof(FontStyle)).ConvertToString(dlg.SelectedFontStyle);
                Properties.Settings.Default.FontWeight = TypeDescriptor.GetConverter(typeof(FontWeight)).ConvertToString(dlg.SelectedFontWeight);
                Properties.Settings.Default.FontStretch = TypeDescriptor.GetConverter(typeof(FontStretch)).ConvertToString(dlg.SelectedFontStretch);

                FontSelect.Content = $"{TypefaceString(MainWindow.KaraokeDisplay.Typeface)} {MainWindow.KaraokeDisplay.FontSize}";
            }

        }

        private static SolidColorBrush ColorTextChanged(TextBox box)
        {
            try
            {
                SolidColorBrush brush = (SolidColorBrush)TypeDescriptor.GetConverter(typeof(SolidColorBrush)).ConvertFromString(box.Text);
                box.Background = brush;
                Color c = brush.Color;
                double m = c.R * 0.21 + c.G * 0.72 + c.B * 0.07;
                box.Foreground = m > 128 ? Brushes.Black : Brushes.White;
                return brush;
            }
            catch (Exception)
            {
                box.Background = Brushes.White;
                box.Foreground = Brushes.Red;
            }
            return null;
        }

        private void TextBoxAF_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.ActiveFillColor = ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.ActiveFillColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            Properties.Settings.Default.ActiveFill = bc.ConvertToString(MainWindow.KaraokeDisplay.ActiveFillColor);
        }

        private void TextBoxStF_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.StandbyFillColor = ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.StandbyFillColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            Properties.Settings.Default.StandbyFill = bc.ConvertToString(MainWindow.KaraokeDisplay.StandbyFillColor);
        }

        private void TextBoxSlF_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.SleepFillColor = ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.SleepFillColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            Properties.Settings.Default.SleepFill = bc.ConvertToString(MainWindow.KaraokeDisplay.SleepFillColor);
        }

        private void TextBoxAS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.ActiveStrokeColor = ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.ActiveStrokeColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            Properties.Settings.Default.ActiveStroke = bc.ConvertToString(MainWindow.KaraokeDisplay.ActiveStrokeColor);
        }

        private void TextBoxStS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.StandbyStrokeColor = ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.StandbyStrokeColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            Properties.Settings.Default.StandbyStroke = bc.ConvertToString(MainWindow.KaraokeDisplay.StandbyStrokeColor);
        }

        private void TextBoxSlS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.SleepStrokeColor = ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.SleepStrokeColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            Properties.Settings.Default.SleepStroke = bc.ConvertToString(MainWindow.KaraokeDisplay.SleepStrokeColor);
        }

        private void TextBoxAB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.ActiveBackColor = ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.ActiveBackColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            Properties.Settings.Default.ActiveBack = bc.ConvertToString(MainWindow.KaraokeDisplay.ActiveBackColor);

        }

        private void TextBoxWB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.Background = ColorTextChanged((TextBox)sender) ?? MainWindow.Background;
            Properties.Settings.Default.WindowBack = bc.ConvertToString(MainWindow.Background);
        }

        private void RadioButtonLeft_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.TextAlignment = TextAlignment.Left;
            Properties.Settings.Default.TextAlignment = TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertToString(TextAlignment.Left);
            MainWindow.KaraokeDisplay.UpdateAll();
        }

        private void RadioButtonCenter_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.TextAlignment = TextAlignment.Center;
            Properties.Settings.Default.TextAlignment = TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertToString(TextAlignment.Center);
            MainWindow.KaraokeDisplay.UpdateAll();
        }

        private void RadioButtonRight_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.TextAlignment = TextAlignment.Right;
            Properties.Settings.Default.TextAlignment = TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertToString(TextAlignment.Right);
            MainWindow.KaraokeDisplay.UpdateAll();
        }

        private void RadioButtonTop_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.KaraokeVerticalAlignment = VerticalAlignment.Top;
            Properties.Settings.Default.VerticalAlignment = TypeDescriptor.GetConverter(typeof(VerticalAlignment)).ConvertToString(VerticalAlignment.Top);
        }

        private void RadioButtonMiddle_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.KaraokeVerticalAlignment = VerticalAlignment.Center;
            Properties.Settings.Default.VerticalAlignment = TypeDescriptor.GetConverter(typeof(VerticalAlignment)).ConvertToString(VerticalAlignment.Center);
        }

        private void RadioButtonBottom_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.KaraokeVerticalAlignment = VerticalAlignment.Bottom;
            Properties.Settings.Default.VerticalAlignment = TypeDescriptor.GetConverter(typeof(VerticalAlignment)).ConvertToString(VerticalAlignment.Bottom);

        }

        private void OffsetLeft_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.LinePadding;
            t.Left = (double)OffsetLeft.Value;
            MainWindow.KaraokeDisplay.LinePadding = t;
            Properties.Settings.Default.OffsetLeft = t.Left;
        }

        private void OffsetRight_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.LinePadding;
            t.Right = (double)OffsetRight.Value;
            MainWindow.KaraokeDisplay.LinePadding = t;
            Properties.Settings.Default.OffsetRight = t.Right;
        }

        private void OffsetVertical_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.VerticalOffsetY = (double)OffsetVertical.Value;
            Properties.Settings.Default.OffsetVertical = MainWindow.KaraokeDisplay.VerticalOffsetY;
        }

        private void Outline_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.StrokeThickness = (double)Outline.Value;
            Properties.Settings.Default.Outline = MainWindow.KaraokeDisplay.StrokeThickness;
        }

        private void LineTop_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.LinePadding;
            t.Top = (double)LineTop.Value;
            MainWindow.KaraokeDisplay.LinePadding = t;
            Properties.Settings.Default.LineTopSpace = t.Top;
            MainWindow.KaraokeDisplay.SetAutoScrollY(MainWindow.KaraokeDisplay.Time);
        }

        private void LineBottom_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.LinePadding;
            t.Bottom = (double)LineBottom.Value;
            MainWindow.KaraokeDisplay.LinePadding = t;
            Properties.Settings.Default.LineBottomSpace = t.Bottom;
            MainWindow.KaraokeDisplay.SetAutoScrollY(MainWindow.KaraokeDisplay.Time);
        }

        private void RubyBottom_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.RubyBottomSpace = (double)RubyBottom.Value;
            Properties.Settings.Default.RubyBottomSpace = MainWindow.KaraokeDisplay.RubyBottomSpace;
            MainWindow.KaraokeDisplay.SetAutoScrollY(MainWindow.KaraokeDisplay.Time);
        }

        private void NoRubyTop_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.NoRubyTopSpace = (double)NoRubyTop.Value;
            Properties.Settings.Default.NoRubySpace = MainWindow.KaraokeDisplay.NoRubyTopSpace;
            MainWindow.KaraokeDisplay.SetAutoScrollY(MainWindow.KaraokeDisplay.Time);
        }

        #endregion Display

        #region Lyrics

        private void InitializeLyrics(MainWindow mainWindow)
        {
            LyricsSerchList.Text = string.Join("\n", mainWindow.LyricsSearcher.SearchList);

            NoLyricsFormat.Text = mainWindow.LyricsSearcher.NoLyricsFormatText;

            IgnoreKaraoke.IsChecked = mainWindow.KaraokeDisplay.IgnoreKaraokeTag;
        }


        private void LyricsSerchList_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;

            MainWindow.LyricsSearcher.SetSearchList(LyricsSerchList.Text);
            Properties.Settings.Default.LyricsSearchList = LyricsSerchList.Text;
        }

        private void NoLyricsFormat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.LyricsSearcher.NoLyricsFormatText = NoLyricsFormat.Text;
            Properties.Settings.Default.NoLyricsFormat = NoLyricsFormat.Text;
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.IgnoreKaraokeTag = (bool)IgnoreKaraoke.IsChecked;
            Properties.Settings.Default.IgnoreKaraoke = MainWindow.KaraokeDisplay.IgnoreKaraokeTag;
        }

        ReplacementInstructions SearcherInstruction;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (SearcherInstruction != null)
            {
                SearcherInstruction.Activate();
                return;
            }

            SearcherInstruction = new(ins =>
            {
                LyricsSerchList.SelectedText = ins;
            });
            SearcherInstruction.Owner = this;
            SearcherInstruction.Closed += (s, e) => { SearcherInstruction = null; };
            SearcherInstruction.Show();
        }

        #endregion Lyrics

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
//            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal);
//            config.FilePath;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


        #region Unsync

        private void InitializeUnsync(MainWindow mainWindow)
        {
            UnsyncFontSelect.Content = $"{TypefaceString(mainWindow.KaraokeDisplay.UnsyncTypeface)} {mainWindow.KaraokeDisplay.UnsyncFontSize}";
            UnsyncOutline.Value = (decimal)mainWindow.KaraokeDisplay.UnsyncStrokeThickness;



            UnsyncFill.Background = mainWindow.KaraokeDisplay.UnsyncFillColor;
            UnsyncFill.Text = mainWindow.KaraokeDisplay.UnsyncFillColor.Color.ToString();
            UnsyncStroke.Background = mainWindow.KaraokeDisplay.UnsyncStrokeColor;
            UnsyncStroke.Text = mainWindow.KaraokeDisplay.UnsyncStrokeColor.Color.ToString();


            switch (mainWindow.KaraokeDisplay.UnsyncTextAlignment)
            {
                case TextAlignment.Left:
                   UnsyncHLeft.IsChecked = true;
                    break;
                case TextAlignment.Center:
                    UnsyncHCenter.IsChecked = true;
                    break;
                case TextAlignment.Right:
                    UnsyncHRight.IsChecked = true;
                    break;
            }

            Thickness t = mainWindow.KaraokeDisplay.UnsyncLinePadding;
            UnsyncOffsetLeft.Value = (decimal)t.Left;
            UnsyncOffsetRight.Value = (decimal)t.Right;
            UnsyncOffsetVertical.Value = (decimal)mainWindow.KaraokeDisplay.UnsyncVerticalOffsetY;

            UnsyncLineTop.Value = (decimal)t.Top;
            UnsyncLineBottom.Value = (decimal)t.Bottom;
            UnsyncRubyBottom.Value = (decimal)mainWindow.KaraokeDisplay.UnsyncRubyBottomSpace;
            UnsyncNoRubyTop.Value = (decimal)mainWindow.KaraokeDisplay.UnsyncNoRubyTopSpace;
        }


        private void UnsyncFontSelect_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FontDialogEx
            {
                SelectedFontFamily = MainWindow.KaraokeDisplay.UnsyncTypeface.FontFamily,
                SelectedFontStyle = MainWindow.KaraokeDisplay.UnsyncTypeface.Style,
                SelectedFontWeight = MainWindow.KaraokeDisplay.UnsyncTypeface.Weight,
                SelectedFontStretch = MainWindow.KaraokeDisplay.UnsyncTypeface.Stretch,
                SelectedFontSize = MainWindow.KaraokeDisplay.UnsyncFontSize,

                Owner = this
            };

            if (dlg.ShowDialog() == true)
            {
                MainWindow.KaraokeDisplay.SetUnsyncFont(new Typeface(dlg.SelectedFontFamily, dlg.SelectedFontStyle, dlg.SelectedFontWeight, dlg.SelectedFontStretch), dlg.SelectedFontSize);

                Properties.Settings.Default.UnsyncFontFamily = dlg.SelectedFontFamily.Source;
                Properties.Settings.Default.UnsyncFontSize = dlg.SelectedFontSize;
                Properties.Settings.Default.UnsyncFontStyle = TypeDescriptor.GetConverter(typeof(FontStyle)).ConvertToString(dlg.SelectedFontStyle);
                Properties.Settings.Default.UnsyncFontWeight = TypeDescriptor.GetConverter(typeof(FontWeight)).ConvertToString(dlg.SelectedFontWeight);
                Properties.Settings.Default.UnsyncFontStretch = TypeDescriptor.GetConverter(typeof(FontStretch)).ConvertToString(dlg.SelectedFontStretch);

                UnsyncFontSelect.Content = $"{TypefaceString(MainWindow.KaraokeDisplay.UnsyncTypeface)} {MainWindow.KaraokeDisplay.UnsyncFontSize}";
            }

        }

        private void UnsyncOutline_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.SetUnsyncThickness((double)UnsyncOutline.Value);
            Properties.Settings.Default.UnsyncOutline = MainWindow.KaraokeDisplay.UnsyncStrokeThickness;
        }
        #endregion Unsync

        private void TextBoxUF_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncFillColor = ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.UnsyncFillColor;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
            Properties.Settings.Default.UnsyncFill = bc.ConvertToString(MainWindow.KaraokeDisplay.UnsyncFillColor);

        }

        private void TextBoxUS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncStrokeColor = ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.UnsyncStrokeColor;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
            Properties.Settings.Default.UnsyncStroke = bc.ConvertToString(MainWindow.KaraokeDisplay.UnsyncStrokeColor);
        }

        private void UnsyncHLeft_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncTextAlignment = TextAlignment.Left;
            Properties.Settings.Default.UnsyncTextAlignment = TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertToString(TextAlignment.Left);
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncHCenter_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncTextAlignment = TextAlignment.Center;
            Properties.Settings.Default.UnsyncTextAlignment = TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertToString(TextAlignment.Center);
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncHRight_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncTextAlignment = TextAlignment.Right;
            Properties.Settings.Default.UnsyncTextAlignment = TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertToString(TextAlignment.Right);
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncOffsetLeft_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.UnsyncLinePadding;
            t.Left = (double)UnsyncOffsetLeft.Value;
            MainWindow.KaraokeDisplay.UnsyncLinePadding = t;
            Properties.Settings.Default.UnsyncOffsetLeft = t.Left;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncOffsetVertical_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncVerticalOffsetY = (double)UnsyncOffsetVertical.Value;
            Properties.Settings.Default.UnsyncOffsetVertical = MainWindow.KaraokeDisplay.UnsyncVerticalOffsetY;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncOffsetRight_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.UnsyncLinePadding;
            t.Right = (double)UnsyncOffsetRight.Value;
            MainWindow.KaraokeDisplay.UnsyncLinePadding = t;
            Properties.Settings.Default.UnsyncOffsetRight = t.Right;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncLineTop_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.UnsyncLinePadding;
            t.Top = (double)UnsyncLineTop.Value;
            MainWindow.KaraokeDisplay.UnsyncLinePadding = t;
            Properties.Settings.Default.UnsyncLineTopSpace = t.Top;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncLineBottom_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.UnsyncLinePadding;
            t.Bottom = (double)UnsyncLineBottom.Value;
            MainWindow.KaraokeDisplay.UnsyncLinePadding = t;
            Properties.Settings.Default.UnsyncLineBottomSpace = t.Bottom;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncRubyBottom_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncRubyBottomSpace = (double)UnsyncRubyBottom.Value;
            Properties.Settings.Default.UnsyncRubyBottomSpace = MainWindow.KaraokeDisplay.UnsyncRubyBottomSpace;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncNoRubyTop_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncNoRubyTopSpace = (double)UnsyncNoRubyTop.Value;
            Properties.Settings.Default.UnsyncNoRubySpace = MainWindow.KaraokeDisplay.UnsyncNoRubyTopSpace;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

    }
}
