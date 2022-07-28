using System;
using System.Globalization;
using Xamarin.Forms;
namespace Wesley.Client.Converters
{

    public class NullableIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var nullable = value as int?;
            var result = string.Empty;
            if (nullable.HasValue && nullable.Value != 0)
            {
                result = nullable.Value.ToString();
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            int? result = null;

            if (int.TryParse(stringValue, out int intValue))
            {
                result = new int?(intValue);
            }

            return result ?? 0;
        }

    }

}



