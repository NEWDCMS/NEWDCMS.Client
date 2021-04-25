using Wesley.Client.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IGlobalService
    {
        Task<APPFeatures> GetAPPFeatures(bool force = false, CancellationToken calToken = default);
        Task<bool> UpdateHistoryBillStatusAsync(int? billType, int? billId = 0, CancellationToken calToken = default);
        void InitData(CancellationToken calToken = default);
        Task SynchronizationPermission(CancellationToken calToken = default);
        Task<bool> ResetVerifiCode(int itemId);
    }
}