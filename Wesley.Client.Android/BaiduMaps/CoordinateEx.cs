using Com.Baidu.Mapapi.Model;
using Wesley.Client.BaiduMaps;
namespace Wesley.Client.Droid
{
    internal static class CoordinateEx
    {
        public static LatLng ToNative(this Coordinate coor)
        {
            return new LatLng(coor.Latitude, coor.Longitude);
        }
    }
}

