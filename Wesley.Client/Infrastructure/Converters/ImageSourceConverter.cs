using System;
using System.Globalization;
using System.Reflection;
using Xamarin.Forms;
namespace Wesley.Client.Converters
{
    /// <summary>
    /// 资源图片地址加载转换
    /// </summary>
    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
            {
                return default(ImageSource);
            }

            var path = value.ToString();

            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                return ImageSource.FromResource(path);
            }
            else
            {
                var assembly = typeof(App).GetTypeInfo().Assembly;
                return ImageSource.FromResource(path, assembly);
            }
        }

        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    string filename = value as string;
        //    return ImageSource.FromStream(() => new MemoryStream(DependencyService.Get<IWRDependencyService>().GetImageBytes(filename)));
        //}

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }

    //class ImageLoader : IWRDependencyService
    //{
    //    public byte[] GetImageBytes(string fileName)
    //    {
    //        fileName = fileName.Replace(".jpg", "").Replace(".png", "");
    //        var resId = Forms.Context.Resources.GetIdentifier(
    //          fileName.ToLower(), "drawable", Forms.Context.PackageName);
    //        var icon = BitmapFactory.DecodeResource(Forms.Context.Resources, resId);
    //        var ms = new MemoryStream();

    //        icon.Compress(Bitmap.CompressFormat.Png, 0, ms);
    //        byte[] bitmapData = ms.ToArray();
    //        return bitmapData;
    //    }
    //}
}
