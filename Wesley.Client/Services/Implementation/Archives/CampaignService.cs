using Wesley.Client.Models.Campaigns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Wesley.Client.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly MakeRequest _makeRequest;
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/archives";

        public CampaignService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取所有可用活动的赠送商品
        /// </summary>
        /// <returns></returns>
        public async Task<IList<CampaignBuyGiveProductModel>> GetAllCampaigns(string name, int terminalId = 0, int channelId = 0, int wareHouseId = 0, int pagenumber = 0, int pageSize = 50, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ICampaignApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetAllCampaigns",
                    storeId,
                    name,
                    terminalId,
                    channelId,
                    wareHouseId,
                    pagenumber,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetAllCampaigns(storeId,
                    name,//360
                    terminalId,//185528
                    channelId,
                    wareHouseId,//259
                    pagenumber,
                    pageSize, calToken),
                    cacheKey, force, calToken);

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
