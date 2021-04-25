using Wesley.Client.Models.Census;
using Wesley.Client.Services;

using FFImageLoading;
using FFImageLoading.Cache;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace Wesley.Client.ViewModels
{
    public class ImageViewerPageViewModel : ViewModelBase
    {
        [Reactive] public IList<DisplayPhoto> ImagesUrls { get; set; } = new ObservableRangeCollection<DisplayPhoto>();

        /// <summary>
        /// 用于照片预览
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="loggingService"></param>
        /// <param name="toastNotificator"></param>
        /// <param name="dialogService"></param>
        public ImageViewerPageViewModel(INavigationService navigationService, IDialogService dialogService) : base(navigationService, dialogService) { }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("ImageInfos"))
            {
                parameters.TryGetValue("ImageInfos", out List<DisplayPhoto> images);

                var list = new List<DisplayPhoto>();

                foreach (var img in images)
                {

                    try
                    {
                        var config = new FFImageLoading.Config.Configuration()
                        {
                            ExecuteCallbacksOnUIThread = true
                        };
                        byte[] byteArray;
                        CacheStream cacheStream = null;
                        try
                        {

                            cacheStream = await ImageService.Instance.Config.DownloadCache.DownloadAndCacheIfNeededAsync(img.DisplayPath,
                               ImageService.Instance.LoadUrl(img.DisplayPath),
                               ImageService.Instance.Config,
                               CancellationToken.None);
                        }
                        catch (Exception)
                        {
                            img.DisplayPath = "Wesley.Client.Resources/images/loading.gif";
                            cacheStream = await ImageService.Instance.Config.DownloadCache.DownloadAndCacheIfNeededAsync(img.DisplayPath,
                              ImageService.Instance.LoadUrl(img.DisplayPath),
                              ImageService.Instance.Config,
                              CancellationToken.None);
                        }

                        if (cacheStream != null)
                        {
                            using (cacheStream.ImageStream)
                            using (var memoryStream = new MemoryStream())
                            {
                                await cacheStream.ImageStream.CopyToAsync(memoryStream);
                                byteArray = memoryStream.ToArray();
                            }

                            img.ThumbnailPhoto = new CustomStreamImageSource()
                            {
                                Key = img.DisplayPath,
                                Stream = (token) => Task.FromResult<Stream>(new MemoryStream(byteArray))
                            };

                            list.Add(img);
                        }
                    }
                    catch (Exception)
                    {

                    }
                };
                this.ImagesUrls = new ObservableRangeCollection<DisplayPhoto>(list);
            }
        }

        public class CustomStreamImageSource : StreamImageSource
        {
            public string Key { get; set; }
        }

    }
}
