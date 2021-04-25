using Wesley.Client.Models.Purchases;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IPurchaseBillService
    {
        Task<IList<PurchaseItemModel>> AsyncPurchaseItemByProductIdForAsync(int? productId, bool? beforeTax, bool force = false, CancellationToken calToken = default);
        Task<bool> AuditingAsync(int billId = 0, CancellationToken calToken = default);
        Task<APIResult<PurchaseItemUpdateModel>> CreateOrUpdateAsync(PurchaseItemUpdateModel data, int billId = 0, CancellationToken calToken = default);
        Task<PurchaseBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default);
        Task<PurchaseBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default);
        Task<IList<PurchaseBillModel>> GetInitDataForListAsync(bool force = false, CancellationToken calToken = default);
        Task<IList<PurchaseBillModel>> GetPurchaseBillsAsync(int? makeuserId, int? businessUserId, int? manufacturerId, int? wareHouseId, string billNumber, string remark, bool? printStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<bool> ReverseAsync(int billId = 0, CancellationToken calToken = default);
    }
}