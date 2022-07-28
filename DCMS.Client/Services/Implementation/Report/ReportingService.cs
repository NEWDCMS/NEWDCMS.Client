using DCMS.Client.Models.Report;
using DCMS.Client.Models.Sales;
using DCMS.Client.Models.WareHouses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DCMS.Client.Services
{
    public class ReportingService : IReportingService
    {

        private readonly MakeRequest _makeRequest;
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/reporting";

        public ReportingService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        public IObservable<APIResult<DashboardReport>> Rx_GetDashboardReportAsync(CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                var businessUserIds = new int[] { Settings.UserId };
                var cacheKey = RefitServiceBuilder.Cacher("GetDashboardReportAsync", storeId, businessUserIds);
                var results = _makeRequest.StartUseCache_Rx(RefitServiceBuilder.Build<IReportingApi>(URL).GetDashboardReportAsync(storeId, businessUserIds, calToken), cacheKey, calToken);
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
        /// 库存查询
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<StockReportProduct>> GetStocksAsync(int? wareHouseId = 0, int? categoryId = 0, int? productId = 0, string productName = "", int? brandId = 0, bool? status = null, int? maxStock = 0, bool? showZeroStack = null, int pagenumber = 50, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetStocksAsync", storeId,
                    wareHouseId,
                    categoryId,
                    productId,
                    productName,
                    brandId,
                    status,
                    maxStock,
                    showZeroStack,
                    pagenumber);

                var results = await _makeRequest.StartUseCache(api.GetStocksAsync(storeId,
                    wareHouseId,
                    categoryId,
                    productId,
                    productName,
                    brandId,
                    status,
                    maxStock,
                    showZeroStack,
                    pagenumber, calToken),
                    cacheKey, force, calToken);

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
        /// 热销排行榜
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<HotSaleRanking>> GetHotSaleRankingAsync(int? terminalId = null, int? businessUserId = null, int? brandId = null, int? categoryId = null, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetHotSaleRankingAsync",
                    storeId,
                    terminalId,
                    businessUserId,
                    brandId,
                    categoryId,
                    force,
                    startTime,
                    endTime);

                var results = await _makeRequest.StartUseCache(api.GetHotSaleRankingAsync(storeId,
                    terminalId,
                    businessUserId,
                    brandId,
                    categoryId,
                    startTime,
                    endTime,
                    calToken),
                    cacheKey,
                    force,
                    calToken);

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

        public IObservable<APIResult<IList<HotSaleRanking>>> Rx_GetHotSaleRankingAsync(int? terminalId = null, int? businessUserId = null, int? brandId = null, int? categoryId = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("Rx_GetHotSaleRankingAsync",
                    storeId,
                    terminalId,
                    businessUserId,
                    brandId,
                    categoryId,
                    startTime?.ToString("yyy-MM-dd"),
                    endTime?.ToString("yyy-MM-dd"));

                var results = _makeRequest.StartUseCache_Rx(api.GetHotSaleRankingAsync(storeId,
                    terminalId,
                    businessUserId,
                    brandId,
                    categoryId,
                    startTime,
                    endTime,
                    calToken),
                    cacheKey,
                    calToken);

                return results;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }


        public async Task<IList<SaleReportItem>> GetHotSaleReportItemAsync(int? productId = null, int? businessUserId = null, DateTime? startTime = null, DateTime? endTime = null, bool force = false, int pagenumber = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var results = await api.GetHotSaleReportItemAsync(storeId,
                    productId: productId,
                    businessUserId: businessUserId,
                    startTime: startTime,
                    endTime: endTime,
                    pagenumber: pagenumber,
                    calToken: calToken);

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
        /// 热定排行榜
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<HotSaleRanking>> GetHotOrderRankingAsync(int? terminalId = null, int? businessUserId = null, int? brandId = null, int? categoryId = null, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetHotOrderRankingAsync", storeId, terminalId, businessUserId, brandId, categoryId, startTime, endTime);
                var results = await _makeRequest.StartUseCache(api.GetHotOrderRankingAsync(storeId, terminalId, businessUserId, brandId, categoryId, startTime, endTime, calToken), cacheKey, force, calToken);
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
        /// 销售商品成本利润排行榜
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<CostProfitRanking>> GetCostProfitRankingAsync(int? terminalId = null, int? businessUserId = null, int? brandId = null, int? categoryId = null, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetCostProfitRankingAsync", storeId, terminalId, businessUserId, brandId, categoryId, startTime, endTime);
                var results = await _makeRequest.StartUseCache(api.GetCostProfitRankingAsync(storeId, terminalId, businessUserId, brandId, categoryId, startTime, endTime, calToken), cacheKey, force, calToken);
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
        /// 获取销量走势图
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<SaleTrending>> GetSaleTrendingAsync(string dateType, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetSaleTrendingAsync", storeId, dateType);
                var results = await _makeRequest.StartUseCache(api.GetSaleTrendingAsync(storeId, dateType, calToken), cacheKey, force, calToken);
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
        /// 销售额分析 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<SaleAnalysis> GetSaleAnalysisAsync(int? businessUserId = null, int? brandId = null, int? productId = null, int? categoryId = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetSaleAnalysisAsync", storeId, businessUserId, brandId, productId, categoryId);
                var results = await _makeRequest.StartUseCache(api.GetSaleAnalysisAsync(storeId, businessUserId, brandId, productId, categoryId, calToken), cacheKey, force, calToken);
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
        /// 客户拜访分析
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<CustomerVistAnalysis> GetCustomerVistAnalysisAsync(int? businessUserId = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);
                var results = await _makeRequest.Start(api.GetCustomerVistAnalysisAsync(storeId, businessUserId, calToken), calToken);
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
        /// 新增加客户分析
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<NewCustomerAnalysis> GetNewCustomerAnalysisAsync(int? businessUserId = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetNewCustomerAnalysisAsync", storeId, businessUserId);
                var results = await _makeRequest.StartUseCache(api.GetNewCustomerAnalysisAsync(storeId, businessUserId, calToken), cacheKey, force, calToken);
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
        /// 订单额分析
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<OrderQuantityAnalysis> GetOrderQuantityAnalysisAsync(int? businessUserId = null, int? brandId = null, int? productId = null, int? catagoryId = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetOrderQuantityAnalysisAsync", storeId, businessUserId, brandId, productId, catagoryId);
                var results = await _makeRequest.StartUseCache(api.GetOrderQuantityAnalysisAsync(storeId, businessUserId, brandId, productId, catagoryId, calToken), cacheKey, force, calToken);
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
        /// 获取经销商品牌销量汇总
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<BrandRanking>> GetBrandRankingAsync(int[] brandIds = null, int? businessUserId = 0, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetBrandRankingAsync", storeId, brandIds, businessUserId, startTime, endTime);
                var results = await _makeRequest.StartUseCache(api.GetBrandRankingAsync(storeId, brandIds, businessUserId, startTime, endTime, calToken), cacheKey, force, calToken);
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
        /// 获取客户排行榜
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<CustomerRanking>> GetCustomerRankingAsync(int? terminalId = null, int? districtId = null, int? businessUserId = null, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetCustomerRankingAsync", storeId, terminalId, districtId, businessUserId, startTime, endTime);
                var results = await _makeRequest.StartUseCache(api.GetCustomerRankingAsync(storeId, terminalId, districtId, businessUserId, startTime, endTime, calToken), cacheKey, force, calToken);
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
        /// 获取业务员销售排行榜
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<BusinessRanking>> GetBusinessRankingAsync(int? businessUserId = null, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetBusinessRankingAsync", storeId, businessUserId, startTime, endTime);

                var results = await _makeRequest.StartUseCache(api.GetBusinessRankingAsync(storeId, businessUserId, startTime, endTime, calToken), cacheKey, force, calToken);
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
        /// 获取经销商滞销排行榜
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<UnSaleRanking>> GetUnSaleRankingAsync(int? businessUserId = null, int? brandId = null, int? categoryId = null, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetUnSaleRankingAsync", storeId,
                    businessUserId,
                    brandId,
                    categoryId,
                    startTime,
                    endTime);

                var results = await _makeRequest.StartUseCache(api.GetUnSaleRankingAsync(storeId,
                    businessUserId,
                    brandId,
                    categoryId,
                    startTime,
                    endTime,
                    calToken),
                    cacheKey, force, calToken);

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
        /// 新增订单分析
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<NewOrderAnalysis> GetNewOrderAnalysisAsync(int? businessUserId = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IReportingApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetNewOrderAnalysisAsync", storeId, businessUserId);
                var results = await _makeRequest.StartUseCache(api.GetNewOrderAnalysisAsync(storeId, businessUserId, calToken), cacheKey, force, calToken);
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
        /// 业务员综合分析
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public IObservable<APIResult<BusinessAnalysis>> Rx_GetBusinessAnalysis(int? type = null, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IReportingApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("GetBusinessAnalysis", storeId, type);
                var results = _makeRequest.StartUseCache_Rx(api.GetBusinessAnalysis(storeId, type, true, calToken), cacheKey, calToken);
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
