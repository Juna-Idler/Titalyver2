using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

using System.Windows.Controls;



namespace Titalyver2
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Titalyver2"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Titalyver2;assembly=Titalyver2"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    /// 

    [ContentProperty("Text")]
    public class OutlineText : FrameworkElement
    {
        static OutlineText()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OutlineText), new FrameworkPropertyMetadata(typeof(OutlineText)));
        }

        private FormattedText FormattedText;
        private Geometry TextGeometry;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(OutlineText),
            new FrameworkPropertyMetadata(OnFormattedTextInvalidated));

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
            "TextAlignment", typeof(TextAlignment), typeof(OutlineText),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register(
            "TextDecorations", typeof(TextDecorationCollection), typeof(OutlineText),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(
            "TextTrimming", typeof(TextTrimming), typeof(OutlineText),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(
            "TextWrapping", typeof(TextWrapping), typeof(OutlineText),
            new FrameworkPropertyMetadata(TextWrapping.NoWrap, OnFormattedTextUpdated));

        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill", typeof(Brush), typeof(OutlineText),
            new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke", typeof(Brush), typeof(OutlineText),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(OutlineText),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(
            typeof(OutlineText), new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(
            typeof(OutlineText), new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(
            typeof(OutlineText), new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(
            typeof(OutlineText), new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(
            typeof(OutlineText), new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public OutlineText()
        {
            this.TextDecorations = new TextDecorationCollection();
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)this.GetValue(TextDecorationsProperty); }
            set { this.SetValue(TextDecorationsProperty, value); }
        }

        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(TextTrimmingProperty); }
            set { SetValue(TextTrimmingProperty, value); }
        }

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            EnsureGeometry();

            Pen pen = new(Stroke, StrokeThickness);
            pen.LineJoin = PenLineJoin.Round;

            drawingContext.DrawGeometry(null, pen, TextGeometry);
            drawingContext.DrawGeometry(Fill, null, TextGeometry);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.EnsureFormattedText();

            this.FormattedText.MaxTextWidth = Math.Min(3579139, availableSize.Width);
            this.FormattedText.MaxTextHeight = availableSize.Height;

            return new Size(this.FormattedText.Width, this.FormattedText.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.EnsureFormattedText();

            this.FormattedText.MaxTextWidth = finalSize.Width;
            this.FormattedText.MaxTextHeight = finalSize.Height;

            this.TextGeometry = null;

            return finalSize;
        }

        private static void OnFormattedTextInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var outlinedTextBlock = (OutlineText)dependencyObject;
            outlinedTextBlock.FormattedText = null;
            outlinedTextBlock.TextGeometry = null;

            outlinedTextBlock.InvalidateMeasure();
            outlinedTextBlock.InvalidateVisual();
        }

        private static void OnFormattedTextUpdated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var outlinedTextBlock = (OutlineText)dependencyObject;
            outlinedTextBlock.UpdateFormattedText();
            outlinedTextBlock.TextGeometry = null;

            outlinedTextBlock.InvalidateMeasure();
            outlinedTextBlock.InvalidateVisual();
        }

        private void EnsureFormattedText()
        {
            if (this.FormattedText != null || this.Text == null)
                return;

            FormattedText = new FormattedText(
                Text,
                CultureInfo.CurrentUICulture,
                FlowDirection,
                new Typeface(FontFamily, FontStyle, FontWeight, FontStretches.Normal),
                FontSize,
                Brushes.Black, 96);

            UpdateFormattedText();
        }

        private void UpdateFormattedText()
        {
            if (FormattedText == null)
                return;

            FormattedText.MaxLineCount = TextWrapping == TextWrapping.NoWrap ? 1 : int.MaxValue;
            FormattedText.TextAlignment = TextAlignment;
            FormattedText.Trimming = TextTrimming;

            FormattedText.SetFontSize(FontSize);
            FormattedText.SetFontStyle(FontStyle);
            FormattedText.SetFontWeight(FontWeight);
            FormattedText.SetFontFamily(FontFamily);
            FormattedText.SetFontStretch(FontStretch);
            FormattedText.SetTextDecorations(TextDecorations);
        }

        private void EnsureGeometry()
        {
            if (TextGeometry != null)
                return;

            EnsureFormattedText();
            TextGeometry = FormattedText.BuildGeometry(new Point());
        }
    }
}
