using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Globalization;

namespace Titalyver2
{
    /// <summary>
    /// </summary>
    public class UnsyncLine : FrameworkElement
    {
        private static readonly Typeface system_typeface;
        static UnsyncLine()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UnsyncLine), new FrameworkPropertyMetadata(typeof(UnsyncLine)));

            foreach (Typeface typeface in SystemFonts.MessageFontFamily.GetTypefaces())
            {
                system_typeface = typeface;
                break;
            }
        }


        public SolidColorBrush TextFillColor { get; set; }
        public SolidColorBrush TextStrokeColor { get; set; }

        public double StrokeThickness { get; private set; }


        public Typeface Typeface { get; set; }
        public double FontSize { get; set; }

        public TextAlignment TextAlignment { get; set; }

        public Thickness Padding { get; set; }

        public double RubyBottomSpace { get; set; }

        public double NoRubyTopSpace { get; set; }

        public void UpdateHeight()
        {
            Height = WordsHeight + Padding.Top + Padding.Bottom;
        }

        public void SetStrokeThickness(double thickness)
        {
            StrokeThickness = thickness;
            TextPen.Thickness = StrokeThickness;
            TextRubyPen.Thickness = StrokeThickness / 2;
        }
        public void SetStrokeColor()
        {
            TextPen.Brush = TextStrokeColor;
            TextRubyPen.Brush = TextStrokeColor;
        }



        public bool RenderSwich { get => (bool)GetValue(RenderSwichProperty); set => SetValue(RenderSwichProperty, value); }
        public static readonly DependencyProperty RenderSwichProperty = DependencyProperty.Register(
            "RenderSwich", typeof(bool), typeof(UnsyncLine), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        public void Update() { RenderSwich = !RenderSwich; }





        private RubyString Line;

        private readonly Pen TextPen;
        private readonly Pen TextRubyPen;

        private UnbreakableWord[] Words;
        private double[] WordsWidth;
        private double[] AlignmentX;
        private double WordsHeight;



        public UnsyncLine()
        {
            IsHitTestVisible = false;
            Width = 0;
            Height = 0;
            Typeface = system_typeface;
            StrokeThickness = 2;
            TextFillColor = Brushes.White;
            TextStrokeColor = Brushes.DarkBlue;

            TextPen = new Pen(TextStrokeColor, StrokeThickness) { LineJoin = PenLineJoin.Round };
            TextRubyPen = new Pen(TextStrokeColor, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };

            SetLyricsLine(new RubyString(""));
        }

        public UnsyncLine(Typeface typeface, double fontSize,
            SolidColorBrush textFillColor, SolidColorBrush textStrokeColor, double strokeTickness,
            Thickness padding, double rubyBottom, double noRubyTop,
            RubyString line, double width, TextAlignment textAlignment)
        {
            IsHitTestVisible = false;
            Width = width;
            Typeface = typeface;
            FontSize = fontSize;
            StrokeThickness = strokeTickness;
            TextFillColor = textFillColor;
            TextStrokeColor = textStrokeColor;

            TextPen = new Pen(TextStrokeColor, StrokeThickness) { LineJoin = PenLineJoin.Round };
            TextRubyPen = new Pen(TextStrokeColor, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };


            Padding = padding;
            RubyBottomSpace = rubyBottom;
            NoRubyTopSpace = noRubyTop;

            TextAlignment = textAlignment;

            SetLyricsLine(line);
        }


        public void SetLyricsLine(RubyString line)
        {
            Line = line;

            List<UnbreakableWord> ubwords = new();
            RubyString remain = null;
            foreach (RubyString.Unit u in Line.Units)
            {
                if (u.HasRuby)
                {
                    if (remain != null)
                    {
                        if (LineBreakWord.IsLink(remain.Units[^1].BaseText[^1], u.BaseText[0]))
                        {
                            remain = remain.AddString(new RubyString(new RubyString.Unit[] { u }));
                            continue;
                        }
                        ubwords.Add(new UnbreakableWord(remain, FlowDirection.LeftToRight, Typeface, FontSize));
                    }
                    remain = new RubyString(new RubyString.Unit[] { u });
                    continue;
                }

                string[] sepalated = LineBreakWord.SepalateWord(u.BaseText);
                foreach (string s in sepalated)
                {
                    RubyString.Unit[] su = { new RubyString.Unit(s) };
                    if (remain != null)
                    {
                        if (LineBreakWord.IsLink(remain.Units[^1].BaseText[^1], s[0]))
                        {
                            remain = remain.AddString(new RubyString(su));
                            continue;
                        }
                        ubwords.Add(new UnbreakableWord(remain, FlowDirection.LeftToRight, Typeface, FontSize));
                    }
                    remain = new RubyString(su);
                }
            }
            if (remain != null)
            {
                ubwords.Add(new UnbreakableWord(remain, FlowDirection.LeftToRight, Typeface, FontSize));
            }

            Words = ubwords.ToArray();

            UpdateWordsLayout();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //            double ww = WordsWidth + Padding.Left + Padding.Right;
            //            double width = double.IsNaN(Width) ? ww : Width;
            return new Size(Width, Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }

        //外部からWidthやTextAlignmentが変更された際にも呼ぶ必要がある
        public void UpdateWordsLayout()
        {
            if (Words == null || Words.Length == 0)
            {
                WordsWidth = new double[1] { 0 };
                AlignmentX = new double[1] { 0 };

                WordsHeight = NoRubyTopSpace + 1.25 * FontSize;
                Height = WordsHeight + Padding.Top + Padding.Bottom;
                return;
            }


            Words[0].OffsetX = 0;
            Words[0].LineNumber = 0;

            double x = Words[0].Width;
            int lineNumber = 0;
            double ruby_padding = -Words[0].RubyEndOffsetX;

            List<double> widths = new();

            for (int i = 1; i < Words.Length; i++)
            {
                UnbreakableWord ubw = Words[i];

                if (ruby_padding + ubw.RubyStartOffsetX < 0)
                {
                    x += -(ruby_padding + ubw.RubyStartOffsetX);
                }

                if (x + ubw.Width > Width - (Padding.Left + Padding.Right))
                {
                    widths.Add(x);
                    ubw.LineNumber = ++lineNumber;
                    ubw.OffsetX = 0;
                    x = ubw.Width;
                    ruby_padding = -ubw.RubyEndOffsetX;
                    continue;
                }

                ubw.LineNumber = lineNumber;
                ubw.OffsetX = x;
                x += ubw.Width;
                ruby_padding = ubw.HasRuby ? -ubw.RubyEndOffsetX : ruby_padding + ubw.Width;
            }
            widths.Add(x);
            WordsWidth = widths.ToArray();
            AlignmentX = new double[WordsWidth.Length];
            for (int i = 0; i < WordsWidth.Length; i++)
            {
                switch (TextAlignment)
                {
                    case TextAlignment.Left:
                        AlignmentX[i] = Padding.Left;
                        break;
                    case TextAlignment.Right:
                        AlignmentX[i] = Width - WordsWidth[i] - Padding.Right;
                        break;
                    case TextAlignment.Center:
                    case TextAlignment.Justify:
                        AlignmentX[i] = (Width - WordsWidth[i]) / 2;
                        break;
                }
            }


            WordsHeight = (Line.HasRuby ? Typeface.CapsHeight * FontSize / 2 + RubyBottomSpace : NoRubyTopSpace)
                          + 1.25 * FontSize;//適当な固定倍率値
            Height = (WordsHeight + Padding.Top + Padding.Bottom) * WordsWidth.Length;

        }



        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Words == null)
                return;

            double offsetY = Padding.Top + (Line.HasRuby ? Typeface.CapsHeight * FontSize / 2 + RubyBottomSpace : NoRubyTopSpace);

            foreach (UnbreakableWord ubw in Words)
            {
                {
                    TranslateTransform tt = new(AlignmentX[ubw.LineNumber] + ubw.OffsetX,
                                                    offsetY + WordsHeight * ubw.LineNumber);
                    drawingContext.PushTransform(tt);
                    drawingContext.DrawGeometry(null, TextPen, ubw.WordGlyphs);
                    drawingContext.DrawGeometry(TextFillColor, null, ubw.WordGlyphs);

                    drawingContext.Pop();
                }
                if (ubw.HasRuby)
                {
                    TranslateTransform tt = new(AlignmentX[ubw.LineNumber] + ubw.OffsetX,
                                                    Padding.Top + WordsHeight * ubw.LineNumber);
                    drawingContext.PushTransform(tt);
                    drawingContext.DrawGeometry(null, TextRubyPen, ubw.RubyGlyphs);
                    drawingContext.DrawGeometry(TextFillColor, null, ubw.RubyGlyphs);
                    drawingContext.Pop();
                }
            }

        }

        private class UnbreakableWord
        {
            public Geometry WordGlyphs { get; }
            public double Width { get; }

            public bool HasRuby => RubyGlyphs != null;
            public Geometry RubyGlyphs { get; }
            public double RubyStartOffsetX { get; }
            public double RubyEndOffsetX { get; }


            public double OffsetX { get; set; }
            public int LineNumber { get; set; }

            public UnbreakableWord(RubyString word, FlowDirection direction, Typeface typeface, double fontSize)
            {
                GeometryGroup bgroup = new();
                GeometryGroup rgroup = new();
                double x = 0;
                double ruby_padding = 0;//ルビパディング

                {
                    FormattedText ft = new(word.Units[0].BaseText,
                                            CultureInfo.CurrentUICulture, direction,
                                            typeface, fontSize,
                                            Brushes.Black, 96);
                    double bwidth = ft.WidthIncludingTrailingWhitespace;
                    if (word.Units[0].HasRuby)
                    {
                        FormattedText rft = new(word.Units[0].RubyText,
                                                CultureInfo.CurrentUICulture, direction,
                                                typeface, fontSize / 2,
                                                Brushes.Black, 96);
                        double rwidth = rft.WidthIncludingTrailingWhitespace;
                        RubyStartOffsetX = ruby_padding = (bwidth - rwidth) / 2;
                        rgroup.Children.Add(rft.BuildGeometry(new Point(RubyStartOffsetX, 0)));
                    }
                    else
                    {
                        RubyStartOffsetX = ruby_padding = bwidth;
                    }
                    bgroup.Children.Add(ft.BuildGeometry(new Point(0, 0)));
                    x = bwidth;
                }

                for (int i = 1; i < word.Units.Length; i++)
                {
                    RubyString.Unit u = word.Units[i];
                    FormattedText ft = new(u.BaseText,
                                            CultureInfo.CurrentUICulture, direction,
                                            typeface, fontSize,
                                            Brushes.Black, 96);
                    double bwidth = ft.WidthIncludingTrailingWhitespace;

                    if (u.HasRuby)
                    {
                        FormattedText rft = new(u.RubyText,
                                                CultureInfo.CurrentUICulture, direction,
                                                typeface, fontSize / 2,
                                                Brushes.Black, 96);
                        double rwidth = rft.WidthIncludingTrailingWhitespace;
                        double ruby_offset = (bwidth - rwidth) / 2;
                        if (ruby_padding + ruby_offset < 0)
                        {
                            x += -(ruby_padding + ruby_offset);
                        }
                        ruby_padding = ruby_offset;
                        rgroup.Children.Add(rft.BuildGeometry(new Point(x + ruby_offset, 0)));
                    }
                    else
                    {
                        ruby_padding += bwidth;
                    }
                    bgroup.Children.Add(ft.BuildGeometry(new Point(x, 0)));
                    x += bwidth;
                }
                Width = x;
                RubyEndOffsetX = -ruby_padding;

                bgroup.Freeze();
                WordGlyphs = bgroup;
                if (rgroup.IsEmpty())
                {
                    RubyGlyphs = null;
                }
                else
                {
                    rgroup.Freeze();
                    RubyGlyphs = rgroup;
                }
            }
        }


    }
}
