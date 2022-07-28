using System;
using System.Globalization;
using System.Reflection;
using Xamarin.Forms;
namespace DCMS.Client.Styles.ValueConverters
{
    public class ImageResourceValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value as string == null)
                return null;

            var imageSrc = ImageSource.FromResource($"DCMS.Client.Resources.Images.{value.ToString()}", typeof(ImageResourceValueConverter).GetTypeInfo().Assembly);
            return imageSrc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
             return null;
        }
    }
}
