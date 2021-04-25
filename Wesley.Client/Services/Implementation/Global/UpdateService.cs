using System;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly MakeRequest _makeRequest;
        //IUpdateApi
        private static string URL => GlobalSettings.StorageEndpoint + "api/version";
        //IMQApi
        //private static string URL2 => GlobalSettings.BaseEndpoint + "api/v3/dcms/system/app/android";


        public UpdateService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        public async Task<UpdateInfo> GetCurrentVersionAsync(CancellationToken calToken = default)
        {
            try
            {
                var api = RefitServiceBuilder.Build<IUpdateApi>(URL);
                var result = await _makeRequest.Start(api.GetCurrentVersionAsync(calToken), calToken);
                if (result != null)
                    return result;
                else
                    return null;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }
    }
}
