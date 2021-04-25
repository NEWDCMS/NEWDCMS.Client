using System;
using System.Globalization;
using Xamarin.Forms;
namespace Wesley.Client.Converters
{

    /// <summary>
    ///  距离(米转换迈)
    /// </summary>
    public class MetersToMilesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double))
            {
                throw new InvalidOperationException("The target must be a int");
            }

            double meters = (double)value;

            return (meters / 1000);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
