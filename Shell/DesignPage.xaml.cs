using System;
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

namespace Shell
{
    /// <summary>
    /// Interaction logic for DesignPage.xaml
    /// </summary>
    public partial class DesignPage : UserControl
    {
        private Measurement _measurement = Measurement.Inch;

        public DesignPage()
        {
            InitializeComponent();
            SizeChanged += DesignPage_SizeChanged;
        }

        private void DesignPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var transform = DrawingSurface.TransformToAncestor(this).Transform(new Point(0, 0));
            HorizontalRuler.Anchor = transform.X - VerticalRuler.ActualWidth;
            VerticalRuler.Anchor = transform.Y - HorizontalRuler.ActualHeight;
        }

        public Measurement Measurement
        {
            get { return _measurement; }
            set
            {
                _measurement = value;
                HorizontalRuler.Measurement = value;
                VerticalRuler.Measurement = value;
            }
        }
    }
}
