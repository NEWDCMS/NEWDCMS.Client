using Wesley.Client.Models.Sales;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/sales/salereservationbill", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    [Headers("Authorization: Bearer")]
    public interface ISaleReservationBillApi
    {

        [Get("/auditing/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> AuditingAsync(int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Post("/createorupdate/{storeId}/{userId}/{billId}")]
        Task<APIResult<SaleReservationBillUpdateModel>> CreateOrUpdateAsync(SaleReservationBillUpdateModel data, int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Get("/getinitdataasync/{storeId}/{userId}")]
        Task<APIResult<SaleReservationBillModel>> GetInitDataAsync(int storeId, int userId, CancellationToken calToken = default);

        [Get("/getbills/{storeId}/{terminalId}/{businessUserId}/{wareHouseId}")]
        Task<APIResult<IList<SaleReservationBillModel>>> GetSaleReservationBillsAsync(int storeId, int? makeuserId, int? terminalId, string terminalName, int? businessUserId, int? deliveryUserId, string billNumber, int? wareHouseId, string remark, int? districtId, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, bool? showReturn = null, bool? alreadyChange = null, int pagenumber = 0, int pageSize = 20, CancellationToken calToken = default);

        [Get("/getSaleReservationBill/{storeId}/{billId}/{userId}")]
        Task<APIResult<SaleReservationBillModel>> GetBillAsync(int storeId, int userId, int billId, CancellationToken calToken = default);

        [Get("/reverse/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> ReverseAsync(int storeId, int userId, int billId = 0, string remark = "", CancellationToken calToken = default);
    }
}