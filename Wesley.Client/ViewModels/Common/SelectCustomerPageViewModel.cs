using Acr.UserDialogs;
using Wesley.Client.Models.Census;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    /// <summary>
    /// 开单选择客户
    /// </summary>
    public class SelectCustomerPageViewModel : ViewModelBaseCutom
    {
        [Reactive] public TerminalModel Selecter { get; set; }

        private readonly ILiteDbService<VisitStore> _conn;

        [Reactive] public bool DataVewEnable { get; set; }
        [Reactive] public bool NullViewEnable { get; set; }
        public double Longitude { get; set; } = 0;
        public double Latitude { get; set; } = 0;


        public SelectCustomerPageViewModel(INavigationService navigationService,
           IProductService productService,
           ITerminalService terminalService,
           IUserService userService,
           IWareHousesService wareHousesService,
           IAccountingService accountingService,
           IDialogService dialogService,
           ILiteDbService<VisitStore> conn) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {

            Title = "选择客户";

            _conn = conn;

            //搜索
            this.WhenAnyValue(x => x.Filter.SerchKey)
                .Select(s => s)
                .Skip(1)
                .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Subscribe(s => 
                {
                    ((ICommand)Load)?.Execute(null);
                })
                .DisposeWith(DeactivateWith);

            //片区选择
            this.WhenAnyValue(x => x.Filter.DistrictId)
               .Where(s => s > 0)
               .Select(s => s)
               .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
               .Subscribe(s => 
               {
                   ((ICommand)Load)?.Execute(null);
               })
               .DisposeWith(DeactivateWith);

            //加载数据
            this.Load = ReactiveCommand.Create(async () =>
            {
                try
                {
                    //重载时排它
                    ItemTreshold = 1;

                    try
                    {
                        if (Terminals != null && Terminals.Any())
                            Terminals?.Clear();
                    }
                    catch (Exception) { }

                    DataVewEnable = false;
                    NullViewEnable = true;
                    using (var dig = UserDialogs.Instance.Loading("加载中..."))
                    {
                        this.Longitude = GlobalSettings.Longitude ?? 0;
                        this.Latitude = GlobalSettings.Latitude ?? 0;

                        string searchStr = Filter?.SerchKey;
                        int? districtId = Filter?.DistrictId;
                        int? channelId = Filter?.ChannelId;
                        int? businessUserId = Filter?.BusinessUserId;
                        int? rankId = Filter?.RankId;
                        int pageNumber = 0;
                        int pageSize = PageSize;
                        int? lineTierId = Filter?.LineId;
                        int distanceOrderBy = Filter.DistanceOrderBy;

                        var tuple = await _terminalService.SearchTerminals(searchStr,
                            districtId,
                            channelId,
                            rankId,
                            lineTierId,
                            businessUserId,
                            true,
                            distanceOrderBy,
                            GlobalSettings.Latitude ?? 0,
                            GlobalSettings.Longitude ?? 0,
                            0.5,
                            pageNumber,
                            pageSize);

                        var series = tuple.Item2;
                        if (series != null && series.Any())
                        {
                            this.Terminals = new AsyncObservableCollection<TerminalModel>(series);
                        }
                        else
                        {
                            ItemTreshold = -1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                finally
                {
                    NullViewEnable = false;
                    DataVewEnable = true;
                }
            });

            //以增量方式加载数据
            this.ItemTresholdReachedCommand = ReactiveCommand.Create(async () =>
            {
                if (ItemTreshold == -1) return;
                try
                {
                    int pageIdex = Terminals.Count / (PageSize == 0 ? 1 : PageSize);
                    if (pageIdex > 0)
                    {
                        using (var dig = UserDialogs.Instance.Loading("加载中..."))
                        {
                            string searchStr = Filter?.SerchKey;
                            int? districtId = Filter?.DistrictId;
                            int? channelId = Filter?.ChannelId;
                            int? businessUserId = Filter?.BusinessUserId;
                            int? rankId = Filter?.RankId;
                            int pageNumber = pageIdex;
                            int pageSize = PageSize;
                            int? lineTierId = Filter?.LineId;
                            int distanceOrderBy = Filter.DistanceOrderBy;

                            var tuple = await _terminalService.SearchTerminals(searchStr,
                                districtId,
                                channelId,
                                rankId,
                                lineTierId,
                                businessUserId,
                                true,
                                distanceOrderBy,
                                GlobalSettings.Latitude ?? 0,
                                GlobalSettings.Longitude ?? 0,
                                0.5,
                                pageNumber,
                                pageSize);

                            var series = tuple.Item2;
                            if (series != null && series.Any())
                            {
                                try
                                {
                                    foreach (var s in series)
                                    {
                                        if (!(this.Terminals?.Select(s => s.Id).Contains(s.Id) ?? false))
                                        {
                                            this.Terminals?.Add(s);

                                        }
                                    }
                                }
                                catch (Exception) { }
  
                            }
                            else
                            {
                                ItemTreshold = -1;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    ItemTreshold = -1;
                }
            });

            //选择
            this.WhenAnyValue(x => x.Selecter)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Skip(1)
                .Where(x => x != null)
                .SubOnMainThread(async item =>
                {
                    Filter.SerchKey = "";
                    await _navigationService.GoBackAsync(("Filter", Filter), ("Terminaler", item));
                })
                .DisposeWith(DeactivateWith);


            this.BindBusyCommand(Load);
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            if (!this.Terminals.Any())
                ((ICommand)Load)?.Execute(null);
        }
    }
}
