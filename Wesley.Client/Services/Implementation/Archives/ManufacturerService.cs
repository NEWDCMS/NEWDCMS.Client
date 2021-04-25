using Wesley.Client.Models.Products;
using Wesley.Client.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly MakeRequest _makeRequest;

        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/archives/manufacturer";

        public ManufacturerService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }


        public async Task<IList<ManufacturerModel>> GetManufacturersAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IManufacturerApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetManufacturersAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetManufacturersAsync(storeId, calToken), cacheKey, force, calToken);
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


        public async Task<AccountingOption> GetAdvancePaymentBalanceAsync(int manufacturerId, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IManufacturerApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetAdvancePaymentBalanceAsync", storeId, manufacturerId);
                var results = await _makeRequest.StartUseCache(api.GetAdvancePaymentBalanceAsync(storeId, manufacturerId, calToken), cacheKey, force, calToken);
                return results?.Data;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }


        public async Task<ManufacturerBalance> GetManufacturerBalance(int manufacturerId, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IManufacturerApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetManufacturerBalance", storeId, manufacturerId);
                var results = await _makeRequest.StartUseCache(api.GetManufacturerBalance(storeId, manufacturerId, calToken), cacheKey, force, calToken);
                return results?.Data;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }
    }
}
