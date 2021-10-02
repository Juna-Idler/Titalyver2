using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

using emanual.Wpf.Dialogs;


namespace Titalyver2
{
    /// <summary>
    /// UnsyncSettings.xaml の相互作用ロジック
    /// </summary>
    public partial class UnsyncSettings : UserControl
    {
        private readonly MainWindow MainWindow;

        public UnsyncSettings(MainWindow mainWindow)
        {
            InitializeComponent();

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

            CheckBox_UnsyncAutoScroll.IsChecked = mainWindow.KaraokeDisplay.UnsyncAutoScroll;
            AutoScrollParams.IsEnabled = mainWindow.KaraokeDisplay.UnsyncAutoScroll;

            UnsyncIntro.Value = (decimal)mainWindow.KaraokeDisplay.UnsyncIntro;
            UnsyncOutro.Value = (decimal)mainWindow.KaraokeDisplay.UnsyncOutro;

            MainWindow = mainWindow;
        }

        private string TypefaceString(Typeface typeface)
        {
            string family = typeface.FontFamily.FamilyNames.ContainsKey(Language) ? typeface.FontFamily.FamilyNames[Language] : typeface.FontFamily.Source;
            return family + " " + typeface.Style.ToString() + " " + typeface.Weight.ToString() + " " + typeface.Stretch.ToString();
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

                Owner = Window.GetWindow(this)
            };

            if (dlg.ShowDialog() == true)
            {
                MainWindow.KaraokeDisplay.SetUnsyncFont(new Typeface(dlg.SelectedFontFamily, dlg.SelectedFontStyle, dlg.SelectedFontWeight, dlg.SelectedFontStretch), dlg.SelectedFontSize);

                SettingsStorage.Default.UnsyncFontFamily = dlg.SelectedFontFamily.Source;
                SettingsStorage.Default.UnsyncFontSize = dlg.SelectedFontSize;
                SettingsStorage.Default.UnsyncFontStyle = TypeDescriptor.GetConverter(typeof(FontStyle)).ConvertToString(dlg.SelectedFontStyle);
                SettingsStorage.Default.UnsyncFontWeight = TypeDescriptor.GetConverter(typeof(FontWeight)).ConvertToString(dlg.SelectedFontWeight);
                SettingsStorage.Default.UnsyncFontStretch = TypeDescriptor.GetConverter(typeof(FontStretch)).ConvertToString(dlg.SelectedFontStretch);

                UnsyncFontSelect.Content = $"{TypefaceString(MainWindow.KaraokeDisplay.UnsyncTypeface)} {MainWindow.KaraokeDisplay.UnsyncFontSize}";
            }

        }

        private void UnsyncOutline_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.SetUnsyncThickness((double)UnsyncOutline.Value);
            SettingsStorage.Default.UnsyncOutline = MainWindow.KaraokeDisplay.UnsyncStrokeThickness;
        }

        private void TextBoxUF_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncFillColor = SettingsWindow.ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.UnsyncFillColor;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
            SettingsStorage.Default.UnsyncFill = SettingsWindow.bc.ConvertToString(MainWindow.KaraokeDisplay.UnsyncFillColor);

        }

        private void TextBoxUS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncStrokeColor = SettingsWindow.ColorTextChanged((TextBox)sender) ?? MainWindow.KaraokeDisplay.UnsyncStrokeColor;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
            SettingsStorage.Default.UnsyncStroke = SettingsWindow.bc.ConvertToString(MainWindow.KaraokeDisplay.UnsyncStrokeColor);
        }


        private void UnsyncHLeft_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncTextAlignment = TextAlignment.Left;
            SettingsStorage.Default.UnsyncTextAlignment = TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertToString(TextAlignment.Left);
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncHCenter_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncTextAlignment = TextAlignment.Center;
            SettingsStorage.Default.UnsyncTextAlignment = TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertToString(TextAlignment.Center);
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncHRight_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncTextAlignment = TextAlignment.Right;
            SettingsStorage.Default.UnsyncTextAlignment = TypeDescriptor.GetConverter(typeof(TextAlignment)).ConvertToString(TextAlignment.Right);
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncOffsetLeft_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.UnsyncLinePadding;
            t.Left = (double)UnsyncOffsetLeft.Value;
            MainWindow.KaraokeDisplay.UnsyncLinePadding = t;
            SettingsStorage.Default.UnsyncOffsetLeft = t.Left;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncOffsetVertical_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncVerticalOffsetY = (double)UnsyncOffsetVertical.Value;
            SettingsStorage.Default.UnsyncOffsetVertical = MainWindow.KaraokeDisplay.UnsyncVerticalOffsetY;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncOffsetRight_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.UnsyncLinePadding;
            t.Right = (double)UnsyncOffsetRight.Value;
            MainWindow.KaraokeDisplay.UnsyncLinePadding = t;
            SettingsStorage.Default.UnsyncOffsetRight = t.Right;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncLineTop_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.UnsyncLinePadding;
            t.Top = (double)UnsyncLineTop.Value;
            MainWindow.KaraokeDisplay.UnsyncLinePadding = t;
            SettingsStorage.Default.UnsyncLineTopSpace = t.Top;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncLineBottom_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.UnsyncLinePadding;
            t.Bottom = (double)UnsyncLineBottom.Value;
            MainWindow.KaraokeDisplay.UnsyncLinePadding = t;
            SettingsStorage.Default.UnsyncLineBottomSpace = t.Bottom;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncRubyBottom_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncRubyBottomSpace = (double)UnsyncRubyBottom.Value;
            SettingsStorage.Default.UnsyncRubyBottomSpace = MainWindow.KaraokeDisplay.UnsyncRubyBottomSpace;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void UnsyncNoRubyTop_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncNoRubyTopSpace = (double)UnsyncNoRubyTop.Value;
            SettingsStorage.Default.UnsyncNoRubySpace = MainWindow.KaraokeDisplay.UnsyncNoRubyTopSpace;
            MainWindow.KaraokeDisplay.ResetUnsyncProp();
        }

        private void CheckBox_UnsyncAutoScroll_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncAutoScroll = CheckBox_UnsyncAutoScroll.IsChecked == true;
            SettingsStorage.Default.UnsyncAutoScroll = MainWindow.KaraokeDisplay.UnsyncAutoScroll;

            AutoScrollParams.IsEnabled = MainWindow.KaraokeDisplay.UnsyncAutoScroll;
        }

        private void UnsyncIntro_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncIntro = (double)UnsyncIntro.Value;
            SettingsStorage.Default.UnsyncIntro = MainWindow.KaraokeDisplay.UnsyncIntro;
        }

        private void UnsyncOutro_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.UnsyncOutro = (double)UnsyncOutro.Value;
            SettingsStorage.Default.UnsyncOutro = MainWindow.KaraokeDisplay.UnsyncOutro;
        }
    }
}
