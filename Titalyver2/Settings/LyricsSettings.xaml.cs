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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Titalyver2
{
    /// <summary>
    /// LyricsSettings.xaml の相互作用ロジック
    /// </summary>
    public partial class LyricsSettings : UserControl
    {
        private readonly MainWindow MainWindow;


        public LyricsSettings(MainWindow mainWindow)
        {
            InitializeComponent();

            LyricsSearchList.Text = string.Join("\n", mainWindow.LyricsSearcher.SearchList);


            IgnoreKaraoke.IsChecked = mainWindow.KaraokeDisplay.IgnoreKaraokeTag;

            PluginTimeout.Value = (decimal)mainWindow.LyricsSearcher.MillisecondsTimeout / 1000;

            MainWindow = mainWindow;
        }


        private void LyricsSearchList_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;

            MainWindow.LyricsSearcher.SetSearchList(LyricsSearchList.Text);
            SettingsStorage.Default.LyricsSearchList = MainWindow.LyricsSearcher.SearchList;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.KaraokeDisplay.IgnoreKaraokeTag = (bool)IgnoreKaraoke.IsChecked;
            SettingsStorage.Default.IgnoreKaraoke = MainWindow.KaraokeDisplay.IgnoreKaraokeTag;
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
                LyricsSearchList.SelectedText = ins;
            });
            SearcherInstruction.Owner = Window.GetWindow(this);
            SearcherInstruction.Closed += (s, e) => { SearcherInstruction = null; };
            SearcherInstruction.Show();
        }

        private void PluginTimeout_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;

            MainWindow.LyricsSearcher.MillisecondsTimeout = (int)(PluginTimeout.Value * 1000);
            SettingsStorage.Default.PluginTimeout = MainWindow.LyricsSearcher.MillisecondsTimeout;
        }

        Settings.TimeoutReport ReportWindow;
        private void TimeoutReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportWindow != null)
            {
                ReportWindow.Close();
            }
            ReportWindow = new(MainWindow.LyricsSearcher.GetTimeoutList());
            ReportWindow.Owner = Window.GetWindow(this);
            ReportWindow.Closed += (s, e) => { ReportWindow = null; };
            ReportWindow.Show();
        }
    }
}
