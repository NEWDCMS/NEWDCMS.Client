using System;
using System.Globalization;
using Xamarin.Forms;
namespace Wesley.Client.Converters
{

    public class SwichConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int type = (int)value;
            return type switch
            {
                0 => false,
                1 => true,
                2 => true,
                3 => true,
                4 => true,
                _ => true,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int type = (int)value;
            return type switch
            {
                0 => false,
                1 => true,
                2 => true,
                3 => true,
                4 => true,
                _ => true,
            };
        }
    }
}
