using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IMicrophoneService
    {
        Task<bool> GetPermissionsAsync();
        void OnRequestPermissionsResult(bool isGranted);
    }
}
