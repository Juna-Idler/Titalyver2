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

namespace Titalyver2
{
    /// <summary>
    /// ManualSearchWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ManualSearchWindow : Window
    {

        private readonly MainWindow MainWindow;
        private readonly ReceiverData Data;
        public ManualSearchWindow(MainWindow mainWindow,ReceiverData data)
        {
            InitializeComponent();

            ListBoxPlugin.ItemsSource = mainWindow.LyricsSearcher.ManualSearchList;
            ListBoxPlugin.SelectedIndex = 0;

            TextBoxTitle.Text = data.Title;
            TextBoxArtists.Text = string.Join(Environment.NewLine, data.Artists);
            TextBoxAlbum.Text = data.Album;
            TextBoxPath.Text = data.FilePath;
            TextBoxParam.Text = "";

            MainWindow = mainWindow;
            Data = data;
        }

        private async void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            Task task = MainWindow.ManualSearchLyrics(ListBoxPlugin.SelectedIndex,
                TextBoxTitle.Text,
                TextBoxArtists.Text.Split(Environment.NewLine),
                TextBoxAlbum.Text,
                TextBoxPath.Text, TextBoxParam.Text, (int)Timeout.Value * 1000,
                CheckBoxAutoSave.IsChecked == true ? Data : null);

            if (CheckBoxKeep.IsChecked == true)
            {
                ButtonSearch.IsEnabled = false;
                await task;
                ButtonSearch.IsEnabled = true;
            }
            else
            {
                Close();
            }
        }
    }
}
