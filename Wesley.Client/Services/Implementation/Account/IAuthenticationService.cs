using Wesley.Client.Models.Users;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IAuthenticationService
    {
        Task<UserModel> CheckStatusAsync(CancellationToken calToken = default);
        Task<int> LoginAsync(string userName, string password, CancellationToken calToken = default);
        Task<bool> LogOutAsync(CancellationToken calToken = default);
        Task<bool> AutoLoginAsync(CancellationToken calToken = default);
        Task<bool> QRLoginAsync(string uuid, CancellationToken calToken = default);
        Task<UserAuthenticationModel> RefreshTokenAsync(CancellationToken calToken = default);
    }
}