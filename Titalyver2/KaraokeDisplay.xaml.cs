using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        //縁の太さ
        [Description("縁の太さ"), Category("Karaoke Display"), DefaultValue(2)]
        public double StrokeThickness { get => (double)GetValue(StrokeThicknessProperty); set => SetValue(StrokeThicknessProperty, value); }
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata(2.0));

        [Description("タイムタグ同期歌詞"), Category("Karaoke Display"), DefaultValue("[00:00.00]テスト｜表示《ひょうじ》[00:10.00]")]
        public string Lyrics { get => (string)GetValue(LyricsProperty); set => SetValue(LyricsProperty, value); }
        public static readonly DependencyProperty LyricsProperty = DependencyProperty.Register(
            "Lyrics", typeof(string), typeof(KaraokeDisplay),
            new FrameworkPropertyMetadata("[00:00.00]テスト｜表示《ひょうじ》[00:10.00]", OnChangeLyrics));



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
        public void ForceMove(double time, double duration = 0.5)
        {
            if (Animation != null)
            {
                BeginAnimation(AutoScrollYProperty, new DoubleAnimation() { BeginTime = null });
                Animation = null;
            }
            if (lyrics.Sync != LyricsContainer.SyncMode.None)
            {
                foreach (KaraokeLine kl in List.Children)
                {
                    if (kl.StartTime <= time && time <= kl.EndTime)
                    {
                        Point p = kl.TranslatePoint(new Point(0, 0), List);
                        Animation = new(-p.Y, new Duration(TimeSpan.FromSeconds(duration)));
                        Animation.Completed += (s, e) => { Animation = null; };
                        BeginAnimation(AutoScrollYProperty, Animation);
                        break;
                    }
                }
            }
            if (Animation == null)
            {
                Animation = new(0, new Duration(TimeSpan.FromSeconds(duration)));
                Animation.Completed += (s, e) => { Animation = null; };
                BeginAnimation(AutoScrollYProperty, Animation);
            }
        }


        public KaraokeDisplay()
        {
            InitializeComponent();
            foreach (Typeface typeface in FontFamily.GetTypefaces())
            {
                this.typeface = typeface;
                break;
            }

            SizeChanged += (s,e) =>
            {
                if (e.WidthChanged)
                {
                    foreach (KaraokeLine kl in List.Children)
                    {
                        kl.Width = ActualWidth;
                    }
                }
            };
        }


        private static void OnChangeLyrics(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeDisplay _this = (KaraokeDisplay)dependencyObject;
            _this.SetLyrics();

        }
        private static void OnChangeTime(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            KaraokeDisplay _this = (KaraokeDisplay)dependencyObject;
            _this.OnChangeTime();

        }

        private void SetLyrics()
        {
            lyrics = new LyricsContainer(Lyrics);
            AtTagTimeOffset = lyrics.AtTagContainer.Offset;

            List.Children.Clear();
            if (lyrics.Sync == LyricsContainer.SyncMode.None)
            {
                using System.IO.StringReader sr = new(Lyrics);
                for (string line = sr.ReadLine(); line != null; line = sr.ReadLine())
                {
                    TextBlock tb = new TextBlock();
                    tb.FontSize = FontSize;
                    tb.Foreground = SleepFillColor;
                    tb.Text = line;
                    _ = List.Children.Add(tb);
                }
            }
            else
            {
                ;
                SolidColorBrush b = new SolidColorBrush(Color.FromArgb(63, 0, 128, 0));
                SolidColorBrush sb = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                foreach (LyricsContainer.Line l in lyrics.Lines)
                {
                    KaraokeLine kl = new(typeface, FontSize,
                                         ActiveFillColor, ActiveStrokeColor,
                                         StandbyFillColor, StandbyStrokeColor,
                                         StrokeThickness, l);
                    kl.TextAlignment = TextAlignment.Center;
                    kl.Padding = new Thickness(10, 5, 10, 5);
                    kl.Width = ActualWidth;
                    kl.ActiveBackColor = b;
                    kl.SleepBackColor = sb;
                    _ = List.Children.Add(kl);
                }
            }
            AutoScrollY = 0;
        }
        private void OnChangeTime() //名前変えたい
        {
            if (lyrics.Sync == LyricsContainer.SyncMode.None)
            {
                Canvas.SetTop(List, ManualScrollY);
                return;
            }
            double time = Time - AtTagTimeOffset;

            foreach (KaraokeLine kl in List.Children)
            {
                if (Animation == null && time < kl.StartTime && kl.StartTime - kl.FadeInTime < time)
                {
                    Point p = kl.TranslatePoint(new Point(0, 0), List);

                    Animation = new(-p.Y, new Duration(TimeSpan.FromSeconds(kl.StartTime - time)));
                    Animation.Completed += (s, e) => { Animation = null; };
                    BeginAnimation(AutoScrollYProperty, Animation);
                }

                if (kl.NeedRender(time))
                {
                    kl.Time = time;
                }
            }
            Canvas.SetTop(List, AutoScrollY + ManualScrollY);
        }



        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            Time = Stopwatch.Elapsed.TotalSeconds + TimeOffset;
            OnChangeTime();
        }





        private Typeface typeface;

        private LyricsContainer lyrics;
        private double AtTagTimeOffset;

        private readonly Stopwatch Stopwatch = new();

        private double TimeOffset;



        private DoubleAnimation Animation;


    }
}
