using Wesley.Client.Models.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Disposables;

namespace Wesley.Client.Services
{
    public class SaleReservationBillService : ISaleReservationBillService
    {
        private readonly MakeRequest _makeRequest;
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/sales/salereservationbill";

        public SaleReservationBillService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取初始绑定
        /// </summary>
        /// <returns></returns>
        public async Task<SaleReservationBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISaleReservationBillApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("SaleReservationBillService.GetInitDataAsync", storeId, userId);
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

        public IObservable<APIResult<SaleReservationBillModel>> Rx_GetInitDataAsync(CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISaleReservationBillApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("SaleReservationBillService.GetInitDataAsync", storeId, userId);
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
        public async Task<APIResult<SaleReservationBillUpdateModel>> CreateOrUpdateAsync(SaleReservationBillUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISaleReservationBillApi>(URL);
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

                var api = RefitServiceBuilder.Build<ISaleReservationBillApi>(URL);
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

                var api = RefitServiceBuilder.Build<ISaleReservationBillApi>(URL);
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
        /// 获取销售订单
        /// </summary>
        /// <param name="makeuserId"></param>
        /// <param name="terminalId"></param>
        /// <param name="terminalName"></param>
        /// <param name="businessUserId"></param>
        /// <param name="deliveryUserId"></param>
        /// <param name="billNumber"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="remark"></param>
        /// <param name="districtId"></param>
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
        public async Task<IList<SaleReservationBillModel>> GetSaleReservationBillsAsync(int? makeuserId, int? terminalId, string terminalName, int? businessUserId, int? deliveryUserId, string billNumber, int? wareHouseId, string remark, int? districtId, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, bool? showReturn = null, bool? alreadyChange = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISaleReservationBillApi>(URL);


                var results = await _makeRequest.Start(api.GetSaleReservationBillsAsync(storeId,
                    makeuserId,
                    terminalId,
                    terminalName,
                    businessUserId,
                    deliveryUserId,
                    billNumber,
                    wareHouseId,
                    remark,
                    districtId,
                    startTime,
                    endTime,
                    auditedStatus,
                    sortByAuditedTime,
                    showReverse,
                    showReturn,
                    alreadyChange,
                    pagenumber,
                    pageSize, calToken), calToken);

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
        public async Task<SaleReservationBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default)
        {
            var model = new SaleReservationBillModel();
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISaleReservationBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("SaleReservationBillService.GetBillAsync", storeId, userId, billId);
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
