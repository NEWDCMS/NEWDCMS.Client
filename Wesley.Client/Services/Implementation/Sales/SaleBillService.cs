using Wesley.Client.Models.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using Xamarin.Forms;

namespace Wesley.Client.Services
{
    public class SaleBillService : ISaleBillService
    {
        private readonly MakeRequest _makeRequest;
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/sales/salebill";

        public SaleBillService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取表单创建初始绑定
        /// </summary>
        /// <returns></returns>
        public async Task<SaleBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISaleBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("SaleBillService.GetInitDataAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetInitDataAsync(storeId, userId, calToken), cacheKey, force, calToken);
                if (results != null && results?.Code >= 0)
                    return results?.Data;
                else
                    return null;
            }
            catch (Exception e)
            {

                e.HandleException();
                return null;
            }
        }

        public IObservable<APIResult<SaleBillModel>> Rx_GetInitDataAsync(CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISaleBillApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("SaleBillService.GetInitDataAsync", storeId, userId);
                var results = _makeRequest.StartUseCache_Rx(api.GetInitDataAsync(storeId, userId, calToken), cacheKey, calToken);
                return results;
            }
            catch (System.ObjectDisposedException e)
            {
                e.HandleException();
                return null;
            }
            catch (Exception e)
            {

                e.HandleException();
                return null;
            }
        }



        /// <summary>
        /// 提交单据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<APIResult<SaleBillUpdateModel>> CreateOrUpdateAsync(SaleBillUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISaleBillApi>(URL);
                var results = await _makeRequest.Start(api.CreateOrUpdateAsync(data, storeId, userId, billId, calToken), calToken);
                return results;
            }
            catch (Exception e)
            {

                e.HandleException();
                return null;
            }
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<ResultData> AuditingAsync(int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISaleBillApi>(URL);
                var results = await _makeRequest.Start(api.AuditingAsync(storeId, userId, billId, calToken), calToken);
                return new ResultData
                {
                    Success = (bool)(results?.Success),
                    Message = results?.Message
                };
            }
            catch (Exception e)
            {
                return new ResultData
                {
                    Success = false,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// 红冲
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<bool> ReverseAsync(int billId = 0, string remark = "", CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ISaleBillApi>(URL);
                var results = await _makeRequest.Start(api.ReverseAsync(storeId, userId, billId, remark, calToken), calToken);
                return (bool)(results?.Success);
            }
            catch (Exception e)
            {

                e.HandleException();
                return false;
            }
        }

        /// <summary>
        ///  获取待签收单据（已经调度且未签收单据） 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<DispatchItemModel>> GetUndeliveredSignsAsync(int? userId, DateTime? start = null, DateTime? end = null, int? businessUserId = null, int? districtId = null, int? terminalId = null, string terminalName = "", string billNumber = "", int? deliveryUserId = null, int? channelId = null, int? rankId = null, int? billTypeId = null, bool? showDispatchReserved = null, bool? dispatchStatus = null, int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                deliveryUserId = userId;

                var api = RefitServiceBuilder.Build<ISaleBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetUndeliveredSignsAsync", storeId,
                    userId,
                    start,
                    end,
                    businessUserId,
                    districtId,
                    terminalId,
                    terminalName,
                    billNumber,
                    deliveryUserId,
                    channelId,
                    rankId,
                    billTypeId,
                    showDispatchReserved,
                    dispatchStatus,
                    pageIndex,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetUndeliveredSignsAsync(storeId,
                    userId,
                    start,
                    end,
                    businessUserId,
                    districtId,
                    terminalId,
                    terminalName,
                    billNumber,
                    deliveryUserId,
                    channelId,
                    rankId,
                    billTypeId,
                    showDispatchReserved,
                    dispatchStatus,
                    pageIndex,
                    pageSize, calToken),
                    cacheKey, force, calToken);

                if (results != null && results?.Code >= 0)
                    return results?.Data.ToList();
                else
                    return null;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }

        /// <summary>
        /// 获取已签收单据
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<DeliverySignModel>> GetDeliveriedSignsAsync(int userId, DateTime? start = null, DateTime? end = null, int? businessUserId = null, int? terminalId = null, int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;

                var api = RefitServiceBuilder.Build<ISaleBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetDeliveriedSignsAsync", storeId,
                    userId,
                    start,
                    end,
                    businessUserId,
                    terminalId,
                    pageIndex,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetDeliveriedSignsAsync(storeId,
                    userId,
                    start,
                    end,
                    businessUserId,
                    terminalId,
                    pageIndex,
                    pageSize, calToken),
                    cacheKey, force, calToken);

                if (results != null && results?.Code >= 0)
                    return results?.Data.ToList();
                else
                    return null;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }

        /// <summary>
        /// 用于送货签收确认
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<APIResult<DeliverySignUpdateModel>> DeliverySignConfirmAsync(DeliverySignUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISaleBillApi>(URL);
                var results = await _makeRequest.Start(api.DeliverySignConfirmAsync(data, storeId, userId, billId, calToken), calToken);
                return results;
            }
            catch (Exception e)
            {

                e.HandleException();
                return null;
            }
        }
        /// <summary>
        /// 用于单据拒签
        /// </summary>
        /// <param name="data"></param>
        /// <param name="billId"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        public async Task<APIResult<DeliverySignUpdateModel>> RefusedConfirmAsync(DeliverySignUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISaleBillApi>(URL);
                var results = await _makeRequest.Start(api.RefusedConfirmAsync(data, storeId, userId, billId, calToken), calToken);
                return results;
            }
            catch (Exception e)
            {

                e.HandleException();
                return null;
            }
        }

        /// <summary>
        /// 获取销售单
        /// </summary>
        /// <param name="makeuserId"></param>
        /// <param name="terminalId"></param>
        /// <param name="terminalName"></param>
        /// <param name="businessUserId"></param>
        /// <param name="districtId"></param>
        /// <param name="deliveryUserId"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="billNumber"></param>
        /// <param name="remark"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="auditedStatus"></param>
        /// <param name="sortByAuditedTime"></param>
        /// <param name="showReverse"></param>
        /// <param name="showReturn"></param>
        /// <param name="paymentMethodType"></param>
        /// <param name="billSourceType"></param>
        /// <param name="handleStatus"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IList<SaleBillModel>> GetSalebillsAsync(int? makeuserId, int? terminalId, string terminalName, int? businessUserId, int? districtId, int? deliveryUserId, int? wareHouseId, string billNumber, string remark, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, bool? showReturn = null, int? paymentMethodType = null, int? billSourceType = null, bool? handleStatus = null, int? sign = null, int pagenumber = 0, int pageSize = 30, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISaleBillApi>(URL);

                var results = await _makeRequest.Start(api.GetSalebillsAsync(storeId,
                    makeuserId,
                    terminalId,
                    terminalName,
                    businessUserId,
                    districtId,
                    deliveryUserId,
                    wareHouseId,
                    billNumber,
                    remark,
                    startTime,
                    endTime,
                    auditedStatus,
                    sortByAuditedTime,
                    showReverse,
                    showReturn,
                    paymentMethodType,
                    billSourceType,
                    handleStatus,
                    sign,
                    pagenumber,
                    pageSize, calToken),calToken);

                if (results != null && results?.Code >= 0)
                    return results?.Data.ToList();
                else
                    return null;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }


        /// <summary>
        /// 获取单据
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<SaleBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default)
        {
            var model = new SaleBillModel();
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISaleBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("SaleBillService.GetBillAsync", storeId, userId, billId);
                var results = await _makeRequest.StartUseCache(api.GetBillAsync(storeId, userId, billId, calToken), cacheKey, force, calToken);
                if (results != null && results?.Code >= 0)
                {
                    model = results?.Data;
                }
                return model;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }
    }
}
