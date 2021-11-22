using Wesley.Client.Models;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/tss", true, isAutoRegistrable: false), Trace]
    [Headers("Authorization: Bearer")]
    public interface ITTSApi
    {
        [Post("/feedback/insertFeedback/{storeId}")]
        Task<APIResult<FeedBack>> InsertFeedbackAsync(FeedBack data, int storeId, CancellationToken calToken = default);

        [Post("/feedback/insertMarketFeedback/{storeId}")]
        Task<APIResult<MarketFeedback>> InsertMarketFeedback(MarketFeedback data, int storeId, CancellationToken calToken = default);
    }
}