



using Wesley.Client.Models.Finances;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/finances/advanceReceipt", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    [Headers("Authorization: Bearer")]
    public interface IAdvanceReceiptApi
    {
        [Get("/getinitdataasync/{storeId}/{userId}")]
        Task<APIResult<AdvanceReceiptBillModel>> GetInitDataAsync(int storeId, int userId, CancellationToken calToken = default);

        [Get("/auditing/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> AuditingAsync(int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Post("/CreateOrUpdate/{storeId}/{userId}/{billId}")]
        Task<APIResult<AdvanceReceiptUpdateModel>> CreateOrUpdateAsync(AdvanceReceiptUpdateModel data, int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Get("/getbills/{storeId}/{customerId}/{payeer}")]
        Task<APIResult<IList<AdvanceReceiptBillModel>>> GetAdvanceReceiptsAsync(int storeId, int? customerId, int? makeuserId, string customerName, int? payeer, string billNumber = "", bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? showReverse = null, bool? sortByAuditedTime = null, int? accountingOptionId = null, int pagenumber = 0, int pageSize = 20, CancellationToken calToken = default);

        [Get("/getAdvanceReceiptBill/{storeId}/{billId}/{userId}")]
        Task<APIResult<AdvanceReceiptBillModel>> GetBillAsync(int storeId, int userId, int billId, CancellationToken calToken = default);

        [Get("/reverse/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> ReverseAsync(int storeId, int userId, int billId = 0, string remark = "", CancellationToken calToken = default);
    }
}