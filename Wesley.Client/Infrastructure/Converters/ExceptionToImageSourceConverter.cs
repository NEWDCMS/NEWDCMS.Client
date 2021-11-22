using System;
using System.Globalization;
using Xamarin.Forms;
namespace Wesley.Client.Converters
{
    public class ExceptionToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var imageName = value switch
            {
                ServerException _ => "the_internet.gif",
                NetworkException _ => "the_internet.gif",
                _ => "the_internet.gif",
            };
            return ImageSource.FromFile(imageName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-Way converter only
             return null;
        }
    }
}
