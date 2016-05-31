using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Shell
{
    public class BaseShape : Control, IShape
    {
        public BaseShape()
        {
            
        }

        public BaseShape(Point location, Size size)
        {
            Location = location;
            Width = size.Width;
            Height = size.Height;
        }

        public virtual string ShapeName { get { return "Shape"; } }

        private AdornerLayer aLayer;
        private Point _location;

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    if (_isSelected)
                    {
                        aLayer = AdornerLayer.GetAdornerLayer(this);
                        aLayer.Add(new ResizingAdorner(this));
                    }
                    else
                    {
                        var adorners = aLayer.GetAdorners(this);
                        if (adorners != null && adorners.Length > 0)
                        {
                            aLayer.Remove(adorners[0]);
                        }
                    }
                }
            }
        }

        public Point Location
        {
            get { return _location; }
            set
            {
                _location = value;
                Canvas.SetLeft(this, _location.X);
                Canvas.SetTop(this, _location.Y);
            }
        }
    }

    public interface IShape
    {
        string ShapeName { get; }
        bool IsSelected { get; set; }
        Point Location { get; set; }
    }
}
