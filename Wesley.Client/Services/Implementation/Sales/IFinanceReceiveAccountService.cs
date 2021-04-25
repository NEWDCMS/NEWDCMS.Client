using Wesley.Client.Models.Sales;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services.Sales
{
    public interface IFinanceReceiveAccountService
    {
        Task<IList<FinanceReceiveAccountBillModel>> GetFinanceReceiveAccounts(DateTime? start, DateTime? end, int? businessUserId, int? payeer, int? accountingOptionId, string billNumber = "", int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default);
        Task<APIResult<FinanceReceiveAccountBillSubmitModel>> SubmitAccountStatementAsync(FinanceReceiveAccountBillSubmitModel data, int id, CancellationToken calToken = default);
    }
}