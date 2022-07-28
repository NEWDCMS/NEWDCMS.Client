using Com.Baidu.Mapapi.Model;
using DCMS.Client.BaiduMaps;
namespace DCMS.Client.Droid
{
    internal static class CoordinateEx
    {
        public static LatLng ToNative(this Coordinate coor)
        {
            return new LatLng(coor.Latitude, coor.Longitude);
        }
    }
}

