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
    /// OthersSettings.xaml の相互作用ロジック
    /// </summary>
    public partial class OthersSettings : UserControl
    {
        private readonly MainWindow MainWindow;

        public OthersSettings(MainWindow mainWindow)
        {
            InitializeComponent();

            CheckBoxSpecify.IsChecked = mainWindow.SpecifyWheelDelta;
            WheelDelta.Value = mainWindow.WheelDelta;

            NoLyricsFormat.Text = mainWindow.LyricsSearcher.NoLyricsFormatText;

            MainWindow = mainWindow;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null)
                return;
            MainWindow.SpecifyWheelDelta = CheckBoxSpecify.IsChecked == true;
            Properties.Settings.Default.SpecifyWheelDelta = MainWindow.SpecifyWheelDelta;
        }

        private void WheelDelta_ValueChanged(object sender, EventArgs e)
        {
            if (MainWindow == null)
                return;
            MainWindow.WheelDelta = (int)WheelDelta.Value;
            Properties.Settings.Default.WheelDelta = MainWindow.WheelDelta;
        }

        private void NoLyricsFormat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;
            MainWindow.LyricsSearcher.NoLyricsFormatText = NoLyricsFormat.Text;
            Properties.Settings.Default.NoLyricsFormat = NoLyricsFormat.Text;
        }

    }
}
