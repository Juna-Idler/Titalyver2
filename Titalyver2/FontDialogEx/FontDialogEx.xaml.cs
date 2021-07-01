/* WPF 対応フォント選択ダイアログボックス

拾い物
俺もWPFに詳しいわけじゃないけど
なんか変なコードが多かったのでめちゃくちゃ書き換えた
Xamlの方はひたすらStackPanelをGridに書き換えまくり

*/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Markup; // XmlLanguage
using System.Windows.Input;
using System.Diagnostics; // Debug
using System.Threading;
using System.Globalization;

namespace emanual.Wpf.Dialogs
{
	public partial class FontDialogEx : Window
    {
		// メンバ変数と初期値

		public string FontLanguageCode
        { get => FontLanguage.IetfLanguageTag; set => FontLanguage = XmlLanguage.GetLanguage(value); }

        public XmlLanguage FontLanguage { get; set; }
		public FontFamily SelectedFontFamily { get; set; }
		public double SelectedFontSize { get; set; }
		public FontStyle SelectedFontStyle { get; set; }
		public FontWeight SelectedFontWeight { get; set; }
		public FontStretch SelectedFontStretch { get; set; }


		public double[] FontSizeArray = new double[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 20, 22, 24, 26, 28, 30, 32, 36, 48, 64, 72, 96 };

		class LanguageListItem
		{
			public string Name { get; set; }
			public string Code { get; set; }

		}

        private static readonly LanguageListItem[] LanguageList = {
				new() { Name = "日本語（ja-jp）",  Code = "ja-jp" },
				new() { Name = "米国英語（en-us）",  Code = "en-us" },
				new() { Name = "中国語（zh-cn）",  Code = "zh-cn" },
				new() { Name = "韓国語（ko-kr）",  Code = "ko-kr" },
				new() { Name = "すべての言語",  Code = "" },
//				new() { Name = "フランス語（fr-FR）",  Code = "fr-FR" },
//				new() { Name = "ドイツ語（de-DE）",  Code = "de-DE" },

			};


		public string SampleText { get; set; } = "サンプル文字列\r\nSample Text";
		//-----------------------------------------------------------------------------------------------
		public FontDialogEx()
		{
			InitializeComponent();
			Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);
            FontLanguage = Language;
			SelectedFontFamily = new FontFamily();
//			this.FontFamily; //なんでもいいから適当に有効な値で初期化
			SelectedFontSize = 20;
		}

		//-----------------------------------------------------------------------------------------------
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			lstFontSize.ItemsSource = FontSizeArray;
			lstFontSize.SelectedIndex = Array.FindIndex(FontSizeArray, m => m == SelectedFontSize || SelectedFontSize < m);
			lstFontSize.ScrollIntoView(lstFontSize.SelectedItem);

			cmbLanguage.ItemsSource = LanguageList;
			cmbLanguage.SelectedIndex = Array.FindIndex(LanguageList, m => XmlLanguage.GetLanguage(m.Code) == this.Language);

			txtSample.Text = SampleText;

			txtFamilyName.Text = SelectedFontFamily.Source;
		}


		//---------------------------------------------------------------------------------------------
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			this.DragMove();
		}

		//---------------------------------------------------------------------------------------------
		private void lstFamilyName_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (lstFamilyName.Items.Count < 1)
				return;


            if (lstFamilyName.SelectedItem is FontNameListItem item)
            {
                txtFamilyName.Text = item.LocalFontName;
                SelectedFontFamily = item.FontFamily;

                this.UpdateTypeFace();
                this.UpdateSampleText();
            }
        }

		//---------------------------------------------------------------------------------------------
		private void lstTypeface_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            if (lstTypeface.SelectedItem is TypefaceListItem item)
            {
                txtTypeface.Text = item.Name; ;
                SelectedFontStretch = item.FontStretch;
                SelectedFontStyle = item.FontStyle;
                SelectedFontWeight = item.FontWeight;

                this.UpdateSampleText();
            }
        }

		//---------------------------------------------------------------------------------------------
		private void lstFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (lstFontSize.Items.Count < 1)
				return;

			SelectedFontSize = (double)lstFontSize.SelectedItem;
			txtFontSize.Text = SelectedFontSize.ToString();
			this.UpdateSampleText();
		}

		//---------------------------------------------------------------------------------------------
		private void cmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbLanguage.Items.Count < 1)
				return;

			LanguageListItem item = cmbLanguage.SelectedItem as LanguageListItem;
			FontLanguage = XmlLanguage.GetLanguage(item.Code);

			this.UpdateFamilyName();
		}

		//---------------------------------------------------------------------------------------------
		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		//---------------------------------------------------------------------------------------------


		class FontNameListItem
        {
			public FontFamily FontFamily { get; set; }  //フォントファミリー
			public string LocalFontName { get; set; }   //フォント名
		}

		// cmbLanguage の選択に伴って、lstFamilyName を更新する
		private void UpdateFamilyName()
		{
			var list = new List<FontNameListItem>();

			// すべての言語のとき
			if (FontLanguage.IetfLanguageTag == "")
			{
				foreach (FontFamily family in Fonts.SystemFontFamilies)
				{
                    list.Add(new() { FontFamily = family, LocalFontName = family.Source });
				}

			}
			else // 特定の言語のとき
			{
				foreach (FontFamily family in Fonts.SystemFontFamilies)
				{
					LanguageSpecificStringDictionary dic = family.FamilyNames;
					if (dic.ContainsKey(FontLanguage))
					{
						list.Add(new() { FontFamily = family, LocalFontName = dic[FontLanguage] });
					}
				}
			}
			lstFamilyName.ItemsSource = list;

			// 指定のフォントを選択状態にする
			int index = list.FindIndex(m => m.FontFamily == SelectedFontFamily);
			lstFamilyName.SelectedIndex = index;
			if (index >= 0)
			{
				txtFamilyName.Text = list[index].LocalFontName;
				lstFamilyName.ScrollIntoView(lstFamilyName.SelectedItem);
			}
		}

		//---------------------------------------------------------------------------------------------

		private class TypefaceListItem
        {
			public string Name { get; set; }
			public FontStyle FontStyle { get; set; }
			public FontStretch FontStretch { get; set; }
			public FontWeight FontWeight { get; set; }
		}


		// lstFamilyName の選択の変更に伴って、lstTypeface を更新する
		private void UpdateTypeFace()
		{
			var list = new List<TypefaceListItem>();
			FontNameListItem name = lstFamilyName.SelectedItem as FontNameListItem;

			foreach (FamilyTypeface typeface in name.FontFamily.FamilyTypefaces)
			{
				if (typeface.AdjustedFaceNames.Count == 0)
					continue;
				KeyValuePair<XmlLanguage, string> pair = typeface.AdjustedFaceNames.First();
				list.Add(new TypefaceListItem() { Name = pair.Value, FontStyle = typeface.Style, FontWeight = typeface.Weight, FontStretch = typeface.Stretch });
			}
			lstTypeface.ItemsSource = list;

			//-------------------------------------------------------
			// 初期値を設定する
			if (lstTypeface.Items.Count > 0)
			{
				lstTypeface.SelectedIndex = 0;
				txtTypeface.Text = ((TypefaceListItem)lstTypeface.SelectedItem).Name;
				lstTypeface.ScrollIntoView(lstTypeface.Items[0]);
			}
		}



		//---------------------------------------------------------------------------------------------
		// txtSampleText のフォントを変更する
		private void UpdateSampleText()
		{
			txtSample.FontFamily = SelectedFontFamily;
			txtSample.FontStyle = SelectedFontStyle;
			txtSample.FontWeight = SelectedFontWeight;
			txtSample.FontStretch = SelectedFontStretch;
			txtSample.FontSize = SelectedFontSize;
		}

		//---------------------------------------------------------------------------------------------


	} // end of FontDialogEx class


} // end of namespace
