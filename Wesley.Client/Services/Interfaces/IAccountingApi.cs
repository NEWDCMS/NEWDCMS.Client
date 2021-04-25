using Wesley.Client.Models.Settings;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    //[Policy("TransientHttpError")]
    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/config/accounting", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    //[Headers("Authorization: Bearer")]
    public interface IAccountingApi
    {
        [Get("/getpaymentmethods/{storeId}")]
        Task<APIResult<IList<AccountingOptionModel>>> GetPaymentMethodsAsync(int storeId, int billTypeId = 0, CancellationToken calToken = default);

        [Get("/getdefaultaccounting/{storeId}")]
        Task<APIResult<Tuple<AccountingOption, List<AccountingOption>, List<AccountingOption>, Dictionary<int, string>>>> GetDefaultAccountingAsync(int storeId, int billTypeId = 0, CancellationToken calToken = default);

    }
}