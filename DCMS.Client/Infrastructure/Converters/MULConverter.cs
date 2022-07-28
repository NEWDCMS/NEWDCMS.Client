using System;
using Xamarin.Forms;
namespace Wesley.Client.Converters
{
    public class MULConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrWhiteSpace(value.ToString()))
            {
                double o = 0;
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                {
                    double.TryParse(value.ToString(), out o);
                }

                double t = 0;
                if (!string.IsNullOrWhiteSpace(parameter.ToString()))
                {
                    double.TryParse(parameter.ToString(), out t);
                }

                return o * t;
            }
            else
            {
                return 0;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrWhiteSpace(value.ToString()))
            {
                double o = 0;
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                {
                    double.TryParse(value.ToString(), out o);
                }

                double t = 0;
                if (!string.IsNullOrWhiteSpace(parameter.ToString()))
                {
                    double.TryParse(parameter.ToString(), out t);
                }

                return o * t;
            }
            else
            {
                return 0;
            }
        }
    }
}



