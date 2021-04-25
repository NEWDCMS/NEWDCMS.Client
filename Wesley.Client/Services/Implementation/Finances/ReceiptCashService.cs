using Wesley.Client.Models.Finances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    /// <summary>
    /// 用于收款单管理
    /// </summary>
    public class ReceiptCashService : IReceiptCashService
    {
        private readonly MakeRequest _makeRequest;
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/finances/receiptcashbill";

        public ReceiptCashService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取表单创建初始绑定
        /// </summary>
        /// <returns></returns>
        public async Task<CashReceiptBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IReceiptCashApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("ReceiptCashService.GetInitDataAsync", storeId, userId);
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
        /// 获取收款列表
        /// </summary>
        /// <returns></returns>
        public async Task<IList<CashReceiptBillModel>> GetReceiptCashsAsync(int? makeuserId, int? customerId, string customerName, int? payeer, string billNumber, string remark, bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? showReverse = null, bool? sortByAuditedTime = null, bool? handleStatus = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IReceiptCashApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetReceiptCashsAsync", storeId,
                    makeuserId,
                    customerId,
                    customerName,
                    payeer,
                    billNumber,
                    remark,
                    auditedStatus,
                    startTime,
                    endTime,
                    showReverse,
                    sortByAuditedTime,
                    handleStatus,
                    pagenumber,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetReceiptCashsAsync(storeId,
                    makeuserId,
                    customerId,
                    customerName,
                    payeer,
                    billNumber,
                    remark,
                    auditedStatus,
                    startTime,
                    endTime,
                    showReverse,
                    sortByAuditedTime,
                    handleStatus,
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
        /// 获取欠款单据
        /// </summary>
        /// <returns></returns>
        public async Task<IList<BillSummaryModel>> GetOwecashBillsAsync(int userId, int? terminalId, int? billTypeId, string billNumber = "", string remark = "", DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                var api = RefitServiceBuilder.Build<IReceiptCashApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetOwecashBillsAsync", storeId,
                    userId,
                    terminalId,
                    billTypeId,
                    billNumber,
                    remark,
                    startTime,
                    endTime,
                    pageIndex,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetOwecashBillsAsync(storeId,
                    userId,
                    terminalId,
                    billTypeId,
                    billNumber,
                    remark,
                    startTime,
                    endTime,
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
        /// 提交单据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<APIResult<CashReceiptUpdateModel>> CreateOrUpdateAsync(CashReceiptUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReceiptCashApi>(URL);

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

                var api = RefitServiceBuilder.Build<IReceiptCashApi>(URL);

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

                var api = RefitServiceBuilder.Build<IReceiptCashApi>(URL);

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
        public async Task<CashReceiptBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IReceiptCashApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("ReceiptCashService.GetBillAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetBillAsync(storeId, userId, billId, calToken), cacheKey, force, calToken);
                return results?.Data;
            }
            catch (Exception e)
            {

                e.HandleException();
                return null;
            }
        }
    }
}
