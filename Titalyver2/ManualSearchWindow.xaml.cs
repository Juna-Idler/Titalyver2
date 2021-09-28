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

        private MainWindow MainWindow;
        public ManualSearchWindow(MainWindow mainWindow,string title, string[] artists, string album, string path, string param)
        {
            InitializeComponent();

            ListBoxPlugin.ItemsSource = mainWindow.LyricsSearcher.ManualSearchList;
            ListBoxPlugin.SelectedIndex = 0;

            TextBoxTitle.Text = title;
            TextBoxArtists.Text = string.Join(Environment.NewLine, artists);
            TextBoxAlbum.Text = album;
            TextBoxPath.Text = path;
            TextBoxParam.Text = param;


            MainWindow = mainWindow;
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            Task task = MainWindow.ManualSearchLyrics(ListBoxPlugin.SelectedIndex,
                TextBoxTitle.Text,
                TextBoxArtists.Text.Split(Environment.NewLine),
                TextBoxAlbum.Text,
                TextBoxPath.Text, TextBoxParam.Text, (int)Timeout.Value);

            Close();
        }
    }
}
