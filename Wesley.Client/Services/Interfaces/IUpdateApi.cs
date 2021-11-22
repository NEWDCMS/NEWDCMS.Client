using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.StorageEndpoint + "api/version", true, isAutoRegistrable: false), Trace]
    public interface IUpdateApi
    {
        [Get("/getLatestVersion")]
        Task<UpdateInfo> GetCurrentVersionAsync(CancellationToken calToken = default);
    }

    ////http://api.jsdcms.com:9998/

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/system/app/android", true, isAutoRegistrable: false), Trace]
    [Headers("Authorization: Bearer")]
    public interface IMQApi
    {

        [Get("/getpushermqendpoint")]
        Task<string> GetPusherMQEndpoint(CancellationToken calToken = default);
    }
}


