using Wesley.Client.Models.Products;
using Wesley.Client.Models.Settings;
using Refit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/archives/manufacturer", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    [Headers("Authorization: Bearer")]
    public interface IManufacturerApi
    {
        [Get("/getAdvancePaymentBalance/{storeId}/{manufacturerId}")]
        Task<APIResult<AccountingOption>> GetAdvancePaymentBalanceAsync(int storeId, int manufacturerId, CancellationToken calToken = default);

        [Get("/getManufacturers/{storeId}")]
        Task<APIResult<IList<ManufacturerModel>>> GetManufacturersAsync(int storeId, CancellationToken calToken = default);

        //http://api.jsdcms.com:9998/api/v3/dcms/archives/manufacturer/getmanufacturerbalance/797/0
        [Get("/getmanufacturerbalance/{storeId}/{manufacturerId}")]
        Task<APIResult<ManufacturerBalance>> GetManufacturerBalance(int storeId, int manufacturerId, CancellationToken calToken = default);
    }
}