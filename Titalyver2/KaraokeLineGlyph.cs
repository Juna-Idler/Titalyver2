using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Titalyver2
{
    /// <summary>
    /// </summary>
    public class KaraokeLineGlyph : FrameworkElement
    {
        private static readonly GlyphTypeface system_gtf;
        static KaraokeLineGlyph()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KaraokeLineGlyph), new FrameworkPropertyMetadata(typeof(KaraokeLineGlyph)));

            foreach (Typeface typeface in SystemFonts.MessageFontFamily.GetTypefaces())
            {
                if (typeface.TryGetGlyphTypeface(out system_gtf))
                {
                    break;
                }
            }
        }

        //背景色とパディングも欲しい　あとルビと本文間の微調整

        [Description("ワイプ後文字色"), Category("Karaoke Line")]
        public SolidColorBrush ActiveFillColor { get => (SolidColorBrush)GetValue(ActiveFillColorProperty); set => SetValue(ActiveFillColorProperty, value); }
        public static readonly DependencyProperty ActiveFillColorProperty = DependencyProperty.Register(
            "ActiveFillColor", typeof(SolidColorBrush), typeof(KaraokeLineGlyph),
            new FrameworkPropertyMetadata(Brushes.White,
                                          FrameworkPropertyMetadataOptions.AffectsRender, OnChangedFillColors));

        [Description("ワイプ後縁色"), Category("Karaoke Line")]
        public SolidColorBrush ActiveStrokeColor { get => (SolidColorBrush)GetValue(ActiveStrokeColorProperty); set => SetValue(ActiveStrokeColorProperty, value); }
        public static readonly DependencyProperty ActiveStrokeColorProperty = DependencyProperty.Register(
            "ActiveStrokeColor", typeof(SolidColorBrush), typeof(KaraokeLineGlyph),
            new FrameworkPropertyMetadata(Brushes.Red,
                                          FrameworkPropertyMetadataOptions.AffectsRender, OnChangedStrokeColor));

        [Description("ワイプ前文字色"), Category("Karaoke Line")]
        public SolidColorBrush StandbyFillColor { get => (SolidColorBrush)GetValue(StandbyFillColorProperty); set => SetValue(StandbyFillColorProperty, value); }
        public static readonly DependencyProperty StandbyFillColorProperty = DependencyProperty.Register(
            "StandbyFillColor", typeof(SolidColorBrush), typeof(KaraokeLineGlyph),
            new FrameworkPropertyMetadata(Brushes.White,
                                          FrameworkPropertyMetadataOptions.AffectsRender, OnChangedFillColors));

        [Description("ワイプ前縁色"), Category("Karaoke Line")]
        public SolidColorBrush StandbyStrokeColor { get => (SolidColorBrush)GetValue(StandbyStrokeColorProperty); set => SetValue(StandbyStrokeColorProperty, value); }
        public static readonly DependencyProperty StandbyStrokeColorProperty = DependencyProperty.Register(
            "StandbyStrokeColor", typeof(SolidColorBrush), typeof(KaraokeLineGlyph),
            new FrameworkPropertyMetadata(Brushes.Blue,
                                          FrameworkPropertyMetadataOptions.AffectsRender, OnChangedStrokeColor));

        [Description("休眠文字色"), Category("Karaoke Line")]
        public SolidColorBrush SleepFillColor { get => (SolidColorBrush)GetValue(SleepFillColorProperty); set => SetValue(SleepFillColorProperty, value); }
        public static readonly DependencyProperty SleepFillColorProperty = DependencyProperty.Register(
            "SleepFillColor", typeof(SolidColorBrush), typeof(KaraokeLineGlyph),
            new FrameworkPropertyMetadata(Brushes.LightGray,
                                          FrameworkPropertyMetadataOptions.AffectsRender));

        [Description("休眠縁色"), Category("Karaoke Line")]
        public SolidColorBrush SleepStrokeColor { get => (SolidColorBrush)GetValue(SleepStrokeColorProperty); set => SetValue(SleepStrokeColorProperty, value); }
        public static readonly DependencyProperty SleepStrokeColorProperty = DependencyProperty.Register(
            "SleepStrokeColor", typeof(SolidColorBrush), typeof(KaraokeLineGlyph),
            new FrameworkPropertyMetadata(Brushes.DarkBlue,
                                          FrameworkPropertyMetadataOptions.AffectsRender));

        //縁の太さ
        [Description("縁の太さ"), Category("Karaoke Line"), DefaultValue(2)]
        public double StrokeThickness { get => (double)GetValue(StrokeThicknessProperty); set => SetValue(StrokeThicknessProperty, value); }
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(KaraokeLineGlyph),
            new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsRender, OnChangeStrokeTickness));


        [Description("文字の大きさ"), Category("Karaoke Line"), DefaultValue(20)]
        public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize", typeof(double), typeof(KaraokeLineGlyph),
            new FrameworkPropertyMetadata(20.0, FrameworkPropertyMetadataOptions.AffectsRender, OnChangedFont));


        [Description("テスト表示用"), Category("Karaoke Line"), DefaultValue("テスト｜表示《ひょうじ》")]
        public string TestText { get => (string)GetValue(TestTextProperty); set => SetValue(TestTextProperty, value); }
        public static readonly DependencyProperty TestTextProperty = DependencyProperty.Register(
            "TestText", typeof(string), typeof(KaraokeLineGlyph),
            new FrameworkPropertyMetadata("テスト｜表示《ひょうじ》", FrameworkPropertyMetadataOptions.AffectsRender,
                (d,e)=> {
                    KaraokeLineGlyph _this = (KaraokeLineGlyph)d;
                    _this.SetLyricsLine(new LyricsContainer.Line("[00:00.00]" + _this.TestText + "[00:10.00][00:10.00]", new AtTagContainer("")));
                    OnChangedFont(d,e);
                }));

        [Description("描画するタイムタグ時間"), Category("Karaoke Line"), DefaultValue(0)]
        public double Time { get => (double)GetValue(TimeProperty); set => SetValue(TimeProperty, value); }
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(
            "Time", typeof(double), typeof(KaraokeLineGlyph),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));


        private static void OnChangedFillColors(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeLineGlyph _this = (KaraokeLineGlyph)dependencyObject;
            _this.SetFillWipe();

        }

        private static void OnChangedStrokeColor(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeLineGlyph _this = (KaraokeLineGlyph)dependencyObject;
            _this.SetStrokeWipe();
        }
        private static void OnChangeStrokeTickness(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeLineGlyph _this = (KaraokeLineGlyph)dependencyObject;

            if (_this.Words == null)
                return;
            foreach (KaraokeWord w in _this.Words)
            {
                w.WipeDrawResource.Pen.Thickness = _this.StrokeThickness / (w.IsRuby ? 2 : 1);
            }
        }
        private static void OnChangedFont(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeLineGlyph _this = (KaraokeLineGlyph)dependencyObject;
            _this.MakeWords();
        }


        public double FadeInTime { get; set; } = 0.5;
        public double FadeOutTime { get; set; } = 1.0;

        public double StartTime { get; private set; }
        public double EndTime { get; private set; }


        public GlyphTypeface GlyphTypeface { get; set; }

        private LyricsContainer.Line Line;
        private readonly SolidColorBrush FadeFillBrush = new();
        private readonly SolidColorBrush FadeStrokeBrush = new();


        private KaraokeWord[] Words;

        private bool IsLastRenderOnSleep;


        private void SetFillWipe()
        {
            if (Words == null)
                return;
            foreach (KaraokeWord w in Words)
            {
                w.WipeDrawResource.WipeFill.GradientStops[0].Color = ActiveFillColor.Color;
                w.WipeDrawResource.WipeFill.GradientStops[1].Color = StandbyFillColor.Color;
            }
        }
        private void SetStrokeWipe()
        {
            if (Words == null)
                return;
            foreach (KaraokeWord w in Words)
            {
                w.WipeDrawResource.WipeStroke.GradientStops[0].Color = ActiveStrokeColor.Color;
                w.WipeDrawResource.WipeStroke.GradientStops[1].Color = StandbyStrokeColor.Color;
            }
        }


        public KaraokeLineGlyph()
        {
            GlyphTypeface = system_gtf;

            SetLyricsLine(new LyricsContainer.Line("[00:00.00]" + TestText + "[00:10.00][00:10.00]",new AtTagContainer("")));
            MakeWords();
        }

        public KaraokeLineGlyph(GlyphTypeface glyphTypeface, double fontSize, SolidColorBrush activeFill,SolidColorBrush activeStroke,
            SolidColorBrush standbyFill, SolidColorBrush standbyStroke, double strokeTickness, LyricsContainer.Line line)
        {
            GlyphTypeface = glyphTypeface;
            FontSize = fontSize;
            ActiveFillColor = activeFill;
            ActiveStrokeColor = activeStroke;
            StandbyFillColor = standbyFill;
            StandbyStrokeColor = standbyStroke;
            StrokeThickness = strokeTickness;

            SetLyricsLine(line);
        }


        public void SetLyricsLine(LyricsContainer.Line line)
        {
            Line = line;
            _ = Line.Complement();
            MakeWords();
        }
        private void MakeWords()
        {
            if (Line == null || GlyphTypeface == null || FontSize == 0)
                return;
            double x = 0;
            double ruby_x = 100;
            double y = Line.HasRuby ? GlyphTypeface.Baseline * FontSize / 2 : 0;
            List <KaraokeWord> words = new(Line.Words.Length);
            foreach (LyricsContainer.Line.WordWithRuby wwr in Line.Words)
            {
                KaraokeWord word = new(wwr.Word, GlyphTypeface, FontSize);
                word.WipeDrawResource = new(ActiveFillColor.Color, StandbyFillColor.Color,
                                            ActiveStrokeColor.Color, StandbyStrokeColor.Color, StrokeThickness);
                if (wwr.HasRuby)
                {
                    KaraokeWord ruby = new(wwr.Ruby, GlyphTypeface, FontSize / 2, true);
                    ruby.WipeDrawResource = new(ActiveFillColor.Color, StandbyFillColor.Color,
                                                ActiveStrokeColor.Color, StandbyStrokeColor.Color, StrokeThickness / 2);

                    if (ruby_x + (word.Width - ruby.Width) / 2 < 0)
                    {
                        x += -ruby_x - ((word.Width - ruby.Width) / 2);
                    }
                    ruby_x = (word.Width - ruby.Width) / 2;

                    ruby.Glyphs.Transform = new TranslateTransform(x + ruby_x, y);
                    ruby.OffsetX = x + ruby_x;
                    words.Add(ruby);
                }
                else
                {
                    ruby_x += word.Width;
                }
                word.Glyphs.Transform = new TranslateTransform(x, y + GlyphTypeface.Baseline * FontSize);
                word.OffsetX = x;
                words.Add(word);
                x += word.Width;
            }
            Words = words.ToArray();
            StartTime = Line.StartTime / 1000.0;
            EndTime = Line.EndTime / 1000.0;
            Height = y + GlyphTypeface.Height * FontSize;

            SetFillWipe();
            SetStrokeWipe();
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

            if (StartTime < Time && Time < EndTime)
            {

                foreach (KaraokeWord w in Words)
//                for (int i = Words.Length -1;i >= 0;i--)
                {
                    //                    KaraokeWord w = Words[i];
                    if (w.StarTime > Time)
                    {
                        w.WipeDrawResource.SetPenBrush(StandbyStrokeColor);
                    }
                    else if (w.EndTime < Time)
                    {
                        w.WipeDrawResource.SetPenBrush(ActiveStrokeColor);
                    }
                    else
                    {
                        w.WipeDrawResource.SetPenWipeBrush();
                        w.WipeDrawResource.SetWipePoint(w.GetWipePointX(Time));
                    }
                    drawingContext.DrawGeometry(null, w.WipeDrawResource.Pen, w.Glyphs);
                }
//                foreach (KaraokeWord w in Words)
                for (int i = Words.Length - 1; i >= 0; i--)

                {
                    KaraokeWord w = Words[i];
                    if (w.StarTime > Time)
                        drawingContext.DrawGeometry(StandbyFillColor, null, w.Glyphs);
                    else if (w.EndTime < Time)
                        drawingContext.DrawGeometry(ActiveFillColor, null, w.Glyphs);
                    else
                    {
                        drawingContext.DrawGeometry(w.WipeDrawResource.WipeFill, null, w.Glyphs);
                    }
                }
                return;
            }

            SolidColorBrush fill = FadeFillBrush, stroke = FadeStrokeBrush;

            if (Time < StartTime && Time > StartTime - FadeInTime)
            {
                double rate = (Time - (StartTime - FadeInTime)) / FadeInTime;
                FadeFillBrush.Color = (StandbyFillColor.Color * (float)rate) + (SleepFillColor.Color * (float)(1 - rate));
                FadeStrokeBrush.Color = (StandbyStrokeColor.Color * (float)rate) + (SleepStrokeColor.Color * (float)(1 - rate));
            }
            else if (Time > EndTime && Time < EndTime + FadeOutTime)
            {
                double rate = (Time - EndTime) / FadeOutTime;
                FadeFillBrush.Color = (SleepFillColor.Color * (float)rate) + (ActiveFillColor.Color * (float)(1 - rate));
                FadeStrokeBrush.Color = (SleepStrokeColor.Color * (float)rate) + (ActiveStrokeColor.Color * (float)(1 - rate));
            }
            else
            {
                fill = SleepFillColor;
                stroke = SleepStrokeColor;
                IsLastRenderOnSleep = true;
            }
            foreach (KaraokeWord w in Words)
            {
                w.WipeDrawResource.SetPenBrush(stroke);
                drawingContext.DrawGeometry(null, w.WipeDrawResource.Pen, w.Glyphs);
            }
            foreach (KaraokeWord w in Words) { drawingContext.DrawGeometry(fill, null, w.Glyphs); }
        }

        private class WipeDrawResource
        {
            public Pen Pen { get; }
            public LinearGradientBrush WipeStroke { get; }
            public LinearGradientBrush WipeFill { get; }

            public WipeDrawResource(Color activeFill,Color standbyFill,
                                    Color activeStroke,Color standbyStroke,
                                    double tickness)
            {
                WipeStroke = new LinearGradientBrush(activeStroke, standbyStroke, 0);
                WipeStroke.MappingMode = BrushMappingMode.Absolute;
                WipeFill = new LinearGradientBrush(activeFill, standbyFill, 0);
                WipeFill.MappingMode = BrushMappingMode.Absolute;
                Pen = new Pen(WipeStroke, tickness);
                Pen.LineJoin = PenLineJoin.Round;
            }

            public void SetPenWipeBrush() { Pen.Brush = WipeStroke; }
            public void SetPenBrush(Brush b) { Pen.Brush = b; }

            public void SetWipePoint(double x)
            {
                WipeFill.StartPoint = WipeStroke.StartPoint = new Point(x, 0);
                WipeFill.EndPoint = WipeStroke.EndPoint = new Point(x + 0.01, 0);
            }
        }


        private class KaraokeWord
        {
            public Geometry Glyphs { get; }
            public double StarTime { get; }
            public double EndTime { get; }
            public double[] Times { get; }
            public double[] Widths { get; }

            public double Width { get; }
            public double Height { get; }

            public bool IsRuby { get; }


            public KaraokeWord(LyricsContainer.Word word, GlyphTypeface glyphTypeface, double fontSize, bool isruby = false)
            {
                GeometryGroup group = new();
                double x = 0;

                List<double> times = new(word.Chars.Length * 2);
                List<double> widths = new(word.Chars.Length * 2);

                double width = 0;
                for (int i = 0; i < word.Chars.Length; i++)
                {
                    if (word.StartTimes[i] >= 0)
                    {
                        times.Add(word.StartTimes[i] / 1000.0);
                        widths.Add(width);
                        width = 0;
                    }
                    string remain = word.Chars[i];
                    //do
                    {
                        int code;
                        if (char.IsSurrogate(remain[0]))
                        {
                            code = char.ConvertToUtf32(remain[0], remain[1]);
                            remain = remain[2..];
                        }
                        else
                        {
                            code = remain[0];
                            remain = remain[1..];
                        }
                        _ = glyphTypeface.CharacterToGlyphMap.TryGetValue(code, out ushort index);
                        Geometry g = glyphTypeface.GetGlyphOutline(index,
                                                                    fontSize,
                                                                    fontSize);//dpiとかよくわからんのでhintingにも同じものを
                        g.Transform = new TranslateTransform(x, 0);
                        width += glyphTypeface.AdvanceWidths[index] * fontSize;
                        x += glyphTypeface.AdvanceWidths[index] * fontSize;
                        group.Children.Add(g);
                    }
                    //while (remain != "");//結合文字とかのマルチコードポイントなグリフの取り方が分からん

                    if (word.EndTimes[i] >= 0)
                    {
                        times.Add(word.EndTimes[i] / 1000.0);
                        widths.Add(width);
                        width = 0;
                    }
                }
                times.RemoveAt(0); times.RemoveAt(times.Count - 1);
                widths.RemoveAt(0); widths.RemoveAt(widths.Count - 1);
                StarTime = word.StartTimes[0] / 1000.0;
                EndTime = word.EndTimes[^1] / 1000.0;
                Times = times.ToArray();
                Widths = widths.ToArray();

                Glyphs = group;
                Width = x;
                Height = glyphTypeface.Height * fontSize;

                IsRuby = isruby;

            }

            //LinearGradientBrushが使いまわせないので各個に持たせる
            public WipeDrawResource WipeDrawResource { get; set; }

            public double OffsetX { get; set; }
            private double WipeX;
            public double GetWipePointX(double time)
            {
                if (time < StarTime)
                    return OffsetX;
                if (time > EndTime)
                    return OffsetX + Width;

                double x = OffsetX;
                double last_time = StarTime;
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
