using Wesley.Client.Models.Finances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface ICostContractService
    {
        Task<ResultData> AuditingAsync(int billId = 0, CancellationToken calToken = default);
        Task<APIResult<CostContractUpdateModel>> CreateOrUpdateAsync(CostContractUpdateModel data, int billId = 0, CancellationToken calToken = default);
        Task<CostContractBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default);
        Task<IList<CostContractBindingModel>> GetCostContractsBindingAsync(int? customerId, int? accountOptionId, int? accountCodeTypeId, int year, int month, int? contractType = 0, bool? auditedStatus = null, bool? showReverse = null, int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<IList<CostContractBillModel>> GetCostContractsAsync(int? makeuserId, int? customerId, string customerName, int? employeeId, string billNumber, string remark, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? showReverse = null, int pagenumber = 0, int pageSize = 50, bool force = false, CancellationToken calToken = default);
        Task<bool> ReverseAsync(int billId = 0, string remark = "", CancellationToken calToken = default);
    }
}