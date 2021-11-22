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
            try
            {
                if (!(value is double))
                {
                    value = (double)0;
                }

                double meters = (double)value;

                return (meters / 1000);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
