using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Shell
{
    public enum Measurement
    {
        Inch,
        Centimeter
    }

    public abstract class Ruler : FrameworkElement
    {
        protected double _measurement = PixelUnitFactor.Inch;
        public static readonly DependencyProperty MeasurementProperty = DependencyProperty.Register(
            "Measurement", typeof(Measurement), typeof(Ruler), new FrameworkPropertyMetadata(Measurement.Inch,
                        FrameworkPropertyMetadataOptions.None,
                        OnMeasurementChanged));

        private static void OnMeasurementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ruler = ((Ruler)d);
            var measurement = ruler.Measurement;
            switch (measurement)
            {
                case Measurement.Inch:
                    ruler._measurement = UnitConverter.InchToPx(1);
                    ruler.InvalidateVisual();
                    break;
                case Measurement.Centimeter:
                    ruler._measurement = UnitConverter.CmToPx(1);
                    ruler.InvalidateVisual();
                    break;
            }
        }

        public Measurement Measurement
        {
            get { return (Measurement)GetValue(MeasurementProperty); }
            set { SetValue(MeasurementProperty, value); }
        }

        public static readonly DependencyProperty AnchorProperty = DependencyProperty.Register("Anchor", typeof(double), typeof(Ruler), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, OnAnchorChanged));

        private static void OnAnchorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Ruler)d).InvalidateVisual();
        }

        public double Anchor
        {
            get { return (double)GetValue(AnchorProperty); }
            set { SetValue(AnchorProperty, value); }
        }

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Ruler), new FrameworkPropertyMetadata(new BrushConverter().ConvertFromString("#383838"), FrameworkPropertyMetadataOptions.None, OnBackgroundChanged));

        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Ruler)d).InvalidateVisual();
        }

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(Ruler), new FrameworkPropertyMetadata(new BrushConverter().ConvertFromString("#969696"), FrameworkPropertyMetadataOptions.None, OnForegroundChanged));

        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Ruler)d).InvalidateVisual();
        }

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        protected Ruler()
        {
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            ClipToBounds = true;
        }
    }

    public class HRuler : Ruler
    {
        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Background, null, new Rect(0, 0, ActualWidth, ActualHeight));

            int i = 0;
            var n = _measurement / 8;

            for (double d = Anchor; d < ActualWidth; d += _measurement)
            {
                dc.DrawLine(new Pen(Foreground, 1), new Point(d, 0), new Point(d, ActualHeight));
                int ntc = 0;
                for (double j = n; j < _measurement; j += n)
                {
                    var w = ntc == 3 ? 8 : (ntc%2 == 0 ? 4 : 6);
                    dc.DrawLine(new Pen(Foreground, 1), new Point(d + j, ActualHeight - w), new Point(d + j, ActualHeight));
                    ntc++;
                }
                dc.DrawText(new FormattedText(i.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 8, Foreground), new Point(d + 4, 1));
                i++;
            }

            i = 1;
            for (double d = Anchor - _measurement; d >= -_measurement; d -= _measurement)
            {
                dc.DrawLine(new Pen(Foreground, 1), new Point(d, 0), new Point(d, ActualHeight)); int ntc = 0;
                for (double j = n; j < _measurement; j += n)
                {
                    var w = ntc == 3 ? 8 : (ntc % 2 == 0 ? 4 : 6);
                    dc.DrawLine(new Pen(Foreground, 1), new Point(d + j, ActualHeight - w), new Point(d + j, ActualHeight));
                    ntc++;
                }
                dc.DrawText(new FormattedText(i.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 8, Foreground), new Point(d + 4, 1));
                i++;
            }

            dc.DrawLine(new Pen(Foreground, 1), new Point(0, ActualHeight), new Point(ActualWidth, ActualHeight));
        }
    }

    public class VRuler : Ruler
    {
        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Background, null, new Rect(0, 0, ActualWidth, ActualHeight));

            int i = 0;
            var n = _measurement/8;

            for (double d = Anchor; d < ActualHeight; d += _measurement)
            {
                dc.DrawLine(new Pen(Foreground, 1), new Point(0, d), new Point(ActualWidth, d));
                int ntc = 0;
                for (double j = n; j < _measurement; j += n)
                {
                    var w = ntc == 3 ? 8 : (ntc % 2 == 0 ? 4 : 6);
                    dc.DrawLine(new Pen(Foreground, 1), new Point(ActualWidth - w, d + j), new Point(ActualWidth, d + j));
                    ntc ++;
                }
                if (i < 10)
                {
                    dc.DrawText(new FormattedText(i.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 8, Foreground), new Point(ActualWidth - 15, d + 2));
                }
                else
                {
                    var charArray = i.ToString(CultureInfo.CurrentCulture).ToCharArray();
                    int zt = 0;
                    foreach (var item in charArray)
                    {
                        dc.DrawText(new FormattedText(item.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 8, Foreground), new Point(ActualWidth - 15, d + 2 + zt));
                        zt += 8;
                    }
                }
                i++;
            }

            i = 1;
            for (double d = Anchor - _measurement; d >= -_measurement; d -= _measurement)
            {
                dc.DrawLine(new Pen(Foreground, 1), new Point(0, d), new Point(ActualWidth, d));
                int ntc = 0;
                for (double j = n; j < _measurement; j += n)
                {
                    var w = ntc == 3 ? 8 : (ntc % 2 == 0 ? 4 : 6);
                    dc.DrawLine(new Pen(Foreground, 1), new Point(ActualWidth - w, d + j), new Point(ActualWidth, d + j));
                    ntc++;
                }
                if (i < 10)
                {
                    dc.DrawText(new FormattedText(i.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 8, Foreground), new Point(ActualWidth - 15, d + 2));
                }
                else
                {
                    var charArray = i.ToString(CultureInfo.CurrentCulture).ToCharArray();
                    int zt = 0;
                    foreach (var item in charArray)
                    {
                        dc.DrawText(new FormattedText(item.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 8, Foreground), new Point(ActualWidth - 15, d + 2 + zt));
                        zt += 8;
                    }
                }
                i++;
            }

            dc.DrawLine(new Pen(Foreground, 1), new Point(ActualWidth, 0), new Point(ActualWidth, ActualHeight));
        }
    }

    internal struct PixelUnitFactor
    {
        public const double Inch = 96.0;
        public const double Cm = 37.7952755905512;
    }

    public static class UnitConverter
    {
        public static double CmToPx(double cm)
        {
            return cm*PixelUnitFactor.Cm;
        }

        public static double InchToPx(double inch)
        {
            return inch * PixelUnitFactor.Inch;
        }

        public static double PxToCm(double px)
        {
            return px/PixelUnitFactor.Cm;
        }

        public static double PxToInch(double inch)
        {
            return inch / PixelUnitFactor.Inch;
        }
    }
}
