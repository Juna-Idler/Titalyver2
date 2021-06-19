using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections.Generic;


namespace Titalyver2
{
    /// <summary>
    /// </summary>
    public class OutlineGlyph : FrameworkElement
    {
        private static readonly GlyphTypeface system_gtf;

        static OutlineGlyph()
        {
//            DefaultStyleKeyProperty.OverrideMetadata(typeof(OutlineGlyphs), new FrameworkPropertyMetadata(typeof(OutlineGlyphs)));

            foreach (Typeface typeface in SystemFonts.MessageFontFamily.GetTypefaces())
            {
                if (typeface.TryGetGlyphTypeface(out system_gtf))
                {
                    break;
                }
            }

        }


        private GlyphTypeface glyphTypeFace;
        private string[] GraphemeArray;
        private Geometry GlyphGeometry;

        public OutlineGlyph()
        {
            glyphTypeFace = system_gtf;

        }


        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(OutlineGlyph), new FrameworkPropertyMetadata(OnGlyphTextInvalidated));
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill", typeof(Brush), typeof(OutlineGlyph), new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush Fill { get => (Brush)GetValue(FillProperty); set => SetValue(FillProperty, value); }

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke", typeof(Brush), typeof(OutlineGlyph), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush Stroke { get => (Brush)GetValue(StrokeProperty); set => SetValue(StrokeProperty, value); }

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(OutlineGlyph), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        public double StrokeThickness { get => (double)GetValue(StrokeThicknessProperty); set => SetValue(StrokeThicknessProperty, value); }




        public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(
            typeof(OutlineGlyph), new FrameworkPropertyMetadata(OnGlyphTextUpdated));
        public FontFamily FontFamily { get => (FontFamily)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }


        public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(
            typeof(OutlineGlyph), new FrameworkPropertyMetadata(OnGlyphTextUpdated));
        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }



        public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(
            typeof(OutlineGlyph), new FrameworkPropertyMetadata(OnGlyphTextUpdated));
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(
            typeof(OutlineGlyph), new FrameworkPropertyMetadata(OnGlyphTextUpdated));
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }


        public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(
            typeof(OutlineGlyph), new FrameworkPropertyMetadata(OnGlyphTextUpdated));
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }






        protected override void OnRender(DrawingContext drawingContext)
        {
            EnsureGeometry();

            Pen pen = new(Stroke, StrokeThickness);
            pen.LineJoin = PenLineJoin.Round;

            drawingContext.DrawGeometry(null, pen, GlyphGeometry);
            drawingContext.DrawGeometry(Fill, null, GlyphGeometry);
        }



        private static void OnGlyphTextInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OutlineGlyph outlinedGlyph = (OutlineGlyph)dependencyObject;
            outlinedGlyph.GlyphGeometry = null;
            outlinedGlyph.GraphemeArray = null;

            outlinedGlyph.InvalidateMeasure();
            outlinedGlyph.InvalidateVisual();
        }

        private static void OnGlyphTextUpdated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OutlineGlyph outlinedGlyph = (OutlineGlyph)dependencyObject;
            outlinedGlyph.GlyphGeometry = null;

            outlinedGlyph.InvalidateMeasure();
            outlinedGlyph.InvalidateVisual();
        }

        private void EnsureGraphemeArrayt()
        {
            if (GraphemeArray != null || Text == null)
                return;

            UpdateText();
        }

        private void UpdateText()
        {
            TextElementEnumerator erator = StringInfo.GetTextElementEnumerator(Text);
            int count = 0;
            while (erator.MoveNext())
                count++;
            GraphemeArray = new string[count];
            erator = StringInfo.GetTextElementEnumerator(Text);
            count = 0;
            while (erator.MoveNext())
                GraphemeArray[count++] = erator.GetTextElement();

        }

        private void EnsureGeometry()
        {
            if (GlyphGeometry != null)
                return;

            EnsureGraphemeArrayt();
            GeometryGroup group = new();
            double x = 0;
            foreach (string grapheme in GraphemeArray)
            {
                string remain = grapheme;

                do
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
//                    try
                    {
                        ushort index = 0;
                        glyphTypeFace.CharacterToGlyphMap.TryGetValue(code, out index);
//                        ushort index = glyphTypeFace.CharacterToGlyphMap[code];
                        Geometry g = glyphTypeFace.GetGlyphOutline(index, FontSize, FontSize);
                        TranslateTransform transform = new(x, 0);
                        g.Transform = transform;
                        x += glyphTypeFace.AdvanceWidths[index] * FontSize;
                        group.Children.Add(g);
                    }
//                    catch (KeyNotFoundException e){}
                }
                while (remain != "");
            }
            TranslateTransform translateTransform = new(0, glyphTypeFace.Baseline * FontSize);
            group.Transform = translateTransform;
             GlyphGeometry = group;
        }
    }
}
