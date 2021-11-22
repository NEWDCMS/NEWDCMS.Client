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
        [Reactive] public IList<ImageSource> ImagesUrls { get; set; } = new ObservableRangeCollection<ImageSource>();

        /// <summary>
        /// 用于照片预览
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="loggingService"></param>
        /// <param name="toastNotificator"></param>
        /// <param name="dialogService"></param>
        public ImageViewerPageViewModel(INavigationService navigationService, IDialogService dialogService
            ) : base(navigationService, dialogService) { }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("ImageInfos"))
            {
                parameters.TryGetValue("ImageInfos", out List<string> images);

                foreach (var url in images)
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
                            cacheStream = await ImageService.Instance.Config.DownloadCache.DownloadAndCacheIfNeededAsync(url,
                               ImageService.Instance.LoadUrl(url),
                               ImageService.Instance.Config,
                               CancellationToken.None);
                        }
                        catch (Exception)
                        {
                            var timg = "Wesley.Client.Resources/images/loading.gif";
                            cacheStream = await ImageService.Instance.Config.DownloadCache.DownloadAndCacheIfNeededAsync(timg,
                              ImageService.Instance.LoadUrl(timg),
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

                            var tp = new CustomStreamImageSource()
                            {
                                Key = url,
                                Stream = (token) => Task.FromResult<Stream>(new MemoryStream(byteArray))
                            };

                            ImagesUrls.Add(tp);
                        }
                    }
                    catch (Exception)
                    {

                    }
                };

                if (ImagesUrls.Count > 0)
                    this.ImagesUrls = new ObservableRangeCollection<ImageSource>(ImagesUrls);
            }
        }

        public class CustomStreamImageSource : StreamImageSource
        {
            public string Key { get; set; }
        }

    }
}
