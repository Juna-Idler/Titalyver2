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

using emanual.Wpf.Dialogs;

using System.ComponentModel;


namespace Titalyver2
{
    /// <summary>
    /// Settings.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {

        private MainWindow MainWindow;

        public SettingsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            MainWindow = mainWindow;


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



			}

		}
	}
}
