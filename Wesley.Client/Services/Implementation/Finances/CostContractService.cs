
using Wesley.Client.Models.Finances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    /// <summary>
    /// 用于费用合同管理
    /// </summary>
    public class CostContractService : ICostContractService
    {
        private readonly MakeRequest _makeRequest;
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/finances/costContract";


        public CostContractService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取费用合同
        /// </summary>
        public async Task<IList<CostContractBillModel>> GetCostContractsAsync(int? makeuserId, int? customerId, string customerName, int? employeeId, string billNumber, string remark, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? showReverse = null, int pagenumber = 0, int pageSize = 50, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ICostContractApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetCostContractsAsync", storeId,
                    makeuserId,
                    customerId,
                    customerName,
                    employeeId,
                    billNumber,
                    remark,
                    startTime,
                    endTime,
                    auditedStatus,
                    showReverse,
                    pagenumber,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetCostContractsAsync(storeId,
                    makeuserId,
                    customerId,
                    customerName,
                    employeeId,
                    billNumber,
                    remark,
                    startTime,
                    endTime,
                    auditedStatus,
                    showReverse,
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
        /// 根据科目类别获取费用合同
        /// </summary>
        /// <param name="forceRefresh"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<CostContractBindingModel>> GetCostContractsBindingAsync(int? customerId, int? accountOptionId, int? accountCodeTypeId, int year, int month, int? contractType = 0, bool? auditedStatus = null, bool? showReverse = null, int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ICostContractApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetCostContractsBindingAsync", storeId,
                    customerId,
                    accountOptionId,
                    accountCodeTypeId,
                    year,
                    month,
                    contractType,
                    auditedStatus,
                    showReverse,
                    pageIndex,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetCostContractsAsync(storeId,
                    customerId,
                    accountOptionId,
                    accountCodeTypeId,
                    year,
                    month,
                    contractType,
                    auditedStatus,
                    showReverse,
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
        public async Task<APIResult<CostContractUpdateModel>> CreateOrUpdateAsync(CostContractUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ICostContractApi>(URL);
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
                var api = RefitServiceBuilder.Build<ICostContractApi>(URL);

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
                var api = RefitServiceBuilder.Build<ICostContractApi>(URL);

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
        public async Task<CostContractBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ICostContractApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("CostContractService.GetBillAsync", storeId, userId);
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
