using Acr.UserDialogs;
using Wesley.Client.BaiduMaps;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Services;
using Wesley.Client.ViewModels;
using Wesley.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace Wesley.Client.Pages.Market
{
    public partial class CurrentCustomerPage : BaseContentPage<CurrentCustomerPageViewModel>
    {
        private readonly IBaiduLocationService _locationService;
        public List<Circle> Circles { get; set; } = new List<Circle>();
        public List<Coordinate> Points { get; set; } = new List<Coordinate>();

        public CurrentCustomerPage()
        {
            try
            {
                InitializeComponent();

                _locationService = App.Resolve<IBaiduLocationService>();

                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems5(ViewModel).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }

                this.disposer.Add
                (
                    MessageBus.Current
                    .Listen<ObservableCollection<TerminalModel>>(Constants.TRACKTERMINALS_KEY)?.Subscribe(items =>
                    {
                        try
                        {
                            if (items != null && items.Any())
                            {
                                map?.Pins?.Clear();
                                Points.Clear();

                                foreach (var loc in items)
                                {
                                    var coordinate = new Coordinate(loc.Location_Lat ?? 0, loc.Location_Lng ?? 0);
                                    AddOverlay(coordinate, "water_drop.png", loc);
                                    Points.Add(new Coordinate()
                                    {
                                        Latitude = loc.Location_Lat ?? 0,
                                        Longitude = loc.Location_Lng ?? 0
                                    });
                                }

                                map?.SendStatusChanged(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0);
                            }
                        }
                        catch (Exception)
                        { }
                    })
                );

            }
            catch (NullReferenceException) { }
            catch (RankException ex) { Crashes.TrackError(ex); }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

        private void Map_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            try
            {
                if (e != null)
                {
                    System.Diagnostics.Debug.Print($"位置更新--------1------------>Latitude:{e.Latitude}  Longitude:{e.Longitude}");

                    var eLatitude = e.Latitude;
                    var eLongitude = e.Longitude;

                    if (null != ViewModel && eLatitude > 0 && eLongitude > 0)
                    {
                        // 判断是否超过25米 e.Latitude, e.Longitude
                        var distance = MapHelper.CalculateDistance(eLatitude, eLongitude, ViewModel.Latitude, ViewModel.Longitude);
                        if (distance >= 25)
                        {
                            if (ViewModel.Terminals.Any())
                            {
                                //if (!ViewModel.IsBusy)
                                //    ViewModel.RefreshTerminals();

                                ViewModel.Latitude = eLatitude;
                                ViewModel.Longitude = eLongitude;

                                //App.Resolve<IDialogService>()?.ShortAlert($"离上次偏移{distance:#.00}米");
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            map.Loaded += MapLoaded;
            map.StatusChanged += Map_StatusChanged;
            Set();
        }

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="x"></param>
        public void MapLoaded(object sender, EventArgs x)
        {
            try
            {
                if (map != null)
                {
                    map.ShowZoomControl = false;
                    map.ShowCompass = true;
                    map.ShowUserLocation = true;
                    map.ShowScaleBar = true;
                    map.UserTrackingMode = UserTrackingMode.Follow;
                    map.Center = new Coordinate(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0);
                    map.ZoomLevel = 19;
                    map?.Pins?.Clear();

                    var circle = new Circle()
                    {
                        Color = Color.FromHex("#5A89FF"),
                        Coordinate = map.Center,
                        Radius = 100,
                        Width = 1
                    };
                    map.Circles.Add(circle);
                }
            }
            catch (Exception ex)
            { }
        }


        /// <summary>
        /// 添加终端位置图标
        /// </summary>
        /// <param name="coord">坐标</param>
        /// <param name="image">图像</param>
        private void AddOverlay(Coordinate coord, string image, TerminalModel terminal)
        {
            try
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
            catch (Exception ex)
            { }
        }


        protected override void OnDisappearing()
        {
            try
            {
                if (map != null)
                {
                    map.Loaded -= MapLoaded;
                    map.StatusChanged -= Map_StatusChanged;
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
            try
            {
                if (map != null)
                {
                    using (UserDialogs.Instance.Loading("定位中...", cancelText: "取消"))
                    {
                        map.ZoomLevel = 19;
                        await _locationService?.UpdateCenter(map);
                    }
                }
            }
            catch (Exception)
            { }
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
                        for (var i = Points.Count - 1; i > 0; i--)
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


        public static TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(2);
        void Set()
        {
            this.disposer ??= new CompositeDisposable();
            this.disposer.Add
            (
                Observable
                    .Interval(Interval)
                    .Subscribe(_ => this.TryRun())
            );
        }
        void TryRun()
        {
            try
            {
                Task.Run(async () =>
                {
                    if (_locationService != null)
                    {
                        if (map != null)
                            await _locationService.UpdateCenter(map);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }
    }
}
