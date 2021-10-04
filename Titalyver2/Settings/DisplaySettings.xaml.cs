using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

using emanual.Wpf.Dialogs;

namespace Titalyver2
{
    /// <summary>
    /// DisplaySettings.xaml の相互作用ロジック
    /// </summary>
    public partial class DisplaySettings : UserControl
    {
        private readonly MainWindow MainWindow;

        public DisplaySettings(MainWindow mainWindow)
        {
            InitializeComponent();


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

            MainWindow = mainWindow;
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

                Owner = Window.GetWindow(this)
            };

            if (dlg.ShowDialog() == true)
            {
                MainWindow.KaraokeDisplay.SetFont(new Typeface(dlg.SelectedFontFamily, dlg.SelectedFontStyle, dlg.SelectedFontWeight, dlg.SelectedFontStretch), dlg.SelectedFontSize);


                SettingsStorage.Default.FontFamily = dlg.SelectedFontFamily.Source;
                SettingsStorage.Default.FontSize = dlg.SelectedFontSize;
                SettingsStorage.Default.FontStyle = TypeDescriptor.GetConverter(typeof(FontStyle)).ConvertToString(dlg.SelectedFontStyle);
                SettingsStorage.Default.FontWeight = TypeDescriptor.GetConverter(typeof(FontWeight)).ConvertToString(dlg.SelectedFontWeight);
                SettingsStorage.Default.FontStretch = TypeDescriptor.GetConverter(typeof(FontStretch)).ConvertToString(dlg.SelectedFontStretch);

                FontSelect.Content = $"{TypefaceString(MainWindow.KaraokeDisplay.Typeface)} {MainWindow.KaraokeDisplay.FontSize}";
            }

        }


        private void TextBoxAF_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.ActiveFillColor = SettingsWindow.ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.ActiveFillColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            SettingsStorage.Default.ActiveFill = SettingsWindow.bc.ConvertToString(MainWindow.KaraokeDisplay.ActiveFillColor);
        }

        private void TextBoxStF_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.StandbyFillColor = SettingsWindow.ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.StandbyFillColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            SettingsStorage.Default.StandbyFill = SettingsWindow.bc.ConvertToString(MainWindow.KaraokeDisplay.StandbyFillColor);
        }

        private void TextBoxSlF_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.SleepFillColor = SettingsWindow.ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.SleepFillColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            SettingsStorage.Default.SleepFill = SettingsWindow.bc.ConvertToString(MainWindow.KaraokeDisplay.SleepFillColor);
        }

        private void TextBoxAS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.ActiveStrokeColor = SettingsWindow.ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.ActiveStrokeColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            SettingsStorage.Default.ActiveStroke = SettingsWindow.bc.ConvertToString(MainWindow.KaraokeDisplay.ActiveStrokeColor);
        }

        private void TextBoxStS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.StandbyStrokeColor = SettingsWindow.ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.StandbyStrokeColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            SettingsStorage.Default.StandbyStroke = SettingsWindow.bc.ConvertToString(MainWindow.KaraokeDisplay.StandbyStrokeColor);
        }

        private void TextBoxSlS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.SleepStrokeColor = SettingsWindow.ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.SleepStrokeColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            SettingsStorage.Default.SleepStroke = SettingsWindow.bc.ConvertToString(MainWindow.KaraokeDisplay.SleepStrokeColor);
        }

        private void TextBoxAB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.ActiveBackColor = SettingsWindow.ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.ActiveBackColor;
            MainWindow.KaraokeDisplay.ResetLineColors();
            SettingsStorage.Default.ActiveBack = SettingsWindow.bc.ConvertToString(MainWindow.KaraokeDisplay.ActiveBackColor);

        }

        private void TextBoxWB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.Background = SettingsWindow.ColorTextChanged((TextBox)sender) ?? MainWindow.Background;
            SettingsStorage.Default.WindowBack = SettingsWindow.bc.ConvertToString(MainWindow.Background);
        }

        private void RadioButtonLeft_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.TextAlignment = TextAlignment.Left;
            SettingsStorage.Default.TextAlignment = TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertToString(TextAlignment.Left);
            MainWindow.KaraokeDisplay.UpdateAll();
        }

        private void RadioButtonCenter_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.TextAlignment = TextAlignment.Center;
            SettingsStorage.Default.TextAlignment = TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertToString(TextAlignment.Center);
            MainWindow.KaraokeDisplay.UpdateAll();
        }

        private void RadioButtonRight_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.TextAlignment = TextAlignment.Right;
            SettingsStorage.Default.TextAlignment = TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertToString(TextAlignment.Right);
            MainWindow.KaraokeDisplay.UpdateAll();
        }

        private void RadioButtonTop_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.KaraokeVerticalAlignment = VerticalAlignment.Top;
            SettingsStorage.Default.VerticalAlignment = TypeDescriptor.GetConverter(typeof(VerticalAlignment)).ConvertToString(VerticalAlignment.Top);
        }

        private void RadioButtonMiddle_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.KaraokeVerticalAlignment = VerticalAlignment.Center;
            SettingsStorage.Default.VerticalAlignment = TypeDescriptor.GetConverter(typeof(VerticalAlignment)).ConvertToString(VerticalAlignment.Center);
        }

        private void RadioButtonBottom_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.KaraokeVerticalAlignment = VerticalAlignment.Bottom;
            SettingsStorage.Default.VerticalAlignment = TypeDescriptor.GetConverter(typeof(VerticalAlignment)).ConvertToString(VerticalAlignment.Bottom);

        }

        private void OffsetLeft_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.LinePadding;
            t.Left = (double)OffsetLeft.Value;
            MainWindow.KaraokeDisplay.LinePadding = t;
            SettingsStorage.Default.OffsetLeft = t.Left;
        }

        private void OffsetRight_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.LinePadding;
            t.Right = (double)OffsetRight.Value;
            MainWindow.KaraokeDisplay.LinePadding = t;
            SettingsStorage.Default.OffsetRight = t.Right;
        }

        private void OffsetVertical_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.VerticalOffsetY = (double)OffsetVertical.Value;
            SettingsStorage.Default.OffsetVertical = MainWindow.KaraokeDisplay.VerticalOffsetY;
        }

        private void Outline_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.StrokeThickness = (double)Outline.Value;
            SettingsStorage.Default.Outline = MainWindow.KaraokeDisplay.StrokeThickness;
        }

        private void LineTop_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.LinePadding;
            t.Top = (double)LineTop.Value;
            MainWindow.KaraokeDisplay.LinePadding = t;
            SettingsStorage.Default.LineTopSpace = t.Top;
            MainWindow.KaraokeDisplay.SetAutoScrollY(MainWindow.KaraokeDisplay.Time);
        }

        private void LineBottom_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.LinePadding;
            t.Bottom = (double)LineBottom.Value;
            MainWindow.KaraokeDisplay.LinePadding = t;
            SettingsStorage.Default.LineBottomSpace = t.Bottom;
            MainWindow.KaraokeDisplay.SetAutoScrollY(MainWindow.KaraokeDisplay.Time);
        }

        private void RubyBottom_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.RubyBottomSpace = (double)RubyBottom.Value;
            SettingsStorage.Default.RubyBottomSpace = MainWindow.KaraokeDisplay.RubyBottomSpace;
            MainWindow.KaraokeDisplay.SetAutoScrollY(MainWindow.KaraokeDisplay.Time);
        }

        private void NoRubyTop_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.NoRubyTopSpace = (double)NoRubyTop.Value;
            SettingsStorage.Default.NoRubySpace = MainWindow.KaraokeDisplay.NoRubyTopSpace;
            MainWindow.KaraokeDisplay.SetAutoScrollY(MainWindow.KaraokeDisplay.Time);
        }


    }
}
