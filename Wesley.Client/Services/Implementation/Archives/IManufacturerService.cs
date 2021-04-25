using Wesley.Client.Models.Products;
using Wesley.Client.Models.Settings;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IManufacturerService
    {
        Task<AccountingOption> GetAdvancePaymentBalanceAsync(int manufacturerId, bool force = false, CancellationToken calToken = default);
        Task<ManufacturerBalance> GetManufacturerBalance(int manufacturerId, bool force = false, CancellationToken calToken = default);
        Task<IList<ManufacturerModel>> GetManufacturersAsync(bool force = false, CancellationToken calToken = default);
    }
}