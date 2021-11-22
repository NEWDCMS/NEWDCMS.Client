using Wesley.Client.Models.Purchases;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/purchases", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    [Headers("Authorization: Bearer")]
    public interface IPurchaseBillApi
    {
        [Get("/purchases/lastpurchaseprice/{storeId}/{productId}")]
        Task<APIResult<List<PurchaseItemModel>>> AsyncPurchaseItemByProductIdForAsync(int storeId, int? productId, bool? beforeTax, CancellationToken calToken = default);

        [Get("/purchases/auditing/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> AuditingAsync(int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Post("/purchases/createorupdate/{storeId}/{userId}/{billId}")]
        Task<APIResult<PurchaseItemUpdateModel>> CreateOrUpdateAsync(PurchaseItemUpdateModel data, int storeId, int userId, int billId, CancellationToken calToken = default);

        [Get("/purchases/getinitdataasync/{storeId}/{userId}")]
        Task<APIResult<PurchaseBillModel>> GetInitDataAsync(int storeId, int userId, CancellationToken calToken = default);

        [Get("/purchases/getInitDataForList/{storeId}")]
        Task<APIResult<IList<PurchaseBillModel>>> GetInitDataForListAsync(int storeId, CancellationToken calToken = default);

        [Get("/purchases/getbills/{storeId}/{businessUserId}/{manufacturerId}/{wareHouseId}")]
        Task<APIResult<IList<PurchaseBillModel>>> GetPurchaseBillsAsync(int storeId, int? makeuserId, int? businessUserId, int? manufacturerId, int? wareHouseId, string billNumber, string remark, bool? printStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, int pagenumber = 0, int pageSize = 20, CancellationToken calToken = default);

        [Get("/purchases/getPurchaseBill/{storeId}/{billId}/{userId}")]
        Task<APIResult<PurchaseBillModel>> GetBillAsync(int storeId, int userId, int billId, CancellationToken calToken = default);

        [Get("/purchases/reverse/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> ReverseAsync(int storeId, int userId, int billId = 0, string remark = "", CancellationToken calToken = default);
    }
}