using Wesley.Client.Models.WareHouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Wesley.Client.Services
{
    public class AllocationService : IAllocationService
    {
        private readonly MakeRequest _makeRequest;

        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/warehouse/allocationbill";

        public AllocationService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取调拨单
        /// </summary>
        /// <param name="forceRefresh"></param>
        /// <param name="makeuserId"></param>
        /// <param name="businessUserId"></param>
        /// <param name="shipmentWareHouseId"></param>
        /// <param name="incomeWareHouseId"></param>
        /// <param name="billNumber"></param>
        /// <param name="remark"></param>
        /// <param name="auditedStatus"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="showReverse"></param>
        /// <param name="sortByAuditedTime"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IList<AllocationBillModel>> GetAllocationsAsync(int? makeuserId, int businessUserId, int? shipmentWareHouseId, int? incomeWareHouseId, string billNumber, string remark, bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? showReverse = null, bool? sortByAuditedTime = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IAllocationApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetAllocationsAsync", storeId,
                    makeuserId,
                    businessUserId,
                    shipmentWareHouseId,
                    incomeWareHouseId,
                    billNumber,
                    remark,
                    auditedStatus,
                    startTime,
                    endTime,
                    showReverse,
                    sortByAuditedTime,
                    pagenumber,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetAllocationsAsync(storeId,
                    makeuserId,
                    businessUserId,
                    shipmentWareHouseId,
                    incomeWareHouseId,
                    billNumber,
                    remark,
                    auditedStatus,
                    startTime,
                    endTime,
                    showReverse,
                    sortByAuditedTime,
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
        /// 获取单据
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<AllocationBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default)
        {
            var model = new AllocationBillModel();
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IAllocationApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("AllocationService.GetBillAsync", storeId, userId, billId);
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
