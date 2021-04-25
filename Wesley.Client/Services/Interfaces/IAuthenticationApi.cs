using Wesley.Client.Models.Users;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IAuthenticationApi
    {
        [Post("/user/login")]
        Task<APIResult<UserAuthenticationModel>> LoginAsync(LoginModel model, CancellationToken calToken = default);

        //api/v3/dcms/auth/user/logout/{userId}
        [Post("/user/logout/{userId}")]
        Task<APIResult<UserModel>> LogOutAsync(RevokeTokenRequest model, int store, int userId, CancellationToken calToken = default);

        [Get("/user/checkStatus/{storeId}/{userId}")]
        Task<APIResult<UserModel>> CheckStatusAsync(int storeId, int userId, CancellationToken calToken = default);

        //http://api.jsdcms.com/api/v3/dcms/auth/refresh-token
        [Post("/refresh-token")]
        Task<APIResult<UserAuthenticationModel>> RefreshTokenAsync(string rtoken, CancellationToken calToken = default);


        //api/v3/dcms/auth
        [Post("/user/qrlogin")]//string uuid, int userId
        Task<APIResult<dynamic>> QRLoginAsync(string uuid, int userId, CancellationToken calToken = default);
    }
}
