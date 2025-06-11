using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageParticleSimulatorWPF.Views
{
    public partial class SimulationWindow : Window
    {
        public SimulationWindow(BitmapImage image)
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                var viewModel = new SimulationViewModel(BallCanvas.ActualWidth, BallCanvas.ActualHeight, 450, image);
                DataContext = viewModel;
            };
        }

    }
}
