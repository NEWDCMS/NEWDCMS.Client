using Wesley.Client.Models.WareHouses;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/warehouse/allocationbill", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    //[Headers("Authorization: Bearer")]
    public interface IAllocationApi
    {
        [Get("/getbills/{storeId}/{businessUserId}/0/{shipmentWareHouseId}/{incomeWareHouseId}")]
        Task<APIResult<IList<AllocationBillModel>>> GetAllocationsAsync(int storeId, int? makeuserId, int businessUserId, int? shipmentWareHouseId, int? incomeWareHouseId, string billNumber, string remark, bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? showReverse = null, bool? sortByAuditedTime = null, int pagenumber = 0, int pageSize = 20, CancellationToken calToken = default);

        [Get("/getbills/{storeId}/{billId}/{userId}")]
        Task<APIResult<AllocationBillModel>> GetBillAsync(int storeId, int userId, int billId, CancellationToken calToken = default);
    }
}