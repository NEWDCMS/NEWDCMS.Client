using DCMS.Client.Models.Census;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DCMS.Client.ViewModels
{
    public class SelectLocationPageViewModel : ViewModelBaseCutom
    {

        private readonly ILiteDbService<TrackingModel> _conn;
        [Reactive] public TrackingModel Selecter { get; set; }
        public IReactiveCommand ReceiveLocationCommand { get; set; }


        public SelectLocationPageViewModel(INavigationService navigationService,
               IProductService productService,
               ITerminalService terminalService,
               IUserService userService,
               IWareHousesService wareHousesService,
               IAccountingService accountingService,
               ILiteDbService<TrackingModel> conn,
               IDialogService dialogService
            ) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
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

                }).DisposeWith(DeactivateWith);


            this.Load = ReactiveCommand.Create(async () =>
            {
                var gps = new List<TrackingModel>();

                try
                {
                    var data = await _conn.Table.FindAllAsync();
                    gps = data?.Where(s => !string.IsNullOrEmpty(s.Address)).ToList();

                    if (!string.IsNullOrEmpty(Filter.SerchKey))
                        gps = gps?.Where(s => s.Address.Contains(Filter.SerchKey)).ToList();

                    gps = gps.Distinct(new FastPropertyComparer<TrackingModel>("Address"))
                    .OrderByDescending(s => s.CreateDateTime)
                    .ThenByDescending(s => s.Id).ToList();

                    if (gps != null && gps.Any())
                        this.GpsEvents = new  ObservableCollection<TrackingModel>(gps);
                }
                catch (Exception ex)
                {

                    Crashes.TrackError(ex);
                }
            });


            this.WhenAnyValue(x => x.Selecter)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Skip(1)
                .Where(x => x != null)
                .SubOnMainThread(async item =>
                {
                    await _navigationService.GoBackAsync(("AddGpsEvent", item));
                    this.Selecter = null;
                })
                .DisposeWith(DeactivateWith);

            this.BindBusyCommand(Load);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
