using Wesley.Client.Models.Finances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IAdvanceReceiptService
    {
        Task<ResultData> AuditingAsync(int billId = 0, CancellationToken calToken = default);
        Task<APIResult<AdvanceReceiptUpdateModel>> CreateOrUpdateAsync(AdvanceReceiptUpdateModel data, int billId = 0, CancellationToken calToken = default);
        Task<IList<AdvanceReceiptBillModel>> GetAdvanceReceiptsAsync(int? customerId, int? makeuserId, string customerName, int? payeer, string billNumber = "", bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? showReverse = null, bool? sortByAuditedTime = null, int? accountingOptionId = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<AdvanceReceiptBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default);
        Task<AdvanceReceiptBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default);
        Task<bool> ReverseAsync(int billId = 0, string remark = "", CancellationToken calToken = default);
    }
}