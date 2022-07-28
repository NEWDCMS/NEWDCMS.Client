using Acr.UserDialogs;
using Wesley.Client.BaiduMaps;
using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Threading;
using System.Windows.Input;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Wesley.Client.Pages.Market
{
    public partial class VisitStorePage : BaseContentPage<VisitStorePageViewModel>
    {
        private CancellationTokenSource cts => new CancellationTokenSource();
        private readonly IBaiduLocationService _locationService;

        public VisitStorePage()
        {
            try
            {
                InitializeComponent();
                _locationService = App.Resolve<IBaiduLocationService>();
               
                ToolbarItems?.Clear();
                var bar = PageExtensions.BulidButton("\uf017", () =>
                 {
                     if (ViewModel != null)
                     {
                         ((ICommand)ViewModel.HistoryCommand)?.Execute(null);
                     }
                 });
                ToolbarItems.Add(bar);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                map.Loaded += MapLoaded;
                map.StatusChanged += Map_StatusChanged;
                Set();
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
        public  void MapLoaded(object sender, EventArgs x)
        {
            if (map != null)
            {
                map.ShowCompass = false;
                map.ShowScaleBar = false;
                map.ZoomLevel = 19;
            }
        }

        private void Map_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.Print($"Latitude:{e.Latitude}  Longitude:{e.Longitude}");
                //Task.Run(async () =>
                //{
                //    await _locationService?.UpdateCenter(map);
                //});
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
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
        /// 定位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OrientationCmd_Clicked(object sender, EventArgs e)
        {
            using (UserDialogs.Instance.Loading("定位中...", cancelText: "取消"))
            {
                map.ZoomLevel = 19;
                await _locationService?.UpdateCenter(map);
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
