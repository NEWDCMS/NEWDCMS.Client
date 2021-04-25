using Wesley.Client.Models.Campaigns;
using Refit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/archives", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    //[Headers("Authorization: Bearer")]
    public interface ICampaignApi
    {
        [Get("/campaign/getAllCampaigns/{storeId}")]
        Task<APIResult<IList<CampaignBuyGiveProductModel>>> GetAllCampaigns(int storeId, string name, int terminalId = 0, int channelId = 0, int wareHouseId = 0, int pagenumber = 0, int pageSize = 50, CancellationToken calToken = default);
    }
}