using Akavache;
using Wesley.Client.Droid.Utils;
using Java.IO;
using System;
using Wesley.Client.Services;

using Wesley.Client.Droid.Services;
[assembly: Xamarin.Forms.Dependency(typeof(CacheManager))]
namespace Wesley.Client.Droid.Services
{
    public class CacheManager : ICacheManager
    {

        /// <summary>
        /// 获取缓存大小
        /// </summary>
        /// <returns></returns>
        public /*synchronized*/ string GetCacheSize()
        {
            long cacheSize = 0;

            try
            {
                String cacheDir = Constant.BASE_PATH;
                cacheSize += FileUtils.GetFolderSize(cacheDir);
                if (FileUtils.IsSdCardAvailable())
                {
                    String extCacheDir = AppUtils.GetAppContext().ExternalCacheDir.Path;
                    cacheSize += FileUtils.GetFolderSize(extCacheDir);
                }
            }
            catch (Exception)
            {
            }

            return FileUtils.FormatFileSizeToString(cacheSize);
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <param name="clearReadPos"></param>
        /// <param name="clearCollect"></param>
        public /*synchronized*/ void ClearCache(bool clearReadPos, bool clearCollect)
        {
            try
            {
                // 删除内存缓存
                String cacheDir = AppUtils.GetAppContext().CacheDir.Path;
                FileUtils.DeleteFileOrDirectory(new File(cacheDir));
                if (FileUtils.IsSdCardAvailable())
                {
                    FileUtils.DeleteFileOrDirectory(new File(Constant.PATH_DATA));
                }
                // 删除记录（SharePreference）
                if (clearReadPos)
                {
                    //防止再次弹出性别选择框，sp要重写入保存的性别
                    //var chooseSex = Settings.UserChooseSex;
                    //SharedPreferencesUtil.getInstance().removeAll();
                    //Settings.UserChooseSex = chooseSex;
                }
                // 清空
                if (clearCollect)
                {
                    //CollectionsManager.getInstance().clear();
                }
                // 清除其他缓存
                BlobCache.LocalMachine.InvalidateAll();
                BlobCache.LocalMachine.Flush();
            }
            catch (Exception )
            {
            }
        }
    }
}