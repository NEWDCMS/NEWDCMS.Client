using Acr.UserDialogs;
using Wesley.Client.BaiduMaps;
using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Wesley.Client.Pages.Common
{
    public partial class SelectLocationPage : BaseContentPage<SelectLocationPageViewModel>
    {
        private IBaiduLocationService _locationService;
        public SelectLocationPage()
        {
            _locationService = App.Resolve<IBaiduLocationService>();
            InitializeComponent();
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
        public void MapLoaded(object sender, EventArgs x)
        {
            if (map != null)
            {
                map.ShowCompass = true;
                map.ShowUserLocation = true;
                map.ShowScaleBar = false;
                map.Center = new Coordinate(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0);
                map.ZoomLevel = 19;
            }
        }

        private void Map_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.Print($"Latitude:{e.Latitude}  Longitude:{e.Longitude}");
                //await _locationService?.UpdateCenter(map);
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
        /// 重新定位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ReceiveLocationCommand_Clicked(object sender, EventArgs e)
        {
            using (UserDialogs.Instance.Loading("定位中...", cancelText: "取消"))
            {
                try
                {
                    if (map != null)
                    {
                        if (ViewModel != null)
                        {
                            ((ICommand)ViewModel.Load)?.Execute(null);
                        }

                        map.ZoomLevel = 19;
                        await _locationService?.UpdateCenter(map);
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
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
