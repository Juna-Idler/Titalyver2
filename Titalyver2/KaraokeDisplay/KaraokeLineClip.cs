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
    public class KaraokeLineClip : FrameworkElement
    {
        private static readonly Typeface system_typeface;
        static KaraokeLineClip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KaraokeLineClip), new FrameworkPropertyMetadata(typeof(KaraokeLineClip)));

            foreach (Typeface typeface in SystemFonts.MessageFontFamily.GetTypefaces())
            {
                system_typeface = typeface;
                break;
            }
        }


        public SolidColorBrush ActiveFillColor { get ; set ; }
        public SolidColorBrush ActiveStrokeColor { get ; set; }

        public SolidColorBrush StandbyFillColor { get; set ; }
        public SolidColorBrush StandbyStrokeColor { get ; set; }

        public SolidColorBrush SleepFillColor { get ; set ; }
        public SolidColorBrush SleepStrokeColor { get ; set ; }

        public double StrokeThickness { get; set ; }

        public SolidColorBrush ActiveBackColor { get ; set; }


        public Typeface Typeface { get; set; }
        public double FontSize { get ; set ; }

        public TextAlignment TextAlignment { get; set; }

        public Thickness Padding { get ; set ; }

        public double RubyBottomSpace { get; set; }

        public double NoRubyTopSpace { get; set; }

        public void UpdateHeight()
        {
            Height = WordsHeight + Padding.Top + Padding.Bottom + (Line.HasRuby ? 0 : NoRubyTopSpace);
        }

        public void SetStrokeThickness()
        {
            ActivePen.Thickness = StrokeThickness;
            ActiveRubyPen.Thickness = StrokeThickness / 2;
            StandbyPen.Thickness = StrokeThickness;
            StandbyRubyPen.Thickness = StrokeThickness / 2;
            SleepPen.Thickness = StrokeThickness;
            SleepRubyPen.Thickness = StrokeThickness / 2;
            FadePen.Thickness = StrokeThickness;
            FadeRubyPen.Thickness = StrokeThickness / 2;
        }

    public double Time { get; private set; }

        public void SetTime(double time) { Time = time; }


        public bool RenderSwich { get => (bool)GetValue(RenderSwichProperty); set => SetValue(RenderSwichProperty, value); }
        public static readonly DependencyProperty RenderSwichProperty = DependencyProperty.Register(
            "RenderSwich", typeof(bool), typeof(KaraokeLineClip), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        public void Update() { RenderSwich = !RenderSwich; }


        [Description("入り時間"), Category("Karaoke Line")]
        public double FadeInTime { get; set; } = 0.5;
        [Description("終わった後の余韻"), Category("Karaoke Line")]
        public double FadeOutTime { get; set; } = 0.25;

        public bool BackColorFade { get; set; } = true;

        public double StartTime { get; private set; }
        public double EndTime { get; private set; }



        private LyricsContainer.Line Line;

        private readonly SolidColorBrush FadeFillBrush = new();
        private readonly SolidColorBrush FadeStrokeBrush = new();
        private readonly SolidColorBrush FadeBackBrush = new();

        private Pen ActivePen;
        private Pen ActiveRubyPen;
        private Pen StandbyPen;
        private Pen StandbyRubyPen;
        private Pen SleepPen;
        private Pen SleepRubyPen;
        private Pen FadePen;
        private Pen FadeRubyPen;

        private KaraokeWord[] Words;
        private KaraokeWord[] Rubys;
        private double WordsWidth;
        private double WordsHeight;


        private bool IsLastRenderOnSleep;
        private double LastRenderTime;


        public KaraokeLineClip()
        {
            Width = 0;
            Height = 0;
            Typeface = system_typeface;
            ActiveFillColor = Brushes.White;
            ActiveStrokeColor = Brushes.Red;
            StandbyFillColor = Brushes.White;
            StandbyStrokeColor = Brushes.Blue;
            StrokeThickness = 2;
            SleepFillColor = Brushes.White;
            SleepStrokeColor = Brushes.DarkBlue;
            ActiveBackColor = Brushes.Transparent;

            ActivePen = new Pen(ActiveStrokeColor, StrokeThickness) { LineJoin = PenLineJoin.Round };
            ActiveRubyPen = new Pen(ActiveStrokeColor, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };
            StandbyPen = new Pen(StandbyStrokeColor, StrokeThickness) { LineJoin = PenLineJoin.Round };
            StandbyRubyPen = new Pen(StandbyStrokeColor, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };
            SleepPen = new Pen(SleepStrokeColor, StrokeThickness) { LineJoin = PenLineJoin.Round };
            SleepRubyPen = new Pen(SleepStrokeColor, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };
            FadePen = new Pen(FadeStrokeBrush, StrokeThickness) { LineJoin = PenLineJoin.Round };
            FadeRubyPen = new Pen(FadeStrokeBrush, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };


            SetLyricsLine(new LyricsContainer.Line(""));
        }

        public KaraokeLineClip(Typeface typeface, double fontSize, SolidColorBrush activeFill,SolidColorBrush activeStroke,
            SolidColorBrush standbyFill, SolidColorBrush standbyStroke, double strokeTickness,
            SolidColorBrush sleepFillColor,SolidColorBrush sleepStrokeColor,  SolidColorBrush activeBackColor,
            Thickness padding,double rubyBottom,double noRubyTop,
            LyricsContainer.Line line,double width)
        {
            Width = width;
            Typeface = typeface;
            FontSize = fontSize;
            ActiveFillColor = activeFill;
            ActiveStrokeColor = activeStroke;
            StandbyFillColor = standbyFill;
            StandbyStrokeColor = standbyStroke;
            StrokeThickness = strokeTickness;
            SleepFillColor = sleepFillColor;
            SleepStrokeColor = sleepStrokeColor;
            ActiveBackColor = activeBackColor;

            ActivePen = new Pen(ActiveStrokeColor, StrokeThickness) { LineJoin = PenLineJoin.Round };
            ActiveRubyPen = new Pen(ActiveStrokeColor, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };
            StandbyPen = new Pen(StandbyStrokeColor, StrokeThickness) { LineJoin = PenLineJoin.Round };
            StandbyRubyPen = new Pen(StandbyStrokeColor, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };
            SleepPen = new Pen(SleepStrokeColor, StrokeThickness) { LineJoin = PenLineJoin.Round };
            SleepRubyPen = new Pen(SleepStrokeColor, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };
            FadePen = new Pen(FadeStrokeBrush,StrokeThickness) { LineJoin = PenLineJoin.Round };
            FadeRubyPen = new Pen(FadeStrokeBrush, StrokeThickness / 2) { LineJoin = PenLineJoin.Round };


            Padding = padding;
            RubyBottomSpace = rubyBottom;
            NoRubyTopSpace = noRubyTop;

            SetLyricsLine(line);
        }


        public void SetLyricsLine(LyricsContainer.Line line)
        {
            Line = line;
            _ = Line.Complement();
            MakeWords();
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



        public void MakeWords()
        {
            if (Line == null || Typeface == null || FontSize == 0)
                return;
            double x = 0;
            double ruby_x = 100;//ルビパディング（初期化値は一文字目の）
            double y = Line.HasRuby ? Typeface.CapsHeight * FontSize / 2 + RubyBottomSpace : 0;
            List <KaraokeWord> words = new(Line.Words.Length);
            List<KaraokeWord> rubys = new(Line.Words.Length);
            foreach (LyricsContainer.Line.WordWithRuby wwr in Line.Words)
            {
                KaraokeWord word = new(wwr.Word, FlowDirection, Typeface, FontSize);
                if (wwr.HasRuby)
                {
                    KaraokeWord ruby = new(wwr.Ruby, FlowDirection, Typeface, FontSize / 2, true);

                    if (ruby_x + (word.Width - ruby.Width) / 2 < 0)
                    {
                        x += -ruby_x - ((word.Width - ruby.Width) / 2);
                    }
                    ruby_x = (word.Width - ruby.Width) / 2;

                    ruby.Glyphs.Transform = new TranslateTransform(x + ruby_x, 0);
                    ruby.Glyphs.Freeze();
                    ruby.OffsetX = x + ruby_x;
                    rubys.Add(ruby);
                }
                else
                {
                    ruby_x += word.Width;
                }
                word.Glyphs.Transform = new TranslateTransform(x, y);
                word.Glyphs.Freeze();
                word.OffsetX = x;
                word.OffsetY = y;
                words.Add(word);
                x += word.Width;
            }
            Words = words.ToArray();
            Rubys = rubys.ToArray();
            StartTime = Line.StartTime / 1000.0;
            EndTime = Line.EndTime / 1000.0;
            WordsWidth = x;
            WordsHeight = y + 1.25 * FontSize;//何故かTypefaceから行の高さを求められないので適当な固定倍率値
            Height = WordsHeight + Padding.Top + Padding.Bottom + (Line.HasRuby ? 0 : NoRubyTopSpace);

        }

        public bool NeedRender(double time)
        {
            return (StartTime < time && time < EndTime) ||
                    (time < StartTime && time > StartTime - FadeInTime) ||
                    (time > EndTime && time < EndTime + FadeOutTime) ||
                    !IsLastRenderOnSleep;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Words == null)
                return;
            IsLastRenderOnSleep = false;
            LastRenderTime = Time;

            double alignment_x = 0;
            double width = double.IsNaN(Width) ? WordsWidth : Width;
            switch (TextAlignment)
            {
                case TextAlignment.Left:
                    alignment_x = Padding.Left;
                    break;
                case TextAlignment.Right:
                    alignment_x = width - WordsWidth - Padding.Right;
                    break;
                case TextAlignment.Center:
                case TextAlignment.Justify:
                    alignment_x = (width - WordsWidth) / 2;
                    break;
            }
            if (ActiveBackColor.Color.A > 0)
            {
                width += double.IsNaN(Width) ? Padding.Left + Padding.Right : 0;
                Rect rect = new(0, 0, width, Height);
                if (BackColorFade)
                {
                    if (StartTime < Time && Time < EndTime - FadeInTime)
                    {
                        drawingContext.DrawRectangle(ActiveBackColor, null, rect);
                    }
                    else if (Time < StartTime && Time > StartTime - FadeInTime)
                    {
                        double rate = (Time - (StartTime - FadeInTime)) / FadeInTime;
                        FadeBackBrush.Color = (ActiveBackColor.Color * (float)rate);
                        drawingContext.DrawRectangle(FadeBackBrush, null, rect);

                    }
                    else if (Time < EndTime && Time > EndTime - FadeInTime)
                    {
                        double rate = (Time - (EndTime - FadeInTime)) / FadeInTime;
                        FadeBackBrush.Color = (ActiveBackColor.Color * (float)(1 - rate));
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

            double offsetY = Padding.Top + (Line.HasRuby ? 0 : NoRubyTopSpace);
            drawingContext.PushTransform(new TranslateTransform(alignment_x, offsetY));

            if (StartTime < Time && Time < EndTime)
            {
                if (Line.Sync == LyricsContainer.SyncMode.Karaoke)
                {

                    foreach (KaraokeWord w in Words)
                    {
                        if (w.StartTime > Time)
                            drawingContext.DrawGeometry(null, StandbyPen, w.Glyphs);
                        else if (w.EndTime < Time)
                            drawingContext.DrawGeometry(null, ActivePen, w.Glyphs);
                        else
                        {
                            w.SetWipClip(Time);
                            drawingContext.PushClip(w.ActiveClip);
                            drawingContext.DrawGeometry(null, ActivePen, w.Glyphs);
                            drawingContext.Pop();
                            drawingContext.PushClip(w.StandbyClip);
                            drawingContext.DrawGeometry(null, StandbyPen, w.Glyphs);
                            drawingContext.Pop();
                        }
                    }
                    foreach (KaraokeWord r in Rubys)
                    {
                        if (r.StartTime > Time)
                            drawingContext.DrawGeometry(null, StandbyRubyPen, r.Glyphs);
                        else if (r.EndTime < Time)
                            drawingContext.DrawGeometry(null, ActiveRubyPen, r.Glyphs);
                        else
                        {
                            r.SetWipClip(Time);
                            drawingContext.PushClip(r.ActiveClip);
                            drawingContext.DrawGeometry(null, ActiveRubyPen, r.Glyphs);
                            drawingContext.Pop();
                            drawingContext.PushClip(r.StandbyClip);
                            drawingContext.DrawGeometry(null, StandbyRubyPen, r.Glyphs);
                            drawingContext.Pop();
                        }
                    }
                    foreach (KaraokeWord w in Words)
                    {
                        if (w.StartTime > Time)
                            drawingContext.DrawGeometry(StandbyFillColor, null, w.Glyphs);
                        else if (w.EndTime < Time)
                            drawingContext.DrawGeometry(ActiveFillColor, null, w.Glyphs);
                        else
                        {
                            drawingContext.PushClip(w.ActiveClip);
                            drawingContext.DrawGeometry(ActiveFillColor, null, w.Glyphs);
                            drawingContext.Pop();
                            drawingContext.PushClip(w.StandbyClip);
                            drawingContext.DrawGeometry(StandbyFillColor, null, w.Glyphs);
                            drawingContext.Pop();
                        }
                    }
                    foreach (KaraokeWord r in Rubys)
                    {
                        if (r.StartTime > Time)
                            drawingContext.DrawGeometry(StandbyFillColor, null, r.Glyphs);
                        else if (r.EndTime < Time)
                            drawingContext.DrawGeometry(ActiveFillColor, null, r.Glyphs);
                        else
                        {
                            drawingContext.PushClip(r.ActiveClip);
                            drawingContext.DrawGeometry(ActiveFillColor, null, r.Glyphs);
                            drawingContext.Pop();
                            drawingContext.PushClip(r.StandbyClip);
                            drawingContext.DrawGeometry(StandbyFillColor, null, r.Glyphs);
                            drawingContext.Pop();
                        }
                    }
                }
                else
                {
                    foreach (KaraokeWord w in Words) { drawingContext.DrawGeometry(null, ActivePen, w.Glyphs); }
                    foreach (KaraokeWord r in Rubys) { drawingContext.DrawGeometry(null, ActiveRubyPen, r.Glyphs); }

                    foreach (KaraokeWord w in Words) { drawingContext.DrawGeometry(ActiveFillColor, null, w.Glyphs); }
                    foreach (KaraokeWord r in Rubys) { drawingContext.DrawGeometry(ActiveFillColor, null, r.Glyphs); }
                }
                drawingContext.Pop();
                return;
            }

            SolidColorBrush fill = FadeFillBrush, stroke = FadeStrokeBrush;

            if (Time < StartTime && Time > StartTime - FadeInTime)
            {
                double rate = (Time - (StartTime - FadeInTime)) / FadeInTime;
                if (Line.Sync == LyricsContainer.SyncMode.Karaoke)
                {
                    FadeFillBrush.Color = (StandbyFillColor.Color * (float)rate) + (SleepFillColor.Color * (float)(1 - rate));
                    FadeStrokeBrush.Color = (StandbyStrokeColor.Color * (float)rate) + (SleepStrokeColor.Color * (float)(1 - rate));
                }
                else
                {
                    FadeFillBrush.Color = (ActiveFillColor.Color * (float)rate) + (SleepFillColor.Color * (float)(1 - rate));
                    FadeStrokeBrush.Color = (ActiveStrokeColor.Color * (float)rate) + (SleepStrokeColor.Color * (float)(1 - rate));
                }
                FadePen.Brush = FadeStrokeBrush;
                FadeRubyPen.Brush = FadeStrokeBrush;
                foreach (KaraokeWord w in Words) { drawingContext.DrawGeometry(null, FadePen, w.Glyphs); }
                foreach (KaraokeWord r in Rubys) { drawingContext.DrawGeometry(null, FadeRubyPen, r.Glyphs); }
                foreach (KaraokeWord w in Words) { drawingContext.DrawGeometry(FadeFillBrush, null, w.Glyphs); }
                foreach (KaraokeWord r in Rubys) { drawingContext.DrawGeometry(FadeFillBrush, null, r.Glyphs); }
            }
            else if (Time > EndTime && Time < EndTime + FadeOutTime)
            {
                double rate = (Time - EndTime) / FadeOutTime;
                FadeFillBrush.Color = (SleepFillColor.Color * (float)rate) + (ActiveFillColor.Color * (float)(1 - rate));
                FadeStrokeBrush.Color = (SleepStrokeColor.Color * (float)rate) + (ActiveStrokeColor.Color * (float)(1 - rate));
                FadePen.Brush = FadeStrokeBrush;
                FadeRubyPen.Brush = FadeStrokeBrush;
                foreach (KaraokeWord w in Words) { drawingContext.DrawGeometry(null, FadePen, w.Glyphs); }
                foreach (KaraokeWord r in Rubys) { drawingContext.DrawGeometry(null, FadeRubyPen, r.Glyphs); }
                foreach (KaraokeWord w in Words) { drawingContext.DrawGeometry(FadeFillBrush, null, w.Glyphs); }
                foreach (KaraokeWord r in Rubys) { drawingContext.DrawGeometry(FadeFillBrush, null, r.Glyphs); }
            }
            else
            {
                IsLastRenderOnSleep = true;
                foreach (KaraokeWord w in Words) { drawingContext.DrawGeometry(null, SleepPen, w.Glyphs); }
                foreach (KaraokeWord r in Rubys) { drawingContext.DrawGeometry(null, SleepRubyPen, r.Glyphs); }
                foreach (KaraokeWord w in Words) { drawingContext.DrawGeometry(SleepFillColor, null, w.Glyphs); }
                foreach (KaraokeWord r in Rubys) { drawingContext.DrawGeometry(SleepFillColor, null, r.Glyphs); }

            }
            drawingContext.Pop();
        }


        private class KaraokeWord
        {
            public Geometry Glyphs { get; }
            public double StartTime { get; }
            public double EndTime { get; }
            public double[] Times { get; }
            public double[] Widths { get; }

            public double Width { get; }
            public double Height { get; }

            public bool IsRuby { get; }


            public KaraokeWord(LyricsContainer.Word word,FlowDirection direction, Typeface typeface, double fontSize, bool isruby = false)
            {
                GeometryGroup group = new();
                double x = 0;

                List<double> times = new(word.Chars.Length * 2);
                List<double> widths = new(word.Chars.Length * 2);

                double width = 0;
                double height = 0;
                for (int i = 0; i < word.Chars.Length; i++)
                {
                    if (word.StartTimes[i] >= 0)
                    {
                        times.Add(word.StartTimes[i] / 1000.0);
                        widths.Add(width);
                        width = 0;
                    }
                    FormattedText ft = new(word.Chars[i],
                                            CultureInfo.CurrentUICulture, direction,
                                            typeface, fontSize,
                                            Brushes.Black, 96);
                    Geometry g = ft.BuildGeometry(new Point(0, 0));
                    if (!g.IsEmpty()) //空白文字の場合、固定値を返してきてTransformを設定できなくて死ぬ
                    {
                        g.Transform = new TranslateTransform(x, 0);
                        group.Children.Add(g);
                    }
                    width += ft.WidthIncludingTrailingWhitespace;
                    x += ft.WidthIncludingTrailingWhitespace;

                    if (word.EndTimes[i] >= 0)
                    {
                        times.Add(word.EndTimes[i] / 1000.0);
                        widths.Add(width);
                        width = 0;
                    }
                    height = Math.Max(height, ft.Height);
                }
                times.RemoveAt(0); times.RemoveAt(times.Count - 1);
                widths.RemoveAt(0); widths.RemoveAt(widths.Count - 1);
                StartTime = word.StartTimes[0] / 1000.0;
                EndTime = word.EndTimes[^1] / 1000.0;
                Times = times.ToArray();
                Widths = widths.ToArray();

                Glyphs = group;
                Width = x;
                Height = height;

                IsRuby = isruby;

            }


            public double OffsetX { get; set; }
            public double OffsetY { get; set; }

            public RectangleGeometry ActiveClip = new RectangleGeometry();
            public RectangleGeometry StandbyClip = new RectangleGeometry();
            public void SetWipClip(double time)
            {
                if (time < StartTime)
                {
                    ActiveClip.Rect = new Rect(OffsetX, OffsetY, 0, Height);
                    StandbyClip.Rect = new Rect(OffsetX, OffsetY, Width, Height);
                    return;
                }
                if (time > EndTime)
                {
                    ActiveClip.Rect = new Rect(OffsetX, OffsetY, Width, Height);
                    ActiveClip.Rect = new Rect(OffsetX + Width, OffsetY, 0, Height);
                    return;
                }

                double x = 0;
                double last_time = StartTime;
                for (int i = 0; i < Times.Length; i++)
                {
                    if (time < Times[i])
                    {
                        double rate = (time - last_time) / (Times[i] - last_time);
                        x += Widths[i] * rate;
                        ActiveClip.Rect = new Rect(OffsetX, OffsetY, x, Height);
                        StandbyClip.Rect = new Rect(OffsetX + x, OffsetY, Width - x, Height);
                        return;
                    }
                    x += Widths[i];
                    last_time = Times[i];
                }
                {
                    double rate = (time - last_time) / (EndTime - last_time);
                    x += (Width - x) * rate;
                    ActiveClip.Rect = new Rect(OffsetX, OffsetY, x, Height);
                    StandbyClip.Rect = new Rect(OffsetX + x, OffsetY, Width - x, Height);
                }

            }




            private double WipeX;
            public double GetWipePointX(double time)
            {
                if (time < StartTime)
                    return OffsetX;
                if (time > EndTime)
                    return OffsetX + Width;

                double x = OffsetX;
                double last_time = StartTime;
                for (int i = 0;i < Times.Length;i++)
                {
                    if (time < Times[i])
                    {
                        double rate = (time - last_time) / (Times[i] - last_time);
                        return WipeX = x + (Widths[i] * rate);
                    }
                    x += Widths[i];
                    last_time = Times[i];
                }
                {
                    double rate = (time - last_time) / (EndTime - last_time);
                    return WipeX = x + ((Width - (x - OffsetX)) * rate);
                }
            }
            public double GetWipePointX() => WipeX;

        }


    }
}
