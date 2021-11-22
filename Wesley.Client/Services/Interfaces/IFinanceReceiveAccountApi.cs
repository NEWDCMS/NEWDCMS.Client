using Wesley.Client.Models.Sales;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/sales/financereceiveaccount", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    [Headers("Authorization: Bearer")]
    public interface IFinanceReceiveAccountApi
    {
        [Get("/getfinancereceiveaccounts/{storeId}/{businessUserId}")]
        Task<APIResult<IList<FinanceReceiveAccountBillModel>>> GetFinanceReceiveAccounts(int storeId, DateTime? start, DateTime? end, int? businessUserId, int? payeer, int? accountingOptionId, string billNumber = "", int pageIndex = 0, int pageSize = 20, CancellationToken calToken = default);

        [Post("/batchsubmitaccountstatements/{storeId}/{userId}")]
        Task<APIResult<FinanceReceiveAccountBillSubmitModel>> SubmitAccountStatementAsync(FinanceReceiveAccountBillSubmitModel data, int storeId, int userId, int id, CancellationToken calToken = default);
    }
}