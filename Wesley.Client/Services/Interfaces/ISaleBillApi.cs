using Wesley.Client.Models.Sales;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/sales/salebill", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    //[Headers("Authorization: Bearer")]
    public interface ISaleBillApi
    {

        [Get("/auditing/{storeId}/{userId}/{billId}")]
        Task<APIResult<bool>> AuditingAsync(int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Post("/createorupdate/{storeId}/{userId}/{billId}")]
        Task<APIResult<SaleBillUpdateModel>> CreateOrUpdateAsync(SaleBillUpdateModel data, int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        /// <summary>
        /// 用于单据签收确认
        /// </summary>
        /// <param name="data"></param>
        /// <param name="storeId"></param>
        /// <param name="userId"></param>
        /// <param name="billId"></param>
        /// <param name="calToken"></param>
        /// <returns></returns>
        [Post("/deliverysignconfirm/{storeId}/{userId}")]
        Task<APIResult<DeliverySignUpdateModel>> DeliverySignConfirmAsync(DeliverySignUpdateModel data, int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        /// <summary>
        /// 用于单据拒签
        /// </summary>
        /// <param name="data"></param>
        /// <param name="storeId"></param>
        /// <param name="userId"></param>
        /// <param name="billId"></param>
        /// <param name="calToken"></param>
        /// <returns></returns>
        [Post("/refusedconfirm/{storeId}/{userId}")]
        Task<APIResult<DeliverySignUpdateModel>> RefusedConfirmAsync(DeliverySignUpdateModel data, int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        /// <summary>
        /// 获取已经签收单据
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="userId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="businessUserId"></param>
        /// <param name="terminalId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="calToken"></param>
        /// <returns></returns>
        [Get("/getDeliveriedSigns/{storeId}/{userId}")]
        Task<APIResult<IList<DeliverySignModel>>> GetDeliveriedSignsAsync(int storeId, int userId, DateTime? start = null, DateTime? end = null, int? businessUserId = null, int? terminalId = null, int pageIndex = 0, int pageSize = 20, CancellationToken calToken = default);

        [Get("/getinitdataasync/{storeId}/{userId}")]
        Task<APIResult<SaleBillModel>> GetInitDataAsync(int storeId, int userId, CancellationToken calToken = default);


        [Get("/getbills/{storeId}/{terminalId}/{businessUserId}/{wareHouseId}")]
        Task<APIResult<IList<SaleBillModel>>> GetSalebillsAsync(int storeId, int? makeuserId, int? terminalId, string terminalName, int? businessUserId, int? districtId, int? deliveryUserId, int? wareHouseId, string billNumber, string remark, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, bool? showReturn = null, int? paymentMethodType = null, int? billSourceType = null, bool? handleStatus = null, int? sign = null, int pagenumber = 0, int pageSize = 30, CancellationToken calToken = default);


        /// <summary>
        /// 获取未签收单据（销售订单，退货订单，换货单）
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="userId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="businessUserId"></param>
        /// <param name="districtId"></param>
        /// <param name="terminalId"></param>
        /// <param name="terminalName"></param>
        /// <param name="billNumber"></param>
        /// <param name="deliveryUserId"></param>
        /// <param name="channelId"></param>
        /// <param name="rankId"></param>
        /// <param name="billTypeId"></param>
        /// <param name="showDispatchReserved"></param>
        /// <param name="dispatchStatus"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="calToken"></param>
        /// <returns></returns>
        [Get("/getUndeliveredSigns/{storeId}/{userId}")]
        //[Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
        Task<APIResult<IList<DispatchItemModel>>> GetUndeliveredSignsAsync(int? storeId, int? userId, DateTime? start = null, DateTime? end = null, int? businessUserId = null, int? districtId = null, int? terminalId = null, string terminalName = "", string billNumber = "", int? deliveryUserId = null, int? channelId = null, int? rankId = null, int? billTypeId = null, bool? showDispatchReserved = null, bool? dispatchStatus = null, int pageIndex = 0, int pageSize = 20, CancellationToken calToken = default);




        [Get("/getSaleBill/{storeId}/{billId}/{userId}")]
        Task<APIResult<SaleBillModel>> GetBillAsync(int storeId, int userId, int billId, CancellationToken calToken = default);


        [Get("/reverse/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> ReverseAsync(int storeId, int userId, int billId = 0, CancellationToken calToken = default);
    }
}