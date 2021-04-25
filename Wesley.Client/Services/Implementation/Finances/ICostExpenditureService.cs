using Wesley.Client.Models.Finances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Wesley.Client.Services
{
    public interface ICostExpenditureService
    {
        Task<bool> AuditingAsync(int billId = 0, CancellationToken calToken = default);
        Task<APIResult<CostExpenditureUpdateModel>> CreateOrUpdateAsync(CostExpenditureUpdateModel data, int billId = 0, CancellationToken calToken = default);
        Task<CostExpenditureBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default);
        Task<IList<CostExpenditureBillModel>> GetCostExpendituresAsync(int? makeuserId, int? customerId, string customerName, int? employeeId, string billNumber, bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? showReverse = null, bool sortByAuditedTime = false, int? sign = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<bool> ReverseAsync(int billId = 0, CancellationToken calToken = default);
    }
}