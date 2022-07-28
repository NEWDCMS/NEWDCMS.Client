namespace Wesley.Client.AutoUpdater.Services
{
    /// <summary>
    /// 用于打开应用商店接口
    /// </summary>
    public interface IStoreOpener
    {
        /// <summary>
        /// 打开应用商店
        /// </summary>
        void OpenStore();
    }
}
