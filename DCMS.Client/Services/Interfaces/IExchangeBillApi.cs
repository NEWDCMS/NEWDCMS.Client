using DCMS.Client.Models.Sales;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DCMS.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/sales/exchangeBill", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    [Headers("Authorization: Bearer")]
    public interface IExchangeBillApi
    {

        [Get("/auditing/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> AuditingAsync(int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Post("/createorupdate/{storeId}/{userId}/{billId}")]
        Task<APIResult<ExchangeBillUpdateModel>> CreateOrUpdateAsync(ExchangeBillUpdateModel data, int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Get("/getbills/{storeId}/{terminalId}/{businessUserId}/{wareHouseId}")]
        Task<APIResult<IList<ExchangeBillModel>>> GetExchangeBillsAsync(int storeId, int? makeuserId, int? terminalId, string terminalName, int? businessUserId, int? deliveryUserId, string billNumber, int? wareHouseId, string remark, int? districtId, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, bool? showReturn = null, bool? alreadyChange = null, int pagenumber = 0, int pageSize = 20, CancellationToken calToken = default);

        [Get("/getexchangebill/{storeId}/{billId}/{userId}")]
        Task<APIResult<ExchangeBillModel>> GetBillAsync(int storeId, int userId, int billId, CancellationToken calToken = default);

        [Get("/reverse/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> ReverseAsync(int storeId, int userId, int billId = 0, string remark = "", CancellationToken calToken = default);

        /// <summary>
        /// 用于收货签收确认
        /// </summary>
        /// <param name="data"></param>
        /// <param name="storeId"></param>
        /// <param name="userId"></param>
        /// <param name="billId"></param>
        /// <param name="calToken"></param>
        /// <returns></returns>
        [Post("/exchangeSignIn/{storeId}/{userId}")]
        Task<APIResult<DeliverySignUpdateModel>> DeliverySignConfirmAsync(DeliverySignUpdateModel data, int storeId, int userId, int billId = 0, CancellationToken calToken = default);


    }
}