using Wesley.Client.Models.Sales;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IReturnReservationBillService
    {
        Task<bool> AuditingAsync(int billId = 0, CancellationToken calToken = default);
        Task<APIResult<ReturnReservationBillUpdateModel>> CreateOrUpdateAsync(ReturnReservationBillUpdateModel data, int billId = 0, CancellationToken calToken = default);
        Task<ReturnReservationBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default);
        Task<ReturnReservationBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default);
        Task<IList<ReturnReservationBillModel>> GetReturnReservationBillsAsync(int? makeuserId, int? terminalId, string terminalName, int? businessUserId, int? deliveryUserId, int? wareHouseId, int? districtId, string billNumber, string remark, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, bool? showReturn = null, bool? alreadyChange = false, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<bool> ReverseAsync(int billId = 0, CancellationToken calToken = default);
    }
}