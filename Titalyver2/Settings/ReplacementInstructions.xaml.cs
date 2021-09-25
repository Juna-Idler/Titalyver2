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
    /// ReplacementInstructions.xaml の相互作用ロジック
    /// </summary>
    public partial class ReplacementInstructions : Window
    {
        public ReplacementInstructions(InsertCallback onInsert)
        {
            InitializeComponent();
            ReplacementList.ItemsSource = Replacements.Keys.ToArray();
            OnInsert = onInsert;
        }

        public delegate void InsertCallback(string replacement);
        public InsertCallback OnInsert;


        private static readonly Dictionary<string, string> Replacements = new()
        {
            { "%title%", "Track title name" },
            { "%artists%", "Artist(s) name" },
            { "%album%", "Album name" },
            { "%directoryname%", "Music file directory" },
            { "%filename%", "Music file name" },
            { "%filename_ext%", "Music file name with extension" },
            { "%path%", "Music file full path" },
            { "%mydocments%", "My Documents folder" },
            { "", "" },
            { "<Player dependent tagname>", "" },
            { "example", "" },
            { "<title><tracktitle><name>", "Track title name" },
            { "<artist>", "Artist(s)" },
            { "<album>", "Album" },
        };

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            object selected = ReplacementList.SelectedItem;
            if (selected != null)
            {
                OnInsert(selected.ToString());

            }
        }
    }
}
