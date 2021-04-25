using Acr.UserDialogs;
using Wesley.Client.BaiduMaps;
using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Wesley.Client.Pages.Common
{
    public partial class SelectLocationPage : BaseContentPage<SelectLocationPageViewModel>
    {
        private CancellationTokenSource cts => new CancellationTokenSource();
        private IDisposable _locationDisposable;
        public SelectLocationPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                if (map != null)
                    map.Loaded += MapLoaded;

                _locationDisposable = Observable
                   .Interval(TimeSpan.FromSeconds(3))
                   .SubOnMainThread(async _ =>
                   {
                       try
                       {
                           await UpdateCenter();
                       }
                       catch (Exception ex)
                       {
                           Crashes.TrackError(ex);
                       }
                   });
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="x"></param>
        public async void MapLoaded(object sender, EventArgs x)
        {
            if (map != null)
            {
                map.ShowCompass = true;
                map.ShowUserLocation = true;
                map.ShowScaleBar = false;
                map.Center = new Coordinate(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0);
                map.ZoomLevel = 19;

                //状态更新时
                map.StatusChanged += (_, e) =>
                {
                    //map.Center = new Coordinate(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0);
                    //当前标注
                    //AddOverlay(map.Center, "water_drop.png");
                };

                //开始
                map.LocationService?.Start();

                //
                await UpdateCenter();
            }
        }

        private async Task UpdateCenter()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request, cts.Token);
                if (location != null)
                {
                    map?.LocationService.Converter(map, location.Latitude, location.Longitude);
                }
            }
            catch (FeatureNotEnabledException fneEx)
            {
                cts.Cancel();
                Crashes.TrackError(fneEx);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                cts.Cancel();
                Crashes.TrackError(fnsEx);
            }
            catch (PermissionException pEx)
            {
                cts.Cancel();
                Crashes.TrackError(pEx);
            }
            catch (Exception ex)
            {
                cts.Cancel();
                Crashes.TrackError(ex);
            }
            finally
            {
                map.Center = new Coordinate(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0);
            }
        }

        /// <summary>
        /// 添加覆盖物
        /// </summary>
        /// <param name="coord">坐标</param>
        /// <param name="image">图像</param>
        private void AddOverlay(Coordinate coord, string image)
        {
            if (map != null)
            {
                map.Pins.Clear();
                var annotation = new Pin
                {
                    Coordinate = coord,
                    Animate = true,
                    Draggable = true,
                    Enabled3D = true,
                    Image = XImage.FromResource(image)
                };
                map.Pins.Add(annotation);
            }
        }

        protected override void OnDisappearing()
        {
            try
            {
                if (_locationDisposable != null)
                {
                    _locationDisposable?.Dispose();
                    cts.Cancel();
                }

                if (map != null)
                {
                    //停止服务
                    map.Loaded -= MapLoaded;
                    //停止
                    map.LocationService?.Stop();
                }

                base.OnDisappearing();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        /// <summary>
        /// 重新定位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ReceiveLocationCommand_Clicked(object sender, EventArgs e)
        {
            using (UserDialogs.Instance.Loading("定位中...", () => { cts.Cancel(); }, cancelText: "取消"))
            {
                map.ZoomLevel = 19;
                await UpdateCenter();
            }
        }
    }
}
