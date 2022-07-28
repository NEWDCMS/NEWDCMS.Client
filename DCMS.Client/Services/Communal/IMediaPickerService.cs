using System;
using System.Threading.Tasks;

namespace DCMS.Client.Services
{
    public interface IMediaPickerService
    {
        /// <summary>
        /// 选取Base64图片
        /// </summary>
        /// <returns></returns>
        Task<string> PickImageAsBase64String();

        /// <summary>
        /// 用于图片创建和编辑
        /// </summary>
        /// <param name="imageArray"></param>
        /// <returns></returns>
        IEditableImage CreateImage(byte[] imageArray);

        /// <summary>
        /// 用于图片创建和编辑异步方法
        /// </summary>
        /// <param name="imageArray"></param>
        /// <returns></returns>
        Task<IEditableImage> CreateImageAsync(byte[] imageArray);
    }

    public interface IEditableImage : IDisposable
    {
        int Width { get; }
        int Height { get; }
        IEditableImage Resize(int width, int height);
        IEditableImage Crop(int x, int y, int width, int height);
        IEditableImage Rotate(float degree);
        byte[] ToJpeg(float quality = 80);
        byte[] ToPng();
        int[] ToArgbPixels();
    }
}
