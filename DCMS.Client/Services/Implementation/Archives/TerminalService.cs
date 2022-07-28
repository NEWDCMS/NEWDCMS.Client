using Wesley.Client.Models.Census;
using Wesley.Client.Models.Report;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Models.Visit;
using Wesley.Infrastructure.Helpers;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Wesley.Client.Services
{
    public class TerminalService : ITerminalService
    {
        private readonly ILiteDbService<VisitStore> _conn;
        private readonly ILiteDbService<TerminalModel> _tliteDb;
        private readonly MakeRequest _makeRequest;
        private readonly static object _lock = new object();
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms";

        public TerminalService(ILiteDbService<VisitStore> conn,
            ILiteDbService<TerminalModel> tliteDb,
            MakeRequest makeRequest)
        {
            _conn = conn;
            _tliteDb = tliteDb;
            _makeRequest = makeRequest;
        }


        /// <summary>
        /// 获取经销商终端
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<APIResult<IList<TerminalModel>>> GetTerminalsAsync(string searchStr = "", int? districtId = 0, int? channelId = 0, int? rankId = 0, int? businessUserId = 0, int? lineId = 0, bool status = true, int pageIndex = 0, int pageSize = 20, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;

                if (!businessUserId.HasValue || (businessUserId.HasValue && businessUserId.Value == 0))
                {
                    businessUserId = Settings.UserId;
                }

                int userId = businessUserId ?? 0;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetTerminalsAsync", storeId,
                    userId,
                    searchStr,
                    districtId,
                    channelId,
                    rankId,
                    status,
                    pageIndex,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetTerminalsAsync(storeId,
                    userId,
                    searchStr,
                    districtId,
                    channelId,
                    rankId,
                    status,
                    pageIndex,
                    pageSize,
                    calToken),
                    cacheKey,
                    true,
                    calToken);

                return results;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }

        public Task<APIResult<IList<TerminalModel>>> GetRealTimeTerminalsAsync(string searchStr = "", int? districtId = 0, int? channelId = 0, int? rankId = 0, int? businessUserId = 0, int? lineId = 0, bool status = true, int pageIndex = 0, int pageSize = 20, double range = 0.3, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;

                if (!businessUserId.HasValue || (businessUserId.HasValue && businessUserId.Value == 0))
                {
                    businessUserId = Settings.UserId;
                }

                int userId = businessUserId ?? 0;

                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetRealTimeTerminalsAsync", storeId,
                    userId,
                    searchStr,
                    districtId,
                    channelId,
                    rankId,
                    status,
                    pageIndex,
                    pageSize,
                    range);

                var results = _makeRequest.StartUseCache(api.GetTerminalsAsync(storeId,
                    userId,
                    searchStr,
                    districtId,
                    channelId,
                    rankId,
                    status,
                    GlobalSettings.Latitude ?? 0,
                    GlobalSettings.Longitude ?? 0,
                    0.3,
                    pageIndex,
                    pageSize,
                    calToken),
                    cacheKey,
                    true,
                    calToken);

                return results;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }


        public async Task<TerminalModel> GetTerminalAsync(int? terminalId, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("GetTerminalAsync", storeId, terminalId);
                var results = await _makeRequest.StartUseCache(api.GetTerminalAsync(storeId, terminalId, calToken), cacheKey, force, calToken);

                if (results == null)
                    return new TerminalModel();

                if (results?.Data != null && results?.Code >= 0)
                    return results?.Data;
                else
                    return new TerminalModel();
            }
            catch (Exception e)
            {
                e.HandleException();
                return new TerminalModel();
            }
        }

        /// <summary>
        /// 获取经销商片区
        /// </summary>
        /// <returns></returns>
        public async Task<IList<DistrictModel>> GetDistrictsAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetDistrictsAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetDistrictsAsync(storeId, userId, calToken), cacheKey, force, calToken);
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
        /// 创建/更新经销商终端
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<APIResult<TerminalModel>> CreateOrUpdateAsync(TerminalModel data, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var results = await _makeRequest.Start(api.CreateOrUpdateAsync(data, storeId, userId, calToken), calToken);
                return results;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }


        /// <summary>
        /// 获取经销商渠道
        /// </summary>
        /// <returns></returns>
        public async Task<IList<ChannelModel>> GetChannelsAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("GetChannelsAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetChannelsAsync(storeId, calToken), cacheKey, force, calToken);
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
        /// 获取经销商线路
        /// </summary>
        /// <returns></returns>
        public async Task<IList<LineTierModel>> GetLineTiersAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("GetLineTiersAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetLineTiersAsync(storeId, calToken), cacheKey, force, calToken);
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
        /// 获取经销商业务员终端拜访支配线路
        /// </summary>
        /// <returns></returns>
        public async Task<IList<LineTierModel>> GetLineTiersByUserAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetLineTiersByUserAsync", storeId, userId);

                var results = await _makeRequest.StartUseCache(api.GetLineTiersByUserAsync(storeId,
                    userId,
                    calToken),
                    cacheKey,
                    force,
                    calToken);

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

        public IObservable<APIResult<IList<LineTierModel>>> Rx_GetLineTiersByUserAsync(CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("Rx_GetLineTiersByUserAsync", storeId, userId);

                var results = _makeRequest.StartUseCache_Rx(api.GetLineTiersByUserAsync(storeId,
                    userId,
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



        /// <summary>
        /// 获取经销商等级
        /// </summary>
        /// <returns></returns>
        public async Task<IList<RankModel>> GetRanksAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("GetRanksAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetRanksAsync(storeId, calToken), cacheKey, force, calToken);

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
        /// 拜访签到
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<APIResult<VisitStore>> SignInVisitStoreAsync(VisitStore data, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);

                var results = await _makeRequest.Start(api.SignInVisitStoreAsync(data, storeId, userId, calToken), calToken);
                if (results == null)
                    return new APIResult<VisitStore>() { Success = false, Message = "服务请求失败" };

                if (results?.Data != null && results?.Code >= 0)
                    return results;
                else
                    return new APIResult<VisitStore>() { Success = false, Message = results?.Message };
            }
            catch (Exception e)
            {
                e.HandleException();
                return new APIResult<VisitStore>() { Success = false, Message = e.Message };
            }

        }


        /// <summary>
        /// 拜访签退
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<APIResult<VisitStore>> SignOutVisitStoreAsync(VisitStore data, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);

                var results = await _makeRequest.Start(api.SignOutVisitStoreAsync(data, storeId, userId, calToken), calToken);

                if (results == null)
                    return null;

                if (results?.Data != null && results?.Code >= 0)
                    return results;
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
        /// 获取业务员门店拜访记录
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<VisitStore>> GetVisitStoresAsync(int? terminalId = 0, int? districtId = 0, int? businessUserId = 0, DateTime? start = null, DateTime? end = null, int pageIndex = 0, int pageSize = 50, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetVisitStoresAsync", storeId,
                    terminalId,
                    districtId,
                    businessUserId,
                    start,
                    end);

                var results = await _makeRequest.StartUseCache(api.GetVisitStoresAsync(storeId,
                    terminalId,
                    districtId,
                    businessUserId,
                    start,
                    end, calToken), cacheKey, force, calToken);

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

        public async Task<VisitStore> GetOutVisitStoreAsync(int? businessUserId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var results = await _makeRequest.Start(api.GetOutVisitStoreAsync(storeId, businessUserId, calToken), calToken);

                if (results == null)
                    return null;

                if (results?.Data != null && results?.Code >= 0)
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
        public async Task<VisitStore> GetLastVisitStoreAsync(int? terminalId = 0, int? businessUserId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var results = await _makeRequest.Start(api.GetLastVisitStoreAsync(storeId, terminalId, businessUserId, calToken), calToken);

                if (results == null)
                    return null;

                if (results?.Data != null && results?.Code >= 0)
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
        /// 获取用户拜访记录
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<BusinessVisitList>> GetAllUserVisitedListAsync(DateTime? date, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("GetAllUserVisitedListAsync", storeId, date, userId);
                var results = await _makeRequest.StartUseCache(api.GetAllUserVisitedListAsync(storeId, date, calToken), cacheKey, force, calToken);

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
        /// 获取业务员拜访排行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<BusinessVisitRank>> GetBusinessVisitRankingAsync(int? businessUserId = 0, DateTime? start = null, DateTime? end = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetBusinessVisitRankingAsync", storeId, businessUserId, start, end);
                var results = await _makeRequest.StartUseCache(api.GetBusinessVisitRankingAsync(storeId, businessUserId, start, end, calToken), cacheKey, force, calToken);

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
        /// 获取业务员轨迹跟踪
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<TrackingModel>> GetUserTrackingAsync(int? businessUserId = 0, DateTime? start = null, DateTime? end = null, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetUserTrackingAsync", storeId, businessUserId, start, end);
                var results = await _makeRequest.StartUseCache(api.GetUserTrackingAsync(storeId, businessUserId, start, end, calToken), cacheKey, force, calToken);

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
        /// 上报业务员轨迹
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> ReportingTrackAsync(List<TrackingModel> data, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var results = await _makeRequest.Start(api.ReportingTrackAsync(data, storeId, userId, calToken), calToken);

                return results.Success;
            }
            catch (System.Net.Sockets.SocketException)
            {
                return false;
            }
            catch (Refit.ApiException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 获取客户拜访活跃度排行榜
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<CustomerActivityRanking>> GetCustomerActivityRankingAsync(int? businessUserId = 0, int? terminalId = 0, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetCustomerActivityRankingAsync", storeId, businessUserId, terminalId);

                var results = await _makeRequest.StartUseCache(api.GetCustomerActivityRankingAsync(storeId, businessUserId, terminalId, calToken), cacheKey, force, calToken);

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
        /// 获取经销商账户余额
        /// </summary>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public async Task<TerminalBalance> GetTerminalBalance(int terminalId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var results = await _makeRequest.Start(api.GetTerminalBalance(storeId, terminalId, calToken), calToken);

                if (results == null)
                    return new TerminalBalance();

                if (results?.Data != null && results?.Code >= 0)
                    return results?.Data;
                else
                    return new TerminalBalance();
            }
            catch (Exception e)
            {
                e.HandleException();
                return new TerminalBalance();
            }
        }

        public IObservable<TerminalBalance> Rx_GetTerminalBalance(int terminalId = 0, CancellationToken calToken = default)
        {
            return Observable.Create<TerminalBalance>(async (observer, token) =>
            {
                try
                {
                    var t = await GetTerminalBalance(terminalId, calToken);
                    if (t != null)
                        observer.OnNext(t);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
                observer.OnCompleted();
            });
        }


        public IObservable<VisitStore> Rx_GetOutVisitStoreAsync(int? businessUserId = 0, CancellationToken calToken = default)
        {
            return Observable.Create<VisitStore>(async (observer, token) =>
            {
                try
                {
                    var v = await GetOutVisitStoreAsync(businessUserId, calToken);
                    if (v != null)
                        observer.OnNext(v);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }

                observer.OnCompleted();
            });
        }
        public IObservable<VisitStore> Rx_GetLastVisitStoreAsync(int? terminalId = 0, int? businessUserId = 0, CancellationToken calToken = default)
        {
            return Observable.Create<VisitStore>(async (observer, token) =>
            {
                try
                {
                    var v = await GetLastVisitStoreAsync(terminalId, businessUserId, calToken);
                    if (v != null)
                        observer.OnNext(v);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }

                observer.OnCompleted();
            });
        }


        public async Task<TerminalModel> RepreTerminal(TerminalModel s)
        {
            if (!string.IsNullOrEmpty(s.DoorwayPhoto))
            {
                s.DoorwayPhoto = s.DoorwayPhoto.StartsWith("http") ? s.DoorwayPhoto : "profile_placeholder.png";
            }
            else
            {
                s.DoorwayPhoto = "profile_placeholder.png";
            }
            s.IsNewAdd = DateTime.Now.Subtract(s.CreatedOnUtc).Days < 3;

            s.RankName = string.IsNullOrEmpty(s.RankName) ? "A级" : s.RankName;
            if (s.RankName == "A级")
            {
                s.RankColor = "#4a89dc";
            }
            else if (s.RankName == "B级")
            {
                s.RankColor = "#626262";
            }
            else if (s.RankName == "C级")
            {
                s.RankColor = "#53a245";
            }
            else if (s.RankName == "D级")
            {
                s.RankColor = "#8942dc";
            }
            s.Distance = MapHelper.CalculateDistance(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0, s.Location_Lat ?? 0, s.Location_Lng ?? 0);
            s.TodayIsVisit = await CheckVisitStore(s);

            return s;
        }

        private async Task<bool> CheckVisitStore(TerminalModel data)
        {
            try
            {
                var t = await GetTerminalByIdAsync(data.Id);
                if (t != null)
                {
                    data.SignOutDateTime = t.SignOutDateTime;
                }

                return data.SignOutDateTime.ToString("yyyy-MM-dd").Equals(DateTime.Now.ToString("yyyy-MM-dd"));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateterminalAsync(int? terminalId, double location_lat, double location_lng, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var results = await _makeRequest.Start(api.UpdateterminalAsync(storeId, terminalId, location_lat, location_lng, calToken), calToken);

                if (results == null)
                    return false;

                if (results?.Data != null && results?.Code >= 0)
                    return results.Data;
                else
                    return false;
            }
            catch (Exception e)
            {
                e.HandleException();
                return false;
            }
        }

        public async Task<bool> CheckTerminalAsync(string name, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var results = await _makeRequest.Start(api.CheckTerminalAsync(storeId, name, calToken), calToken);
                if (results != null)
                {
                    return (bool)results.Data;
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
        /// 本地查询
        /// </summary>
        /// <param name="searchStr"></param>
        /// <param name="districtId"></param>
        /// <param name="channelId"></param>
        /// <param name="rankId"></param>
        /// <param name="lineId"></param>
        /// <param name="businessUserId"></param>
        /// <param name="status"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="range"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuple<int, IList<TerminalModel>>> SearchTerminals(string searchStr = "", int? districtId = 0, int? channelId = 0, int? rankId = 0, int? lineId = 0, int? businessUserId = 0, bool status = true, int distanceOrderBy = 0, double lat = 0, double lng = 0, double range = 1.5, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var results = new APIResult<IList<TerminalModel>>();

                if (range == 0)
                {
                    results = await GetTerminalsAsync(
                       searchStr,
                       districtId,
                       channelId,
                       rankId,
                       businessUserId,
                       lineId,
                       status,
                       pageIndex,
                       pageSize,
                       new CancellationToken());
                }
                else
                {
                    results = await GetRealTimeTerminalsAsync(searchStr,
                      districtId,
                      channelId,
                      rankId,
                      businessUserId,
                      lineId,
                      status,
                      pageIndex,
                      pageSize,
                      range,
                      new CancellationToken());
                }

                var pending = new List<TerminalModel>();

                if (results?.Data != null && results?.Code >= 0)
                {
                    pending = results?.Data.ToList();

                    if (distanceOrderBy > 0)
                    {
                        if (distanceOrderBy == 1)
                            pending = pending.OrderBy(s => s.Distance).ToList();
                        else if (distanceOrderBy == 2)
                            pending = pending.OrderByDescending(s => s.Distance).ToList();
                    }

                    pending = pending.DistinctBy(p => p.Id).ToList();

                    //匹配加工
                    foreach (var s in pending)
                    {
                        if (!string.IsNullOrEmpty(s.DoorwayPhoto))
                        {
                            s.DoorwayPhoto = s.DoorwayPhoto.StartsWith("http") ? s.DoorwayPhoto : "profile_placeholder.png";
                        }
                        else
                        {
                            s.DoorwayPhoto = "profile_placeholder.png";
                        }
                        s.IsNewAdd = DateTime.Now.Subtract(s.CreatedOnUtc).Days < 3;

                        s.RankName = string.IsNullOrEmpty(s.RankName) ? "A级" : s.RankName;

                        if (s.RankName.Contains("a") || s.RankName.Contains("A"))
                        {
                            s.RankName = "A级";
                            s.RankColor = "#4a89dc";
                        }
                        else if (s.RankName.Contains("b") || s.RankName.Contains("B"))
                        {
                            s.RankName = "B级";
                            s.RankColor = "#626262";
                        }
                        else if (s.RankName.Contains("c") || s.RankName.Contains("C"))
                        {
                            s.RankName = "C级";
                            s.RankColor = "#53a245";
                        }
                        else if (s.RankName.Contains("d") || s.RankName.Contains("D"))
                        {
                            s.RankName = "D级";
                            s.RankColor = "#8942dc";
                        }

                        s.TodayIsVisit = await CheckVisitStore(s);
                        s.BossCall = string.IsNullOrEmpty(s.BossCall) ? "" : s.BossCall;
                        s.Address = string.IsNullOrEmpty(s.Address) ? "" : s.Address;
                        s.LastSigninDateTimeName = string.IsNullOrEmpty(s.LastSigninDateTimeName) ? "" : s.LastSigninDateTimeName;
                        s.RankName = string.IsNullOrEmpty(s.RankName) ? "" : s.RankName;
                    }

                }

                return new Tuple<int, IList<TerminalModel>>(results?.Rows ?? 0, pending);
            }
            catch (Exception)
            {
                return new Tuple<int, IList<TerminalModel>>(0, new List<TerminalModel>());
            }
        }



        public async Task<TerminalModel> GetTerminalByIdAsync(int? terminalId)
        {
            try
            {
                return await _tliteDb.Table.FindByIdAsync(terminalId ?? 0);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> UpdateTerminal(TerminalModel terminal)
        {
            return await _tliteDb.UpsertAsync(terminal);
        }

        public async Task<bool> AddTerminal(TerminalModel terminal)
        {
            if (await GetTerminalByIdAsync(terminal.Id) == null)
            {
                return await _tliteDb.InsertAsync(terminal) > 0;
            }
            else 
            {
                return await UpdateTerminal(terminal);
            }
        }
    }
}
