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
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class CustomerArchivesPageViewModel : ViewModelBaseCutom
    {
        private readonly ILiteDbService<VisitStore> _conn;
        public ReactiveCommand<object, Unit> BusinessSelected { get; }
        [Reactive] public TerminalModel Selecter { get; set; }
        [Reactive] public bool DataVewEnable { get; set; } = true;
        [Reactive] public bool NullViewEnable { get; set; }
        public double Longitude { get; set; } = 0;
        public double Latitude { get; set; } = 0;


        public ReactiveCommand<TerminalModel, Unit> OpenNavigationToCommand { get; }

        public CustomerArchivesPageViewModel(INavigationService navigationService,
               IProductService productService,
               IUserService userService,
               ITerminalService terminalService,
               IWareHousesService wareHousesService,
               IAccountingService accountingService,
               IDialogService dialogService,
               ILiteDbService<VisitStore> conn) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "客户档案";
            _conn = conn;

            //搜索
            this.WhenAnyValue(x => x.Filter.SerchKey)
                .Skip(1)
                .Select(s => s)
                .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Subscribe(s => ((ICommand)Load)?.Execute(null)).DisposeWith(DeactivateWith);

            //片区选择
            this.WhenAnyValue(x => x.Filter.DistrictId)
               .Where(s => s > 0)
               .Select(s => s)
               .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
               .Subscribe(s => ((ICommand)Load)?.Execute(null))
               .DisposeWith(DeactivateWith);

            //加载数据
            this.Load = ReactiveCommand.Create(async () =>
            {
                try
                {
                    //重载时排它
                    ItemTreshold = 1;
                    Terminals?.Clear();

                    DataVewEnable = false;
                    NullViewEnable = true;

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
                                0,
                                0,
                                0,
                                pageIndex: pageNumber,
                                pageSize: pageSize);

                    var series = tuple.Item2;
                    Title = $"客户档案({tuple.Item1})家";

                    if (series != null && series.Any())
                    {
                        this.Terminals = new AsyncObservableCollection<TerminalModel>(series);
                    }
                    else 
                    {
                        ItemTreshold = -1;
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

                            //string searchStr = "", int? districtId = 0, int? channelId = 0, int? rankId = 0, int? lineId = 0, int? businessUserId = 0, bool status = true, int distanceOrderBy = 0, double lat = 0, double lng = 0, double range = 1.5, int pageIndex = 0, int pageSize = 20
                            var tuple = await _terminalService.SearchTerminals(searchStr,
                                districtId,
                                channelId,
                                rankId,
                                lineTierId,
                                businessUserId,
                                true,
                                distanceOrderBy,
                                0,
                                0,
                                0,
                                pageIndex: pageNumber,
                                pageSize: pageSize);

                            var series = tuple.Item2;
                            if (series != null && series.Any())
                            {
                                foreach (var s in series)
                                {
                                    if (!(this.Terminals?.Select(s => s.Id).Contains(s.Id) ?? false))
                                    {
                                        this.Terminals?.Add(s);
                                    }
                                }
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

            //打开导航
            this.OpenNavigationToCommand = ReactiveCommand.Create<TerminalModel>(e =>
            {
                try
                {
                    if (e != null)
                    {
                        var _baiduLocationService = App.Resolve<IBaiduNavigationService>();
                        _baiduLocationService?.OpenNavigationTo(e.Location_Lat ?? 0, e.Location_Lng ?? 0, e.Address);
                    }
                }
                catch (Exception)
                {
                    _dialogService.LongAlert("没有安装导航软件");
                }
            });

            //编辑客户
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
            .Skip(1)
            .Where(x => x != null)
            .SubOnMainThread(async item =>
           {
               if (item != null)
                   await this.NavigateAsync("AddCustomerPage", ("Terminaler", item), ("Edit", true));
               this.Selecter = null;
           })
           .DisposeWith(DeactivateWith);

            //添加客户
            this.AddCommand = ReactiveCommand.Create<object>(async e => await this.NavigateAsync("AddCustomerPage", ("Edit", false)));

            //员工选择
            this.BusinessSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectUser((data) =>
                 {
                     if (data != null)
                     {
                         Filter.BusinessUserId = data.Id;
                         Filter.BusinessUserName = data.Column;
                         ((ICommand)Load)?.Execute(null);
                     }
                 }, Enums.UserRoleType.Employees);
            });
            this.BindBusyCommand(Load);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                if (parameters.ContainsKey("reffPage"))
                {
                    parameters.TryGetValue("reffPage", out string reffPage);
                    if (reffPage.Equals("AddCustomerPage"))
                    {
                        ((ICommand)Load)?.Execute(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        public override void OnAppearing()
        {
            base.OnAppearing();

            if (!this.Terminals.Any())
                ((ICommand)Load)?.Execute(null);
        }
    }
}
