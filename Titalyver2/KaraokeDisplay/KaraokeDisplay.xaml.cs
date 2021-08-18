using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Media.Animation;


namespace Titalyver2
{
    /// <summary>
    /// KaraokeDisplay.xaml の相互作用ロジック
    /// </summary>
    public partial class KaraokeDisplay : UserControl
    {
        [Description("ワイプ後文字色"), Category("Karaoke Display")]
        public SolidColorBrush ActiveFillColor { get => (SolidColorBrush)GetValue(ActiveFillColorProperty); set => SetValue(ActiveFillColorProperty, value); }
        public static readonly DependencyProperty ActiveFillColorProperty = DependencyProperty.Register(
            "ActiveFillColor", typeof(SolidColorBrush), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(Brushes.White));

        [Description("ワイプ後縁色"), Category("Karaoke Display")]
        public SolidColorBrush ActiveStrokeColor { get => (SolidColorBrush)GetValue(ActiveStrokeColorProperty); set => SetValue(ActiveStrokeColorProperty, value); }
        public static readonly DependencyProperty ActiveStrokeColorProperty = DependencyProperty.Register(
            "ActiveStrokeColor", typeof(SolidColorBrush), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(Brushes.Red));

        [Description("ワイプ前文字色"), Category("Karaoke Display")]
        public SolidColorBrush StandbyFillColor { get => (SolidColorBrush)GetValue(StandbyFillColorProperty); set => SetValue(StandbyFillColorProperty, value); }
        public static readonly DependencyProperty StandbyFillColorProperty = DependencyProperty.Register(
            "StandbyFillColor", typeof(SolidColorBrush), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(Brushes.White));

        [Description("ワイプ前縁色"), Category("Karaoke Display")]
        public SolidColorBrush StandbyStrokeColor { get => (SolidColorBrush)GetValue(StandbyStrokeColorProperty); set => SetValue(StandbyStrokeColorProperty, value); }
        public static readonly DependencyProperty StandbyStrokeColorProperty = DependencyProperty.Register(
            "StandbyStrokeColor", typeof(SolidColorBrush), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(Brushes.Blue));

        [Description("休眠文字色"), Category("Karaoke Display")]
        public SolidColorBrush SleepFillColor { get => (SolidColorBrush)GetValue(SleepFillColorProperty); set => SetValue(SleepFillColorProperty, value); }
        public static readonly DependencyProperty SleepFillColorProperty = DependencyProperty.Register(
            "SleepFillColor", typeof(SolidColorBrush), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(Brushes.LightGray));

        [Description("休眠縁色"), Category("Karaoke Display")]
        public SolidColorBrush SleepStrokeColor { get => (SolidColorBrush)GetValue(SleepStrokeColorProperty); set => SetValue(SleepStrokeColorProperty, value); }
        public static readonly DependencyProperty SleepStrokeColorProperty = DependencyProperty.Register(
            "SleepStrokeColor", typeof(SolidColorBrush), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(Brushes.DarkBlue));

        public SolidColorBrush ActiveBackColor { get; set; } = new(Color.FromArgb(63, 0, 128, 0));


        //縁の太さ
        [Description("縁の太さ"), Category("Karaoke Display"), DefaultValue(2)]
        public double StrokeThickness { get => (double)GetValue(StrokeThicknessProperty); set => SetValue(StrokeThicknessProperty, value); }
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(2.0,OnChangeThickness));
        private static void OnChangeThickness(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeDisplay _this = (KaraokeDisplay)dependencyObject;
            _this.OnChangeThickness();

        }

        [Description("文字の配置"), Category("Karaoke Display"), DefaultValue(TextAlignment.Left)]
        public TextAlignment TextAlignment { get => (TextAlignment)GetValue(TextAlignmentProperty); set => SetValue(TextAlignmentProperty, value); }
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
            "TextAlignment", typeof(TextAlignment), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(TextAlignment.Center, FrameworkPropertyMetadataOptions.AffectsRender, OnChangeTextAlignment));
        private static void OnChangeTextAlignment(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeDisplay _this = (KaraokeDisplay)dependencyObject;
            _this.OnChangeTextAlignment();
        }

        public VerticalAlignment KaraokeVerticalAlignment { get => (VerticalAlignment)GetValue(KaraokeVerticalAlignmentProperty); set => SetValue(KaraokeVerticalAlignmentProperty, value); }
        public static readonly DependencyProperty KaraokeVerticalAlignmentProperty = DependencyProperty.Register(
            "KaraokeVerticalAlignment", typeof(VerticalAlignment), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(VerticalAlignment.Top, FrameworkPropertyMetadataOptions.AffectsRender, OnChangeKVerticalAlignment));
        private static void OnChangeKVerticalAlignment(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeDisplay _this = (KaraokeDisplay)dependencyObject;
            _this.OnChangeKVAlignment();
        }


        [Description("行の余白"), Category("Karaoke Display")]
        public Thickness LinePadding { get => (Thickness)GetValue(LinePaddingProperty); set => SetValue(LinePaddingProperty, value); }
        public static readonly DependencyProperty LinePaddingProperty = DependencyProperty.Register(
            "LinePadding", typeof(Thickness), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender, OnChangeLineSpace));
        private static void OnChangeLineSpace(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeDisplay _this = (KaraokeDisplay)dependencyObject;
            _this.OnChangeLineSpace();
        }

        [Description("ルビ無し行のルビ代わりの隙間"), Category("Karaoke Display")]
        public double NoRubyTopSpace { get => (double)GetValue(NoRubyTopSpaceProperty); set => SetValue(NoRubyTopSpaceProperty, value); }
        public static readonly DependencyProperty NoRubyTopSpaceProperty = DependencyProperty.Register(
            "NoRubyTopSpace", typeof(double), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnChangeLineSpace));


        [Description("ルビと親文字の間"), Category("Karaoke Display")]
        public double RubyBottomSpace { get => (double)GetValue(RubyBottomSpaceProperty); set => SetValue(RubyBottomSpaceProperty, value); }
        public static readonly DependencyProperty RubyBottomSpaceProperty = DependencyProperty.Register(
            "RubyBottomSpace", typeof(double), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnChangeRubyBottomSpace));
        private static void OnChangeRubyBottomSpace(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeDisplay _this = (KaraokeDisplay)dependencyObject;
            _this.OnChangeRubyBottomSpace();
        }



        [Description("フォント"), Category("Karaoke Display")]
        public FontFamily LineFontFamily { get => (FontFamily)GetValue(LineFontFamilyProperty); set => SetValue(LineFontFamilyProperty, value); }
        public static readonly DependencyProperty LineFontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(
            typeof(KaraokeDisplay), new FrameworkPropertyMetadata(OnChangedTypeface));

        [Description("フォント"), Category("Karaoke Display")]
        public FontStyle LineFontStyle { get => (FontStyle)GetValue(LineFontStyleProperty); set => SetValue(LineFontStyleProperty, value); }
        public static readonly DependencyProperty LineFontStyleProperty = TextElement.FontStyleProperty.AddOwner(
            typeof(KaraokeDisplay), new FrameworkPropertyMetadata(OnChangedTypeface));

        [Description("フォント"), Category("Karaoke Display")]
        public FontWeight LineFontWeight { get => (FontWeight)GetValue(LineFontWeightProperty); set => SetValue(LineFontWeightProperty, value); }
        public static readonly DependencyProperty LineFontWeightProperty = TextElement.FontWeightProperty.AddOwner(
            typeof(KaraokeDisplay), new FrameworkPropertyMetadata(OnChangedTypeface));

        [Description("フォント"), Category("Karaoke Display")]
        public FontStretch LineFontStretch { get => (FontStretch)GetValue(LineFontStretchProperty); set => SetValue(LineFontStretchProperty, value); }
        public static readonly DependencyProperty LineFontStretchProperty = TextElement.FontStretchProperty.AddOwner(
            typeof(KaraokeDisplay), new FrameworkPropertyMetadata(OnChangedTypeface));

        private static void OnChangedTypeface(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeDisplay _this = (KaraokeDisplay)dependencyObject;
            _this.Typeface = new Typeface(_this.FontFamily, _this.FontStyle, _this.FontWeight, _this.FontStretch);

            if (_this.Lyrics != null)
                _this.MakeKaraokeLines();
        }





        [Description("描画するタイムタグ時間"), Category("Karaoke Display"), DefaultValue(0)]
        public double Time { get => (double)GetValue(TimeProperty); set => SetValue(TimeProperty, value); }
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(
            "Time", typeof(double), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(0.0, OnChangeTime));


        public double ManualScrollY { get => (double)GetValue(ManualScrollYProperty); set => SetValue(ManualScrollYProperty, value); }
        public static readonly DependencyProperty ManualScrollYProperty = DependencyProperty.Register(
            "ManualScrollY", typeof(double), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnChangeTime));

        //アニメーションさせるために依存プロパティ
        private double AutoScrollY { get => (double)GetValue(AutoScrollYProperty); set => SetValue(AutoScrollYProperty, value); }
        private static readonly DependencyProperty AutoScrollYProperty = DependencyProperty.Register(
            "AutoScrollY", typeof(double), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnChangeTime));

        public double VerticalOffsetY { get; set; }


        public void Start()
        {
            if (!Stopwatch.IsRunning)
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            TimeOffset = Time;
            Stopwatch.Restart();
        }
        public void Start(double time)
        {
            if (!Stopwatch.IsRunning)
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            TimeOffset = Time = time;
            Stopwatch.Restart();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }
        public void Stop()
        {
            if (Stopwatch.IsRunning)
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            Stopwatch.Stop();
        }
        public bool Starting { get { return Stopwatch.IsRunning; } }

        public void SetAutoScrollY(double time)
        {
            if (Lyrics.Sync != LyricsContainer.SyncMode.None)
            {
                AutoScrollY = GetAutoScroolY(time);
                return;
            }
        }


        public Typeface Typeface { get; private set; }

        public void SetFont(Typeface typeface,double size)
        {
            FontSize = size;
            Typeface = typeface;
            MakeKaraokeLines();
        }


        public KaraokeDisplay()
        {
            InitializeComponent();
            IsHitTestVisible = false;

            foreach (Typeface typeface in FontFamily.GetTypefaces())
            {
                this.Typeface = typeface;
                break;
            }
            Typeface = new Typeface(Typeface.FontFamily, FontStyles.Italic, FontWeights.Bold, FontStretches.Condensed);

            SizeChanged += (s,e) =>
            {
                if (e.WidthChanged)
                {
                    if (Lyrics == null)
                        return;
                    switch (Lyrics.Sync)
                    {
                        case LyricsContainer.SyncMode.None:
                            foreach (TextBlock tb in List.Children)
                            {
                                tb.Width = ActualWidth;
                            }
                            break;
                        case LyricsContainer.SyncMode.Line:
                            foreach (LineSyncLine sl in List.Children)
                            {
                                sl.Width = ActualWidth;
                                sl.UpdateWordsLayout();
                            }
                            break;
                        case LyricsContainer.SyncMode.Karaoke:
                            foreach (KaraokeLineClip kl in List.Children)
                            {
                                kl.Width = ActualWidth;
                            }
                            break;
                    }
                }
            };
        }


        private static void OnChangeTime(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeDisplay _this = (KaraokeDisplay)dependencyObject;
            _this.UpdateFrame();
        }

        public void SetLyrics(string lyrics)
        {
            LyricsText = lyrics;
            Lyrics = new LyricsContainer(lyrics);
            AtTagTimeOffset = Lyrics.AtTagContainer.Offset;
            MakeKaraokeLines();
        }
        public void UpdateAll()
        {
            if (Lyrics == null)
                return;

            switch (Lyrics.Sync)
            {
                case LyricsContainer.SyncMode.None:
//                    foreach (TextBlock tb in List.Children)
                    {
                    }
                    break;
                case LyricsContainer.SyncMode.Line:
                    foreach (LineSyncLine sl in List.Children)
                    {
                        sl.Update();
                    }
                    break;
                case LyricsContainer.SyncMode.Karaoke:
                    foreach (KaraokeLineClip kl in List.Children)
                    {
                        kl.Update();
                    }
                    break;
            }

        }
        public void ResetLineColors()
        {
            if (Lyrics == null)
                return;

            ActiveFillColor.Freeze();
            ActiveStrokeColor.Freeze();
            StandbyFillColor.Freeze();
            StandbyStrokeColor.Freeze();
            SleepFillColor.Freeze();
            SleepStrokeColor.Freeze();
            ActiveBackColor.Freeze();

            switch (Lyrics.Sync)
            {
                case LyricsContainer.SyncMode.None:
//                    foreach (TextBlock tb in List.Children)
                    {
                    }
                    break;
                case LyricsContainer.SyncMode.Line:
                    foreach (LineSyncLine sl in List.Children)
                    {
                        sl.ActiveFillColor = ActiveFillColor;
                        sl.SleepFillColor = SleepFillColor;
                        sl.ActiveBackColor = ActiveBackColor;

                        sl.ActiveStrokeColor = ActiveStrokeColor;
                        sl.SleepStrokeColor = SleepStrokeColor;
                        sl.SetStrokeColor();

                        sl.Update();                    }
                    break;
                case LyricsContainer.SyncMode.Karaoke:
                    foreach (KaraokeLineClip kl in List.Children)
                    {
                        kl.ActiveFillColor = ActiveFillColor;
                        kl.StandbyFillColor = StandbyFillColor;
                        kl.SleepFillColor = SleepFillColor;
                        kl.ActiveBackColor = ActiveBackColor;

                        kl.ActiveStrokeColor = ActiveStrokeColor;
                        kl.StandbyStrokeColor = StandbyStrokeColor;
                        kl.SleepStrokeColor = SleepStrokeColor;
                        kl.SetStrokeColor();

                        kl.Update();
                    }
                    break;
            }
        }

        private void MakeKaraokeLines()
        {
            List.Children.Clear();
            switch (Lyrics.Sync)
            {
                case LyricsContainer.SyncMode.None:
                    {
                        using System.IO.StringReader sr = new(LyricsText);
                        for (string line = sr.ReadLine(); line != null; line = sr.ReadLine())
                        {
                            TextBlock tb = new()
                            {
                                FontSize = FontSize,
                                Foreground = ActiveFillColor,
                                Text = line,
                                Padding = LinePadding,
                                TextAlignment = TextAlignment,
                                Width = ActualWidth,
                            };
                            _ = List.Children.Add(tb);
                        }
                    }
                    break;
                case LyricsContainer.SyncMode.Line:
                    {
                        foreach (LyricsContainer.Line l in Lyrics.Lines)
                        {
                           LineSyncLine sl = new(Typeface, FontSize,
                                                 ActiveFillColor, ActiveStrokeColor,
                                                 StrokeThickness, SleepFillColor, SleepStrokeColor, ActiveBackColor,
                                                 LinePadding, RubyBottomSpace, NoRubyTopSpace,
                                                 l, ActualWidth);
                            sl.TextAlignment = TextAlignment;
                            _ = List.Children.Add(sl);
                        }
                        UpdateFrame();
                    }
                    break;
                case LyricsContainer.SyncMode.Karaoke:
                    {
                        foreach (LyricsContainer.Line l in Lyrics.Lines)
                        {
                            KaraokeLineClip kl = new(Typeface, FontSize,
                                                 ActiveFillColor, ActiveStrokeColor,
                                                 StandbyFillColor, StandbyStrokeColor,
                                                 StrokeThickness, SleepFillColor, SleepStrokeColor, ActiveBackColor,
                                                 LinePadding, RubyBottomSpace, NoRubyTopSpace,
                                                 l, ActualWidth);
                            kl.TextAlignment = TextAlignment;
                            _ = List.Children.Add(kl);
                        }
                        UpdateFrame();
                    }
                    break;
            }
        }

        private double GetAutoScroolY(double time)
        {
            switch (Lyrics.Sync)
            {
                case LyricsContainer.SyncMode.Line:
                    {
                        int prev = 0;
                        for (int i = 1; i < List.Children.Count; i++)
                        {
                            if (time >= ((LineSyncLine)List.Children[i]).StartTime)
                            {
                                prev = i;
                                continue;
                            }

                            LineSyncLine prevsl = (LineSyncLine)List.Children[prev];
                            LineSyncLine sl = (LineSyncLine)List.Children[i];

                            Point zero = new(0, 0);
                            Point prevp = prevsl.TranslatePoint(zero, List);
                            double y;
                            double lineheight;
                            if (time < sl.StartTime - sl.FadeInTime)
                            {
                                y = -prevp.Y;
                                lineheight = prevsl.Height;
                            }
                            else
                            {
                                Point p = sl.TranslatePoint(zero, List);
                                double duration = Math.Min(sl.FadeInTime, sl.StartTime - prevsl.StartTime);
                                double rate = (time - (sl.StartTime - duration)) / duration;
                                y = -((p.Y - prevp.Y) * rate + prevp.Y);
                                lineheight = sl.Height * rate + prevsl.Height * (1 - rate);
                            }
                            switch (KaraokeVerticalAlignment)
                            {
                                case VerticalAlignment.Top:
                                    y += 0;
                                    break;
                                case VerticalAlignment.Center:
                                    y += (Height - lineheight) / 2;
                                    break;
                                case VerticalAlignment.Bottom:
                                    y += Height - lineheight;
                                    break;
                            }
                            return y;
                        }


                    }
                    break;
                case LyricsContainer.SyncMode.Karaoke:
                    {
                        int prev = 0;
                        for (int i = 1; i < List.Children.Count; i++)
                        {
                            if (time >= ((KaraokeLineClip)List.Children[i]).StartTime)
                            {
                                prev = i;
                                continue;
                            }
                            double prevheight = 0;
                            while (prev - 1 >= 0 && ((KaraokeLineClip)List.Children[prev - 1]).StartTime == ((KaraokeLineClip)List.Children[prev]).StartTime)
                            {
                                prevheight += ((KaraokeLineClip)List.Children[prev]).Height;
                                prev--;
                            }
                            prevheight += ((KaraokeLineClip)List.Children[prev]).Height;

                            KaraokeLineClip prevkl = (KaraokeLineClip)List.Children[prev];
                            KaraokeLineClip kl = (KaraokeLineClip)List.Children[i];

                            double height = 0;
                            while (i + 1 < List.Children.Count && ((KaraokeLineClip)List.Children[i + 1]).StartTime == ((KaraokeLineClip)List.Children[i]).StartTime)
                            {
                                height += ((KaraokeLineClip)List.Children[i]).Height;
                                i++;
                            }
                            height += ((KaraokeLineClip)List.Children[i]).Height;

                            Point zero = new(0, 0);
                            Point prevp = prevkl.TranslatePoint(zero, List);
                            double y;
                            double lineheight;
                            if (time < kl.StartTime - kl.FadeInTime)
                            {
                                y = -prevp.Y;
                                lineheight = prevheight;
                            }
                            else
                            {
                                Point p = kl.TranslatePoint(zero, List);
                                double duration = Math.Min(kl.FadeInTime, kl.StartTime - prevkl.StartTime);
                                double rate = (time - (kl.StartTime - duration)) / duration;
                                y = -((p.Y - prevp.Y) * rate + prevp.Y);
                                lineheight = height * rate + prevheight * (1 - rate);
                            }
                            switch (KaraokeVerticalAlignment)
                            {
                                case VerticalAlignment.Top:
                                    y += 0;
                                    break;
                                case VerticalAlignment.Center:
                                    y += (Height - lineheight) / 2;
                                    break;
                                case VerticalAlignment.Bottom:
                                    y += Height - lineheight;
                                    break;
                            }
                            return y;
                        }
                    }
                    break;
            }
            return 0;
        }
        private void UpdateFrame()
        {
            double time = Time - AtTagTimeOffset + UserTimeOffset;
            switch (Lyrics.Sync)
            {
                case LyricsContainer.SyncMode.None:
                    Canvas.SetTop(List, ManualScrollY);
                    return;
                    break;
                case LyricsContainer.SyncMode.Line:
                    foreach (LineSyncLine sl in List.Children)
                    {
                        sl.SetTime(time);
                        if (sl.NeedRender(time))
                        {
                            sl.Update();
                        }
                    }
                    break;
                case LyricsContainer.SyncMode.Karaoke:
                    foreach (KaraokeLineClip kl in List.Children)
                    {
                        kl.SetTime(time);
                        if (kl.NeedRender(time))
                        {
                            kl.Update();
                        }
                    }
                    break;
            }
            AutoScrollY = GetAutoScroolY(time);
            Canvas.SetTop(List, AutoScrollY + ManualScrollY + VerticalOffsetY);
        }



        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            Time = Stopwatch.Elapsed.TotalSeconds + TimeOffset;
//            UpdateFrame();
        }

        private void OnChangeTextAlignment()
        {
            if (Lyrics == null)
                return;

            switch (Lyrics.Sync)
            {
                case LyricsContainer.SyncMode.None:
                    foreach (TextBlock tb in List.Children)
                    {
                        tb.TextAlignment = TextAlignment;
                    }
                    break;
                case LyricsContainer.SyncMode.Line:
                    foreach (LineSyncLine sl in List.Children)
                    {
                        sl.TextAlignment = TextAlignment;
                        sl.UpdateWordsLayout();
                    }
                    break;
                case LyricsContainer.SyncMode.Karaoke:
                    foreach (KaraokeLineClip kl in List.Children)
                    {
                        kl.TextAlignment = TextAlignment;
                    }
                    break;
            }
        }
        private void OnChangeKVAlignment()
        {
            SetAutoScrollY(Time);
        }

        private void OnChangeLineSpace()
        {
            if (Lyrics == null)
                return;

            switch (Lyrics.Sync)
            {
                case LyricsContainer.SyncMode.None:
                    foreach (TextBlock tb in List.Children)
                    {
                        tb.Padding = LinePadding;
                    }
                    break;
                case LyricsContainer.SyncMode.Line:
                    foreach (LineSyncLine sl in List.Children)
                    {
                        sl.Padding = LinePadding;
                        sl.NoRubyTopSpace = NoRubyTopSpace;
                        sl.UpdateHeight();
                        sl.Update();
                    }
                    break;
                case LyricsContainer.SyncMode.Karaoke:
                    foreach (KaraokeLineClip kl in List.Children)
                    {
                        kl.Padding = LinePadding;
                        kl.NoRubyTopSpace = NoRubyTopSpace;
                        kl.UpdateHeight();
                        kl.Update();
                    }
                    break;
            }
            UpdateLayout();
        }
        private void OnChangeRubyBottomSpace()
        {
            if (Lyrics == null)
                return;

            switch (Lyrics.Sync)
            {
                case LyricsContainer.SyncMode.None:
                    foreach (TextBlock tb in List.Children)
                    {
                    }
                    break;
                case LyricsContainer.SyncMode.Line:
                    foreach (LineSyncLine sl in List.Children)
                    {
                        sl.RubyBottomSpace = RubyBottomSpace;
                        sl.Update();
                    }
                    break;
                case LyricsContainer.SyncMode.Karaoke:
                    foreach (KaraokeLineClip kl in List.Children)
                    {
                        kl.RubyBottomSpace = RubyBottomSpace;
                        kl.MakeWords();
                    }
                    break;
            }
            UpdateLayout();
        }

        private void OnChangeThickness()
        {
            if (Lyrics == null)
                return;

            switch (Lyrics.Sync)
            {
                case LyricsContainer.SyncMode.None:
//                    foreach (TextBlock tb in List.Children)
                    {
                    }
                    break;
                case LyricsContainer.SyncMode.Line:
                    foreach (LineSyncLine sl in List.Children)
                    {
                        sl.StrokeThickness = StrokeThickness;
                        sl.SetStrokeThickness();
                        sl.Update();
                    }
                    break;
                case LyricsContainer.SyncMode.Karaoke:
                    foreach (KaraokeLineClip kl in List.Children)
                    {
                        kl.StrokeThickness = StrokeThickness;
                        kl.SetStrokeThickness();
                        kl.Update();
                    }
                    break;
            }
        }



        private LyricsContainer Lyrics;
        private string LyricsText;
        private double AtTagTimeOffset;

        private readonly Stopwatch Stopwatch = new();

        private double TimeOffset;

        public double UserTimeOffset { get; set; }


    }
}
