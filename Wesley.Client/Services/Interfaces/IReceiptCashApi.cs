using Wesley.Client.Models.Finances;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/finances/receiptcashbill", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    [Headers("Authorization: Bearer")]
    public interface IReceiptCashApi
    {

        [Get("/getinitdataasync/{storeId}/{userId}")]
        Task<APIResult<CashReceiptBillModel>> GetInitDataAsync(int storeId, int userId, CancellationToken calToken = default);

        [Get("/auditing/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> AuditingAsync(int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Post("/createorupdate/{storeId}/{userId}/{billId}")]
        Task<APIResult<CashReceiptUpdateModel>> CreateOrUpdateAsync(CashReceiptUpdateModel data, int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Get("/getowecashbills/{storeId}/{userId}")]
        Task<APIResult<IList<BillSummaryModel>>> GetOwecashBillsAsync(int storeId, int userId, int? terminalId, int? billTypeId, string billNumber = "", string remark = "", DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 0, int pageSize = 20, CancellationToken calToken = default);

        [Get("/getbills/{storeId}/{customerId}/{payeer}")]
        Task<APIResult<IList<CashReceiptBillModel>>> GetReceiptCashsAsync(int storeId, int? makeuserId, int? customerId, string customerName, int? payeer, string billNumber, string remark, bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? showReverse = null, bool? sortByAuditedTime = null, bool? handleStatus = null, int pagenumber = 0, int pageSize = 20, CancellationToken calToken = default);

        [Get("/getCashReceiptBill/{storeId}/{billId}/{userId}")]
        Task<APIResult<CashReceiptBillModel>> GetBillAsync(int storeId, int userId, int billId, CancellationToken calToken = default);

        [Get("/reverse/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> ReverseAsync(int storeId, int userId, int billId = 0, string remark = "", CancellationToken calToken = default);
    }
}