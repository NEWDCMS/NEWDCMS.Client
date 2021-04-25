using Acr.UserDialogs;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    /// <summary>
    /// 拜访选择终端客户
    /// </summary>
    public class CurrentCustomerPageViewModel : ViewModelBaseCutom
    {
        public ReactiveCommand<object, Unit> LineSelected { get; }
        public ReactiveCommand<object, Unit> OtherCustomerCommand { get; }
        public ReactiveCommand<object, Unit> AddCustomerCommand { get; }
        public ReactiveCommand<TerminalModel, Unit> OpenNavigationToCommand { get; }
        public ReactiveCommand<int, Unit> DistanceOrderCommand { get; }

        [Reactive] public bool ListVewEnable { get; set; } = true;
        [Reactive] public bool MapVewEnable { get; set; }
        [Reactive] public bool OrderByDistance { get; set; }
        [Reactive] public TerminalModel Selecter { get; set; }


        public CurrentCustomerPageViewModel(INavigationService navigationService,
           IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "选择终端客户";

            //搜索
            this.WhenAnyValue(x => x.Filter.SerchKey)
                .Select(s => s)
                .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Subscribe(s => ((ICommand)Load)?.Execute(null)).DisposeWith(DestroyWith);

            //片区选择
            this.WhenAnyValue(x => x.Filter.DistrictId)
               .Where(s => s > 0)
               .Select(s => s)
               .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
               .Subscribe(s => ((ICommand)Load)?.Execute(null))
               .DisposeWith(DestroyWith);

            //距离排序
            this.WhenAnyValue(x => x.Filter.DistanceOrderBy)
              .Select(s => s)
              .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
              .Subscribe(s => ((ICommand)Load)?.Execute(null)).DisposeWith(DestroyWith);

            //加载数据
            this.Load = TerminalsLoader.Load(async () =>
            {
                //重载时排它
                ItemTreshold = 1;
                try
                {
                    //清除列表
                    Terminals.Clear();
                    var items = await _terminalService.GetTerminalsPage(Filter, LineTier, 0, PageSize, force: ForceRefresh, calToken: cts.Token);
                    foreach (var item in items)
                    {
                        if (Terminals.Count(s => s.Id == item.Id) == 0)
                        {
                            Terminals.Add(item);
                        }
                    }
                    MessageBus.Current.SendMessage(Terminals, Constants.TRACKTERMINALS_KEY);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                return Terminals;
            });
            //以增量方式加载数据
            this.ItemTresholdReachedCommand = ReactiveCommand.Create(async () =>
            {
                int pageIdex = 0;
                if (Terminals.Count != 0)
                    pageIdex = Terminals.Count / (PageSize == 0 ? 1 : PageSize);

                if (PageCounter < pageIdex)
                {
                    PageCounter = pageIdex;
                    using (var dig = UserDialogs.Instance.Loading("加载中..."))
                    {
                        try
                        {
                            var items = await _terminalService.GetTerminalsPage(Filter, LineTier, pageIdex, PageSize, force: ForceRefresh, calToken: cts.Token);
                            var previousLastItem = Terminals.Last();
                            foreach (var item in items)
                            {
                                if (Terminals.Count(s => s.Id == item.Id) == 0)
                                {
                                    Terminals.Add(item);
                                }
                            }

                            if (items.Count() == 0 || items.Count() == Terminals.Count)
                            {
                                ItemTreshold = -1;
                            }

                            MessageBus.Current.SendMessage(Terminals, Constants.TRACKTERMINALS_KEY);
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                            ItemTreshold = -1;
                        }
                    }
                }
            }, this.WhenAny(x => x.Terminals, x => x.GetValue().Count > 0));

            //线路选择
            this.LineSelected = ReactiveCommand.Create<object>(async e => await SelectUserLine((data) =>
            {
                if (data != null)
                {
                    Filter.LineId = data.Id;
                    Filter.LineName = data.Name;
                    if (Filter.LineId > 0)
                    {
                        var series = data.Terminals.OrderByDescending(t => t.Id);
                        series?.ToList().ForEach(s =>
                        {
                            s.RankName = string.IsNullOrEmpty(s.RankName) ? "A级" : s.RankName;
                            s.Distance = MapHelper.CalculateDistance(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0, s.Location_Lat ?? 0, s.Location_Lng ?? 0);
                        });
                        Terminals = new ObservableRangeCollection<TerminalModel>(series);
                        MessageBus.Current.SendMessage(Terminals, Constants.TRACKTERMINALS_KEY);
                    }
                }
            }, Filter.LineId));

            //打开导航
            this.OpenNavigationToCommand = ReactiveCommand.Create<TerminalModel>(e =>
            {
                try
                {
                    var _baiduLocationService = App.Resolve<IBaiduNavigationService>();
                    _baiduLocationService?.OpenNavigationTo(e.Location_Lat ?? 0, e.Location_Lng ?? 0, e.Address);
                }
                catch (Exception)
                {
                    _dialogService.LongAlert("没有安装导航软件");
                }
            });

            //距离排序
            this.DistanceOrderCommand = ReactiveCommand.Create<int>(e =>
            {
                if (!OrderByDistance)
                {
                    this.OrderByDistance = true;
                    this.Filter.DistanceOrderBy = 1;
                }
                else
                {
                    this.OrderByDistance = false;
                    this.Filter.DistanceOrderBy = 2;
                }
            });

            //片区选择
            this.DistrictSelected = ReactiveCommand.Create<object>(async e => await this.NavigateAsync("SelectAreaPage", null));

            //附近客户
            this.OtherCustomerCommand = ReactiveCommand.Create<object>(e =>
            {
                if (this.MapVewEnable == false)
                {
                    Title = "附近客户";
                    this.ListVewEnable = false;
                    this.MapVewEnable = true;
                    Filter.LineId = 0;
                    ((ICommand)RefreshCommand)?.Execute(null);
                }
                else
                {
                    Title = "选择客户";
                    this.ListVewEnable = true;
                    this.MapVewEnable = false;
                }
            });

            //添加客户
            this.AddCustomerCommand = ReactiveCommand.Create<object>(async e => await this.NavigateAsync("AddCustomerPage"));

            //客户选择
            this.WhenAnyValue(x => x.Selecter)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Skip(1)
                .Where(x => x != null)
                .SubOnMainThread(async item =>
                {
                    await this.NavigateAsync("VisitStorePage", ("Terminaler", item));
                    this.Selecter = null;
                });


            //菜单选择
            this.SetMenus((x) =>
            {
                switch (x)
                {
                    case Enums.MenuEnum.NEARCUSTOMER:
                        {
                            if (this != null)
                            {
                                Title = "附近客户";
                                Filter.LineId = 0;
                                ((ICommand)RefreshCommand)?.Execute(null);
                            }
                        }
                        break;
                    case Enums.MenuEnum.LINESELECT:
                        {
                            if (this != null)
                            {
                                ((ICommand)LineSelected)?.Execute(null);
                            }
                        }
                        break;
                }
            }, 18, 19);


            this.ItemTresholdReachedCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.LineSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.DistrictSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.OtherCustomerCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.AddCustomerCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
