using DCMS.Client.Models.Settings;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DCMS.Client.Services
{
    public interface IAccountingService
    {
        Task<Tuple<AccountingOption, List<AccountingOption>, List<AccountingOption>, Dictionary<int, string>>> GetDefaultAccountingAsync(int billTypeId = 0, bool force = false, CancellationToken calToken = default);
        Task<IList<AccountingOptionModel>> GetPaymentMethodsAsync(int billTypeId = 0, bool force = false, CancellationToken calToken = default);


    }
}