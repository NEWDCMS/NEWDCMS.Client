using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IUpdateService
    {
        Task<UpdateInfo> GetCurrentVersionAsync(CancellationToken calToken = default);
    }
}