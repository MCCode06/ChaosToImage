using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ImageParticleSimulatorWPF.Models
{
    public class Ball : INotifyPropertyChanged
    {
        private Point _position;
        public Point Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
            }
        }

        private Vector _velocity;
        public Vector Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                OnPropertyChanged(nameof(Velocity));
            }
        }

        public Vector FiredVelocity { get; set; }

        private double _radius = 5;
        public double Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                OnPropertyChanged(nameof(Radius));
            }
        }

        private Color _color = Colors.White;
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
                OnPropertyChanged(nameof(Brush));
            }
        }

        public SolidColorBrush Brush => new SolidColorBrush(Color);

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
