using Wesley.Client.Models.Sales;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    public interface ISaleBillService
    {
        Task<bool> AuditingAsync(int billId = 0, CancellationToken calToken = default);
        Task<APIResult<SaleBillUpdateModel>> CreateOrUpdateAsync(SaleBillUpdateModel data, int billId = 0, CancellationToken calToken = default);
        Task<APIResult<DeliverySignUpdateModel>> DeliverySignConfirmAsync(DeliverySignUpdateModel data, int billId = 0, CancellationToken calToken = default);
        Task<APIResult<DeliverySignUpdateModel>> RefusedConfirmAsync(DeliverySignUpdateModel data, int billId = 0, CancellationToken calToken = default);
        Task<SaleBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default);
        Task<IList<DeliverySignModel>> GetDeliveriedSignsAsync(int userId, DateTime? start = null, DateTime? end = null, int? businessUserId = null, int? terminalId = null, int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<SaleBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default);
        Task<IList<SaleBillModel>> GetSalebillsAsync(int? makeuserId, int? terminalId, string terminalName, int? businessUserId, int? districtId, int? deliveryUserId, int? wareHouseId, string billNumber, string remark, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, bool? showReturn = null, int? paymentMethodType = null, int? billSourceType = null, bool? handleStatus = null, int? sign = null, int pagenumber = 0, int pageSize = 30, bool force = false, CancellationToken calToken = default);
        Task<IList<DispatchItemModel>> GetUndeliveredSignsAsync(int? userId, DateTime? start = null, DateTime? end = null, int? businessUserId = null, int? districtId = null, int? terminalId = null, string terminalName = "", string billNumber = "", int? deliveryUserId = null, int? channelId = null, int? rankId = null, int? billTypeId = null, bool? showDispatchReserved = null, bool? dispatchStatus = null, int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<bool> ReverseAsync(int billId = 0, CancellationToken calToken = default);
    }
}