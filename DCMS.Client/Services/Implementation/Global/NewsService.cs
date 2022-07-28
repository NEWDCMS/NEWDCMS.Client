using Wesley.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Wesley.Client.Services
{
    public class NewsService : INewsService
    {
        private readonly MakeRequest _makeRequest;

        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/news";

        public NewsService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取活动
        /// </summary>
        /// <returns></returns>
        public async Task<IList<NewsInfoModel>> GetNewsAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<INewsApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetNewsAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GeLatestNewsAsync(storeId, calToken), cacheKey, force, calToken);
                if (results != null && results?.Code >= 0)
                    return results?.Data.ToList();
                else
                    return null;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }


        /// <summary>
        /// 获取活动
        /// </summary>
        /// <returns></returns>
        public async Task<NewsInfoModel> GetNewsAsync(int newsId, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<INewsApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetNewsAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetNewsAsync(newsId, calToken), cacheKey, force, calToken);
                if (results != null && results?.Code >= 0)
                    return results?.Data;
                else
                    return null;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }


        /// <summary>
        /// 获取推荐咨询活动
        /// </summary>
        /// <returns></returns>
        public async Task<IList<NewsInfoModel>> GetTopNewsAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<INewsApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetTopNewsAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetTopNewsAsync(storeId, calToken), cacheKey, force, calToken);
                if (results != null && results?.Code >= 0)
                    return results?.Data.ToList();
                else
                    return null;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }
    }
}
