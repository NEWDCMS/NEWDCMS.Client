using Wesley.Client.Models.Finances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Wesley.Client.Services
{
    public interface IReceiptCashService
    {
        Task<bool> AuditingAsync(int billId = 0, CancellationToken calToken = default);
        Task<APIResult<CashReceiptUpdateModel>> CreateOrUpdateAsync(CashReceiptUpdateModel data, int billId = 0, CancellationToken calToken = default);
        Task<CashReceiptBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default);
        Task<CashReceiptBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default);
        Task<IList<BillSummaryModel>> GetOwecashBillsAsync(int userId, int? terminalId, int? billTypeId, string billNumber = "", string remark = "", DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<IList<CashReceiptBillModel>> GetReceiptCashsAsync(int? makeuserId, int? customerId, string customerName, int? payeer, string billNumber, string remark, bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? showReverse = null, bool? sortByAuditedTime = null, bool? handleStatus = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<bool> ReverseAsync(int billId = 0, CancellationToken calToken = default);
    }
}