using Wesley.Client.Models.WareHouses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Wesley.Client.Services
{
    public interface IInventoryService
    {
        Task<bool> CancelTakeInventoryAsync(int billId = 0, CancellationToken calToken = default);
        Task<IList<InventoryPartTaskBillModel>> CheckInventoryAsync(int wareHouseId, CancellationToken calToken = default);
        Task<APIResult<InventoryPartTaskUpdateModel>> CreateOrUpdateAsync(InventoryPartTaskUpdateModel data, int billId = 0, CancellationToken calToken = default);
        Task<IList<InventoryPartTaskBillModel>> GetInventoryAllsAsync(int? makeuserId, int? inventoryPerson, int? wareHouseId, string billNumber, string remark, bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, int? inventoryStatus = -1, bool? showReverse = null, bool? sortByCompletedTime = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<InventoryPartTaskBillModel> GetInventoryPartTaskBillAsync(int billId, bool force = false, CancellationToken calToken = default);
        Task<bool> SetInventoryCompletedAsync(int billId = 0, CancellationToken calToken = default);
    }
}