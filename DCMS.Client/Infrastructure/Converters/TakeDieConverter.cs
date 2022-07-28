using System;
using System.Globalization;
using Xamarin.Forms;
namespace Wesley.Client.Converters
{
    public class TakeDieConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int type = (int)value;
            int.TryParse(parameter.ToString(), out int targetValue);
            return ((type != 0) && type % targetValue == 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int type = (int)value;
            int.TryParse(parameter.ToString(), out int targetValue);
            return ((type != 0) && type % targetValue == 0);
        }
    }



}
