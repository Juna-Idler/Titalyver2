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
    /// ManualSearch.xaml の相互作用ロジック
    /// </summary>
    public partial class ManualSearchSettings : UserControl
    {
        private readonly MainWindow MainWindow;

        public ManualSearchSettings(MainWindow mainWindow)
        {
            InitializeComponent();

            ManualSerchList.Text = string.Join("\n", mainWindow.LyricsSearcher.ManualSearchList);
            MainWindow = mainWindow;

        }

        private void ManualSerchList_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;

            MainWindow.LyricsSearcher.SetManualSearchList(ManualSerchList.Text);
            SettingsStorage.Default.ManualSearchList = MainWindow.LyricsSearcher.ManualSearchList;
        }
    }
}
