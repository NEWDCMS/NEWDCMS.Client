using System;
using System.Globalization;
using Xamarin.CommunityToolkit.Extensions.Internals;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Wesley.Client.Camera
{
    /// <summary>
    /// MediaCapturedEventArgs类型转换器
    /// </summary>
    public class MediaCapturedEventArgsConverter : ValueConverterExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is MediaCapturedEventArgs changedEventArgs
                ? changedEventArgs
                : throw new ArgumentException("应为MediaCapturedEventArgs类型的值", nameof(value));

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture){
             return null;
        }
    }
}