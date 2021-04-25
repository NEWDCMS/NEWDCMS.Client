using Refit;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace Wesley.Client.Services
{
    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/auth/user", true, isAutoRegistrable: false), Trace]
    public interface IAuthorizationApi
    {
        [Get("/bearer/{store}/{userid}")]
        [Headers("Authorization: Bearer")]
        Task<HttpResponseMessage> AuthBearerAsync(int store, int userid, CancellationToken calToken = default);
    }
}
