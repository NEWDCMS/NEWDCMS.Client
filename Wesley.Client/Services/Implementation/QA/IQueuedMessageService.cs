using Wesley.Client.Models.QA;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wesley.Client.Services.QA
{
    public interface IQueuedMessageService
    {
        IObservable<APIResult<IList<QueuedMessage>>> Rx_GetQueuedMessages(int? storeId, int[] mTypeId, string toUser, bool? sentStatus, bool? orderByCreatedOnUtc, int? maxSendTries, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}