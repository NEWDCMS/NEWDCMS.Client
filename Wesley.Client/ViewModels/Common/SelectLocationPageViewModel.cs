using Wesley.Client.Models.Census;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Wesley.Client.ViewModels
{
    public class SelectLocationPageViewModel : ViewModelBaseCutom
    {

        private readonly LocalDatabase _conn;
        [Reactive] public TrackingModel Selecter { get; set; }
        public IReactiveCommand ReceiveLocationCommand { get; set; }


        public SelectLocationPageViewModel(INavigationService navigationService,
               IProductService productService,
               ITerminalService terminalService,
               IUserService userService,
               IWareHousesService wareHousesService,
               IAccountingService accountingService,
               LocalDatabase conn,
               IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {

            Title = "选择我的位置";

            _conn = conn;

            //搜索
            this.WhenAnyValue(x => x.Filter.SerchKey)
                .Select(s => s)
                .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Subscribe(s =>
                {
                    if (!string.IsNullOrEmpty(s))
                        ((ICommand)Load)?.Execute(null);

                }).DisposeWith(DestroyWith);


            this.Load = GpsEventsLoader.Load(async () =>
            {
                var gps = new List<TrackingModel>();

                try
                {
                    gps = await _conn.LocationSyncEvents.ToListAsync();
                    gps = gps?.Where(s => !string.IsNullOrEmpty(s.Address)).ToList();

                    if (!string.IsNullOrEmpty(Filter.SerchKey))
                        gps = gps?.Where(s => s.Address.Contains(Filter.SerchKey)).ToList();

                    gps = gps.Distinct(new FastPropertyComparer<TrackingModel>("Address"))
                    .OrderByDescending(s => s.CreateDateTime)
                    .ThenByDescending(s => s.Id).ToList();

                    if (gps != null && gps.Any())
                        this.GpsEvents = new ObservableRangeCollection<TrackingModel>(gps);
                }
                catch (Exception ex)
                {

                    Crashes.TrackError(ex);
                }

                return await Task.FromResult(gps);
            });

            //重新定位
            this.ReceiveLocationCommand = ReactiveCommand.Create<string>(e =>
           {

           });

            this.WhenAnyValue(x => x.Selecter)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Skip(1)
                .Where(x => x != null)
                .SubOnMainThread(async item =>
                {
                    await _navigationService.GoBackAsync(("AddGpsEvent", item));
                    this.Selecter = null;
                });

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
