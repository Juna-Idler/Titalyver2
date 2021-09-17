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

            LyricsSerchList.Text = string.Join("\n", mainWindow.LyricsSearcher.SearchList);

            NoLyricsFormat.Text = mainWindow.LyricsSearcher.NoLyricsFormatText;

            IgnoreKaraoke.IsChecked = mainWindow.KaraokeDisplay.IgnoreKaraokeTag;

            PluginTimeout.Value = (decimal)mainWindow.LyricsSearcher.MillisecondsTimeout / 1000;

            MainWindow = mainWindow;
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
            SearcherInstruction.Owner = Window.GetWindow(this);
            SearcherInstruction.Closed += (s, e) => { SearcherInstruction = null; };
            SearcherInstruction.Show();
        }

        private void PluginTimeout_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null) return;

            MainWindow.LyricsSearcher.MillisecondsTimeout = (int)(PluginTimeout.Value * 1000);
            Properties.Settings.Default.PluginTimeout = MainWindow.LyricsSearcher.MillisecondsTimeout;
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
