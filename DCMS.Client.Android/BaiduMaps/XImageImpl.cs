using Android.Graphics;
using Com.Baidu.Mapapi.Map;
using DCMS.Client.BaiduMaps;

namespace DCMS.Client.Droid
{
    internal static class XImageImpl
    {

        internal static BitmapDescriptor ToNative(this XImage image)
        {

            switch (image.Source)
            {
                default:
                    return null;
                case BaiduMaps.ImageSource.File:
                    //return BitmapDescriptorFactory.FromFile(image.FileName);
                    return BitmapDescriptorFactory.FromPath(image.FileName);
                case ImageSource.Bundle:
                    return BitmapDescriptorFactory.FromAsset(image.BundleName);
                case ImageSource.Resource:
                    {
                        //Resource.Drawable.q10660 not include .mp3
                        //int rsid = (int)typeof(Resource.Drawable).GetField(image.ResourceName).GetValue(null);
                        //return BitmapDescriptorFactory.FromResource(rsid);
                        //var rsid = Xamarin.Forms.Platform.Android.ResourceManager.g(image.ResourceId);
                        return BitmapDescriptorFactory.FromResource(image.ResourceId);
                    }
                case ImageSource.Stream:
                    return BitmapDescriptorFactory.FromBitmap(BitmapFactory.DecodeStream(image.Stream));
            }

        }
    }
}

