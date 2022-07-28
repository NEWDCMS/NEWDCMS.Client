using Wesley.Client.Models.Products;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Wesley.Client.Converters
{

    public class StringArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string[] strings && parameter is string format)
            {
                try
                {
                    format = "";
                    for (var i = 0; i < strings.Length; i++)
                    {
                        format += "{" + i + "} ";
                    }
                    return string.Format(format, strings);
                }
                catch (Exception)
                {
                }
            }
            return string.Empty;
        }

        //Must implement this if Binding with Mode=TwoWay
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
             return null;
        }
    }


    public class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2)
            {
                int.TryParse((values[0] ?? 0).ToString(), out int p);
                int.TryParse((values[1] ?? 0).ToString(), out int type);
                return new MultiValue { Pid = p, Type = type };
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
             return null;
        }
    }
}
