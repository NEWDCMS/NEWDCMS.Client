using Wesley.Client.Models.Finances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    /// <summary>
    /// 用于费用支出管理
    /// </summary>
    public class CostExpenditureService : ICostExpenditureService
    {

        private readonly MakeRequest _makeRequest;
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/finances/costexpenditurebill";

        public CostExpenditureService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取费用支出单据
        /// </summary>
        public async Task<IList<CostExpenditureBillModel>> GetCostExpendituresAsync(int? makeuserId, int? customerId, string customerName, int? employeeId, string billNumber, bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? showReverse = null, bool sortByAuditedTime = false, int? sign = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ICostExpenditureApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetCostExpendituresAsync", storeId,
                    makeuserId,
                    customerId,
                    customerName,
                    employeeId,
                    billNumber,
                    auditedStatus,
                    startTime,
                    endTime,
                    showReverse,
                    sortByAuditedTime,
                    sign,
                    pagenumber,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetCostExpendituresAsync(storeId,
                    makeuserId,
                    customerId,
                    customerName,
                    employeeId,
                    billNumber,
                    auditedStatus,
                    startTime,
                    endTime,
                    showReverse,
                    sortByAuditedTime,
                    sign,
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
        public async Task<APIResult<CostExpenditureUpdateModel>> CreateOrUpdateAsync(CostExpenditureUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {

            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ICostExpenditureApi>(URL);

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
                var api = RefitServiceBuilder.Build<ICostExpenditureApi>(URL);

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
                var api = RefitServiceBuilder.Build<ICostExpenditureApi>(URL);

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
        /// 获取单据
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<CostExpenditureBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ICostExpenditureApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("CostExpenditureService.GetBillAsync", storeId, userId, billId);
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
