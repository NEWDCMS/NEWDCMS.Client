using Wesley.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Wesley.Client.Services
{
    public class GlobalService : IGlobalService
    {
        private readonly MakeRequest _makeRequest;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly ITerminalService _terminalService;
        private readonly IWareHousesService _wareHousesService;
        private readonly ISettingService _settingService;

        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms";


        public GlobalService(MakeRequest makeRequest,
            IUserService userService,
            ISettingService settingService,
            IProductService productService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService)
        {
            _makeRequest = makeRequest;
            _userService = userService;
            _productService = productService;
            _terminalService = terminalService;
            _wareHousesService = wareHousesService;
            _settingService = settingService;
        }

        /// <summary>
        /// 清除历史记录
        /// </summary>
        /// <param name="billType"></param>
        /// <param name="billId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateHistoryBillStatusAsync(int? billType, int? billId = 0, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IGlobalApi>(URL);
                var results = await _makeRequest.Start(api.UpdateHistoryBillStatusAsync(storeId,
                    userId,
                    billType,
                    billId, calToken), calToken);

                if (results != null && results?.Code >= 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                e.HandleException();
                return false;
            }
        }


        /// <summary>
        /// 初始同步基础数据
        /// </summary>
        /// <returns></returns>
        public void InitData(CancellationToken calToken = default)
        {
            try
            {
                _settingService.GetCompanySettingAsync(new CancellationToken());
                //_userService.GetBusinessUsersAsync(null, "Salesmans", calToken: calToken);
                //_productService.GetSpecificationAttributeOptionsAsync(calToken: calToken);
                //_productService.GetBrandsAsync("", 0, 50, calToken: calToken);
                //_productService.GetAllCategoriesAsync(calToken: calToken);
                //_terminalService.GetRanksAsync(calToken: calToken);
                //_terminalService.GetLineTiersByUserAsync(calToken: calToken);
                //_terminalService.GetLineTiersAsync(calToken: calToken);
                //_terminalService.GetChannelsAsync(calToken: calToken);
                //_terminalService.GetDistrictsAsync(calToken: calToken);
            }
            catch (Exception e)
            {
                e.HandleException();
            }
        }

        /// <summary>
        /// 同步权限
        /// </summary>
        public void SynchronizationPermission(CancellationToken calToken = default)
        {
            _userService?.GetPermissionRecordSettingAsync((result) =>
           {
               if (result != null)
                    //用户有效权限集合
                    Settings.AvailablePermissionRecords = JsonConvert.SerializeObject(result);
           }, calToken);
        }

        /// <summary>
        /// 获取APP功能项
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void GetAPPFeatures(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                calToken = new CancellationToken();

                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IGlobalApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("GetAPPFeatures", storeId, userId);
                _makeRequest.StartUseCache_Rx(api.GetAPPFeatures(storeId, userId, calToken), cacheKey, calToken)?.Subscribe((results) =>
                    {
                        if (results != null && results?.Code >= 0)
                        {

                            var oldAppDatas = Settings.AppDatas;
                            if (force)
                            {
                                Settings.AppDatas = "";
                                Settings.SubscribeDatas = "";
                                Settings.ReportsDatas = "";
                            }

                            //报表功能
                            var rtapps = results?.Data.ReportsDatas;
                            if (!string.IsNullOrEmpty(Settings.ReportsDatas) && Settings.ReportsDatas != "[]")
                            {

                                var apps = GlobalSettings.ReportsDatas;
                                if (rtapps != null)
                                {
                                    foreach (var r in rtapps)
                                    {
                                        if (!apps.Select(s => s.Id).Contains(r.Id))
                                        {
                                            apps.Add(r);
                                        }
                                    }
                                }
                                if (apps != null && apps.Any())
                                    Settings.ReportsDatas = JsonConvert.SerializeObject(apps);
                            }
                            else
                            {
                                if (rtapps != null && rtapps.Any())
                                    Settings.ReportsDatas = JsonConvert.SerializeObject(rtapps);
                            }

                            //应用功能
                            var tapps = results?.Data.AppDatas;
                            if (!string.IsNullOrEmpty(Settings.AppDatas) && Settings.AppDatas != "[]")
                            {
                                if (tapps != null)
                                {
                                    var apps = GlobalSettings.AppDatas;
                                    foreach (var r in tapps)
                                    {
                                        if (!apps.Select(s => s.Id).Contains(r.Id))
                                        {
                                            apps.Add(r);
                                        }
                                    }
                                    if (apps != null && apps.Any())
                                        Settings.AppDatas = JsonConvert.SerializeObject(apps);
                                }
                            }
                            else
                            {
                                //oldAppDatas
                                if (!string.IsNullOrEmpty(oldAppDatas))
                                {
                                    var oldapps = JsonConvert.DeserializeObject<List<Module>>(oldAppDatas);
                                    if (oldapps != null && oldapps.Any())
                                    {
                                        var oldNoSelects = oldapps.Where(s => s.Selected == false).ToList();
                                        foreach (var am in oldNoSelects)
                                        {
                                            var cur = tapps.Where(s => s.Id == am.Id).FirstOrDefault();
                                            if (cur != null)
                                            {
                                                cur.Selected = false;
                                            }
                                        }
                                    }
                                }

                                if (tapps != null && tapps.Any())
                                    Settings.AppDatas = JsonConvert.SerializeObject(tapps);
                            }

                            //订阅功能
                            var stapps = results?.Data.SubscribeDatas;
                            if (!string.IsNullOrEmpty(Settings.SubscribeDatas) && Settings.SubscribeDatas != "[]")
                            {

                                var apps = GlobalSettings.SubscribeDatas;
                                if (stapps != null)
                                {
                                    foreach (var r in stapps)
                                    {
                                        if (!apps.Select(s => s.Id).Contains(r.Id))
                                        {
                                            apps.Add(r);
                                        }
                                    }
                                }

                                if (apps != null && apps.Any())
                                    Settings.SubscribeDatas = JsonConvert.SerializeObject(apps);
                            }
                            else
                            {
                                if (stapps != null && stapps.Any())
                                    Settings.SubscribeDatas = JsonConvert.SerializeObject(stapps);
                            }
                        }
                    });

            }
            catch (Exception e)
            {
                e.HandleException();
            }
        }


        /// <summary>
        /// 重置验证码
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public async Task<bool> ResetVerifiCode(int itemId)
        {
            try
            {
                var sMSParams = new SMSParams
                {
                    Id = itemId,
                    StoreId = Settings.StoreId,
                    Mobile = Settings.UserMobile,
                    RType = 0
                };

                var api = RefitServiceBuilder.Build<IGlobalApi>(URL);
                var results = await _makeRequest.Start(api.ResetVerifiCode(sMSParams));
                return (bool)(results?.Success);
            }
            catch (Exception e)
            {
                e.HandleException();
                return false;
            }
        }
    }
}
