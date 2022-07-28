namespace DCMS.Client.Services
{
    /// <summary>
    /// 用于IO操作
    /// </summary>
    public interface IFileOperator
    {
        string GetLocalFilePath(string filename);
    }

}
