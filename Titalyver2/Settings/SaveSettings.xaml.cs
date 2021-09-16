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
using System.IO;

namespace Titalyver2
{
    /// <summary>
    /// SaveSettings.xaml の相互作用ロジック
    /// </summary>
    public partial class SaveSettings : UserControl
    {
        private readonly MainWindow MainWindow;

        public SaveSettings(MainWindow mainWindow)
        {
            InitializeComponent();

            CheckBoxAutoSave.IsChecked = mainWindow.AutoSave;
            SavePath.Text = string.Join("\n", mainWindow.LyricsSaver.SaveList);

            switch (mainWindow.LyricsSaver.Extension)
            {
                case LyricsSaver.EnumExtension.DependSync:
                    ExtDepend2.IsChecked = true;
                    break;
                case LyricsSaver.EnumExtension.DependSync3:
                    ExtDepend3.IsChecked = true;
                    break;
                case LyricsSaver.EnumExtension.Lrc:
                    ExtLrc.IsChecked = true;
                    break;
                case LyricsSaver.EnumExtension.Txt:
                    ExtTxt.IsChecked = true;
                    break;
                default:
                    goto case LyricsSaver.EnumExtension.DependSync;
            }

            switch (mainWindow.LyricsSaver.Overwrite)
            {
                case LyricsSaver.EnumOverwrite.Silently:
                    Silently.IsChecked = true;
                    break;
                case LyricsSaver.EnumOverwrite.Dialog:
                    Dialog.IsChecked = true;
                    break;
                case LyricsSaver.EnumOverwrite.Dont:
                    Dont.IsChecked = true;
                    break;
                default:
                    goto case LyricsSaver.EnumOverwrite.Silently;
            }

            MainWindow = mainWindow;
        }

        private void SavePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow == null) return;

            MainWindow.LyricsSaver.SetSaveList(SavePath.Text);
            Properties.Settings.Default.LyricsSearchList = SavePath.Text;
        }

        private void RadioButtonSaveExt_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;

            if (ExtDepend2.IsChecked == true)
            {
                MainWindow.LyricsSaver.Extension = LyricsSaver.EnumExtension.DependSync;
            }
            else if (ExtDepend3.IsChecked == true)
            {
                MainWindow.LyricsSaver.Extension = LyricsSaver.EnumExtension.DependSync3;
            }
            else if (ExtLrc.IsChecked == true)
            {
                MainWindow.LyricsSaver.Extension = LyricsSaver.EnumExtension.Lrc;
            }
            else if (ExtTxt.IsChecked == true)
            {
                MainWindow.LyricsSaver.Extension = LyricsSaver.EnumExtension.Txt;
            }
            Properties.Settings.Default.SaveExtension = (int)MainWindow.LyricsSaver.Extension;

        }

        private void RadioButtonSaveOverwrite_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;

            if (Silently.IsChecked == true)
            {
                MainWindow.LyricsSaver.Overwrite = LyricsSaver.EnumOverwrite.Silently;
            }
            else if (Dialog.IsChecked == true)
            {
                MainWindow.LyricsSaver.Overwrite = LyricsSaver.EnumOverwrite.Dialog;
            }
            else if (Dont.IsChecked == true)
            {
                MainWindow.LyricsSaver.Overwrite = LyricsSaver.EnumOverwrite.Dont;
            }
            Properties.Settings.Default.SaveOverwrite = (int)MainWindow.LyricsSaver.Overwrite;
        }


        private void CheckBoxAutoSave_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.AutoSave = CheckBoxAutoSave.IsChecked == true;
            Properties.Settings.Default.AutoSave = MainWindow.AutoSave;
        }

        ReplacementInstructions SaverInstruction;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SaverInstruction != null)
            {
                SaverInstruction.Activate();
                return;
            }

            SaverInstruction = new(ins =>
            {
                SavePath.SelectedText = ins;
            });
            SaverInstruction.Owner = Window.GetWindow(this);
            SaverInstruction.Closed += (s, e) => { SaverInstruction = null; };
            SaverInstruction.Show();
        }
    }
}
