using DCMS.Client.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace DCMS.Client.Services
{
    public class AccountingService : IAccountingService
    {

        private readonly MakeRequest _makeRequest;

        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/DCMS/config/accounting";

        public AccountingService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取指定单据类型的收付款方式
        /// </summary>
        /// <param name="billTypeId"></param>
        /// <returns></returns>
        public async Task<IList<AccountingOptionModel>> GetPaymentMethodsAsync(int billTypeId = 0, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IAccountingApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("GetPaymentMethodsAsync", storeId, billTypeId);
                var results = await _makeRequest.StartUseCache(api.GetPaymentMethodsAsync(storeId, billTypeId, calToken), cacheKey, force, calToken);
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
        /// 获取指定单据类型的初始默认收付款方式
        /// </summary>
        /// <param name="billTypeId"></param>
        /// <returns></returns>
        public async Task<Tuple<AccountingOption, List<AccountingOption>, List<AccountingOption>, Dictionary<int, string>>> GetDefaultAccountingAsync(int billTypeId = 0, bool force = false, CancellationToken calToken = default)
        {
            var model = new Tuple<AccountingOption, List<AccountingOption>, List<AccountingOption>, Dictionary<int, string>>(null, null, null, null);
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IAccountingApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("GetDefaultAccountingAsync", storeId, billTypeId);
                var results = await _makeRequest.StartUseCache(api.GetDefaultAccountingAsync(storeId, billTypeId, calToken), cacheKey, force, calToken);
                if (results != null && results?.Code >= 0)
                {
                    model = results?.Data;
                }
                return model;
            }
            catch (Exception e)
            {

                e.HandleException();
                return null;
            }
        }
    }
}
