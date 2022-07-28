using System;
using System.Globalization;
using Xamarin.Forms;
namespace Wesley.Client.Converters
{

    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime == null)
                {
                    return "";
                }
                if (parameter is string pattern)
                {
                    return dateTime.ToString(pattern);
                }
                else
                {
                    return dateTime.ToString();
                }
            }
            else if (value is TimeSpan timeSpan)
            {
                if (timeSpan == null)
                {
                    return "";
                }
                if (parameter is string pattern)
                {
                    return timeSpan.ToString(pattern);
                }
                else
                {
                    return timeSpan.ToString();
                }
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
