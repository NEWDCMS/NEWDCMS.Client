using Wesley.Client.Models.Sales;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IReturnBillService
    {
        Task<ResultData> AuditingAsync(int billId = 0, CancellationToken calToken = default);
        Task<APIResult<ReturnBillUpdateModel>> CreateOrUpdateAsync(ReturnBillUpdateModel data, int billId = 0, CancellationToken calToken = default);
        Task<ReturnBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default);
        Task<ReturnBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default);
        IObservable<APIResult<ReturnBillModel>> Rx_GetInitDataAsync(CancellationToken calToken = default);
        Task<IList<ReturnBillModel>> GetReturnBillsAsync(int? makeuserId, int? terminalId, string terminalName, int? businessUserId, int? deliveryUserId, int? wareHouseId, int? districtId, string remark, string billNumber, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, bool? showReturn = null, int? paymentMethodType = null, int? billSourceType = null, bool? handleStatus = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<bool> ReverseAsync(int billId = 0, string remark = "", CancellationToken calToken = default);
    }
}