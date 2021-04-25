using System.IO;
using System.Threading.Tasks;

namespace Wesley.BitImageEditor
{
    /// <summary>for internal use by <see cref="Wesley.BitImageEditor"/></summary>
    public interface IImageHelper
    {
        /// <summary>for internal use by <see cref="Wesley.BitImageEditor"/></summary>
        Task<Stream> GetImageAsync();
        /// <summary>for internal use by <see cref="Wesley.BitImageEditor"/></summary>
        Task<bool> SaveImageAsync(byte[] data, string filename, string folder = null);
    }
}
