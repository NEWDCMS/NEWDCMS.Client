using Com.Baidu.Mapapi.Utils;
using Wesley.Client.BaiduMaps;
namespace Wesley.Client.Droid
{
    /// <summary>
    /// 距离计算
    /// </summary>
    public class CalculateUtilsImpl : ICalculateUtils
    {
        public double CalculateDistance(Coordinate p1, Coordinate p2)
        {
            return DistanceUtil.GetDistance(p1.ToNative(), p2.ToNative());
        }

        public void Converter(double lat, double lng)
        {
            //初始化坐标转换工具类，指定源坐标类型和坐标数据
            CoordinateConverter converter = new CoordinateConverter()
                    .From(CoordinateConverter.CoordType.Gps)
                    .Coord(new Com.Baidu.Mapapi.Model.LatLng(lat, lng));
            //desLatLng 转换后的坐标
            var lct = converter.Convert();
            System.Diagnostics.Debug.Print($"{lct.Latitude} {lct.Longitude}");
            GlobalSettings.UpdatePoi(lct.Latitude, lct.Longitude);
        }
    }
}

