using Com.Baidu.Mapapi.Model;
using Wesley.Client.BaiduMaps;
namespace Wesley.Client.Droid
{
    internal static class LatLngEx
    {
        public static Coordinate ToUnity(this LatLng coor)
        {
            return new Coordinate(coor.Latitude, coor.Longitude);
        }
    }
}

