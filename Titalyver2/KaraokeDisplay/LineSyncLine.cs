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
    public class LineSyncLine : FrameworkElement
    {
        private static readonly Typeface system_typeface;
        static LineSyncLine()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineSyncLine), new FrameworkPropertyMetadata(typeof(LineSyncLine)));

            foreach (Typeface typeface in SystemFonts.MessageFontFamily.GetTypefaces())
            {
                system_typeface = typeface;
                break;
            }
        }


        public SolidColorBrush ActiveFillColor { get; set; }
        public SolidColorBrush ActiveStrokeColor { get; set; }

        public SolidColorBrush SleepFillColor { get; set; }
        public SolidColorBrush SleepStrokeColor { get; set; }

        public double StrokeThickness { get; set; }

        public SolidColorBrush ActiveBackColor { get; set; }


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

        public void SetStrokeThickness()
        {
            ActivePen.Thickness = StrokeThickness;
            ActiveRubyPen.Thickness = StrokeThickness / 2;
            SleepPen.Thickness = StrokeThickness;
            SleepRubyPen.Thickness = StrokeThickness / 2;
            FadePen.Thickness = StrokeThickness;
            FadeRubyPen.Thickness = StrokeThickness / 2;
        }
        public void SetStrokeColor()
        {
            ActivePen.Brush = ActiveStrokeColor;
            ActiveRubyPen.Brush = ActiveStrokeColor;
            SleepPen.Brush = SleepStrokeColor;
            SleepRubyPen.Brush = SleepStrokeColor;
        }

        public double Time { get; private set; }

        public void SetTime(double time) { Time = time; }


        public bool RenderSwich { get => (bool)GetValue(RenderSwichProperty); set => SetValue(RenderSwichProperty, value); }
        public static readonly DependencyProperty RenderSwichProperty = DependencyProperty.Register(
            "RenderSwich", typeof(bool), typeof(LineSyncLine), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        public void Update() { RenderSwich = !RenderSwich; }


        [Description("入り時間"), Category("Karaoke Line")]
        public double FadeInTime { get; set; } = 0.5;

        public bool BackColorFade { get; set; } = true;

        public double StartTime { get; private set; }
        public double EndTime { get; private set; }



        private LyricsContainer.Line Line;

        private readonly SolidColorBrush FadeFillBrush = new();
        private readonly SolidColorBrush FadeStrokeBrush = new();
        private readonly SolidColorBrush FadeBackBrush = new();

        private readonly Pen ActivePen;
        private readonly Pen ActiveRubyPen;
        private readonly Pen SleepPen;
        private readonly Pen SleepRubyPen;
        private readonly Pen FadePen;
        private readonly Pen FadeRubyPen;

        private UnbreakableWord[] Words;
        private double[] WordsWidth;
        private double[] AlignmentX;
        private double WordsHeight;



        private enum EnumLastRenderState
        {
            Active,
            Sleep,
            Fade,
        }

        private EnumLastRenderState LastRenderState;


        public LineSyncLine()
        {
            IsHitTestVisible = false;
            Width = 0;
            Height = 0;
            Typeface = system_typeface;
            ActiveFillColor = Brushes.White;
            ActiveStrokeColor = Brushes.Red;
            StrokeThickness = 2;
            SleepFillColor = Brushes.White;
            SleepStrokeColor = Brushes.DarkBlue;
            ActiveBackColor = Brushes.Transparent;

            ActivePen = new Pen(ActiveStrokeColor, StrokeThickness) { LineJoin = PenLineJoin.Round };
            ActiveRubyPen = new Pen(ActiveStrokeColor, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };
            SleepPen = new Pen(SleepStrokeColor, StrokeThickness) { LineJoin = PenLineJoin.Round };
            SleepRubyPen = new Pen(SleepStrokeColor, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };
            FadePen = new Pen(FadeStrokeBrush, StrokeThickness) { LineJoin = PenLineJoin.Round };
            FadeRubyPen = new Pen(FadeStrokeBrush, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };


            SetLyricsLine(new LyricsContainer.Line(""));
        }

        public LineSyncLine(Typeface typeface, double fontSize,
            SolidColorBrush activeFill, SolidColorBrush activeStroke, double strokeTickness,
            SolidColorBrush sleepFillColor, SolidColorBrush sleepStrokeColor, SolidColorBrush activeBackColor,
            Thickness padding, double rubyBottom, double noRubyTop,
            LyricsContainer.Line line, double width,TextAlignment textAlignment)
        {
            IsHitTestVisible = false;
            Width = width;
            Height = 0;
            Typeface = typeface;
            FontSize = fontSize;
            ActiveFillColor = activeFill;
            ActiveStrokeColor = activeStroke;
            StrokeThickness = strokeTickness;
            SleepFillColor = sleepFillColor;
            SleepStrokeColor = sleepStrokeColor;
            ActiveBackColor = activeBackColor;

            ActivePen = new Pen(ActiveStrokeColor, StrokeThickness) { LineJoin = PenLineJoin.Round };
            ActiveRubyPen = new Pen(ActiveStrokeColor, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };
            SleepPen = new Pen(SleepStrokeColor, StrokeThickness) { LineJoin = PenLineJoin.Round };
            SleepRubyPen = new Pen(SleepStrokeColor, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };
            FadePen = new Pen(FadeStrokeBrush,StrokeThickness) { LineJoin = PenLineJoin.Round };
            FadeRubyPen = new Pen(FadeStrokeBrush, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };


            Padding = padding;
            RubyBottomSpace = rubyBottom;
            NoRubyTopSpace = noRubyTop;

            TextAlignment = textAlignment;

            SetLyricsLine(line);
        }


        public void SetLyricsLine(LyricsContainer.Line line)
        {
            if (line.Sync == LyricsContainer.SyncMode.Unsync)
                return;
            StartTime = line.StartTime;
            Line = line;

            List<UnbreakableWord> ubwords = new();
            RubyString remain = null;
            foreach (LyricsContainer.Line.WordWithRuby w in Line.Words)
            {
                if (w.HasRuby)
                {
                    RubyString.Unit[] u = { new RubyString.Unit(w.Word.Text, w.Ruby.Text) };
                    if (remain != null)
                    {
                        if (LineBreakWord.IsLink(remain.Units[^1].BaseText[^1], w.Word.Text[0]))
                        {
                            remain = remain.AddString(new RubyString(u));
                            continue;
                        }
                        ubwords.Add(new UnbreakableWord(remain, FlowDirection.LeftToRight, Typeface, FontSize));
                    }
                    remain = new RubyString(u);
                    continue;
                }

                string[] sepalated = LineBreakWord.SepalateWord(w.Word.Text);
                foreach (string s in sepalated)
                {
                    RubyString.Unit[] u = { new RubyString.Unit(s) };
                    if (remain != null)
                    {
                        if (LineBreakWord.IsLink(remain.Units[^1].BaseText[^1], s[0]))
                        {
                            remain = remain.AddString(new RubyString(u));
                            continue;
                        }
                        ubwords.Add(new UnbreakableWord(remain, FlowDirection.LeftToRight, Typeface, FontSize));
                    }
                    remain = new RubyString(u);
                }
            }
            if (remain != null)
            {
                ubwords.Add(new UnbreakableWord(remain, FlowDirection.LeftToRight, Typeface, FontSize));
            }

            Words = ubwords.ToArray();
            StartTime = Line.StartTime / 1000.0;
            EndTime = Line.EndTime / 1000.0;

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

            for (int i = 1;i < Words.Length;i++)
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
            for (int i = 0;i < WordsWidth.Length;i++)
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

        public bool NeedRender(double time)
        {
            return (StartTime - FadeInTime < time && time < StartTime) || (EndTime - FadeInTime < time && time < EndTime) || LastRenderState == EnumLastRenderState.Fade ||
                   (!((LastRenderState == EnumLastRenderState.Active) && StartTime < time && time < EndTime - FadeInTime) &&
                    !((LastRenderState == EnumLastRenderState.Sleep) && (time < StartTime - FadeInTime || EndTime < time)));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Words == null)
                return;


            if (ActiveBackColor.Color.A > 0)
            {
                Rect rect = new(0, 0, Width, Height);
                if (BackColorFade)
                {
                    if (StartTime < Time && Time < EndTime - FadeInTime)
                    {
                        drawingContext.DrawRectangle(ActiveBackColor, null, rect);
                    }
                    else if (StartTime - FadeInTime < Time && Time < StartTime)
                    {
                        double rate = (Time - (StartTime - FadeInTime)) / FadeInTime;
                        FadeBackBrush.Color = (ActiveBackColor.Color * (float)rate);
                        drawingContext.DrawRectangle(FadeBackBrush, null, rect);

                    }
                    else if (EndTime - FadeInTime < Time && Time < EndTime)
                    {
                        double duration = Math.Min(FadeInTime, EndTime - StartTime);
                        double rate = (Time - (EndTime - duration)) / duration;
                        Color c = ActiveBackColor.Color;
                        c.A = (byte)(c.A * (1 - rate));
                        FadeBackBrush.Color = c;
                        drawingContext.DrawRectangle(FadeBackBrush, null, rect);
                    }
                }
                else
                {
                    if (StartTime < Time && Time < EndTime - FadeInTime)
                    {
                        drawingContext.DrawRectangle(ActiveBackColor, null, rect);
                    }
                    else if (Time < StartTime && Time > StartTime - FadeInTime)
                    {
                        double rate = (Time - (StartTime - FadeInTime)) / FadeInTime;
                        double y = rect.Height * rate;
                        rect.Height = y;
                        drawingContext.DrawRectangle(ActiveBackColor, null, rect);
                    }
                    else if (Time < EndTime && Time > EndTime - FadeInTime)
                    {
                        double rate = (Time - (EndTime - FadeInTime)) / FadeInTime;
                        double y = rect.Height * rate;
                        rect.Height -= y;
                        rect.Y = y;
                        drawingContext.DrawRectangle(ActiveBackColor, null, rect);
                    }
                }
            }

            double offsetY = Padding.Top + (Line.HasRuby ? Typeface.CapsHeight * FontSize / 2 + RubyBottomSpace : NoRubyTopSpace);

            Brush brush = null;
            Pen wordPen = null;
            Pen rubyPen = null;

            if (StartTime < Time && Time < EndTime - FadeInTime)
            {
                brush = ActiveFillColor;
                wordPen = ActivePen;
                rubyPen = ActiveRubyPen;
                LastRenderState = EnumLastRenderState.Active;
            }
            else if (StartTime - FadeInTime < Time && Time < StartTime)
            {
                double rate = (Time - (StartTime - FadeInTime)) / FadeInTime;
                FadeFillBrush.Color = (ActiveFillColor.Color * (float)rate) + (SleepFillColor.Color * (float)(1 - rate));
                FadeStrokeBrush.Color = (ActiveStrokeColor.Color * (float)rate) + (SleepStrokeColor.Color * (float)(1 - rate));
                FadePen.Brush = FadeStrokeBrush;
                FadeRubyPen.Brush = FadeStrokeBrush;
                brush = FadeFillBrush;
                wordPen = FadePen;
                rubyPen = FadeRubyPen;
                LastRenderState = EnumLastRenderState.Fade;
            }
            else if (EndTime - FadeInTime < Time && Time < EndTime)
            {
                double duration = Math.Min(FadeInTime, EndTime - StartTime);
                double rate = (Time - (EndTime - duration)) / duration;
                FadeFillBrush.Color = (SleepFillColor.Color * (float)rate) + (ActiveFillColor.Color * (float)(1 - rate));
                FadeStrokeBrush.Color = (SleepStrokeColor.Color * (float)rate) + (ActiveStrokeColor.Color * (float)(1 - rate));
                FadePen.Brush = FadeStrokeBrush;
                FadeRubyPen.Brush = FadeStrokeBrush;
                brush = FadeFillBrush;
                wordPen = FadePen;
                rubyPen = FadeRubyPen;
                LastRenderState = EnumLastRenderState.Fade;
            }
            else
            {
                brush = SleepFillColor;
                wordPen = SleepPen;
                rubyPen = SleepRubyPen;
                LastRenderState = EnumLastRenderState.Sleep;
            }

            foreach (UnbreakableWord ubw in Words)
            {
                {
                    TranslateTransform tt = new (AlignmentX[ubw.LineNumber] + ubw.OffsetX,
                                                    offsetY + WordsHeight * ubw.LineNumber);
                    drawingContext.PushTransform(tt);
                    drawingContext.DrawGeometry(null, wordPen, ubw.WordGlyphs);
                    drawingContext.DrawGeometry(brush, null, ubw.WordGlyphs);

                    drawingContext.Pop();
                }
                if (ubw.HasRuby)
                {
                    TranslateTransform tt = new (AlignmentX[ubw.LineNumber] + ubw.OffsetX,
                                                    Padding.Top + WordsHeight * ubw.LineNumber);
                    drawingContext.PushTransform(tt);
                    drawingContext.DrawGeometry(null, rubyPen, ubw.RubyGlyphs);
                    drawingContext.DrawGeometry(brush, null, ubw.RubyGlyphs);
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
