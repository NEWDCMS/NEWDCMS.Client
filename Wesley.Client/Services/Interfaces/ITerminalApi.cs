using Wesley.Client.Models.Census;
using Wesley.Client.Models.Report;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Models.Visit;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    //[Headers("Authorization: Bearer")]
    public interface ITerminalApi
    {
        [Get("/census/vist/getAllUserVisitedList/{storeId}")]
        Task<APIResult<IList<BusinessVisitList>>> GetAllUserVisitedListAsync(int storeId, DateTime? date, CancellationToken calToken = default);

        [Get("/census/vist/getVisitedRanking/{storeId}")]
        Task<APIResult<IList<BusinessVisitRank>>> GetBusinessVisitRankingAsync(int storeId, int? businessUserId = 0, DateTime? start = null, DateTime? end = null, CancellationToken calToken = default);

        [Get("/archives/channel/getChannels/{storeId}")]
        Task<APIResult<IList<ChannelModel>>> GetChannelsAsync(int storeId, CancellationToken calToken = default);

        [Get("/census/vist/getCustomerActivityRanking/{storeId}")]
        Task<APIResult<IList<CustomerActivityRanking>>> GetCustomerActivityRankingAsync(int storeId, int? businessUserId = 0, int? terminalId = 0, CancellationToken calToken = default);

        [Get("/archives/terminal/getAllDistricts/{storeId}/{userId}")]
        Task<APIResult<IList<DistrictModel>>> GetDistrictsAsync(int storeId, int? userId = 0, CancellationToken calToken = default);

        [Get("/census/vist/getLastVisitStore/{storeId}")]
        Task<APIResult<VisitStore>> GetLastVisitStoreAsync(int storeId, int? terminalId = 0, int? businessUserId = 0, CancellationToken calToken = default);

        [Get("/archives/linetier/getLineTiers/{storeId}")]
        Task<APIResult<IList<LineTierModel>>> GetLineTiersAsync(int storeId, CancellationToken calToken = default);

        [Get("/archives/linetier/getLineTiersByUser/{storeId}/{userId}")]
        Task<APIResult<IList<LineTierModel>>> GetLineTiersByUserAsync(int storeId, int userId, CancellationToken calToken = default);

        [Get("/census/vist/getOutVisitStore/{storeId}")]
        Task<APIResult<VisitStore>> GetOutVisitStoreAsync(int storeId, int? businessUserId = 0, CancellationToken calToken = default);

        [Get("/archives/rank/getRanks/{storeId}")]
        Task<APIResult<IList<RankModel>>> GetRanksAsync(int storeId, CancellationToken calToken = default);

        [Get("/archives/terminal/{storeId}")]
        Task<APIResult<TerminalModel>> GetTerminalAsync(int storeId, int? terminalId, CancellationToken calToken = default);

        [Get("/archives/terminal/getTerminals/{storeId}/{userId}")]
        Task<APIResult<IList<TerminalModel>>> GetTerminalsAsync(int storeId, int userId, string searchStr = "", int? districtId = 0, int? channelId = 0, int? rankId = 0, bool status = true, int pageIndex = 0, int pageSize = 20, CancellationToken calToken = default);

        [Get("/census/vist/getUserTracking/{storeId}")]
        Task<APIResult<IList<TrackingModel>>> GetUserTrackingAsync(int storeId, int? businessUserId = 0, DateTime? start = null, DateTime? end = null, CancellationToken calToken = default);

        [Get("/census/vist/getVisitStores/{storeId}")]
        Task<APIResult<IList<VisitStore>>> GetVisitStoresAsync(int storeId, int? terminalId = 0, int? districtId = 0, int? businessUserId = 0, DateTime? start = null, DateTime? end = null, CancellationToken calToken = default);

        [Get("/archives/terminal/getterminalbalance/{storeId}")]
        Task<APIResult<TerminalBalance>> GetTerminalBalance(int storeId, int terminalId = 0, CancellationToken calToken = default);

        [Post("/census/vist/reportingTrack/{storeId}")]
        Task<APIResult<dynamic>> ReportingTrackAsync(List<TrackingModel> data, int storeId, int userId, CancellationToken calToken = default);

        [Post("/census/vist/signInVisitStore/{storeId}")]
        Task<APIResult<VisitStore>> SignInVisitStoreAsync(VisitStore data, int storeId, int userId, CancellationToken calToken = default);

        [Post("/census/vist/signOutVisitStore/{storeId}")]
        Task<APIResult<VisitStore>> SignOutVisitStoreAsync(VisitStore data, int storeId, int userId, CancellationToken calToken = default);

        [Post("/archives/terminal/createTerminal/{storeId}/{userId}")]
        Task<APIResult<TerminalModel>> CreateOrUpdateAsync(TerminalModel data, int storeId, int userId, CancellationToken calToken = default);

        [Post("/archives/terminal/updateterminal/{storeId}")]
        Task<APIResult<dynamic>> UpdateterminalAsync(int storeId, int? terminalId, double location_lat, double location_lng, CancellationToken calToken = default);

        [Get("/archives/terminal/checkTerminal/{store}")]
        Task<APIResult<dynamic>> CheckTerminalAsync(int store, string name, CancellationToken calToken = default);
    }
}