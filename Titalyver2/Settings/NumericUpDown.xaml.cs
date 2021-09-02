using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;


namespace Titalyver2
{
    /// <summary>
    /// NumericUpDown.xaml の相互作用ロジック
    /// 適当に作ったのでまだ穴があります
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        [Description("ボタンの幅"), Category("NumericUpDown"), DefaultValue(16.0)]
        public double ButtonWidth { get => (double)GetValue(ButtonWidthProperty); set => SetValue(ButtonWidthProperty, value); }
        public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register(
            "ButtonWidth", typeof(double), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(16.0,FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

        [Description("ボタンの幅"), Category("NumericUpDown")]
        public Thickness ButtonPadding { get => (Thickness)GetValue(ButtonPaddingProperty); set => SetValue(ButtonPaddingProperty, value); }
        public static readonly DependencyProperty ButtonPaddingProperty = DependencyProperty.Register(
            "ButtonPadding", typeof(Thickness), typeof(NumericUpDown),
            new FrameworkPropertyMetadata( new Thickness(0,0,0,0), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));


        [Description("値"), Category("NumericUpDown"), DefaultValue(0)]
        public decimal Value { get => (decimal)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(decimal), typeof(NumericUpDown), new FrameworkPropertyMetadata(0m));

        [Description("最大値"), Category("NumericUpDown"), DefaultValue(100)]
        public decimal Maximum { get => (decimal)GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum", typeof(decimal), typeof(NumericUpDown), new FrameworkPropertyMetadata(100m));
        [Description("最小値"), Category("NumericUpDown"), DefaultValue(0)]
        public decimal Minimum { get => (decimal)GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum", typeof(decimal), typeof(NumericUpDown), new FrameworkPropertyMetadata(0m));

        [Description("増減値"), Category("NumericUpDown"), DefaultValue(1)]
        public decimal Increment { get => (decimal)GetValue(IncrementProperty); set => SetValue(IncrementProperty, value); }
        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register(
            "Increment", typeof(decimal), typeof(NumericUpDown), new FrameworkPropertyMetadata(1m));

        [Description("文字の配置"), Category("NumericUpDown"), DefaultValue(TextAlignment.Center)]
        public TextAlignment TextAlignment { get => (TextAlignment)GetValue(TextAlignmentProperty); set => SetValue(TextAlignmentProperty, value); }
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
            "TextAlignment", typeof(TextAlignment), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(TextAlignment.Center));


        [Description("値が変化した時"), Category("NumericUpDown")]
        public event EventHandler ValueChanged;

        [Description("リセット値"), Category("NumericUpDown")]
        public decimal ResetValue { get; set; } = 0;

        public NumericUpDown()
        {
            InitializeComponent();
        }

        private void RepeatButtonUp_Click(object sender, RoutedEventArgs e)
        {
            Number.Text = (Value + Increment).ToString();
        }

        private void RepeatButtonDown_Click(object sender, RoutedEventArgs e)
        {
            Number.Text = (Value - Increment).ToString();

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (decimal.TryParse(tb.Text, out decimal d))
            {
                if (d > Maximum || d < Minimum)
                {
                    d = Math.Max(Math.Min(d, Maximum), Minimum);
                    tb.Text = d.ToString();
                    return;
                }
                if (Value != d)
                {
                    Value = d;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                tb.Text = Value.ToString();
            }
        }

        private void userControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                RepeatButtonUp_Click(null, null);
            else if (e.Delta < 0)
                RepeatButtonDown_Click(null, null);
        }

//最初PreviewでないKeyDownでやろうとしたら、TextBoxにフォーカスがあると送ってくれなかった
        private void userControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                RepeatButtonUp_Click(null, null);
            else if (e.Key == Key.Down)
                RepeatButtonDown_Click(null, null);
        }

        private void userControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Number.Text = ResetValue.ToString();
            }
        }
    }
}
