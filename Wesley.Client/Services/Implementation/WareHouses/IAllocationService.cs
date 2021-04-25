using Wesley.Client.Models.WareHouses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Wesley.Client.Services
{
    public interface IAllocationService
    {
        Task<IList<AllocationBillModel>> GetAllocationsAsync(int? makeuserId, int businessUserId, int? shipmentWareHouseId, int? incomeWareHouseId, string billNumber, string remark, bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? showReverse = null, bool? sortByAuditedTime = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<AllocationBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default);
    }
}