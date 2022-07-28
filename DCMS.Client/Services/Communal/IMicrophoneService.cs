using System.Threading.Tasks;

namespace DCMS.Client.Services
{
    public interface IMicrophoneService
    {
        Task<bool> GetPermissionsAsync();
        void OnRequestPermissionsResult(bool isGranted);
    }
}
