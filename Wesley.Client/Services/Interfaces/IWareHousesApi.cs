using Wesley.Client.Models.WareHouses;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/warehouse", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    [Headers("Authorization: Bearer")]
    public interface IWareHousesApi
    {
        [Get("/allocationbill/auditing/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> AuditingAsync(int storeId, int userId, int billId, CancellationToken calToken = default);

        [Post("/allocationbill/createorupdate/{storeId}/{userId}/{billId}")]
        Task<APIResult<AllocationUpdateModel>> CreateOrUpdateAllocationbillAsync(AllocationUpdateModel data, int storeId, int userId, int billId, CancellationToken calToken = default);

        [Post("/inventoryreportbill/createorupdate/{storeId}/{userId}/{billId}")]
        Task<APIResult<InventoryReportBillModel>> CreateOrUpdateAsync(InventoryReportBillModel data, int storeId, int userId, int billId, CancellationToken calToken = default);

        [Get("/inventoryreportsummary/getbills/{storeId}/{businessUserId}/{terminalId}")]
        Task<APIResult<IList<InventoryReportSummaryModel>>> GetInventoryReportAsync(int storeId, int? makeuserId, int? businessUserId, int? terminalId, int? channelId, int? rankId, int? districtId, int? productId, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 0, int pageSize = 20, CancellationToken calToken = default);

        [Get("/wareHouse/getWareHouses/{storeId}")]
        Task<APIResult<IList<WareHouseModel>>> GetWareHousesAsync(int storeId, int? makeuserId, int? btype, string searchStr = "", int pageIndex = 0, int pageSize = 50, CancellationToken calToken = default);
    }
}