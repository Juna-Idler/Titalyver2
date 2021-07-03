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
                case VerticalAlignment.Top :
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
            OffsetVertical.Value = (decimal)mainWindow.KaraokeDisplay.OffsetY;

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
            var dlg = new emanual.Wpf.Dialogs.FontDialogEx();

            dlg.SelectedFontFamily = MainWindow.KaraokeDisplay.Typeface.FontFamily;
            dlg.SelectedFontStyle = MainWindow.KaraokeDisplay.Typeface.Style;
            dlg.SelectedFontWeight = MainWindow.KaraokeDisplay.Typeface.Weight;
            dlg.SelectedFontStretch = MainWindow.KaraokeDisplay.Typeface.Stretch;
            dlg.SelectedFontSize = MainWindow.KaraokeDisplay.FontSize;


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
                box.Foreground = m < 128 ? Brushes.White : Brushes.Black;
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

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
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

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
            MainWindow.KaraokeDisplay.OffsetY = (double)OffsetVertical.Value;
            Properties.Settings.Default.OffsetVertical = MainWindow.KaraokeDisplay.OffsetY;
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
        }

        private void LineBottom_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            Thickness t = MainWindow.KaraokeDisplay.LinePadding;
            t.Bottom = (double)LineBottom.Value;
            MainWindow.KaraokeDisplay.LinePadding = t;
            Properties.Settings.Default.LineBottomSpace = t.Bottom;
        }

        private void RubyBottom_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.RubyBottomSpace = (double)RubyBottom.Value;
            Properties.Settings.Default.RubyBottomSpace = MainWindow.KaraokeDisplay.RubyBottomSpace;
        }

        private void NoRubyTop_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.NoRubyTopSpace = (double)NoRubyTop.Value;
            Properties.Settings.Default.NoRubySpace = MainWindow.KaraokeDisplay.NoRubyTopSpace;
        }
    }
}
