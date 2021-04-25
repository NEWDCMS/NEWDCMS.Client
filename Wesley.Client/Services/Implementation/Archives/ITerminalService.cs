using Wesley.Client.Models;
using Wesley.Client.Models.Census;
using Wesley.Client.Models.Report;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Models.Visit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface ITerminalService
    {
        Task<APIResult<TerminalModel>> CreateOrUpdateAsync(TerminalModel data, CancellationToken calToken = default);
        Task<IList<BusinessVisitList>> GetAllUserVisitedListAsync(DateTime? date, bool force = false, CancellationToken calToken = default);
        Task<IList<BusinessVisitRank>> GetBusinessVisitRankingAsync(int? businessUserId = 0, DateTime? start = null, DateTime? end = null, bool force = false, CancellationToken calToken = default);
        Task<IList<ChannelModel>> GetChannelsAsync(bool force = false, CancellationToken calToken = default);
        Task<IList<CustomerActivityRanking>> GetCustomerActivityRankingAsync(int? businessUserId = 0, int? terminalId = 0, bool force = false, CancellationToken calToken = default);
        Task<IList<DistrictModel>> GetDistrictsAsync(bool force = false, CancellationToken calToken = default);
        Task<VisitStore> GetLastVisitStoreAsync(int? terminalId = 0, int? businessUserId = 0, CancellationToken calToken = default);
        Task<IList<LineTierModel>> GetLineTiersAsync(bool force = false, CancellationToken calToken = default);
        Task<IList<LineTierModel>> GetLineTiersByUserAsync(bool force = false, CancellationToken calToken = default);
        Task<VisitStore> GetOutVisitStoreAsync(int? businessUserId = 0, CancellationToken calToken = default);
        Task<IList<RankModel>> GetRanksAsync(bool force = false, CancellationToken calToken = default);
        Task<TerminalModel> GetTerminalAsync(int? terminalId, bool force = false, CancellationToken calToken = default);
        Task<TerminalBalance> GetTerminalBalance(int terminalId = 0, CancellationToken calToken = default);
        Task<IList<TerminalModel>> GetTerminalsAsync(string searchStr = "", int? districtId = 0, int? channelId = 0, int? rankId = 0, int? businessUserId = 0, bool status = true, int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<IList<TrackingModel>> GetUserTrackingAsync(int? businessUserId = 0, DateTime? start = null, DateTime? end = null, bool force = false, CancellationToken calToken = default);
        Task<IList<VisitStore>> GetVisitStoresAsync(int? terminalId = 0, int? districtId = 0, int? businessUserId = 0, DateTime? start = null, DateTime? end = null, bool force = false, CancellationToken calToken = default);
        Task<bool> ReportingTrackAsync(List<TrackingModel> data, CancellationToken calToken = default);
        Task<APIResult<VisitStore>> SignInVisitStoreAsync(VisitStore data, CancellationToken calToken = default);
        Task<APIResult<VisitStore>> SignOutVisitStoreAsync(VisitStore data, CancellationToken calToken = default);
        Task<IList<TerminalModel>> GetTerminalsPage(FilterModel filter, LineTierModel lineTier, int pageNumber, int pageSize, bool force = false, CancellationToken calToken = default);
        Task<APIResult<dynamic>> UpdateterminalAsync(int? terminalId, double location_lat, double location_lng, CancellationToken calToken = default);
        Task<bool> CheckTerminalAsync(string name, CancellationToken calToken = default);

    }

}