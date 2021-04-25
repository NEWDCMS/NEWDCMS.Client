using Wesley.Client.Models.Finances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    /// <summary>
    /// 用于预收款管理
    /// </summary>
    public class AdvanceReceiptService : IAdvanceReceiptService
    {
        private readonly MakeRequest _makeRequest;

        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/finances/advanceReceipt";

        public AdvanceReceiptService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取表单创建初始绑定
        /// </summary>
        /// <returns></returns>
        public async Task<AdvanceReceiptBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IAdvanceReceiptApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("AdvanceReceiptService.GetInitDataAsync", storeId, userId);
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
        /// 获取预收款单据
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<AdvanceReceiptBillModel>> GetAdvanceReceiptsAsync(int? customerId, int? makeuserId, string customerName, int? payeer, string billNumber = "", bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? showReverse = null, bool? sortByAuditedTime = null, int? accountingOptionId = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IAdvanceReceiptApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetAdvanceReceiptsAsync", storeId,
                    customerId,
                    makeuserId,
                    customerName,
                    payeer,
                    billNumber,
                    auditedStatus,
                    startTime,
                    endTime,
                    showReverse,
                    sortByAuditedTime,
                    accountingOptionId,
                    pagenumber,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetAdvanceReceiptsAsync(storeId,
                    customerId,
                    makeuserId,
                    customerName,
                    payeer,
                    billNumber,
                    auditedStatus,
                    startTime,
                    endTime,
                    showReverse,
                    sortByAuditedTime,
                    accountingOptionId,
                    pagenumber,
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
        /// 提交单据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<APIResult<AdvanceReceiptUpdateModel>> CreateOrUpdateAsync(AdvanceReceiptUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IAdvanceReceiptApi>(URL);

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
                var api = RefitServiceBuilder.Build<IAdvanceReceiptApi>(URL);

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
                var api = RefitServiceBuilder.Build<IAdvanceReceiptApi>(URL);

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
        /// 获取单据
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<AdvanceReceiptBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default)
        {
            var model = new AdvanceReceiptBillModel();
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IAdvanceReceiptApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("AdvanceReceiptService.GetBillAsync", storeId, userId);
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
