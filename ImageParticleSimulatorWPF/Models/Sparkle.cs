using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ImageParticleSimulatorWPF.Models
{
    public class Sparkle
    {
        public Point Position { get; set; }
        public Brush Color { get; set; } = Brushes.White;
        public double Size { get; set; } = 6.0;
    }
}
