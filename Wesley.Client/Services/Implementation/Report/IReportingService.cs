using Wesley.Client.Models.Report;
using Wesley.Client.Models.WareHouses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Wesley.Client.Services
{
    public interface IReportingService
    {
        Task<IList<BrandRanking>> GetBrandRankingAsync(int[] brandIds = null, int? businessUserId = 0, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default);
        Task<IList<BusinessRanking>> GetBusinessRankingAsync(int? businessUserId = null, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default);
        Task<IList<CostProfitRanking>> GetCostProfitRankingAsync(int? terminalId = null, int? businessUserId = null, int? brandId = null, int? categoryId = null, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default);
        Task<IList<CustomerRanking>> GetCustomerRankingAsync(int? terminalId = null, int? districtId = null, int? businessUserId = null, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default);
        Task<CustomerVistAnalysis> GetCustomerVistAnalysisAsync(int? businessUserId = null, bool force = false, CancellationToken calToken = default);
        Task<DashboardReport> GetDashboardReportAsync(bool force = false, CancellationToken calToken = default);
        Task<IList<HotSaleRanking>> GetHotOrderRankingAsync(int? terminalId = null, int? businessUserId = null, int? brandId = null, int? categoryId = null, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default);
        Task<IList<HotSaleRanking>> GetHotSaleRankingAsync(int? terminalId = null, int? businessUserId = null, int? brandId = null, int? categoryId = null, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default);
        Task<NewCustomerAnalysis> GetNewCustomerAnalysisAsync(int? businessUserId = null, bool force = false, CancellationToken calToken = default);
        Task<NewOrderAnalysis> GetNewOrderAnalysisAsync(int? businessUserId = null, bool force = false, CancellationToken calToken = default);
        Task<OrderQuantityAnalysis> GetOrderQuantityAnalysisAsync(int? businessUserId = null, int? brandId = null, int? productId = null, int? catagoryId = null, bool force = false, CancellationToken calToken = default);
        Task<SaleAnalysis> GetSaleAnalysisAsync(int? businessUserId = null, int? brandId = null, int? productId = null, int? categoryId = null, bool force = false, CancellationToken calToken = default);
        Task<IList<SaleTrending>> GetSaleTrendingAsync(string dateType, bool force = false, CancellationToken calToken = default);
        Task<IList<StockReportProduct>> GetStocksAsync(int? wareHouseId = 0, int? categoryId = 0, int? productId = 0, string productName = "", int? brandId = 0, bool? status = null, int? maxStock = 0, bool? showZeroStack = null, int pagenumber = 50, bool force = false, CancellationToken calToken = default);
        Task<IList<UnSaleRanking>> GetUnSaleRankingAsync(int? businessUserId = null, int? brandId = null, int? categoryId = null, DateTime? startTime = null, DateTime? endTime = null, bool force = false, CancellationToken calToken = default);
    }
}