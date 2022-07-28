using DCMS.Client.Models;
using Refit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DCMS.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/news", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    public interface INewsApi
    {
        [Get("/news/{newsId}")]
        [Headers("Authorization: Bearer")]
        Task<APIResult<NewsInfoModel>> GetNewsAsync(int newsId, CancellationToken calToken = default);

        [Get("/latestnews/0/50")]
        [Headers("Authorization: Bearer")]
        Task<APIResult<IList<NewsInfoModel>>> GeLatestNewsAsync(int storeId, CancellationToken calToken = default);

        [Get("/topnews/0/6")]
        [Headers("Authorization: Bearer")]
        Task<APIResult<IList<NewsInfoModel>>> GetTopNewsAsync(int storeId, CancellationToken calToken = default);
    }
}
