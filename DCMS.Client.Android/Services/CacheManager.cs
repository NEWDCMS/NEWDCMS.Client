using Akavache;
using DCMS.Client.Services;
using System;


namespace DCMS.Client.Droid.Services
{
    public class CacheManager : ICacheManager
    {

        /// <summary>
        /// 获取缓存大小
        /// </summary>
        /// <returns></returns>
        public string GetCacheSize()
        {
            //var path = LocalDatabase.DatabasePathPath;
            //long cacheSize = 0;
            return "0KB";
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <param name="clearReadPos"></param>
        /// <param name="clearCollect"></param>
        public void ClearCache(bool clearReadPos, bool clearCollect)
        {
            try
            {
                // 清除其他缓存
                BlobCache.LocalMachine.InvalidateAll();
                BlobCache.LocalMachine.Flush();
            }
            catch (Exception)
            {
            }
        }
    }
}