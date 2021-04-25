using Wesley.Client.Models;
using Wesley.Client.Models.Census;
using Wesley.Client.Models.Report;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Models.Visit;
using Wesley.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Wesley.Client.Services
{
    public class TerminalService : ITerminalService
    {
        private readonly LocalDatabase _conn;
        private readonly MakeRequest _makeRequest;

        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms";

        public TerminalService(LocalDatabase conn,
            MakeRequest makeRequest)
        {
            _conn = conn;
            _makeRequest = makeRequest;
        }


        /// <summary>
        /// 获取经销商终端
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<TerminalModel>> GetTerminalsAsync(string searchStr = "", int? districtId = 0, int? channelId = 0, int? rankId = 0, int? businessUserId = 0, bool status = true, int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
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


        public async Task<TerminalModel> GetTerminalAsync(int? terminalId, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("GetTerminalAsync", storeId, terminalId);
                var results = await _makeRequest.StartUseCache(api.GetTerminalAsync(storeId, terminalId, calToken), cacheKey, force, calToken);
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
        /// 创建经销商终端
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
                var results = await _makeRequest.StartUseCache(api.GetLineTiersByUserAsync(storeId, userId, calToken), cacheKey, force, calToken);
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
        public async Task<IList<VisitStore>> GetVisitStoresAsync(int? terminalId = 0, int? districtId = 0, int? businessUserId = 0, DateTime? start = null, DateTime? end = null, bool force = false, CancellationToken calToken = default)
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
            catch (Exception e)
            {
                e.HandleException();
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
        /// 增量获取终端客户
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="lineTier"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        public async Task<IList<TerminalModel>> GetTerminalsPage(FilterModel filter, LineTierModel lineTier, int pageNumber, int pageSize, bool force = false, CancellationToken calToken = default)
        {
            string searchStr = filter?.SerchKey;
            int? districtId = filter?.DistrictId;
            int? channelId = filter?.ChannelId;
            int? businessUserId = filter?.BusinessUserId;
            int? rankId = filter?.RankId;
            bool status = true;

            var pending = new List<TerminalModel>();

            if (filter?.LineId == 0)
            {
                var result = await GetTerminalsAsync(searchStr, districtId, channelId, rankId, businessUserId, status, pageNumber, pageSize, force, calToken);
                if (result != null)
                {
                    var series = result.OrderByDescending(t => t.Id)?.ToList();
                    if (series != null && series.Any())
                    {
                        foreach (var s in series)
                        {
                            pending.Add(await RepreTerminal(s));
                        }
                    }
                }
            }
            else if (filter?.LineId > 0)
            {
                var result = await GetLineTiersByUserAsync(force, calToken);
                if (result != null)
                {
                    var lines = result.ToList();
                    var terminals = lines.Where(l => l.Id == (lineTier?.Id ?? 0)).FirstOrDefault()?.Terminals;
                    var series = terminals.OrderByDescending(t => t.Id)?.ToList();
                    if (series != null && series.Any())
                    {
                        foreach (var s in series)
                        {
                            pending.Add(await RepreTerminal(s));
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(searchStr))
                pending = pending.Where(l => l.Name.Contains(searchStr) || l.BossCall.Contains(searchStr) || l.Code.Contains(searchStr))
                    .OrderByDescending(s => s.Name)
                    .ToList();

            if (filter.DistanceOrderBy > 0)
            {
                if (filter.DistanceOrderBy == 1)
                    pending = pending.OrderBy(s => s.Distance).ToList();
                else if (filter.DistanceOrderBy == 2)
                    pending = pending.OrderByDescending(s => s.Distance).ToList();
            }

            return pending;
        }
        private async Task<TerminalModel> RepreTerminal(TerminalModel s)
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

            s.TodayIsVisit = await _conn.CheckVisitStore(s.Id);

            return s;
        }

        public async Task<APIResult<dynamic>> UpdateterminalAsync(int? terminalId, double location_lat, double location_lng, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var results = await _makeRequest.Start(api.UpdateterminalAsync(storeId, terminalId, location_lat, location_lng, calToken), calToken);
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


        public async Task<bool> CheckTerminalAsync(string name, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ITerminalApi>(URL);
                var results = await _makeRequest.Start(api.CheckTerminalAsync(storeId, name, calToken), calToken);
                return (bool)results.Data;
            }
            catch (Exception e)
            {
                e.HandleException();
                return false;
            }
        }
    }
}
