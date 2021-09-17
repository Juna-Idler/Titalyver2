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

namespace Titalyver2.Settings
{
    /// <summary>
    /// TimeoutReport.xaml の相互作用ロジック
    /// </summary>
    public partial class TimeoutReport : Window
    {
        public TimeoutReport(LyricsSearcherPlugins.TimeoutReport[] timeout_list)
        {
            InitializeComponent();

            List.ItemsSource = timeout_list;
        }
    }
}
