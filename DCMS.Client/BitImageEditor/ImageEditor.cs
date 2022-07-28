using Wesley.BitImageEditor.EditorPage;
using Wesley.BitImageEditor.Helper;
using SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Wesley.BitImageEditor
{

    public class ImageEditor
    {
        /// <summary>
        /// 延迟初始化
        /// </summary>
        private static readonly Lazy<ImageEditor> lazy = new Lazy<ImageEditor>(() => new ImageEditor());
        private ImageEditor()
        {
            IPlatformHelper platform = DependencyService.Get<IPlatformHelper>();
           // if (!(platform?.IsInitialized ?? false))
               // throw new Exception("Wesley.BitImageEditor must be initialized on the platform");
        }

        /// <summary>
        /// 返回ImageEditor实例
        /// </summary>
        public static ImageEditor Instance { get => lazy.Value; }
        internal IImageHelper ImageHelper => DependencyService.Get<IImageHelper>();

        /// <summary>
        /// “True”-如果编辑器当前正在运行
        /// </summary>
        public static bool IsOpened { get; private set; } = false;

        private const string defaultFolderName = "BitooBitImages";
        private string folderName;
        private bool mainPageIsChanged = false;
        private Page mainPage;
        private TaskCompletionSource<byte[]> taskCompletionEditImage;
        private bool imageEditLock;
        private bool imageSetLock;
        private ImageEditorPage page;

        /// <summary>
        /// 用于保存图像的文件夹的名称
        /// </summary>
        public string FolderName
        {
            get => string.IsNullOrWhiteSpace(folderName) ? defaultFolderName : defaultFolderName;
            set => folderName = value;
        }
        private bool ImageEditLock
        {
            get => imageEditLock;
            set => imageEditLock = IsOpened = value;
        }


        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="data"></param>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public async Task<bool> SaveImage(byte[] data, string imageName) => await ImageHelper.SaveImageAsync(data, imageName);

        /// <summary>
        /// 返回编辑图像
        /// </summary>
        /// <param name="bitmap">为空时，用户可以从库中选择图像</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<byte[]> GetEditedImage(SKBitmap bitmap = null, ImageEditorConfig config = null)
        {
            if (!ImageEditLock)
            {
                ImageEditLock = true;
                //为空时从本地获取
                if (bitmap == null)
                {
                    using (Stream stream = await ImageHelper.GetImageAsync())
                    {
                        bitmap = stream != null ? SKBitmap.Decode(stream) : null;
                    }
                }
                if (config == null)
                    config = new ImageEditorConfig();

                //await Task.Delay(100);
                var edit = await PushImageEditorPage(bitmap, config);
                var data = bitmap != null ? edit : null;
                ImageEditLock = false;
                return data;
            }
            else
                return null;
        }

        /// <summary>
        /// 返回设置图片任务结果
        /// </summary>
        /// <param name="bitmap"></param>
        internal void SetImage(SKBitmap bitmap = null)
        {
            if (!imageSetLock)
            {
                imageSetLock = true;
                if (bitmap != null)
                {
                    taskCompletionEditImage.SetResult(SkiaHelper.SKBitmapToBytes(bitmap));
                }
                else
                    taskCompletionEditImage.SetResult(null);

                //释放托管资源
                if (page != null)
                {
                    page.Dispose();
                    page = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

        /// <summary>
        /// 打开编辑页面
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private async Task<byte[]> PushImageEditorPage(SKBitmap bitmap, ImageEditorConfig config)
        {
            try
            {
                taskCompletionEditImage = new TaskCompletionSource<byte[]>();
                if (bitmap != null)
                {
                    page = new ImageEditorPage(bitmap, config);
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        await Application.Current.MainPage.Navigation.PushModalAsync(page);
                    }
                    else
                    {
                        mainPage = Application.Current.MainPage;
                        Application.Current.MainPage = page;
                        mainPageIsChanged = true;
                    }
                }
                else
                    taskCompletionEditImage.SetResult(null);

                //等待SetImage 调用，以确定编辑任务完成
                byte[] data = await taskCompletionEditImage.Task;

                if (mainPageIsChanged)
                    Application.Current.MainPage = mainPage;
                else
                    await Application.Current.MainPage.Navigation.PopModalAsync();

                mainPage = null;
                ImageEditLock = false;
                imageSetLock = false;
                return data;
            }
            catch (Exception)
            {
                SetImage(null);
                return null;
            }
        }
    }
}
