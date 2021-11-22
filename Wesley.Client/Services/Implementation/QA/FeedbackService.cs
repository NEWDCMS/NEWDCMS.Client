using Wesley.Client.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services.QA
{
    public class FeedbackService : IFeedbackService
    {
        private readonly MakeRequest _makeRequest;
        //
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/tss";

        public FeedbackService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<APIResult<FeedBack>> CreateOrUpdateAsync(FeedBack data, CancellationToken calToken = default)
        {
            try
            {
                var api = RefitServiceBuilder.Build<ITTSApi>(URL);
                var results = await _makeRequest.Start(api.InsertFeedbackAsync(data, Settings.StoreId));
                return results;
            }
            catch (Exception)
            {

                return null;
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<APIResult<MarketFeedback>> CreateOrUpdateMarketAsync(MarketFeedback data, CancellationToken calToken = default)
        {
            try
            {
                var api = RefitServiceBuilder.Build<ITTSApi>(URL);
                var results = await _makeRequest.Start(api.InsertMarketFeedback(data, Settings.StoreId));
                return results;
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}
