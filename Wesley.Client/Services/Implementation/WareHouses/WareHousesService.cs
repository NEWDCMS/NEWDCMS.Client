using Wesley.Client.Models.WareHouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public class WareHousesService : IWareHousesService
    {
        private readonly MakeRequest _makeRequest;
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/warehouse";

        public WareHousesService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取库存
        /// </summary>
        /// <returns></returns>
        public async Task<IList<WareHouseModel>> GetWareHousesAsync(int type, string searchStr = "", int pageIndex = 0, int pageSize = 50, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IWareHousesApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetWareHousesAsync",
                    storeId,
                    type,
                    searchStr,
                    pageIndex,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetWareHousesAsync(storeId,
                    type,
                    searchStr,
                    pageIndex,
                    pageSize, calToken),
                    cacheKey, force, calToken);

                if (results != null && results?.Code >= 0)
                    return results?.Data?.ToList();
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
        /// 门店库存上报
        /// </summary>
        /// <param name="data"></param>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<APIResult<InventoryReportBillModel>> CreateOrUpdateAsync(InventoryReportBillModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IWareHousesApi>(URL);
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
        /// 获取库存上报表
        /// </summary>
        /// <param name="forceRefresh"></param>
        /// <param name="makeuserId"></param>
        /// <param name="businessUserId"></param>
        /// <param name="terminalId"></param>
        /// <param name="channelId"></param>
        /// <param name="rankId"></param>
        /// <param name="districtId"></param>
        /// <param name="productId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IList<InventoryReportSummaryModel>> GetInventoryReportAsync(int? makeuserId, int? businessUserId, int? terminalId, int? channelId, int? rankId, int? districtId, int? productId, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IWareHousesApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetInventoryReportAsync", storeId, makeuserId, businessUserId, terminalId, channelId, rankId, districtId, productId, startTime, endTime, pageIndex, pageSize);
                var results = await _makeRequest.StartUseCache(api.GetInventoryReportAsync(storeId, makeuserId, businessUserId, terminalId, channelId, rankId, districtId, productId, startTime, endTime, pageIndex, pageSize, calToken), cacheKey, force, calToken);
                if (results != null && results?.Code >= 0)
                    return results?.Data?.ToList();
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
        /// 创建库存调拨单
        /// </summary>
        /// <param name="data"></param>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<APIResult<AllocationUpdateModel>> CreateOrUpdateAllocationbillAsync(AllocationUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IWareHousesApi>(URL);
                var results = await _makeRequest.Start(api.CreateOrUpdateAllocationbillAsync(data, storeId, userId, billId, calToken), calToken);
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

                var api = RefitServiceBuilder.Build<IWareHousesApi>(URL);
                var results = await _makeRequest.Start(api.AuditingAsync(storeId, userId, billId, calToken), calToken);
                return (bool)(results?.Success);
            }
            catch (Exception e)
            {

                e.HandleException();
                return false;
            }
        }
    }
}
