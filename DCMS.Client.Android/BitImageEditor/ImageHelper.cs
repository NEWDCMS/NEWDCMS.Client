using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using DCMS.BitImageEditor.Droid;
using Java.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using System;

[assembly: Dependency(typeof(ImageHelper))]
namespace DCMS.BitImageEditor.Droid
{
    public class ImageHelper : IImageHelper
    {
        internal static TaskCompletionSource<System.IO.Stream> PickImageTaskCompletionSource { set; get; }
        public Task<System.IO.Stream> GetImageAsync()
        {
            // Define the Intent for getting images
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);

            // Start the picture-picker activity (resumes in MainActivity.cs)
            Platform.CurrentActivity.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Picture"),
                Platform.PickImageId);

            // Save the TaskCompletionSource object as a MainActivity property
            PickImageTaskCompletionSource = new TaskCompletionSource<System.IO.Stream>();

            // Return Task object
            return PickImageTaskCompletionSource.Task;
        }
        public async Task<bool> SaveImageAsync(byte[] data, string filename, string folder = null)
        {
            if (folder == null)
                folder = ImageEditor.Instance.FolderName;
            try
            {
                File picturesDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
                File folderDirectory = picturesDirectory;

                if (!string.IsNullOrEmpty(folder))
                {
                    folderDirectory = new File(picturesDirectory, folder);
                    folderDirectory.Mkdirs();
                }

                using (File bitmapFile = new File(folderDirectory, filename))
                {
                    bitmapFile.CreateNewFile();

                    using (FileOutputStream outputStream = new FileOutputStream(bitmapFile))
                    {
                        await outputStream.WriteAsync(data);
                    }

                    // Make sure it shows up in the Photos gallery promptly.
                    MediaScannerConnection.ScanFile(Platform.CurrentActivity,
                                                    new string[] { bitmapFile.Path },
                                                    new string[] { "image/png", "image/jpeg" }, null);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
        public static void OnActivityResult(Result resultCode, Intent intent)
        {
            if ((resultCode == Result.Ok) && (intent != null))
            {
                Android.Net.Uri uri = intent.Data;
                System.IO.Stream stream = Platform.CurrentActivity.ContentResolver.OpenInputStream(uri);

                // Set the Stream as the completion of the Task
                PickImageTaskCompletionSource.SetResult(stream);
            }
            else
            {
                PickImageTaskCompletionSource.SetResult(null);
            }
        }

        public async Task<System.IO.Stream> ToImageSourceStream(ImageSource image)
        {
            try
            {
                System.IO.Stream baos = null;
                var bitmap = await GetImageFromImageSource(image, Forms.Context);
                if (bitmap != null)
                {
                    await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, baos);
                    baos.Flush();

                    if (bitmap != null)
                        bitmap.Dispose();
                }
                return baos;
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
                return null;
            }
        }

        private Task<Bitmap> GetBitmap(ImageSource image)
        {
            return GetImageFromImageSource(image, Forms.Context);
        }

        private async Task<Bitmap> GetImageFromImageSource(ImageSource imageSource, Context context)
        {
            try
            {
                IImageSourceHandler handler;

                if (imageSource is FileImageSource)
                {
                    handler = new FileImageSourceHandler();
                }
                else if (imageSource is StreamImageSource)
                {
                    handler = new StreamImagesourceHandler(); // sic
                }
                else if (imageSource is UriImageSource)
                {
                    handler = new ImageLoaderSourceHandler(); // sic
                }
                else
                {
                    throw new NotImplementedException();
                }

                return await handler.LoadImageAsync(imageSource, context);
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
                return null;
            }
        }
    }
}
