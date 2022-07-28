using DCMS.Client.Services;
using System.IO;
using Xamarin.Forms;
using DCMS.Client.Droid.Services;

[assembly: Dependency(typeof(FileOperator))]
namespace DCMS.Client.Droid.Services
{
    /// <summary>
    /// 用于IO操作
    /// </summary>
    public class FileOperator : IFileOperator
    {
        public string GetLocalFilePath(string filename)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}