using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Shell
{
    public class DrawingSurface : Canvas
    {
        public static readonly DependencyProperty MeasurementProperty = DependencyProperty.Register(
            "Measurement", typeof (Measurement), typeof (DrawingSurface), new FrameworkPropertyMetadata(Measurement.Inch));
        public Measurement Measurement
        {
            get { return (Measurement) GetValue(MeasurementProperty); }
            set { SetValue(MeasurementProperty, value); }
        }

        public double PageWidth
        {
            get
            {
                switch (Measurement)
                {
                    case Measurement.Centimeter:
                        return UnitConverter.PxToCm(Width);
                    default:
                        return UnitConverter.PxToInch(Width);
                }
            }
            set
            {
                switch (Measurement)
                {
                    case Measurement.Centimeter:
                        Width = UnitConverter.CmToPx(value);
                        break;
                    default:
                        Width = UnitConverter.InchToPx(value);
                        break;
                }
            }
        }
        public double PageHeight
        {
            get
            {
                switch (Measurement)
                {
                    case Measurement.Centimeter:
                        return UnitConverter.PxToCm(Height);
                    default:
                        return UnitConverter.PxToInch(Height);
                }
            }
            set
            {
                switch (Measurement)
                {
                    case Measurement.Centimeter:
                        Height = UnitConverter.CmToPx(value);
                        break;
                    default:
                        Height = UnitConverter.InchToPx(value);
                        break;
                }
            }
        }
        private const int Min = 10;


        public List<IShape> SelectedShapes { get; private set; }
        public DrawingSurface()
        {
            ClipToBounds = true;
            SelectedShapes = new List<IShape>();
            PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            PreviewMouseMove += OnPreviewMouseMove;
            PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
        }

        private Point _downPoint;
        private Point _upPoint;
        private Rectangle _drawingHelper;

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMouseCaptured)
            {
                _downPoint = e.GetPosition(this);
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (Cursor == Cursors.Cross)
                    {
                    }
                    else if (Cursor == Cursors.Arrow)
                    {
                        var shape = GetShapeAtLocation(_downPoint);
                        if (shape != null)
                        {
                            if (!shape.IsSelected)
                            {
                                foreach (var selectedShape in SelectedShapes)
                                {
                                    selectedShape.IsSelected = false;
                                }
                                SelectedShapes.Clear();
                                SelectedShapes.Add(shape);
                                shape.IsSelected = true;
                            }
                            Cursor = Cursors.SizeAll;
                            e.Handled = true;
                        }
                        else
                        {
                            foreach (var selectedShape in SelectedShapes)
                            {
                                selectedShape.IsSelected = false;
                            }
                            SelectedShapes.Clear();
                            e.Handled = true;
                        }
                    }
                }
                CaptureMouse();
            }
        }

        private IShape GetShapeAtLocation(Point downPoint)
        {
            var hitTestResult = VisualTreeHelper.HitTest(this, downPoint);
            var visualHit = hitTestResult.VisualHit;
            if (visualHit == null) return null;
            if (Equals(visualHit, this)) return null;
            var shape = visualHit as IShape;
            while (shape == null)
            {
                visualHit = VisualTreeHelper.GetParent(visualHit);
                if (Equals(visualHit, this)) return null;

                shape = visualHit as IShape;
            }

            return shape;
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(this);
            if (IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                if (Cursor == Cursors.Cross)
                {
                    if (_drawingHelper == null)
                    {
                        _drawingHelper = new Rectangle();
                        _drawingHelper.StrokeThickness = 1.0;
                        _drawingHelper.StrokeDashArray = new DoubleCollection {3, 2};
                        _drawingHelper.Stroke = Brushes.Blue;
                        Children.Add(_drawingHelper);
                    }

                    var w = point.X - _downPoint.X;
                    var h = point.Y - _downPoint.Y;

                    if (w > 0 && h > 0)
                    {
                        SetLeft(_drawingHelper, _downPoint.X);
                        SetTop(_drawingHelper, _downPoint.Y);
                        _drawingHelper.Width = w;
                        _drawingHelper.Height = h;
                    }
                    else if (w > 0 && h < 0)
                    {
                        SetLeft(_drawingHelper, _downPoint.X);
                        _drawingHelper.Width = w;

                        var top = _downPoint.Y + h;
                        if (top < 0)
                        {
                            top = 0;
                            h = -_downPoint.Y;
                        }
                        SetTop(_drawingHelper, top);
                        _drawingHelper.Height = -h;
                    }
                    else if (w < 0 && h > 0)
                    {
                        var left = _downPoint.X + w;
                        if (left < 0)
                        {
                            left = 0;
                            w = -_downPoint.X;
                        }
                        SetLeft(_drawingHelper, left);
                        _drawingHelper.Width = -w;

                        SetTop(_drawingHelper, _downPoint.Y);
                        _drawingHelper.Height = h;
                    }

                    else if (w < 0 && h < 0)
                    {
                        var left = _downPoint.X + w;
                        if (left < 0)
                        {
                            left = 0;
                            w = -_downPoint.X;
                        }
                        SetLeft(_drawingHelper, left);
                        _drawingHelper.Width = -w;

                        var top = _downPoint.Y + h;
                        if (top < 0)
                        {
                            top = 0;
                            h = -_downPoint.Y;
                        }
                        SetTop(_drawingHelper, top);
                        _drawingHelper.Height = -h;
                    }
                }
                else if (Cursor == Cursors.SizeAll) //
                {
                    foreach (IShape shape in SelectedShapes)
                    {
                        var newLeft = shape.Location.X + point.X - _downPoint.X;
                        var newTop = shape.Location.Y + point.Y - _downPoint.Y;
                        if (newLeft < 0) newLeft = 0;
                        if (newTop < 0) newTop = 0;
                        SetLeft((UIElement) shape, newLeft);
                        SetTop((UIElement) shape, newTop);
                    }
                }
            }
            else
            {
                var shape = GetShapeAtLocation(point);
                if (shape != null)
                {
                    if (shape.IsSelected)
                    {
                        Cursor = Cursors.SizeAll;
                    }
                    else
                    {
                        if(Cursor != Cursors.Cross)
                            Cursor = Cursors.Arrow;
                    }
                }
                else
                {
                    if (Cursor != Cursors.Cross)
                        Cursor = Cursors.Arrow;
                }
            }
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                var point = e.GetPosition(this);
                if (point == _downPoint)
                {
                    if (Cursor == Cursors.Cross)
                    {
                        var shape = GetShapeAtLocation(_downPoint);
                        if (shape != null)
                        {
                            if (!shape.IsSelected)
                            {
                                foreach (var selectedShape in SelectedShapes)
                                {
                                    selectedShape.IsSelected = false;
                                }
                                SelectedShapes.Clear();
                                SelectedShapes.Add(shape);
                                shape.IsSelected = true;
                            }
                            e.Handled = true;
                        }
                        else
                        {
                            foreach (var selectedShape in SelectedShapes)
                            {
                                selectedShape.IsSelected = false;
                            }
                            SelectedShapes.Clear();
                            e.Handled = true;
                        }
                    }

                    Cursor = Cursors.Arrow;
                }
                else
                {
                    if (Cursor == Cursors.Cross)
                    {
                        Children.Remove(_drawingHelper);
                        if (IsValidDrawing(_drawingHelper))
                        {
                            var shape = new Frame(new Point(GetLeft(_drawingHelper), GetTop(_drawingHelper)), new Size(_drawingHelper.Width, _drawingHelper.Height));
                            Children.Add(shape);
                            foreach (var selectedShape in SelectedShapes)
                            {
                                selectedShape.IsSelected = false;
                            }
                            SelectedShapes.Clear();
                            SelectedShapes.Add(shape);
                            shape.IsSelected = true;
                            Cursor = Cursors.Arrow;
                        }
                    }
                    else if (Cursor == Cursors.SizeAll)
                    {
                        foreach (IShape shape in SelectedShapes)
                        {
                            shape.Location = new Point(GetLeft((UIElement)shape), GetTop((UIElement)shape));
                        }
                    }
                }
                _drawingHelper = null;
                ReleaseMouseCapture();
            }
        }

        private bool IsValidDrawing(Rectangle drawingHelper)
        {
            if (drawingHelper == null) return false;
            if (double.IsNaN(drawingHelper.Width) || double.IsNaN(drawingHelper.Height) || drawingHelper.Width < Min || drawingHelper.Height < Min) return false;
            return true;
        }

        private Type _drawingType;
        public void InitDrawing(Type type)
        {
            _drawingType = type;
            Cursor = Cursors.Cross;
        }
    }

    public class ResizingAdorner : Adorner
    {
        // Resizing adorner uses Thumbs for visual elements.  
        // The Thumbs have built-in mouse input handling.
        Thumb topLeft, topRight, bottomLeft, bottomRight;

        // To store and manage the adorner's visual children.
        VisualCollection visualChildren;

        // Initialize the ResizingAdorner.
        public ResizingAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            // Call a helper method to initialize the Thumbs
            // with a customized cursors.
            BuildAdornerCorner(ref topLeft, Cursors.SizeNWSE);
            BuildAdornerCorner(ref topRight, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomLeft, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomRight, Cursors.SizeNWSE);

            // Add handlers for resizing.
            bottomLeft.DragDelta += new DragDeltaEventHandler(HandleBottomLeft);
            bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
            topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeft);
            topRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);
        }

        // Handler for resizing from the bottom-right.
        void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-right.
        void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);

            double height_old = adornedElement.Height;
            double height_new = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
            double top_old = Canvas.GetTop(adornedElement);
            adornedElement.Height = height_new;
            Canvas.SetTop(adornedElement, top_old - (height_new - height_old));
        }

        // Handler for resizing from the top-left.
        void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);

            double width_old = adornedElement.Width;
            double width_new = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            double left_old = Canvas.GetLeft(adornedElement);
            adornedElement.Width = width_new;
            Canvas.SetLeft(adornedElement, left_old - (width_new - width_old));

            double height_old = adornedElement.Height;
            double height_new = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
            double top_old = Canvas.GetTop(adornedElement);
            adornedElement.Height = height_new;
            Canvas.SetTop(adornedElement, top_old - (height_new - height_old));
        }

        // Handler for resizing from the bottom-left.
        void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);

            double width_old = adornedElement.Width;
            double width_new = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            double left_old = Canvas.GetLeft(adornedElement);
            adornedElement.Width = width_new;
            Canvas.SetLeft(adornedElement, left_old - (width_new - width_old));
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            // desiredWidth and desiredHeight are the width and height of the element that's being adorned.  
            // These will be used to place the ResizingAdorner at the corners of the adorned element.  
            double desiredWidth = AdornedElement.DesiredSize.Width;
            double desiredHeight = AdornedElement.DesiredSize.Height;
            // adornerWidth & adornerHeight are used for placement as well.
            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;

            topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            topRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            bottomLeft.Arrange(new Rect(-adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            bottomRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));

            // Return the final size.
            return finalSize;
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics.
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 5;
            cornerThumb.Background = Brushes.Transparent;

            visualChildren.Add(cornerThumb);
        }

        // This method ensures that the Widths and Heights are initialized.  Sizing to content produces
        // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height
        // need to be set first.  It also sets the maximum size of the adorned element.
        void EnforceSize(FrameworkElement adornedElement)
        {
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = adornedElement.DesiredSize.Width;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = adornedElement.DesiredSize.Height;

            FrameworkElement parent = adornedElement.Parent as FrameworkElement;
            if (parent != null)
            {
                adornedElement.MaxHeight = parent.ActualHeight;
                adornedElement.MaxWidth = parent.ActualWidth;
            }
        }
        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }
}