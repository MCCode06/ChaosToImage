using System;
using System.Globalization;
using System.Windows.Data;

namespace ImageParticleSimulatorWPF.ViewModels
{
    public class DoubleToDiameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double radius)
                return radius * 2;

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
