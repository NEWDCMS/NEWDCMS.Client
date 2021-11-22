using Wesley.Client.Models.WareHouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    /// <summary>
    /// 用于盘点服务管理
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly MakeRequest _makeRequest;
        //
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/warehouse/inventoryPartTask";
        ///api/v{version}/dcms/warehouse/inventoryparttask/createorupdate/{store}/{userId}/{billId}
        public InventoryService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取全部盘点单据
        /// </summary>
        /// <param name="makeuserId"></param>
        /// <param name="inventoryPerson"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="billNumber"></param>
        /// <param name="remark"></param>
        /// <param name="auditedStatus"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="inventoryStatus"></param>
        /// <param name="showReverse"></param>
        /// <param name="sortByCompletedTime"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IList<InventoryPartTaskBillModel>> GetInventoryAllsAsync(int? makeuserId, int? inventoryPerson, int? wareHouseId, string billNumber, string remark, bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, int? inventoryStatus = -1, bool? showReverse = null, bool? sortByCompletedTime = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IInventoryApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetInventoryAllsAsync", storeId,
                    makeuserId,
                    inventoryPerson,
                    wareHouseId,
                    billNumber,
                    remark,
                    auditedStatus,
                    startTime,
                    endTime,
                    inventoryStatus,
                    showReverse,
                    sortByCompletedTime,
                    pagenumber,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetInventoryAllsAsync(storeId,
                    makeuserId,
                    inventoryPerson,
                    wareHouseId,
                    billNumber,
                    remark,
                    auditedStatus,
                    startTime,
                    endTime,
                    inventoryStatus,
                    showReverse,
                    sortByCompletedTime,
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
        /// 获取盘点单
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<InventoryPartTaskBillModel> GetInventoryPartTaskBillAsync(int billId, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IInventoryApi>(URL);
                var results = await _makeRequest.Start(api.GetInventoryPartTaskBillAsync(storeId, userId, billId, calToken));
                return results?.Data;
            }
            catch (Exception e)
            {

                e.HandleException();
                return null;
            }
        }


        /// <summary>
        /// 检查盘点
        /// </summary>
        /// <param name="wareHouseId"></param>
        /// <returns></returns>
        public async Task<IList<InventoryPartTaskBillModel>> CheckInventoryAsync(int wareHouseId, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IInventoryApi>(URL);
                var results = await _makeRequest.Start(api.CheckInventoryAsync(storeId, userId, wareHouseId, calToken), calToken);
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
        /// 创建/更新
        /// </summary>
        /// <param name="data"></param>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<APIResult<InventoryPartTaskUpdateModel>> CreateOrUpdateAsync(InventoryPartTaskUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IInventoryApi>(URL);
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
        /// 取消盘点
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<bool> CancelTakeInventoryAsync(int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IInventoryApi>(URL);
                var results = await _makeRequest.Start(api.CancelTakeInventoryAsync(storeId, userId, billId, calToken), calToken);
                if (results?.Data != null && results?.Code >= 0)
                {
                    return (bool)(results?.Success);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {

                e.HandleException();
                return false;
            }
        }

        /// <summary>
        /// 完成盘点
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<bool> SetInventoryCompletedAsync(int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IInventoryApi>(URL);
                var results = await _makeRequest.Start(api.SetInventoryCompletedAsync(storeId, userId, billId, calToken), calToken);
                if (results?.Data != null && results?.Code >= 0)
                {
                    return (bool)(results?.Success);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {

                e.HandleException();
                return false;
            }
        }

    }
}
