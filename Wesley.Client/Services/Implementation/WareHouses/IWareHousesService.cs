using Wesley.Client.Enums;
using Wesley.Client.Models.WareHouses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Wesley.Client.Services
{
    public interface IWareHousesService
    {
        Task<ResultData> AuditingAsync(int billId = 0, CancellationToken calToken = default);
        Task<APIResult<AllocationUpdateModel>> CreateOrUpdateAllocationbillAsync(AllocationUpdateModel data, int billId = 0, CancellationToken calToken = default);
        Task<APIResult<InventoryReportBillModel>> CreateOrUpdateAsync(InventoryReportBillModel data, int billId = 0, CancellationToken calToken = default);
        Task<IList<InventoryReportSummaryModel>> GetInventoryReportAsync(int? makeuserId, int? businessUserId, int? terminalId, int? channelId, int? rankId, int? districtId, int? productId, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<IList<WareHouseModel>> GetWareHousesAsync(BillTypeEnum btype, string searchStr = "", int pageIndex = 0, int pageSize = 50, bool force = false, CancellationToken calToken = default);
    }
}