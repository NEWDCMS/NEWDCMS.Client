using DCMS.Client.Models.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DCMS.Client.Services
{
    public class ExchangeBillService : IExchangeBillService
    {
        private readonly MakeRequest _makeRequest;
        //
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/sales/exchangeBill";

        public ExchangeBillService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 提交单据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<APIResult<ExchangeBillUpdateModel>> CreateOrUpdateAsync(ExchangeBillUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IExchangeBillApi>(URL);
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

                var api = RefitServiceBuilder.Build<IExchangeBillApi>(URL);
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

                var api = RefitServiceBuilder.Build<IExchangeBillApi>(URL);
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
        /// 获取换货单
        /// </summary>
        /// <returns></returns>
        public async Task<IList<ExchangeBillModel>> GetExchangeBillsAsync(int? makeuserId, int? terminalId, string terminalName, int? businessUserId, int? deliveryUserId, string billNumber, int? wareHouseId, string remark, int? districtId, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, bool? showReturn = null, bool? alreadyChange = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IExchangeBillApi>(URL);

                var results = await _makeRequest.Start(api.GetExchangeBillsAsync(storeId,
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
        public async Task<ExchangeBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default)
        {
            var model = new ExchangeBillModel();

            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IExchangeBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("ExchangeBillService.GetBillAsync", storeId, userId, billId);
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

        /// <summary>
        /// 用于换货签收确认
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<APIResult<DeliverySignUpdateModel>> DeliverySignConfirmAsync(DeliverySignUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IExchangeBillApi>(URL);
                var results = await _makeRequest.Start(api.DeliverySignConfirmAsync(data, storeId, userId, billId, calToken), calToken);
                return results;
            }
            catch (Exception e)
            {

                e.HandleException();
                return null;
            }
        }


    }
}
