using Wesley.Client.Models;
using Refit;
using System.Threading;
using System.Threading.Tasks;


namespace Wesley.Client.Services
{
    //
    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:5:00"), Trace]
    //[Headers("Authorization: Bearer")]
    public interface IGlobalApi
    {
        [Get("/global/updateHistoryBillStatus/{storeId}/{userId}")]
        Task<APIResult<bool>> UpdateHistoryBillStatusAsync(int storeId, int userId, int? billType, int? billId = 0, CancellationToken calToken = default);

        [Get("/system/getappfeatures")]
        Task<APIResult<APPFeatures>> GetAPPFeatures(int storeId, int userId, CancellationToken calToken = default);

        [Put("/system/resetVerifiCode")]
        Task<APIResult<bool>> ResetVerifiCode(SMSParams sMSParams, CancellationToken calToken = default);
    }
}