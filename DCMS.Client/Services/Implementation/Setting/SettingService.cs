using DCMS.Client.Models.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace DCMS.Client.Services
{
    public class SettingService : ISettingService
    {
        private readonly MakeRequest _makeRequest;

        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/config/setting";

        public SettingService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取公司配置
        /// </summary>
        /// <returns></returns>
        public void GetCompanySettingAsync(CancellationToken calToken = default)
        {
            var model = new CompanySettingModel();
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<ISettingApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("GetCompanySettingAsync", storeId, userId);
                _makeRequest.StartUseCache_Rx(api.GetCompanySettingAsync(storeId, calToken), cacheKey, calToken)?.Subscribe((results) =>
                    {
                        if (results != null && results?.Code >= 0)
                        {
                            model = results?.Data;
                            if (model != null)
                            {
                                Settings.CompanySetting = JsonConvert.SerializeObject(model);
                                if (string.IsNullOrEmpty(Settings.DefaultPricePlan) || Settings.DefaultPricePlan == "0_0")
                                    Settings.DefaultPricePlan = model.DefaultPricePlan;
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
        /// 获取备注
        /// </summary>
        /// <param name="cts"></param>
        /// <returns></returns>
        public async Task<Dictionary<int, string>> GetRemarkConfigListSetting(CancellationToken calToken = default)
        {
            //{"Success":true,"Data":{"98":"赠品","99":"费用","100":"付款未到货","101":"搭赠","102":"兑奖","103":"捆绑","104":"满立减","105":"1元换购","106":"空瓶兑换","107":"费用货补","108":"代垫","109":"古井","110":"商品抵费用","111":"有成本兑奖赠酒"}}
            Dictionary<int, string> dic = new Dictionary<int, string>();
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<ISettingApi>(URL);
                var results = await _makeRequest.Start(api.GetRemarkConfigListSetting(storeId, calToken), calToken);
                if (results != null && results?.Code >= 0)
                {
                    dic = results?.Data;
                }
                return dic;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }
    }
}
