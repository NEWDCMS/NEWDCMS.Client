using Android.App;
using Com.Baidu.Mapapi;
namespace Wesley.Client.Droid
{
    public static class FormsBaiduMaps
    {
        public static void Init(Application application)
        {
            SDKInitializer.Initialize(application);
            //包括BD09LL和GCJ02两种坐标，默认是BD09LL坐标。
            SDKInitializer.CoordType = Com.Baidu.Mapapi.CoordType.Bd09ll;
        }
    }
}
