using System;
using System.Globalization;
using Xamarin.Forms;
namespace Wesley.Client.Converters
{
    //a % b == 0 SwichConverter
    public class EqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int type = (int)value;
            int.TryParse(parameter.ToString(), out int targetValue);
            return type.Equals(targetValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int type = (int)value;
            int.TryParse(parameter.ToString(), out int targetValue);
            return type.Equals(targetValue);
        }
    }
}
