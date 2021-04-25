using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Wesley.Client.Services
{
    public interface IPermissionsService
    {
        /// <summary>
        /// 后台运行设置
        /// </summary>
        void BackgroundOperationSetting();
        /// <summary>
        /// 电池优化设置
        /// </summary>
        void BatteryOptimizationSetting();
        /// <summary>
        /// 获取权限
        /// </summary>
        /// <returns></returns>
        Task<bool> GetPermissionsAsync();
        /// <summary>
        /// 请求结构
        /// </summary>
        /// <param name="isGranted"></param>
        void OnRequestPermissionsResult(bool isGranted);
        void RequestLocationAndCameraPermission();

        Task<PermissionStatus> GetLocationConsent();
    }
}
