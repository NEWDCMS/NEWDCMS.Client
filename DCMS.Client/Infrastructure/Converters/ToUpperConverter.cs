using System;
using System.Globalization;
using Xamarin.Forms;
namespace Wesley.Client.Converters
{
    public class ToUpperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str.ToUpper();
            }

            return "error!";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str.ToUpper();
            }

            return "error!";
        }
    }
}