namespace Wesley.Client.AutoUpdater.Services
{
    /// <summary>
    /// 文件打开器接口
    /// </summary>
    public interface IFileOpener
    {
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="name"></param>
        void OpenFile(byte[] data, string name);
    }
}
