using Com.Baidu.Mapapi;
using DCMS.Client.BaiduMaps;

namespace DCMS.Client.Droid
{
    public class MapManagerImpl : IMapManager
    {
        public BaiduMaps.CoordType CoordinateType
        {
            get
            {
                var type = SDKInitializer.CoordType;
                if (Com.Baidu.Mapapi.CoordType.Gcj02 == type)
                {
                    return BaiduMaps.CoordType.GCJ02;
                }
                return BaiduMaps.CoordType.BD09LL;
            }

            set
            {
                if (BaiduMaps.CoordType.GCJ02 == value)
                {
                    SDKInitializer.CoordType = Com.Baidu.Mapapi.CoordType.Gcj02;
                }
                else
                {
                    SDKInitializer.CoordType = Com.Baidu.Mapapi.CoordType.Bd09ll;
                }
            }
        }
    }
}
