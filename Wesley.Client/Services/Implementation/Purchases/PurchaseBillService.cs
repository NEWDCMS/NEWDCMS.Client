using Wesley.Client.Models.Purchases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public class PurchaseBillService : IPurchaseBillService
    {
        private readonly MakeRequest _makeRequest;
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/purchases";
        public PurchaseBillService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }


        /// <summary>
        /// 获取采购列表初始绑定
        /// </summary>
        /// <returns></returns>
        public async Task<IList<PurchaseBillModel>> GetInitDataForListAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IPurchaseBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetInitDataForListAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetInitDataForListAsync(storeId, calToken), cacheKey, force, calToken);
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
        /// 获取采购表单创建初始绑定
        /// </summary>
        /// <returns></returns>
        public async Task<PurchaseBillModel> GetInitDataAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IPurchaseBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("PurchaseBillService.GetInitDataAsync", storeId, userId);
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
        public async Task<APIResult<PurchaseItemUpdateModel>> CreateOrUpdateAsync(PurchaseItemUpdateModel data, int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IPurchaseBillApi>(URL);

                var results = await _makeRequest.Start(api.CreateOrUpdateAsync(data, storeId, userId, billId, calToken), calToken);
                return results;
            }
            catch (Exception e)
            {

                e.HandleException();
                return null;
            }
        }

        public async Task<bool> AuditingAsync(int billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IPurchaseBillApi>(URL);

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

                var api = RefitServiceBuilder.Build<IPurchaseBillApi>(URL);

                var results = await _makeRequest.Start(api.ReverseAsync(storeId, userId, billId, calToken), calToken);
                return (bool)(results?.Success);
            }
            catch (Exception e)
            {

                e.HandleException();
                return false;
            }
        }

        public async Task<IList<PurchaseItemModel>> AsyncPurchaseItemByProductIdForAsync(int? productId, bool? beforeTax, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IPurchaseBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("AsyncPurchaseItemByProductIdForAsync", storeId, productId, beforeTax);
                var results = await _makeRequest.StartUseCache(api.AsyncPurchaseItemByProductIdForAsync(storeId, productId, beforeTax, calToken), cacheKey, force, calToken);
                if (results != null && results.Code >= 0)
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
        /// 获取采购单列表
        /// </summary>
        /// <param name="makeuserId"></param>
        /// <param name="businessUserId"></param>
        /// <param name="manufacturerId"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="billNumber"></param>
        /// <param name="remark"></param>
        /// <param name="printStatus"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="auditedStatus"></param>
        /// <param name="sortByAuditedTime"></param>
        /// <param name="showReverse"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IList<PurchaseBillModel>> GetPurchaseBillsAsync(int? makeuserId, int? businessUserId, int? manufacturerId, int? wareHouseId, string billNumber, string remark, bool? printStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? sortByAuditedTime = null, bool? showReverse = null, int pagenumber = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IPurchaseBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetPurchaseBillsAsync", storeId,
                    makeuserId,
                    businessUserId,
                    manufacturerId,
                    wareHouseId,
                    billNumber,
                    remark,
                    printStatus,
                    startTime,
                    endTime,
                    auditedStatus,
                    sortByAuditedTime,
                    showReverse,
                    pagenumber,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetPurchaseBillsAsync(storeId,
                    makeuserId,
                    businessUserId,
                    manufacturerId,
                    wareHouseId,
                    billNumber,
                    remark,
                    printStatus,
                    startTime,
                    endTime,
                    auditedStatus,
                    sortByAuditedTime,
                    showReverse,
                    pagenumber,
                    pageSize, calToken),
                    cacheKey, force, calToken);

                if (results != null && results.Code >= 0)
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
        public async Task<PurchaseBillModel> GetBillAsync(int billId, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IPurchaseBillApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("PurchaseBillService.GetBillAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetBillAsync(storeId, userId, billId, calToken), cacheKey, force, calToken);
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
    }
}
