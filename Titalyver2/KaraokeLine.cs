using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Titalyver2
{
    /// <summary>
    /// </summary>
    public class KaraokeLine : FrameworkElement
    {
        private static readonly GlyphTypeface system_gtf;
        static KaraokeLine()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KaraokeLine), new FrameworkPropertyMetadata(typeof(KaraokeLine)));

            foreach (Typeface typeface in SystemFonts.MessageFontFamily.GetTypefaces())
            {
                if (typeface.TryGetGlyphTypeface(out system_gtf))
                {
                    break;
                }
            }
        }

        //ワイプ後文字色
        [Description("ワイプ後文字色"), Category("Karaoke Line"), DefaultValue(typeof(SolidColorBrush),"white")]
        public SolidColorBrush ActiveFillColor { get => (SolidColorBrush)GetValue(ActiveFillColorProperty); set => SetValue(ActiveFillColorProperty, value); }
        public static readonly DependencyProperty ActiveFillColorProperty = DependencyProperty.Register(
            "ActiveFillColor", typeof(SolidColorBrush), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(Brushes.White,
                                          FrameworkPropertyMetadataOptions.AffectsRender, OnChangedFillColors));
        //ワイプ前文字色
        [Description("ワイプ前文字色"), Category("Karaoke Line"), DefaultValue(typeof(SolidColorBrush), "192, 192, 192")]
        public SolidColorBrush StandbyFillColor { get => (SolidColorBrush)GetValue(StandbyFillColorProperty); set => SetValue(StandbyFillColorProperty, value); }
        public static readonly DependencyProperty StandbyFillColorProperty = DependencyProperty.Register(
            "StandbyFillColor", typeof(SolidColorBrush), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(Brushes.LightGray,
                                          FrameworkPropertyMetadataOptions.AffectsRender, OnChangedFillColors));

        //ワイプ後縁色
        [Description("ワイプ後縁色"), Category("Karaoke Line")]
        public SolidColorBrush ActiveStrokeColor { get => (SolidColorBrush)GetValue(ActiveStrokeColorProperty); set => SetValue(ActiveStrokeColorProperty, value); }
        public static readonly DependencyProperty ActiveStrokeColorProperty = DependencyProperty.Register(
            "ActiveStrokeColor", typeof(SolidColorBrush), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(Brushes.Red,
                                          FrameworkPropertyMetadataOptions.AffectsRender, OnChangedStrokeParams));
        //ワイプ前縁色
        [Description("ワイプ前縁色"), Category("Karaoke Line")]
        public SolidColorBrush StandbyStrokeColor { get => (SolidColorBrush)GetValue(StandbyStrokeColorProperty); set => SetValue(StandbyStrokeColorProperty, value); }
        public static readonly DependencyProperty StandbyStrokeColorProperty = DependencyProperty.Register(
            "StandbyStrokeColor", typeof(SolidColorBrush), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(Brushes.Blue,
                                          FrameworkPropertyMetadataOptions.AffectsRender, OnChangedStrokeParams));
        //縁の太さ
        [Description("縁の太さ"), Category("Karaoke Line"), DefaultValue(2)]
        public double StrokeThickness { get => (double)GetValue(StrokeThicknessProperty); set => SetValue(StrokeThicknessProperty, value); }
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(2.0,FrameworkPropertyMetadataOptions.AffectsRender, OnChangedStrokeParams));

        [Description("文字の大きさ"), Category("Karaoke Line"), DefaultValue(20)]
        public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize", typeof(double), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(20.0, FrameworkPropertyMetadataOptions.AffectsRender, OnChangedFont));

        [Description("テスト表示用"), Category("Karaoke Line"), DefaultValue("Lyrics line")]
        public string TestText { get => (string)GetValue(TestTextProperty); set => SetValue(TestTextProperty, value); }
        public static readonly DependencyProperty TestTextProperty = DependencyProperty.Register(
            "TestText", typeof(string), typeof(KaraokeLine),
            new FrameworkPropertyMetadata("Lyrics line", FrameworkPropertyMetadataOptions.AffectsRender,
                (d,e)=> {
                    KaraokeLine _this = (KaraokeLine)d;
                    _this.SetLyricsLine(new LyricsContainer.Line("[00:00.00]" + _this.TestText + "[00:10.00][00:10.00]"));
                    OnChangedFont(d,e);
                }));

        [Description("描画するタイムタグ時間"), Category("Karaoke Line"), DefaultValue(0)]
        public double Time { get => (double)GetValue(TimeProperty); set => SetValue(TimeProperty, value); }
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(
            "Time", typeof(double), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));


        private static void OnChangedFillColors(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeLine _this = (KaraokeLine)dependencyObject;
            _this.ActiveFill = _this.ActiveFillColor;
            _this.StandbyFill = _this.StandbyFillColor;
            _this.SetFillWipe();

        }

        private static void OnChangedStrokeParams(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeLine _this = (KaraokeLine)dependencyObject;
            _this.ActiveStroke.Brush = _this.ActiveStrokeColor;
            _this.StandbyStroke.Brush = _this.StandbyStrokeColor;
            _this.ActiveStroke.Thickness = _this.StandbyStroke.Thickness = _this.StrokeThickness;
            _this.SetStrokeWipe();
        }
        private static void OnChangedFont(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeLine _this = (KaraokeLine)dependencyObject;
            _this.MakeWords();
        }



        private GlyphTypeface GlyphTypeface;

        private LinearGradientBrush FillWipe;
        private LinearGradientBrush StrokeWipe;

        private Pen ActiveStroke;
        private Pen StandbyStroke;
        private SolidColorBrush ActiveFill;
        private SolidColorBrush StandbyFill;


        private LyricsContainer.Line Line;
        private double StartTime;
        private double EndTime;

        private double FadeInTime = 0;
        private double FadeOutTime = 0;


        private KaraokeWord[] Words;


        private void SetFillWipe()
        {
            if (FillWipe == null)
            {
                GradientStop[] gsf = { new (ActiveFillColor.Color,0),new (ActiveFillColor.Color,0.5),
                                  new (StandbyFillColor.Color,0.5),new (StandbyFillColor.Color,1)};
                FillWipe = new(new(gsf), 0);
                FillWipe.MappingMode = BrushMappingMode.Absolute;
            }
            else
            {
                FillWipe.GradientStops[0].Color = ActiveFillColor.Color;
                FillWipe.GradientStops[1].Color = ActiveFillColor.Color;
                FillWipe.GradientStops[2].Color = StandbyFillColor.Color;
                FillWipe.GradientStops[3].Color = StandbyFillColor.Color;

            }
        }
        private void SetStrokeWipe()
        {
            if (StrokeWipe == null)
            {
                GradientStop[] gss = { new (ActiveStrokeColor.Color,0),new (ActiveStrokeColor.Color,0.5),
                                  new (StandbyStrokeColor.Color,0.5),new (StandbyStrokeColor.Color,1)};
                StrokeWipe = new(new(gss), 0);
                StrokeWipe.MappingMode = BrushMappingMode.Absolute;
            }
            else
            {
                StrokeWipe.GradientStops[0].Color = ActiveStrokeColor.Color;
                StrokeWipe.GradientStops[1].Color = ActiveStrokeColor.Color;
                StrokeWipe.GradientStops[2].Color = StandbyStrokeColor.Color;
                StrokeWipe.GradientStops[3].Color = StandbyStrokeColor.Color;
            }

        }


        public KaraokeLine()
        {
            GlyphTypeface = system_gtf;


            SetFillWipe();
            SetStrokeWipe();

            ActiveStroke = new Pen(ActiveStrokeColor, StrokeThickness);
            ActiveStroke.LineJoin = PenLineJoin.Round;
            StandbyStroke = new Pen(StandbyStrokeColor, StrokeThickness);
            StandbyStroke.LineJoin = PenLineJoin.Round;

            ActiveFill = Brushes.White;
            StandbyFill = Brushes.LightGray;

            SetLyricsLine(new LyricsContainer.Line("[00:00.00]" + TestText + "[00:10.00][00:10.00]"));

            MakeWords();


        }

        public void SetLyricsLine(LyricsContainer.Line line)
        {
            Line = line;
            StartTime = line.StartTime;
            EndTime = line.EndTime;

            _ = Line.Complement();
            MakeWords();
        }
        private void MakeWords()
        {
            double x = 0;
            double ruby_x = 100;
            double y = Line.HasRuby ? GlyphTypeface.Baseline * FontSize / 2 : 0;
            List <KaraokeWord> words = new(Line.Words.Length);
            foreach (LyricsContainer.Line.WordWithRuby wwr in Line.Words)
            {
                KaraokeWord word = new(wwr.Word, GlyphTypeface, FontSize);
                if (wwr.HasRuby)
                {
                    KaraokeWord ruby = new(wwr.Ruby, GlyphTypeface, FontSize / 2, true);

                    if (ruby_x + (word.Width - ruby.Width)/2 < 0)
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
            StartTime = Line.StartTime;
            EndTime = Line.EndTime;
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Words == null)
                return;

            if (StartTime < Time && Time < EndTime)
            {
                Pen wipePen = new(StrokeWipe, StrokeThickness);
                wipePen.LineJoin = PenLineJoin.Round;

                foreach (KaraokeWord w in Words)
                {
                    if (w.StarTime > Time)
                        drawingContext.DrawGeometry(null, StandbyStroke, w.Glyphs);
                    else if (w.EndTime < Time)
                        drawingContext.DrawGeometry(null, ActiveStroke, w.Glyphs);
                    else
                    {
                        double wipe_x = w.GetWipePointX(Time);
                        StrokeWipe.StartPoint = new Point(wipe_x, 0);
                        StrokeWipe.EndPoint = new Point(wipe_x + 0.1, 0);
                        drawingContext.DrawGeometry(null, wipePen, w.Glyphs);
                    }
                }
                foreach (KaraokeWord w in Words)
                {
                    if (w.StarTime > Time)
                        drawingContext.DrawGeometry(StandbyFill, null, w.Glyphs);
                    else if (w.EndTime < Time)
                        drawingContext.DrawGeometry(ActiveFill, null, w.Glyphs);
                    else
                    {
                        double wipe_x = w.GetWipePointX();
                        FillWipe.StartPoint = new Point(wipe_x, 0);
                        FillWipe.EndPoint = new Point(wipe_x + 0.1, 0);

                        drawingContext.DrawGeometry(FillWipe, null, w.Glyphs);
                    }
                }
                return;
            }

            if (Time < StartTime && Time > StartTime - FadeInTime)
            {

                return;
            }
            if (Time > EndTime && Time < EndTime + FadeOutTime)
            {
                return;
            }

            foreach (KaraokeWord w in Words) { drawingContext.DrawGeometry(null, StandbyStroke, w.Glyphs); }
            foreach (KaraokeWord w in Words) { drawingContext.DrawGeometry(StandbyFill, null, w.Glyphs); }
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

            public double OffsetX { get; set; }

            public KaraokeWord(LyricsContainer.Word word, GlyphTypeface glyphTypeface , double fontSize, bool isruby = false)
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
                        ushort index = 0;
                        glyphTypeface.CharacterToGlyphMap.TryGetValue(code, out index);
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
                times.RemoveAt(0);times.RemoveAt(times.Count - 1);
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
                    return WipeX = x + ((Width - x - OffsetX) * rate);
                }
            }
            public double GetWipePointX() => WipeX;

        }


    }
}
