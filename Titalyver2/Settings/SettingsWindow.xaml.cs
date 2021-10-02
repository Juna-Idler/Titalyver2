using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        public static readonly BrushConverter bc = new();
        public static SolidColorBrush ColorTextChanged(TextBox box)
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




        public SettingsWindow(MainWindow mainWindow)
        {
            Owner = mainWindow;
            InitializeComponent();
            Language = Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);


            TabItemDisplay.Content = new DisplaySettings(mainWindow);

            TabItemLyrics.Content = new LyricsSettings(mainWindow);

            TabItemUnsync.Content = new UnsyncSettings(mainWindow);

            TabItemSave.Content = new SaveSettings(mainWindow);

            TabItemOthers.Content = new OthersSettings(mainWindow);

            TabItemManual.Content = new ManualSearchSettings(mainWindow);

        }


        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SettingsStorage.Default.Save();

        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

    }
}
