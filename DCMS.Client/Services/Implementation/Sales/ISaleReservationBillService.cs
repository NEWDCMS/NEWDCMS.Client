using DCMS.Client.Models.Sales;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Disposables;

namespace DCMS.Client.Services
{
    public interface ISaleReservationBillService
    {
        Task<ResultData> AuditingAsync(int billId = 0, CancellationToken calToken = default);
        Task<APIResult<SaleReservationBillUpdateModel>> CreateOrUpdateAsync(SaleReservationBillUpdateModel data, int billId = 0, CancellationToken calToken = default);
        Task<SaleReservationBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default);
        Task<SaleReservationBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default);
        IObservable<APIResult<SaleReservationBillModel>> Rx_GetInitDataAsync(CancellationToken calToken = default);
        Task<IList<SaleReservationBillModel>> GetSaleReservationBillsAsync(int? makeuserId, int? terminalId, string terminalName, int? businessUserId, int? deliveryUserId, string billNumber, int? wareHouseId, string remark, int? districtId, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, bool? showReturn = null, bool? alreadyChange = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<bool> ReverseAsync(int billId = 0, string remark = "", CancellationToken calToken = default);
    }
}