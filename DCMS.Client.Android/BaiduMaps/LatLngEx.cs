using Com.Baidu.Mapapi.Model;
using DCMS.Client.BaiduMaps;
namespace DCMS.Client.Droid
{
    internal static class LatLngEx
    {
        public static Coordinate ToUnity(this LatLng coor)
        {
            return new Coordinate(coor.Latitude, coor.Longitude);
        }
    }
}

