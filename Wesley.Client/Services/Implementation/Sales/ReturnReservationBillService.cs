using Wesley.Client.Models.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public class ReturnReservationBillService : IReturnReservationBillService
    {

        private readonly MakeRequest _makeRequest;
        //
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/sales/returnreservationbill";

        public ReturnReservationBillService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取表单创建初始绑定
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnReservationBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IReturnReservationBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("ReturnReservationBillService.GetInitDataAsync", storeId, userId);
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
        public async Task<APIResult<ReturnReservationBillUpdateModel>> CreateOrUpdateAsync(ReturnReservationBillUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IReturnReservationBillApi>(URL);
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

                var api = RefitServiceBuilder.Build<IReturnReservationBillApi>(URL);
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

                var api = RefitServiceBuilder.Build<IReturnReservationBillApi>(URL);
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
        /// 获取退货订单
        /// </summary>
        /// <param name="makeuserId"></param>
        /// <param name="terminalId"></param>
        /// <param name="terminalName"></param>
        /// <param name="businessUserId"></param>
        /// <param name="deliveryUserId"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="districtId"></param>
        /// <param name="billNumber"></param>
        /// <param name="remark"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="auditedStatus"></param>
        /// <param name="sortByAuditedTime"></param>
        /// <param name="showReverse"></param>
        /// <param name="showReturn"></param>
        /// <param name="alreadyChange"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IList<ReturnReservationBillModel>> GetReturnReservationBillsAsync(int? makeuserId, int? terminalId, string terminalName, int? businessUserId, int? deliveryUserId, int? wareHouseId, int? districtId, string billNumber, string remark, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, bool? showReturn = null, bool? alreadyChange = false, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReturnReservationBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetReturnReservationBillsAsync", storeId,
                    makeuserId,
                    terminalId,
                    terminalName,
                    businessUserId,
                    deliveryUserId,
                    wareHouseId,
                    districtId,
                    billNumber,
                    remark,
                    startTime,
                    endTime,
                    auditedStatus,
                    sortByAuditedTime,
                    showReverse,
                    showReturn,
                    alreadyChange,
                    pagenumber,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetReturnReservationBillsAsync(storeId,
                    makeuserId,
                    terminalId,
                    terminalName,
                    businessUserId,
                    deliveryUserId,
                    wareHouseId,
                    districtId,
                    billNumber,
                    remark,
                    startTime,
                    endTime,
                    auditedStatus,
                    sortByAuditedTime,
                    showReverse,
                    showReturn,
                    alreadyChange,
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
        public async Task<ReturnReservationBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default)
        {
            var model = new ReturnReservationBillModel();
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReturnReservationBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("ReturnReservationBillService.GetBillAsync", storeId, userId, billId);
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
