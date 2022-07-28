using System;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Wesley.Client.Converters
{

    public class WrongBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }
    public class PercentageValueToUnicodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                //&#xf200;//var  s= "\uf07a";
                var str = value.ToString().Substring(3, 4);
                return (char)System.Convert.ToInt32(str, 16) + "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string ToUnicodeString(string str)
        {
            StringBuilder strResult = new StringBuilder();
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    strResult.Append("\\u");
                    strResult.Append(((int)str[i]).ToString("x"));
                }
            }
            return strResult.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

