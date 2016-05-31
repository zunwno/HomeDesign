using System.Windows;

namespace Shell
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnFrameClick(object sender, RoutedEventArgs e)
        {
            DesignPage.DrawingSurface.InitDrawing(typeof (Frame));
        }
    }
}