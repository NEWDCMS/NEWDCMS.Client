using System;
using System.Globalization;
using Xamarin.Forms;
namespace Wesley.Client.Converters
{

    public class DateTimeEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = string.Empty;
            try
            {
                var format = parameter as string;
                var nullable = value as DateTime?;
                if (nullable.HasValue)
                {
                    result = nullable.Value.ToString(string.IsNullOrEmpty(format) ? "HH:mm" : format);
                }
            }
            catch (Exception) { }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            DateTime? result = null;
            if (DateTime.TryParse(stringValue, out DateTime dateValue))
            {
                result = new Nullable<DateTime>(dateValue);
            }
            return result;
        }
    }

}



