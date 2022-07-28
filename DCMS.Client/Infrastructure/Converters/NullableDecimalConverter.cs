using System;
using System.Globalization;
using Xamarin.Forms;

namespace Wesley.Client.Converters
{
    public class NullableDecimalConverter : IValueConverter
    {
        //源属性传给目标属性时
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var nullable = value as decimal?;
            var result = "0.00";
            if (nullable.HasValue && nullable.Value != 0)
            {
                result = nullable.Value.ToString("#0.00");
            }
            return result;
        }

        //目标属性传给源属性时
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            decimal? result = null;
            if (decimal.TryParse(stringValue, out decimal intValue))
            {
                result = new decimal?(intValue);
            }
            return result;
        }
    }
}



