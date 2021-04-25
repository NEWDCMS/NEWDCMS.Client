using Wesley.Client.Enums;
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


        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms";


        public GlobalService(MakeRequest makeRequest,
            IUserService userService,
            IProductService productService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService)
        {
            _makeRequest = makeRequest;
            _userService = userService;
            _productService = productService;
            _terminalService = terminalService;
            _wareHousesService = wareHousesService;
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
        public async void InitData(CancellationToken calToken = default)
        {
            try
            {
                var taskList = new List<Task>
                {
                    _userService.GetBusinessUsersAsync(null, "Salesmans",calToken: calToken),
                    _productService.GetSpecificationAttributeOptionsAsync(calToken:calToken),
                    _productService.GetBrandsAsync("", 0, 50, calToken:calToken),
                    _productService.GetAllCategoriesAsync(calToken:calToken),
                    _terminalService.GetRanksAsync(calToken:calToken),
                    _terminalService.GetLineTiersByUserAsync(calToken:calToken),
                    _terminalService.GetLineTiersAsync(calToken:calToken),
                    _terminalService.GetChannelsAsync(calToken:calToken),
                    _terminalService.GetDistrictsAsync(calToken:calToken),
                    _wareHousesService.GetWareHousesAsync((int)WareHouseType.CangKu, calToken:calToken),
                    _wareHousesService.GetWareHousesAsync((int)WareHouseType.CheLiang, calToken:calToken)
                };

                await Task.WhenAll(taskList.ToArray()).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                e.HandleException();
            }
        }

        /// <summary>
        /// 同步权限
        /// </summary>
        public async Task SynchronizationPermission(CancellationToken calToken = default)
        {

            await _userService?.GetPermissionRecordSettingAsync((result) =>
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
        public async Task<APPFeatures> GetAPPFeatures(bool force = false, CancellationToken calToken = default)
        {
            var model = new APPFeatures();
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IGlobalApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetAPPFeatures", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetAPPFeatures(storeId, userId, calToken), cacheKey, force, calToken);
                if (results != null && results?.Code >= 0)
                {
                    model.ReportsDatas = results?.Data.ReportsDatas;
                    model.AppDatas = results?.Data.AppDatas;
                    model.SubscribeDatas = results?.Data.SubscribeDatas;

                    //报表功能
                    if (!string.IsNullOrEmpty(Settings.ReportsDatas) && Settings.ReportsDatas != "[]")
                    {
                        var tapps = results?.Data.ReportsDatas;
                        var apps = GlobalSettings.ReportsDatas;
                        if (tapps != null)
                        {
                            foreach (var r in tapps)
                            {
                                if (!apps.Select(s => s.Id).Contains(r.Id))
                                {
                                    apps.Add(r);
                                }
                            }
                        }
                        Settings.ReportsDatas = JsonConvert.SerializeObject(apps);
                    }
                    else
                    {
                        Settings.ReportsDatas = JsonConvert.SerializeObject(results?.Data.ReportsDatas);
                    }

                    //应用功能
                    if (!string.IsNullOrEmpty(Settings.AppDatas) && Settings.AppDatas != "[]")
                    {
                        var tapps = results?.Data.AppDatas;
                        var apps = GlobalSettings.AppDatas;
                        if (tapps != null)
                        {
                            foreach (var r in tapps)
                            {
                                if (!apps.Select(s => s.Id).Contains(r.Id))
                                {
                                    apps.Add(r);
                                }
                            }
                        }
                        Settings.AppDatas = JsonConvert.SerializeObject(apps);
                    }
                    else
                    {
                        Settings.AppDatas = JsonConvert.SerializeObject(results?.Data.AppDatas);
                    }

                    //订阅功能
                    if (!string.IsNullOrEmpty(Settings.SubscribeDatas) && Settings.SubscribeDatas != "[]")
                    {
                        var tapps = results?.Data.SubscribeDatas;
                        var apps = GlobalSettings.SubscribeDatas;
                        if (tapps != null)
                        {
                            foreach (var r in tapps)
                            {
                                if (!apps.Select(s => s.Id).Contains(r.Id))
                                {
                                    apps.Add(r);
                                }
                            }
                        }
                        Settings.SubscribeDatas = JsonConvert.SerializeObject(apps);
                    }
                    else
                    {
                        Settings.SubscribeDatas = JsonConvert.SerializeObject(results?.Data.SubscribeDatas);
                    }
                }

                return model;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
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
