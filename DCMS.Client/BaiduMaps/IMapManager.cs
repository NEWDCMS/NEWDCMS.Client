namespace Wesley.Client.BaiduMaps
{

    /// <summary>
    /// 默认为bd09经纬度坐标
    /// </summary>
    public enum CoordType
    {
        BD09LL,
        GCJ02
    }

    public interface IMapManager
    {
        CoordType CoordinateType { get; set; }
    }
}
