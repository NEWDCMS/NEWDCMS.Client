using Wesley.Client.Models.Report;
using Wesley.Client.Models.WareHouses;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/reporting", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:10:00"), Trace]
    //[Headers("Authorization: Bearer")]
    public interface IReportingApi
    {
        [Get("/saleReport/getBrandRanking/{storeId}")]
        Task<APIResult<IList<BrandRanking>>> GetBrandRankingAsync(int storeId, [Query(CollectionFormat.Multi)] int[] brandIds = null, int? businessUserId = 0, DateTime? startTime = null, DateTime? endTime = null, CancellationToken calToken = default);

        [Get("/saleReport/getBusinessRanking/{storeId}")]
        Task<APIResult<IList<BusinessRanking>>> GetBusinessRankingAsync(int storeId, int? businessUserId = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken calToken = default);

        [Get("/saleReport/getCostProfitRanking/{storeId}")]
        Task<APIResult<IList<CostProfitRanking>>> GetCostProfitRankingAsync(int storeId, int? terminalId = null, int? businessUserId = null, int? brandId = null, int? categoryId = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken calToken = default);

        [Get("/saleReport/getCustomerRanking/{storeId}")]
        Task<APIResult<IList<CustomerRanking>>> GetCustomerRankingAsync(int storeId, int? terminalId = null, int? districtId = null, int? businessUserId = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken calToken = default);

        [Get("/saleReport/getCustomerVistAnalysis/{storeId}")]
        Task<APIResult<CustomerVistAnalysis>> GetCustomerVistAnalysisAsync(int storeId, int? businessUserId = null, CancellationToken calToken = default);

        [Get("/dashboard/getDashboardReport/{storeId}")]
        Task<APIResult<DashboardReport>> GetDashboardReportAsync(int storeId, int[] businessUserIds = null, CancellationToken calToken = default);

        [Get("/saleReport/getHotOrderRanking/{storeId}")]
        Task<APIResult<IList<HotSaleRanking>>> GetHotOrderRankingAsync(int storeId, int? terminalId = null, int? businessUserId = null, int? brandId = null, int? categoryId = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken calToken = default);


        [Get("/saleReport/getHotSaleRanking/{storeId}")]
        Task<APIResult<IList<HotSaleRanking>>> GetHotSaleRankingAsync(int storeId, int? terminalId = null, int? businessUserId = null, int? brandId = null, int? categoryId = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken calToken = default);

        [Get("/saleReport/getNewCustomerAnalysis/{storeId}")]
        Task<APIResult<NewCustomerAnalysis>> GetNewCustomerAnalysisAsync(int storeId, int? businessUserId = null, CancellationToken calToken = default);

        [Get("/saleReport/getNewOrderAnalysis/{storeId}")]
        Task<APIResult<NewOrderAnalysis>> GetNewOrderAnalysisAsync(int storeId, int? businessUserId = null, CancellationToken calToken = default);

        [Get("/saleReport/getOrderQuantityAnalysis/{storeId}")]
        Task<APIResult<OrderQuantityAnalysis>> GetOrderQuantityAnalysisAsync(int storeId, int? businessUserId = null, int? brandId = null, int? productId = null, int? catagoryId = null, CancellationToken calToken = default);

        [Get("/saleReport/getSaleAnalysis/{storeId}")]
        Task<APIResult<SaleAnalysis>> GetSaleAnalysisAsync(int storeId, int? businessUserId = null, int? brandId = null, int? productId = null, int? categoryId = null, CancellationToken calToken = default);

        [Get("/saleReport/getSaleTrending/{storeId}/{dateType}")]
        Task<APIResult<IList<SaleTrending>>> GetSaleTrendingAsync(int storeId, string dateType, CancellationToken calToken = default);

        [Get("/stock/getStocks/{storeId}")]
        Task<APIResult<IList<StockReportProduct>>> GetStocksAsync(int storeId, int? wareHouseId = 0, int? categoryId = 0, int? productId = 0, string productName = "", int? brandId = 0, bool? status = null, int? maxStock = 0, bool? showZeroStack = null, int pagenumber = 50, CancellationToken calToken = default);

        [Get("/saleReport/getUnSaleRanking/{storeId}")]
        Task<APIResult<IList<UnSaleRanking>>> GetUnSaleRankingAsync(int storeId, int? businessUserId = null, int? brandId = null, int? categoryId = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken calToken = default);
    }
}