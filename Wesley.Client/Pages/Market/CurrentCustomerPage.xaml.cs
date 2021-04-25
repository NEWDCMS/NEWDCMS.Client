using Acr.UserDialogs;
using Wesley.Client.BaiduMaps;
using Wesley.Client.Models.Terminals;
using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using ReactiveUI;
using Shiny;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Wesley.Client.Pages.Market
{

    public partial class CurrentCustomerPage : BaseContentPage<CurrentCustomerPageViewModel>
    {
        private CancellationTokenSource cts => new CancellationTokenSource();
        private IDisposable _locationDisposable;

        /// <summary>
        /// 画圆
        /// </summary>
        public List<Circle> Circles { get; set; } = new List<Circle>();

        /// <summary>
        /// 表示当前加载所有标注点
        /// </summary>
        public List<Coordinate> Points { get; set; } = new List<Coordinate>();


        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Content == null)
            {
                try
                {
                    InitializeComponent();
                    ToolbarItems.Clear();
                    foreach (var toolBarItem in this.GetToolBarItems5(ViewModel).ToList())
                    {
                        ToolbarItems.Add(toolBarItem);
                    }

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

                    MessageBus.Current
                         .Listen<ObservableCollection<TerminalModel>>(Constants.TRACKTERMINALS_KEY)
                         .Subscribe(items =>
                         {
                             map?.Pins?.Clear();
                             Points.Clear();

                             foreach (var loc in items)
                             {
                                 //Log.Write("red_location:", $"{loc.Location_Lat} = {loc.Location_Lng}");
                                 var coordinate = new Coordinate(loc.Location_Lat ?? 0, loc.Location_Lng ?? 0);
                                 AddOverlay(coordinate, "water_drop.png", loc);
                                 Points.Add(new Coordinate()
                                 {
                                     Latitude = loc.Location_Lat ?? 0,
                                     Longitude = loc.Location_Lng ?? 0
                                 });
                             }
                             map?.SendStatusChanged();
                         });
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
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
                map.ShowScaleBar = true;
                map.UserTrackingMode = UserTrackingMode.Follow;
                map.Center = new Coordinate(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0);
                map.ZoomLevel = 19;
                map?.Pins?.Clear();

                //状态更新时
                map.StatusChanged += (_, e) =>
                {
                    //map.Center = new Coordinate(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0);
                };

                var circle = new Circle()
                {
                    Color = Color.FromHex("#5A89FF"),
                    Coordinate = map.Center,
                    Radius = 100,
                    Width = 1
                };
                map.Circles.Add(circle);

                //开始
                map.LocationService?.Start();

                //
                await UpdateCenter();
            }
        }


        /// <summary>
        /// 添加终端位置图标
        /// </summary>
        /// <param name="coord">坐标</param>
        /// <param name="image">图像</param>
        private void AddOverlay(Coordinate coord, string image, TerminalModel terminal)
        {
            if (map != null)
            {
                var macker = new Pin
                {
                    Title = terminal.Name,
                    Coordinate = coord,
                    Animate = true,
                    Draggable = true,
                    Enabled3D = true,
                    Image = XImage.FromResource(image),
                    Terminal = terminal
                };

                macker.Clicked += async (o, e) =>
                 {
                     if (e != null && e.Terminal != null)
                     {
                         await ViewModel?.NavigateAsync("VisitStorePage", ("Terminaler", e?.Terminal));
                     }
                 };
                map.Pins.Add(macker);
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
                    map?.LocationService?.Stop();
                }

                base.OnDisappearing();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 选择终端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OtherCustomer_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (this.BindingContext is CurrentCustomerPageViewModel vm)
                {
                    ((System.Windows.Input.ICommand)vm.OtherCustomerCommand)?.Execute(null);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 恢复当前位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FixedPosition_Clicked(object sender, EventArgs e)
        {
            if (map != null)
            {
                using (UserDialogs.Instance.Loading("定位中...", () => { cts.Cancel(); }, cancelText: "取消"))
                {
                    map.ZoomLevel = 19;
                    await UpdateCenter();
                }
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
            catch (Xamarin.Essentials.PermissionException pEx)
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
        /// 自动缩放视图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetZoomBtn_Clicked(object sender, EventArgs e)
        {
            if (map != null)
            {
                SetZoom();
                map.ZoomToSpan = true;
            }
        }

        /// <summary>
        /// 根据原始数据计算中心坐标和缩放级别，并为地图设置中心坐标和缩放级别
        /// </summary>
        private void SetZoom()
        {
            try
            {
                if (map != null)
                {
                    if (Points != null && Points.Any())
                    {
                        var maxLng = Points[0].Longitude;
                        var minLng = Points[0].Longitude;
                        var maxLat = Points[0].Latitude;
                        var minLat = Points[0].Latitude;

                        Coordinate res;
                        for (var i = Points.Count - 1; i >= 0; i--)
                        {
                            res = Points[i];
                            if (res.Longitude > maxLng) maxLng = res.Longitude;
                            if (res.Longitude < minLng) minLng = res.Longitude;
                            if (res.Latitude > maxLat) maxLat = res.Latitude;
                            if (res.Latitude < minLat) minLat = res.Latitude;
                        };

                        var cenLng = (maxLng + minLng) / 2;
                        var cenLat = (maxLat + minLat) / 2;
                        var zoom = GetZoom(maxLng, minLng, maxLat, minLat);

                        map.Center = new Coordinate(cenLat, cenLng);
                        map.ZoomLevel = zoom;
                    }
                    else
                    {
                        //没有坐标，显示全中国  
                        map.Center = new Coordinate(35.563611, 103.388611);
                        map.ZoomLevel = 5;
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private int GetZoom(double maxLng, double minLng, double maxLat, double minLat)
        {
            try
            {
                //[1:20米（简称20米，后同），50米，100米，200米，500米，1公里，2公里，5公里，10公里，20公里，25公里，50公里，100公里，200公里，500公里，1000公里，2000公里，5000公里，10000公里]

                //级别18到3
                var zoom = new int[] { 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000 };

                //[19级，18级，17级，16级，15级，14级，13级，12级，11级，10级，9级，8级，7级，6级，5级，4级，3级，2级，1级]

                // 创建点坐标A  
                var pointA = new Coordinate(maxLng, maxLat);

                // 创建点坐标B  
                var pointB = new Coordinate(minLng, minLat);

                //获取两点距离,保留小数点后两位  
                var calculateUtils = App.Resolve<ICalculateUtils>();
                var distance = calculateUtils?.CalculateDistance(pointA, pointB);

                for (int i = 0, zoomLen = zoom.Length; i < zoomLen; i++)
                {
                    if (zoom[i] - distance > 0)
                    {
                        //之所以会多3，是因为地图范围常常是比例尺距离的10倍以上,所以级别会增加3
                        return 19 - i;
                    }
                };

                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }


    }
}
