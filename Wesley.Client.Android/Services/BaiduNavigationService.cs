using Android.Content;
using Wesley.Client.Services;
using System;

using Wesley.Client.Droid.Services;
[assembly: Xamarin.Forms.Dependency(typeof(BaiduNavigationService))]
namespace Wesley.Client.Droid.Services
{
    public class BaiduNavigationService : IBaiduNavigationService
    {
        /// <summary>
        /// 导航到
        /// </summary>
        /// <param name="latitude">目的地latitude</param>
        /// <param name="longitude">目的地longitude</param>
        /// <param name="addressName">目的地名称</param>
        public void OpenNavigationTo(double latitude, double longitude, string addressName)
        {
            try
            {
                var currentActivity = MainActivity.Instance;
                var uri = Android.Net.Uri.Parse("baidumap://map/direction?destination=latlng:" + latitude + "," + longitude + "|name:" + addressName + "&mode=driving");
                var mapIntent = new Intent(Intent.ActionView, uri);
                currentActivity?.StartActivity(mapIntent);
            }
            catch (Exception) { }
        }
    }
}