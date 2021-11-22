using Wesley.Client.Models.Configuration;
using Refit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/config/setting", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    [Headers("Authorization: Bearer")]
    public interface ISettingApi
    {
        [Get("/getCompanySetting/{storeId}")]
        Task<APIResult<CompanySettingModel>> GetCompanySettingAsync(int storeId, CancellationToken calToken = default);


        [Get("/getRemarkConfigListSetting/{storeId}")]
        Task<APIResult<Dictionary<int, string>>> GetRemarkConfigListSetting(int storeId, CancellationToken calToken = default);
    }
}