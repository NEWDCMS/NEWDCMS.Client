using Acr.UserDialogs;
using DCMS.ChartJS.Models;
using DCMS.Client.Enums;
using DCMS.Client.Models;
using DCMS.Client.Models.Census;
using DCMS.Client.Models.Sales;
using DCMS.Client.Pages;
using DCMS.Client.Pages.Market;
using DCMS.Client.Services;
using DCMS.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Rg.Plugins.Popup.Services;
using System.Reactive.Disposables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Diagnostics;

namespace DCMS.Client.ViewModels
{
    /// <summary>
    /// 首页
    /// </summary>
    public class HomePageViewModel : ViewModelBaseCutom
    {
        private readonly INewsService _newsService;
        private readonly IReportingService _reportingService;
        private readonly ILiteDbService<MessageInfo> _conn;
        private readonly IGlobalService _globalService;
        private readonly static object _lock = new object();
        [Reactive] public int ChartIndex { get; set; }
        [Reactive] public ChartViewConfig LineConfig { get; set; }
        [Reactive] public ChartViewConfig HotBarConfig { get; set; }
        [Reactive] public ObservableCollection<Module> ToolsApps { get; set; } = new ObservableCollection<Module>();
        [Reactive] public ObservableCollection<Module> TopAppSeries { get; set; } = new ObservableCollection<Module>();
        [Reactive] public string UserFace { get; set; } = "profile_placeholder.png";
        [Reactive] public ObservableCollection<NewsInfoModel> NewsSeries { get; set; } = new ObservableCollection<NewsInfoModel>();
        [Reactive] public ObservableCollection<VisitStore> EventItems { get; set; } = new ObservableCollection<VisitStore>();

        public IReactiveCommand AddAPPCommand => this.Navigate("AddAppPage");
        public ReactiveCommand<string, Unit> ChatSelectedCommand { get; }
        public ReactiveCommand<Module, Unit> InvokeAppCommand { get; }
        public new IReactiveCommand RefreshCommand { get; set; }

        public IReactiveCommand TopAppLoad { get; set; }
        public IReactiveCommand DashboardReportLoad { get; set; }
        public IReactiveCommand PermissionLoad { get; set; }

        public ReactiveCommand<object, Unit> ChartLoad { get; set; }
        public IReactiveCommand HotChartLoad { get; set; }

        public IReactiveCommand ShowMenuPagePopUpCommand { get; }
        public new ReactiveCommand<string, Unit> NavigateCommand => ReactiveCommand.Create<string>(async (r) => await this.NavigateAsync(r, ("Reference", this.PageName)));

        [Reactive] public int AppRowHeigth { get; set; } = 240;
        [Reactive] public int TodayOrderQuantity { get; set; }
        [Reactive] public int TodayVisitQuantity { get; set; }
        [Reactive] public decimal TodaySaleAmount { get; set; }
        [Reactive] public int TodayAddTerminalQuantity { get; set; }

        [Reactive] public bool IsTopAppSeriesNull { get; set; }
        

        public HomePageViewModel(INavigationService navigationService,
            IProductService productService,
            IReportingService reportingService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            INewsService newsService,
            ILiteDbService<MessageInfo> conn,
            IGlobalService globalService,
            IAccountingService accountingService,
            IDialogService dialogService) : base(navigationService,
                productService,
                terminalService,
                userService,
                wareHousesService,
                accountingService,
                dialogService)
        {
            _newsService = newsService;
            _reportingService = reportingService;
            _conn = conn;
            _globalService = globalService;

            this.RefreshCommand = ReactiveCommand.Create<object>(e =>
            {
                //强制刷新
                this.ForceRefresh = true;
                ((ICommand)DashboardReportLoad)?.Execute(null);
                ((ICommand)Load)?.Execute(null);
                ((ICommand)TopAppLoad)?.Execute(null);
            });

            this.Load = ReactiveCommand.Create(() =>
            {
                try
                {
                    //头像
                    if (!string.IsNullOrEmpty(Settings.FaceImage) && Settings.FaceImage.StartsWith("http"))
                        UserFace = Settings.FaceImage;
                    this.StoreName = string.IsNullOrEmpty(Settings.StoreName) ? "" : Settings.StoreName;
                    this.UserName = string.IsNullOrEmpty(Settings.UserRealName) ? "" : Settings.UserRealName;

                    //工具
                    var tools = new List<Module>()
                   {
                       new Module
                       {
                           Icon = "&#xf0d1;",
                           Name = "拜访门店",
                           Navigation = "VisitStorePage"
                       },new Module
                       {
                           Icon = "&#xf274;",
                           Name = "签收",
                           Navigation = "DeliveryReceiptPage"
                       },new Module
                       {
                           Icon = "&#xf0ce;",
                           Name = "拜访记录",
                           Navigation = "VisitRecordsPage"
                       },new Module
                       {
                           Icon = "&#xf075;",
                           Name = "市场反馈",
                           Navigation = "MarketFeedbackPage"
                       },new Module
                       {
                           Icon = "&#xf1c0;",
                           Name= "库存上报",
                           Navigation= "InventoryReportPage"
                       }
                   };
                    this.ToolsApps = new ObservableCollection<Module>(tools);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });

            //快捷方式
            this.TopAppLoad = ReactiveCommand.Create(() =>
            {
                try
                {
                    IsTopAppSeriesNull = false;

                    var apps = GlobalSettings.AppDatas;
                    if (apps.Any() && apps.Count >= 11)
                    {
                        apps = apps.Where(s => s.Selected).Take(11).ToList();
                        foreach (var a in apps)
                        {
                            if (!string.IsNullOrEmpty(a.Name))
                                a.Name = a.Name.Length > 4 ? CommonHelper.CutString(a.Name, 0, 4) : a.Name;
                        }
                        apps.Add(new Module()
                        {
                            Id = 1,
                            Navigation = "AddAppPage",
                            Name = "更多",
                            Icon = "&#xf009;",
                            Color = "#dddddd",
                            BgColor = "#BCBCBC",
                            Selected = true
                        });
                    }
                    if (apps.Any())
                        this.TopAppSeries = new ObservableCollection<Module>(apps);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                finally 
                {
                    IsTopAppSeriesNull = true;
                }
            });

            //控制面板统计
            this.DashboardReportLoad = ReactiveCommand.Create(() =>
            {
                bool firstOrDefault = false;
                _reportingService.Rx_GetDashboardReportAsync(new System.Threading.CancellationToken())
                .Subscribe((results) =>
                {
                    System.Diagnostics.Debug.Print("Rx_GetDashboardReportAsync------------------------------------------------>");
                    lock (_lock)
                    {
                        //确保只取缓存数据
                        if (!firstOrDefault)
                        {
                            firstOrDefault = true;

                            if (results != null && results?.Code >= 0)
                            {
                                var pending = results?.Data;
                                this.TodayOrderQuantity = pending?.TodayOrderQuantity ?? 0;
                                this.TodayVisitQuantity = pending?.TodayVisitQuantity ?? 0;
                                this.TodaySaleAmount = pending?.TodaySaleAmount ?? 0;
                                this.TodayAddTerminalQuantity = pending?.TodayAddTerminalQuantity ?? 0;
                            }
                        }
                    }
                }).DisposeWith(DeactivateWith);
            });

            //刷新权限
            this.PermissionLoad = ReactiveCommand.Create(() =>
            {
                try
                {
                    System.Diagnostics.Debug.Print($"DataSyncJob:同步数据.....");

                    if (Settings.IsAuthenticated)
                    {
                        var _globalService = App.Resolve<IGlobalService>();
                        var _settingService = App.Resolve<ISettingService>();

                        Parallel.Invoke(() =>
                        {
                            _globalService?.GetAPPFeatures(false, new System.Threading.CancellationToken());

                        }, () =>
                        {
                            _globalService?.SynchronizationPermission(new System.Threading.CancellationToken());
                        }, () =>
                        {
                            _settingService?.GetCompanySettingAsync(new System.Threading.CancellationToken());
                        });
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });

            //统计图表
            this.ChartLoad = ReactiveCommand.Create<object>((e) =>
            {
                //今日 1  昨天 3 前天 4 本月 8
                var tag = int.Parse(e.ToString());
                if (tag == 0) tag = 8;
                this.ChartIndex = tag;
                this.IsChat2Null = false;
                this.Is6Busy = true;

                _reportingService.Rx_GetBusinessAnalysis(tag, new System.Threading.CancellationToken())
                .Subscribe((results) =>
                {
                    if (results != null && results?.Code >= 0)
                    {
                        var pending = results?.Data;
                        if (pending != null)
                        {
                            var data = new ChartViewConfig()
                            {
                                BackgroundColor = Color.White,
                                ChartConfig = new ChartConfig
                                {
                                    type = DCMS.ChartJS.ChartTypes.Line,
                                    data = GetChartData(pending),
                                    options = new ChartOptions
                                    {
                                        borderWidth = 1
                                    }
                                }
                            };
                            LineConfig = data;
                        }
                        else
                        {
                            this.Is6Busy = false;
                            this.IsChat2Null = true;
                        }
                    }
                })
                .DisposeWith(DeactivateWith);
            });

            //热销排行
            this.HotChartLoad = ReactiveCommand.Create(() =>
            {
                this.IsChatNull = false;
                DateTime? startTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01 00:00:00"));
                DateTime? endTime = DateTime.Now;
                this.Is5Busy = true;
                bool firstOrDefault = false;

                //初始化 
                _reportingService.Rx_GetHotSaleRankingAsync(0, 0, 0, 0, startTime.Value, endTime.Value, new System.Threading.CancellationToken())
                .Subscribe((results) =>
                {
                    System.Diagnostics.Debug.Print("Rx_GetHotSaleRankingAsync------------------------------------------------>");

                    lock (_lock)
                    {
                        //确保只取缓存数据
                        if (!firstOrDefault)
                        {
                            firstOrDefault = true;
                            if (results != null && results?.Code >= 0)
                            {
                                var analysis = results?.Data;
                                if (analysis != null && analysis.Any())
                                {
                                    var ranks = analysis.OrderByDescending(s => s.TotalSumNetQuantity).ToList();
                                    if (ranks.Count > 10)
                                    {
                                        ranks = ranks.Take(10).ToList();
                                    }
                                    var data = new ChartViewConfig()
                                    {
                                        BackgroundColor = Color.White,
                                        ChartConfig = new ChartConfig
                                        {
                                            type = DCMS.ChartJS.ChartTypes.Bar,
                                            data = ChartDataProvider.GetHotSalesRanking(ranks, false),
                                            options = new ChartOptions
                                            {
                                                indexAxis = "y",
                                                plugins = new ChartPlugins
                                                {
                                                    legend = new ChartLegend
                                                    {
                                                        display = false
                                                    }
                                                },
                                                scales = new Scales
                                                {
                                                    xAxes = new XAxes[]
                                                    {
                                              new XAxes
                                              {
                                                  ticks = new Ticks
                                                  {
                                                      beginAtZero = true
                                                  },
                                                  position = "top"
                                              }
                                                    }
                                                }
                                            }
                                        }
                                    };
                                    HotBarConfig = data;
                                }
                                else
                                {
                                    this.Is5Busy = false;
                                    this.IsChatNull = true;
                                }
                            }
                        }
                    }
                }).DisposeWith(DeactivateWith);
            });

            //统计选择跳转
            this.ChatSelectedCommand = ReactiveCommand.CreateFromTask<string>(async (r) =>
            {
                await this.NavigateAsync(r);
            });

            //调用APP应用
            this.InvokeAppCommand = ReactiveCommand.CreateFromTask<Module>(async (r) =>
            {
                await this.Access(r, AccessStateEnum.View, async () =>
                {
                    using (UserDialogs.Instance.Loading("加载中..."))
                    {
                        if (r.Navigation == "VisitStorePage")
                        {
                            var check = await CheckSignIn();
                            if (!check)
                            {
                                await this.NavigateAsync($"{nameof(CurrentCustomerPage)}");
                                return;
                            }
                            else
                            {
                                await this.NavigateAsync(r.Navigation, ("Reference", this.PageName));
                            }
                        }
                        else if (r.Navigation == "MyInfoPage")
                        {
                            await this.NavigateAsync($"{nameof(MyInfoPage)}");
                            return;
                        }
                        else
                        {
                            await this.NavigateAsync(r.Navigation, ("Reference", this.PageName));
                        }
                    }
                });
            });

            //抽屉菜单选择
            this.ShowMenuPagePopUpCommand = ReactiveCommand.CreateFromTask<Module>(async (r) =>
            {
                if (IsTouched)
                    return;
                IsTouched = true;
                await PopupNavigation.Instance.PushAsync(new ChangeMenuPage());
                IsTouched = false;
            });

            this.BindBusyCommand(Load);
            this.BindBusy1Command(DashboardReportLoad);
            this.BindBusy5Command(HotChartLoad);
            this.BindBusy6Command(ChartLoad);
            this.BindBusy3Command(TopAppLoad);

            this.RefreshCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.Load.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.DashboardReportLoad.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.TopAppLoad.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.DashboardReportLoad.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.PermissionLoad.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.ChartLoad.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.HotChartLoad.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.ChatSelectedCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.InvokeAppCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.ShowMenuPagePopUpCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
        }

        private ChartData GetChartData(BusinessAnalysis analysis)
        {
            var labels = analysis.UserNames;
            var dataSets = new List<ChartNumberDataset>();

            var colors = RandomChartBuilder.GetDefaultColors();

            dataSets.Add(new ChartNumberDataset
            {
                type = DCMS.ChartJS.ChartTypes.Line,
                label = "拜访量",
                data = analysis.VistCounts,
                tension = 0.4,
                backgroundColor = analysis.VistCounts.Select((d, i) =>
               {
                   return $"rgb({colors[0].Item1},{colors[0].Item2},{colors[0].Item3})";
               }),

            });
            dataSets.Add(new ChartNumberDataset
            {
                type = DCMS.ChartJS.ChartTypes.Line,
                label = "销单数",
                data = analysis.SaleCounts,
                tension = 0.4,
                backgroundColor = analysis.VistCounts.Select((d, i) =>
                {
                    return $"rgb({colors[3].Item1},{colors[3].Item2},{colors[3].Item3})";
                })
            });

            dataSets.Add(new ChartNumberDataset
            {
                type = DCMS.ChartJS.ChartTypes.Line,
                label = "销订数",
                data = analysis.OrderCounts,
                tension = 0.4,
                backgroundColor = analysis.VistCounts.Select((d, i) =>
                {
                    return $"rgb({colors[5].Item1},{colors[5].Item2},{colors[5].Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ThrottleLoad(() =>
            {
                ((ICommand)Load)?.Execute(null);
                ((ICommand)ChartLoad)?.Execute(8);
                ((ICommand)HotChartLoad)?.Execute(null);
                ((ICommand)DashboardReportLoad)?.Execute(null);
                ((ICommand)PermissionLoad)?.Execute(null);
                ((ICommand)TopAppLoad)?.Execute(null);
            }, ((this.TopAppSeries?.Count ?? 0) == 0 || (this.EventItems?.Count ?? 0) == 0));
        }
    }
}

