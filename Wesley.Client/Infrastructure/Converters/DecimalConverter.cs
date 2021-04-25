using System;
using System.Globalization;
using Xamarin.Forms;
namespace Wesley.Client.Converters
{
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            decimal thedecimal = (decimal)value;
            return thedecimal.ToString("#.00");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;

            if (string.IsNullOrEmpty(strValue))
            {
                strValue = "";
            }

            if (decimal.TryParse(strValue, out decimal resultdecimal))
            {
                return resultdecimal;
            }
            return null;
        }

    }
}



