namespace Wesley.Client.AutoUpdater.Services
{
    /// <summary>
    /// 用于获取操作系统版本接口
    /// </summary>
    public interface IOperatingSystemVersionProvider
    {
        /// <summary>
        /// 获取操作系统版本号
        /// </summary>
        /// <returns></returns>
		string GetOperatingSystemVersionString();
        /// <summary>
        /// 检查更新
        /// </summary>
        void Check(UpdateInfo version);
        /// <summary>
        /// 检查更新
        /// </summary>
        void CheckUpdate(UpdateInfo version);

        void StarInit();

        bool CheckNewVersion(UpdateInfo updateInfo);

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        string GetVersion();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetDeviceId();
    }
}