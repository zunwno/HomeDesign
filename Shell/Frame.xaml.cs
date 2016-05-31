using System.Windows;
using System.Windows.Media;

namespace Shell
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Frame 
    {
        public Frame(Point location, Size size) : base(location, size)
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness", typeof (Thickness), typeof (Frame), new PropertyMetadata(new Thickness(10)));

        public Thickness Thickness
        {
            get { return (Thickness) GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof (Brush), typeof (Frame), new PropertyMetadata(new BrushConverter().ConvertFromString("#707070")));

        public Brush Color
        {
            get { return (Brush) GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
    }
}
