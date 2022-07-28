using System;

namespace Wesley.Infrastructure.Helpers
{
    /// <summary>
    /// 用于计算经纬度坐标之间的距离
    /// </summary>
    public static class MapHelper
    {

        //地球半径，单位米
        private const double EARTH_RADIUS = 6378137;
        /// <summary>
        /// 计算两点位置的距离，返回两点的距离，单位 米
        /// 该公式为GOOGLE提供，误差小于0.2米
        /// </summary>
        /// <param name="lat1">第一点纬度</param>
        /// <param name="lng1">第一点经度</param>
        /// <param name="lat2">第二点纬度</param>
        /// <param name="lng2">第二点经度</param>
        /// <returns></returns>
        public static double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = lat1 * Math.PI / 180d;
            double radLng1 = lng1 * Math.PI / 180d;
            double radLat2 = lat2 * Math.PI / 180d;
            double radLng2 = lng2 * Math.PI / 180d;
            double a = radLat1 - radLat2;
            double b = radLng1 - radLng2;
            double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * 6378137;
            return result;
        }

        /// <summary>
        /// 经纬度转化成弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double Rad(double d)
        {
            return d * Math.PI / 180d;
        }
    }
}