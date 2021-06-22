﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;
using System.Windows.Media.Animation;

namespace Titalyver2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private LyricsContainer lyrics;

        private readonly List<KaraokeLine> Lines = new();

        private readonly Stopwatch Stopwatch = new();

        private double ManualScrollY;

        public double AutoScrollY { get => (double)GetValue(AutoScrollYProperty); set => SetValue(AutoScrollYProperty, value); }
        public static readonly DependencyProperty AutoScrollYProperty = DependencyProperty.Register(
            "AutoScrollY", typeof(double), typeof(MainWindow),
            new FrameworkPropertyMetadata(0.0));



        public MainWindow()
        {
            InitializeComponent();

            RegisterName(LineList.Name, LineList);

            CompositionTarget.Rendering += CompositionTarget_Rendering;


            GlyphTypeface system_gtf = null;
            foreach (Typeface typeface in SystemFonts.MessageFontFamily.GetTypefaces())
            {
                if (typeface.TryGetGlyphTypeface(out system_gtf))
                {
                    break;
                }

            }

            IEnumerable<FontFamily> FontList;

            FontList = Fonts.SystemFontFamilies;
            GlyphTypeface glyph = null;
            foreach (FontFamily font in FontList)
            {
                GlyphTypeface gtf = null;
                foreach (Typeface typeface in font.GetTypefaces())
                {
                    if (typeface.TryGetGlyphTypeface(out gtf))
                    {
                        break;
                    }
                }
                if (gtf != null)
                {
                    glyph = gtf;
                    Uri uri = gtf.FontUri;
                }

            }

            MessageReceiver demander = new MessageReceiver();
            _ = demander.GetData();

//            string path = "C:/Users/junai/source/repos/Titalyver2/02 サンキュ！ (fullsize).kra";
//            string text = System.IO.File.ReadAllText(path);

            lyrics = new LyricsContainer(lyrics_text);
            TestLine.Time = 1;
            GlyphTypeface bold_gtf = new GlyphTypeface(system_gtf.FontUri, StyleSimulations.BoldSimulation);
            LineList.Children.Clear();
            foreach (var l in lyrics.Lines)
            {
                KaraokeLine kl = new(bold_gtf, 30, Brushes.White, Brushes.Red, Brushes.White, Brushes.Blue, 5, l);
                kl.Margin = new Thickness(30,0,0,0);
                LineList.Children.Add(kl);
                Lines.Add(kl);
            }

            Stopwatch.Start();
        }

        private DoubleAnimation Animation;


        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            double time = Stopwatch.Elapsed.TotalSeconds;

            foreach (var kl in Lines)
            {
                if (Animation == null && time < kl.StartTime && kl.StartTime - kl.FadeInTime < time)
                {
                    Point p = kl.TranslatePoint(new Point(0, 0), LineList);

                    Animation = new(-p.Y, new Duration(TimeSpan.FromSeconds(kl.StartTime - time)));
                    Animation.Completed += (s,e) => { Animation = null; };
                    BeginAnimation(AutoScrollYProperty, Animation);
                }

                if (kl.NeedRender(time))
                    kl.Time = time;
            }
            Canvas.SetTop(LineList, AutoScrollY + ManualScrollY);
        }

        private const string lyrics_text = @"
@offset=-0.3

[00:14.48]
[00:15.65]｜[00:15.65]風《[00:15.65]か[00:15.86]ぜ》[00:16.07]が｜[00:16.31]渡《[00:16.31]わ[00:16.52]た》[00:17.03]る[00:17.66]　｜[00:18.12]広《[00:18.12]ひ[00:18.42]ろ》[00:18.78]い｜[00:18.97]空《[00:18.97]そ[00:19.43]ら》[00:19.65]は[00:20.78]
[00:22.82]｜[00:22.82]蒼《[00:22.82]あ[00:23.04]お》[00:23.25]く｜[00:23.50]澄《[00:23.50]す》[00:23.74]ん[00:24.23]で｜[00:25.29]海《[00:25.29]う[00:25.58]み》[00:25.90]へ[00:26.15]と｜[00:26.58]続《[00:26.58]つ[00:26.81]づ》[00:27.28]い[00:27.49]て[00:28.77]
[00:29.32]
[00:29.95]｜[00:29.95]痛《[00:29.95]い[00:30.17]た》[00:30.40]む｜[00:30.65]想《[00:30.65]お[00:30.88]も》[00:31.40]い[00:32.00]　｜[00:32.43]悲《[00:32.43]か[00:32.75]な》[00:33.06]し[00:33.29]い[00:33.73]こ[00:33.97]と[00:35.16]
[00:37.13]み[00:37.35]ん[00:37.58]な｜[00:37.80]溶《[00:37.80]と》[00:38.05]け[00:38.50]て｜[00:39.61]消《[00:39.61]き》[00:39.94]え[00:40.23]そ[00:40.44]う[00:41.16]で[00:42.31]
[00:42.78]
[00:43.18]｜[00:43.18]幸《[00:43.18]し[00:43.50]あ[00:43.83]わ》[00:44.05]せ[00:44.52]に[00:44.73]は[00:46.50]い[00:46.71]ろ[00:46.94]ん[00:47.18]な｜[00:47.42]色[00:48.48]《[00:47.42]い[00:47.65]ろ[00:48.48]》
[00:50.35]｜[00:50.35]溶《[00:50.35]と》[00:50.65]け｜[00:51.02]合《[00:51.02]あ》[00:51.24]っ[00:51.69]て[00:51.89]る[00:52.54]か[00:52.80]ら[00:53.52]
[00:53.95]き[00:54.39]っ[00:54.80]と……[00:56.58]ね[00:57.53]
[00:57.62]
[00:57.98]｜[00:57.98]巡《[00:57.98]め[00:58.18]ぐ》[00:58.38]り｜[00:58.84]会《[00:58.84]あ》[00:59.06]っ[00:59.51]て｜[01:00.20]行《[01:00.20]ゆ》[01:00.60]き｜[01:00.83]往《[01:00.83]ゆ》[01:01.30]く[01:01.61]
[01:01.97]｜[01:01.97]出《[01:01.97]で》｜[01:02.42]会《[01:02.42]あ》[01:02.63]い[01:03.09]と｜[01:03.56]別《[01:03.56]わ[01:03.98]か》[01:04.19]れ[01:04.44]と[01:04.83]
[01:04.86]
[01:05.12]そ[01:05.62]う[01:06.71] [01:06.91]い[01:07.36]く[01:07.78]つ[01:08.27]も[01:08.67]の[01:09.18]Destiny[01:10.92]
[01:11.40]
[01:11.78]こ[01:12.27]こ[01:12.53]で｜[01:13.61]会《[01:13.61]あ》[01:14.08]え[01:14.30]た[01:14.96]　｜[01:15.42]巡《[01:15.42]め[01:15.86]ぐ》[01:16.06]り｜[01:17.18]会《[01:17.18]あ》[01:17.54]え[01:18.06]た[01:18.53]
[01:18.98]｜[01:18.98]奇《[01:18.98]き》｜[01:19.42]跡《[01:19.42]せ[01:19.89]き》[01:20.33]に[01:20.57]そ[01:21.23]っ[01:21.45]と[01:22.10]　[01:22.56]Thank'[01:23.46]you[01:25.27]
[01:26.17]
[01:36.91]
[01:38.03]｜[01:38.03]誰《[01:38.03]だ[01:38.26]れ》[01:38.49]も[01:38.72]き[01:38.97]っ[01:39.42]と[01:40.07]　｜[01:40.48]同《[01:40.48]お[01:40.78]な》[01:41.11]じ[01:41.36]よ[01:41.82]う[01:42.04]に[01:43.31]
[01:45.16]｜[01:45.16]重《[01:45.16]お[01:45.37]も》[01:45.62]い｜[01:45.85]荷《[01:45.85]に》｜[01:46.08]物《[01:46.08]も[01:46.54]つ》｜[01:47.64]背《[01:47.64]せ》｜[01:47.93]負《[01:47.93]お》[01:48.28]っ[01:48.50]て[01:48.94]い[01:49.15]る[01:49.65]け[01:49.87]ど[01:50.82]
[01:51.69]
[01:52.32]｜[01:52.32]過《[01:52.32]か》｜[01:52.53]去《[01:52.53]こ》[01:52.76]と｜[01:53.00]未《[01:53.00]み》｜[01:53.24]来《[01:53.24]ら[01:53.45]い》[01:53.68]と[01:54.38]　｜[01:54.81]今《[01:54.81]い[01:55.16]ま》[01:55.42]の｜[01:55.69]自《[01:55.69]じ》｜[01:56.12]分[01:57.51]《[01:56.12]ぶ[01:56.37]ん[01:57.51]》
[01:59.49]｜[01:59.49]目《[01:59.49]め》[01:59.73]を｜[01:59.95]逸《[01:59.95]そ》[02:00.17]ら[02:00.40]せ[02:00.90]ば｜[02:02.00]見《[02:02.00]み》｜[02:02.30]失《[02:02.30]う[02:02.63]し[02:02.83]な》[02:03.30]う[02:03.53]の[02:05.07]
[02:05.28]
[02:05.59]｜[02:05.59]振《[02:05.59]ふ》[02:05.88]り｜[02:06.19]向《[02:06.19]む》[02:06.41]か[02:06.88]ず[02:07.14]に｜[02:08.91]俯《[02:08.91]う[02:09.10]つ[02:09.33]む》[02:09.56]か[02:09.76]ず[02:10.03]に[02:11.38]
[02:12.72]｜[02:12.72]真《[02:12.72]ま》[02:13.05]っ｜[02:13.38]直《[02:13.38]す》[02:13.60]ぐ[02:14.03]に[02:14.34]い[02:14.94]よ[02:15.19]う[02:15.88]
[02:16.29]き[02:16.75]っ[02:17.20]と……[02:18.97]ね[02:19.92]
[02:20.13]
[02:20.36]｜[02:20.36]巡《[02:20.36]め[02:20.56]ぐ》[02:20.78]り｜[02:21.26]会《[02:21.26]あ》[02:21.46]い[02:21.93]の｜[02:22.55]奇《[02:22.55]き》｜[02:22.99]跡《[02:22.99]せ[02:23.20]き》[02:23.68]に[02:24.02]
[02:24.34]｜[02:24.34]笑《[02:24.34]え》｜[02:24.80]顔《[02:24.80]が[02:25.01]お》[02:25.47]で｜[02:25.94]感《[02:25.94]か[02:26.39]ん》｜[02:26.59]謝《[02:26.59]しゃ》[02:26.82]を[02:27.11]
[02:27.14]
[02:27.51]ね[02:27.95]え[02:28.85] [02:29.25]ほ[02:29.76]ん[02:30.19]と[02:31.06]に[02:31.52]Thank'[02:32.23]you[02:33.29]
[02:34.21]こ[02:34.63]こ[02:35.00]に｜[02:35.99]居《[02:35.99]い》[02:36.46]る[02:36.70]よ[02:37.38]　[02:37.79]ず[02:38.26]っ[02:38.45]と｜[02:39.56]居《[02:39.56]い》[02:39.86]る[02:40.24]か[02:40.47]ら[02:40.94]
[02:41.36]｜[02:41.36]何《[02:41.36]な[02:41.81]ん》｜[02:42.29]度《[02:42.29]ど》[02:42.71]も｜[02:42.93]言《[02:42.93]ゆ》[02:43.65]う[02:43.86]よ[02:44.53]　[02:44.97]Love [02:45.86]you[02:47.65]

[02:58.49]
[02:59.30]｜[02:59.30]迷《[02:59.30]ま[02:59.63]よ》[02:59.96]う｜[03:00.17]時《[03:00.17]と[03:00.66]き》[03:00.88]も｜[03:02.62]悩《[03:02.62]な[03:02.86]や》[03:03.07]む｜[03:03.30]日《[03:03.30]ひ》[03:03.54]に[03:03.80]も[03:05.14]
[03:06.46]｜[03:06.46]笑《[03:06.46]え》｜[03:06.84]顔《[03:06.84]が[03:07.17]お》[03:07.39]を｜[03:07.81]忘《[03:07.81]わ[03:08.07]す》[03:08.71]れ[03:08.93]ず[03:09.64]
[03:10.06]き[03:10.47]っ[03:10.92]と……[03:12.74]ね[03:13.63]
[03:13.83]
[03:14.06]｜[03:14.06]巡《[03:14.06]め[03:14.28]ぐ》[03:14.48]り｜[03:14.92]会《[03:14.92]あ》[03:15.17]っ[03:15.63]て｜[03:16.25]行《[03:16.25]ゆ》[03:16.71]き｜[03:16.96]往《[03:16.96]ゆ》[03:17.41]く[03:17.71]
[03:18.12]｜[03:18.12]笑《[03:18.12]え》｜[03:18.53]顔《[03:18.53]が[03:18.75]お》[03:19.20]と｜[03:19.66]涙《[03:19.66]な[03:20.12]み[03:20.32]だ》[03:20.55]と[03:20.74]
[03:21.00]
[03:21.22]そ[03:21.68]う[03:22.60]　[03:23.00]い[03:23.46]く[03:23.92]つ[03:24.37]も[03:24.82]の[03:25.30]Destiny[03:27.05]
[03:27.56]
[03:27.97]こ[03:28.32]こ[03:28.63]で｜[03:29.73]会《[03:29.73]あ》[03:30.17]え[03:30.42]た[03:31.08]　｜[03:31.55]巡《[03:31.55]め[03:31.99]ぐ》[03:32.25]り｜[03:33.33]会《[03:33.33]あ》[03:33.59]え[03:34.24]た[03:34.71]
[03:35.14]｜[03:35.14]奇《[03:35.14]き》｜[03:35.57]跡《[03:35.57]せ[03:36.02]き》[03:36.45]に[03:36.68]そ[03:37.34]っ[03:37.58]と[03:38.25]　[03:38.70]Thank'[03:39.59]you[03:41.41]
[03:45.02]
[03:45.88]｜[03:45.88]奇《[03:45.88]き》｜[03:46.32]跡《[03:46.32]せ[03:46.77]き》[03:47.19]に[03:47.42]そ[03:48.10]っ[03:48.33]と[03:49.46]　[03:49.90]サ[03:50.11]ン[03:50.34]キュ！[03:50.58]
[03:52.18]

@ruby_parent=｜
@ruby_begin=《
@ruby_end=》
";

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ManualScrollY += e.Delta;
            Canvas.SetTop(LineList, AutoScrollY + ManualScrollY );
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                ManualScrollY = 0;
                Canvas.SetTop(LineList, AutoScrollY);
            }
        }
    }
}
