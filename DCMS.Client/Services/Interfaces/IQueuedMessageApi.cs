using DCMS.Client.Models;
using Refit;
using System.Collections.Generic;
using DCMS.Client.Models.QA;
using System.Threading.Tasks;
using System;

namespace DCMS.Client.Services
{

    public interface IPagedList<T> : IList<T>
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }

    [Headers("Authorization: Bearer")]
    public interface IQueuedMessageApi
    {
 
        [Get("/getqueuedmessages/{storeId}")]
        Task<APIResult<IList<QueuedMessage>>> GetQueuedMessages(int storeId, [Query(CollectionFormat.Multi)] int[] mTypeId, string toUser, bool sentStatus, bool orderByCreatedOnUtc, int maxSendTries, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
