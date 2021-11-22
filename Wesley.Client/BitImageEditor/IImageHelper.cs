using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Wesley.BitImageEditor
{
    public interface IImageHelper
    {
        Task<Stream> GetImageAsync();
        Task<bool> SaveImageAsync(byte[] data, string filename, string folder = null);
        Task<System.IO.Stream> ToImageSourceStream(ImageSource image);
    }
}
