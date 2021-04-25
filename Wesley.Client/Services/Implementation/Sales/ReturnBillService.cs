using Wesley.Client.Models.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public class ReturnBillService : IReturnBillService
    {

        private readonly MakeRequest _makeRequest;
        //
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/sales/returnbill";

        public ReturnBillService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取表单创建初始绑定
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IReturnBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("ReturnBillService.GetInitDataAsync", storeId, userId);
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



        /// <summary>
        /// 提交单据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<APIResult<ReturnBillUpdateModel>> CreateOrUpdateAsync(ReturnBillUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IReturnBillApi>(URL);
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
        public async Task<bool> AuditingAsync(int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IReturnBillApi>(URL);
                var results = await _makeRequest.Start(api.AuditingAsync(storeId, userId, billId, calToken), calToken);
                return (bool)(results?.Success);
            }
            catch (Exception e)
            {

                e.HandleException();
                return false;
            }
        }



        /// <summary>
        /// 红冲
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<bool> ReverseAsync(int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IReturnBillApi>(URL);

                var results = await _makeRequest.Start(api.ReverseAsync(storeId, userId, billId, calToken), calToken);
                return (bool)(results?.Success);
            }
            catch (Exception e)
            {

                e.HandleException();
                return false;
            }
        }

        /// <summary>
        /// 获取退货单
        /// </summary>
        /// <param name="makeuserId"></param>
        /// <param name="terminalId"></param>
        /// <param name="terminalName"></param>
        /// <param name="businessUserId"></param>
        /// <param name="deliveryUserId"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="districtId"></param>
        /// <param name="remark"></param>
        /// <param name="billNumber"></param>
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
        public async Task<IList<ReturnBillModel>> GetReturnBillsAsync(int? makeuserId, int? terminalId, string terminalName, int? businessUserId, int? deliveryUserId, int? wareHouseId, int? districtId, string remark, string billNumber, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, bool? showReturn = null, int? paymentMethodType = null, int? billSourceType = null, bool? handleStatus = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IReturnBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetReturnBillsAsync", storeId,
                    makeuserId,
                    terminalId,
                    terminalName,
                    businessUserId,
                    deliveryUserId,
                    wareHouseId,
                    districtId,
                    remark,
                    billNumber,
                    startTime,
                    endTime,
                    auditedStatus,
                    sortByAuditedTime,
                    showReverse,
                    showReturn,
                    paymentMethodType,
                    billSourceType,
                    handleStatus,
                    pagenumber,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetReturnBillsAsync(storeId,
                    makeuserId,
                    terminalId,
                    terminalName,
                    businessUserId,
                    deliveryUserId,
                    wareHouseId,
                    districtId,
                    remark,
                    billNumber,
                    startTime,
                    endTime,
                    auditedStatus,
                    sortByAuditedTime,
                    showReverse,
                    showReturn,
                    paymentMethodType,
                    billSourceType,
                    handleStatus,
                    pagenumber,
                    pageSize, calToken), cacheKey, force, calToken);

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
        public async Task<ReturnBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default)
        {
            var model = new ReturnBillModel();
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IReturnBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("ReturnBillService.GetBillAsync", storeId, userId, billId);
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
