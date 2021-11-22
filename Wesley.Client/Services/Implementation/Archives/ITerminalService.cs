using Wesley.Client.Models;
using Wesley.Client.Models.Census;
using Wesley.Client.Models.Report;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Models.Visit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using Acr.UserDialogs;

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


        Task<APIResult<IList<TerminalModel>>> GetTerminalsAsync(string searchStr = "", int? districtId = 0, int? channelId = 0, int? rankId = 0, int? businessUserId = 0, int? lineId = 0, bool status = true, int pageIndex = 0, int pageSize = 20, CancellationToken calToken = default);
        Task<APIResult<IList<TerminalModel>>> GetRealTimeTerminalsAsync(string searchStr = "", int? districtId = 0, int? channelId = 0, int? rankId = 0, int? businessUserId = 0, int? lineId = 0, bool status = true, int pageIndex = 0, int pageSize = 20, double range = 0.3, CancellationToken calToken = default);



        Task<IList<TrackingModel>> GetUserTrackingAsync(int? businessUserId = 0, DateTime? start = null, DateTime? end = null, bool force = false, CancellationToken calToken = default);
        Task<IList<VisitStore>> GetVisitStoresAsync(int? terminalId = 0, int? districtId = 0, int? businessUserId = 0, DateTime? start = null, DateTime? end = null, int pageIndex = 0, int pageSize = 50, bool force = false, CancellationToken calToken = default);
        Task<bool> ReportingTrackAsync(List<TrackingModel> data, CancellationToken calToken = default);
        Task<APIResult<VisitStore>> SignInVisitStoreAsync(VisitStore data, CancellationToken calToken = default);
        Task<APIResult<VisitStore>> SignOutVisitStoreAsync(VisitStore data, CancellationToken calToken = default);

        Task<bool> UpdateterminalAsync(int? terminalId, double location_lat, double location_lng, CancellationToken calToken = default);
        Task<bool> CheckTerminalAsync(string name, CancellationToken calToken = default);
        Task<TerminalModel> RepreTerminal(TerminalModel s);

        IObservable<APIResult<IList<LineTierModel>>> Rx_GetLineTiersByUserAsync(CancellationToken calToken = default);

        Task<Tuple<int, IList<TerminalModel>>> SearchTerminals(string searchStr = "", int? districtId = 0, int? channelId = 0, int? rankId = 0, int? lineId = 0, int? businessUserId = 0, bool status = true, int distanceOrderBy = 0, double lat = 0, double lng = 0, double range = 0.5, int pageIndex = 0, int pageSize = 20);

        IObservable<TerminalBalance> Rx_GetTerminalBalance(int terminalId = 0, CancellationToken calToken = default);
        IObservable<VisitStore> Rx_GetLastVisitStoreAsync(int? terminalId = 0, int? businessUserId = 0, CancellationToken calToken = default);
        IObservable<VisitStore> Rx_GetOutVisitStoreAsync(int? businessUserId = 0, CancellationToken calToken = default);



        Task<TerminalModel> GetTerminalByIdAsync(int? terminalId);
        Task<bool> UpdateTerminal(TerminalModel terminal);
        Task<bool> AddTerminal(TerminalModel terminal);
    }

}