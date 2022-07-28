using DCMS.Client.Models.QA;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DCMS.Client.Services.QA
{
    public class QueuedMessageService : IQueuedMessageService
    {
        private readonly MakeRequest _makeRequest;
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/skd";
        public QueuedMessageService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        public IObservable<APIResult<IList<QueuedMessage>>> Rx_GetQueuedMessages(int? storeId, int[] mTypeId, string toUser, bool? sentStatus, bool? orderByCreatedOnUtc, int? maxSendTries, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                var api = RefitServiceBuilder.Build<IQueuedMessageApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetQueuedMessages", 
                    storeId, 
                    string.Join("_", mTypeId),
                    toUser, 
                    sentStatus,
                    orderByCreatedOnUtc,
                    maxSendTries,
                    startTime, 
                    endTime, 
                    pageIndex, 
                    pageSize);

                var results = _makeRequest.StartUseCache_Rx(api.GetQueuedMessages(storeId ?? 0, mTypeId, toUser, sentStatus ?? false, orderByCreatedOnUtc ?? true, 0, startTime, endTime, pageIndex, pageSize), cacheKey, new CancellationToken());

                return results;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}