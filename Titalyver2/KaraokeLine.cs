﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Globalization;

namespace Titalyver2
{
    /// <summary>
    /// </summary>
    public class KaraokeLine : FrameworkElement
    {
        private static readonly Typeface system_typeface;
        static KaraokeLine()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KaraokeLine), new FrameworkPropertyMetadata(typeof(KaraokeLine)));

            foreach (Typeface typeface in SystemFonts.MessageFontFamily.GetTypefaces())
            {
                system_typeface = typeface;
                break;
            }
        }

        //背景色とパディングも欲しい　あとルビと本文間の微調整

        [Description("ワイプ後文字色"), Category("Karaoke Line")]
        public SolidColorBrush ActiveFillColor { get => (SolidColorBrush)GetValue(ActiveFillColorProperty); set => SetValue(ActiveFillColorProperty, value); }
        public static readonly DependencyProperty ActiveFillColorProperty = DependencyProperty.Register(
            "ActiveFillColor", typeof(SolidColorBrush), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(Brushes.White,
                                          FrameworkPropertyMetadataOptions.AffectsRender, OnChangedFillColors));

        [Description("ワイプ後縁色"), Category("Karaoke Line")]
        public SolidColorBrush ActiveStrokeColor { get => (SolidColorBrush)GetValue(ActiveStrokeColorProperty); set => SetValue(ActiveStrokeColorProperty, value); }
        public static readonly DependencyProperty ActiveStrokeColorProperty = DependencyProperty.Register(
            "ActiveStrokeColor", typeof(SolidColorBrush), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(Brushes.Red,
                                          FrameworkPropertyMetadataOptions.AffectsRender, OnChangedStrokeColor));

        [Description("ワイプ前文字色"), Category("Karaoke Line")]
        public SolidColorBrush StandbyFillColor { get => (SolidColorBrush)GetValue(StandbyFillColorProperty); set => SetValue(StandbyFillColorProperty, value); }
        public static readonly DependencyProperty StandbyFillColorProperty = DependencyProperty.Register(
            "StandbyFillColor", typeof(SolidColorBrush), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(Brushes.White,
                                          FrameworkPropertyMetadataOptions.AffectsRender, OnChangedFillColors));

        [Description("ワイプ前縁色"), Category("Karaoke Line")]
        public SolidColorBrush StandbyStrokeColor { get => (SolidColorBrush)GetValue(StandbyStrokeColorProperty); set => SetValue(StandbyStrokeColorProperty, value); }
        public static readonly DependencyProperty StandbyStrokeColorProperty = DependencyProperty.Register(
            "StandbyStrokeColor", typeof(SolidColorBrush), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(Brushes.Blue,
                                          FrameworkPropertyMetadataOptions.AffectsRender, OnChangedStrokeColor));

        [Description("休眠文字色"), Category("Karaoke Line")]
        public SolidColorBrush SleepFillColor { get => (SolidColorBrush)GetValue(SleepFillColorProperty); set => SetValue(SleepFillColorProperty, value); }
        public static readonly DependencyProperty SleepFillColorProperty = DependencyProperty.Register(
            "SleepFillColor", typeof(SolidColorBrush), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(Brushes.LightGray,
                                          FrameworkPropertyMetadataOptions.AffectsRender));

        [Description("休眠縁色"), Category("Karaoke Line")]
        public SolidColorBrush SleepStrokeColor { get => (SolidColorBrush)GetValue(SleepStrokeColorProperty); set => SetValue(SleepStrokeColorProperty, value); }
        public static readonly DependencyProperty SleepStrokeColorProperty = DependencyProperty.Register(
            "SleepStrokeColor", typeof(SolidColorBrush), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(Brushes.DarkBlue,
                                          FrameworkPropertyMetadataOptions.AffectsRender));

        //縁の太さ
        [Description("縁の太さ"), Category("Karaoke Line"), DefaultValue(2)]
        public double StrokeThickness { get => (double)GetValue(StrokeThicknessProperty); set => SetValue(StrokeThicknessProperty, value); }
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsRender, OnChangeStrokeTickness));


        [Description("文字の大きさ"), Category("Karaoke Line"), DefaultValue(20)]
        public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize", typeof(double), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(20.0, FrameworkPropertyMetadataOptions.AffectsRender, OnChangedFont));

        [Description("文字の配置"), Category("Karaoke Line"), DefaultValue(TextAlignment.Left)]
        public TextAlignment TextAlignment { get => (TextAlignment)GetValue(TextAlignmentProperty); set => SetValue(TextAlignmentProperty, value); }
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
            "TextAlignment", typeof(TextAlignment), typeof(KaraokeLine),
            new FrameworkPropertyMetadata( TextAlignment.Left,FrameworkPropertyMetadataOptions.AffectsRender ));

        [Description("文字の余白"), Category("Karaoke Line")]
        public Thickness Padding { get => (Thickness)GetValue(PaddingProperty); set => SetValue(PaddingProperty, value); }
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
            "PaddingProperty", typeof(Thickness), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender));


        [Description("アクティブ行背景色"), Category("Karaoke Line")]
        public SolidColorBrush ActiveBackColor { get => (SolidColorBrush)GetValue(ActiveBackColorProperty); set => SetValue(ActiveBackColorProperty, value); }
        public static readonly DependencyProperty ActiveBackColorProperty = DependencyProperty.Register(
            "ActiveBackColor", typeof(SolidColorBrush), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(Brushes.Transparent,
                                          FrameworkPropertyMetadataOptions.AffectsRender));

        [Description("休眠行背景色"), Category("Karaoke Line")]
        public SolidColorBrush SleepBackColor { get => (SolidColorBrush)GetValue(SleepBackColorProperty); set => SetValue(SleepBackColorProperty, value); }
        public static readonly DependencyProperty SleepBackColorProperty = DependencyProperty.Register(
            "SleepBackColor", typeof(SolidColorBrush), typeof(KaraokeLine),
            new FrameworkPropertyMetadata(Brushes.Transparent,
                                          FrameworkPropertyMetadataOptions.AffectsRender));




        [Description("テスト表示用"), Category("Karaoke Line"), DefaultValue("テスト｜表示《ひょうじ》")]
        public string TestText { get => (string)GetValue(TestTextProperty); set => SetValue(TestTextProperty, value); }
        public static readonly DependencyProperty TestTextProperty = DependencyProperty.Register(
            "TestText", typeof(string), typeof(KaraokeLine),
            new FrameworkPropertyMetadata("テスト｜表示《ひょうじ》", FrameworkPropertyMetadataOptions.AffectsRender,
                (d,e)=> {
                    KaraokeLine _this = (KaraokeLine)d;
                    _this.SetLyricsLine(new LyricsContainer.Line("[00:00.00]" + _this.TestText + "[00:10.00][00:10.00]", new AtTagContainer("")));
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
            _this.SetFillWipe();

        }

        private static void OnChangedStrokeColor(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeLine _this = (KaraokeLine)dependencyObject;
            _this.SetStrokeWipe();
        }
        private static void OnChangeStrokeTickness(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeLine _this = (KaraokeLine)dependencyObject;

            if (_this.Words == null)
                return;
            foreach (KaraokeWord w in _this.Words)
            {
                w.WipeDrawResource.Pen.Thickness = _this.StrokeThickness / (w.IsRuby ? 2 : 1);
            }
        }
        private static void OnChangedFont(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeLine _this = (KaraokeLine)dependencyObject;
            _this.MakeWords();
        }


        public double FadeInTime { get; set; } = 0.5;
        public double FadeOutTime { get; set; } = 0.75;

        public double StartTime { get; private set; }
        public double EndTime { get; private set; }


        private Typeface Typeface;

        private LyricsContainer.Line Line;
        private readonly SolidColorBrush FadeFillBrush = new();
        private readonly SolidColorBrush FadeStrokeBrush = new();
        private readonly SolidColorBrush FadeBackBrush = new();


        private KaraokeWord[] Words;
        private double WordsWidth;
        private double WordsHeight;

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


        public KaraokeLine()
        {
            Typeface = system_typeface;

            SetLyricsLine(new LyricsContainer.Line("[00:00.00]" + TestText + "[00:10.00][00:10.00]",new AtTagContainer("")));
            MakeWords();
        }

        public KaraokeLine(Typeface typeface, double fontSize, SolidColorBrush activeFill,SolidColorBrush activeStroke,
            SolidColorBrush standbyFill, SolidColorBrush standbyStroke, double strokeTickness, LyricsContainer.Line line)
        {
            Typeface = typeface;
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

        protected override Size MeasureOverride(Size availableSize)
        {
            double ww = WordsWidth + Padding.Left + Padding.Right;
            double width = double.IsNaN(Width) ? ww : Width;
            return new Size(width, WordsHeight + Padding.Top + Padding.Bottom);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }



        private void MakeWords()
        {
            if (Line == null || Typeface == null || FontSize == 0)
                return;
            double x = 0;
            double ruby_x = 100;//ルビパディング（初期化値は一文字目の）
            double y = Line.HasRuby ? Typeface.CapsHeight * FontSize / 2 : 0;
            List <KaraokeWord> words = new(Line.Words.Length);
            foreach (LyricsContainer.Line.WordWithRuby wwr in Line.Words)
            {
                KaraokeWord word = new(wwr.Word, FlowDirection, Typeface, FontSize);
                word.WipeDrawResource = new(ActiveFillColor.Color, StandbyFillColor.Color,
                                            ActiveStrokeColor.Color, StandbyStrokeColor.Color, StrokeThickness);
                if (wwr.HasRuby)
                {
                    KaraokeWord ruby = new(wwr.Ruby, FlowDirection, Typeface, FontSize / 2, true);
                    ruby.WipeDrawResource = new(ActiveFillColor.Color, StandbyFillColor.Color,
                                                ActiveStrokeColor.Color, StandbyStrokeColor.Color, StrokeThickness / 2);

                    if (ruby_x + (word.Width - ruby.Width) / 2 < 0)
                    {
                        x += -ruby_x - ((word.Width - ruby.Width) / 2);
                    }
                    ruby_x = (word.Width - ruby.Width) / 2;

                    ruby.Glyphs.Transform = new TranslateTransform(x + ruby_x, 0);
                    ruby.OffsetX = x + ruby_x;
                    words.Add(ruby);
                }
                else
                {
                    ruby_x += word.Width;
                }
                word.Glyphs.Transform = new TranslateTransform(x, y);
                word.OffsetX = x;
                words.Add(word);
                x += word.Width;
            }
            Words = words.ToArray();
            StartTime = Line.StartTime / 1000.0;
            EndTime = Line.EndTime / 1000.0;
            WordsWidth = x;
            WordsHeight = y + 1.25 * FontSize;//何故かTypefaceから行の高さを求められないので適当な固定倍率値

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
            {
                width += double.IsNaN(Width) ? Padding.Left + Padding.Right : 0;
                Rect rect = new(0, 0, width, WordsHeight + Padding.Top + Padding.Bottom);
                if (StartTime < Time && Time < EndTime - FadeInTime)
                {
                    FadeBackBrush.Color = ActiveBackColor.Color;
                }
                else if (Time < StartTime && Time > StartTime - FadeInTime)
                {
                    double rate = (Time - (StartTime - FadeInTime)) / FadeInTime;
                    FadeBackBrush.Color = (ActiveBackColor.Color * (float)rate) + (SleepBackColor.Color * (float)(1 - rate));
                }
                else if (Time < EndTime && Time > EndTime - FadeInTime)
                {
                    double rate = (Time - (EndTime - FadeInTime)) / FadeInTime;
                    FadeBackBrush.Color = (SleepBackColor.Color * (float)rate) + (ActiveBackColor.Color * (float)(1 - rate));
                }
                else
                {
                    FadeBackBrush.Color = SleepBackColor.Color;
                }
                drawingContext.DrawRectangle(FadeBackBrush, null, rect);
            }


            drawingContext.PushTransform(new TranslateTransform(alignment_x, Padding.Top));

            if (StartTime < Time && Time < EndTime)
            {
                if (Line.Sync == LyricsContainer.SyncMode.Karaoke)
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
                }
                else
                {
                    foreach (KaraokeWord w in Words)
                    {
                        w.WipeDrawResource.SetPenBrush(ActiveStrokeColor);
                        drawingContext.DrawGeometry(null, w.WipeDrawResource.Pen, w.Glyphs);
                    }
                    foreach (KaraokeWord w in Words) { drawingContext.DrawGeometry(ActiveFillColor, null, w.Glyphs); }
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
            drawingContext.Pop();
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

            public bool IsRuby { get; }


            public KaraokeWord(LyricsContainer.Word word,FlowDirection direction, Typeface typeface, double fontSize, bool isruby = false)
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
                }
                times.RemoveAt(0); times.RemoveAt(times.Count - 1);
                widths.RemoveAt(0); widths.RemoveAt(widths.Count - 1);
                StarTime = word.StartTimes[0] / 1000.0;
                EndTime = word.EndTimes[^1] / 1000.0;
                Times = times.ToArray();
                Widths = widths.ToArray();

                Glyphs = group;
                Width = x;

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
