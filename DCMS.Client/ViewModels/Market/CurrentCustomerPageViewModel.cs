using Acr.UserDialogs;
using DCMS.Client.Enums;
using DCMS.Client.Models;
using DCMS.Client.Models.Census;
using DCMS.Client.Models.Terminals;
using DCMS.Client.Pages;
using DCMS.Client.Services;
using DCMS.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace DCMS.Client.ViewModels
{
    /// <summary>
    /// 拜访选择终端客户
    /// </summary>
    public class CurrentCustomerPageViewModel : ViewModelBaseCutom
    {
        private readonly ILiteDbService<VisitStore> _conn;
        public ReactiveCommand<object, Unit> LineSelected { get; }
        public ReactiveCommand<object, Unit> OtherCustomerCommand { get; }
        public ReactiveCommand<object, Unit> AddCustomerCommand { get; }
        public ReactiveCommand<TerminalModel, Unit> OpenNavigationToCommand { get; }
        public ReactiveCommand<int, Unit> DistanceOrderCommand { get; }
        [Reactive] public bool DataVewEnable { get; set; }
        [Reactive] public bool NullViewEnable { get; set; } = true;
        [Reactive] public bool ListVewEnable { get; set; } = true;
        [Reactive] public bool MapVewEnable { get; set; }
        [Reactive] public bool OrderByDistance { get; set; }
        [Reactive] public TerminalModel Selecter { get; set; }
        public double Longitude { get; set; } = 0;
        public double Latitude { get; set; } = 0;
        private static object _tsLock = new object();


        bool itemTreshold = false;
        public CurrentCustomerPageViewModel(INavigationService navigationService,
           IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IDialogService dialogService,
            ILiteDbService<VisitStore> conn) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "拜访选择终端客户";
            _conn = conn;

            this.PageSize = 20;


            //搜索(默认触发Load)
            this.WhenAnyValue(x => x.Filter.SerchKey)
                .Skip(1)
                .Select(s => s)
                .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Subscribe(s => ((ICommand)Load)?.Execute(null))
                .DisposeWith(DeactivateWith);

            //片区选择
            this.WhenAnyValue(x => x.Filter.DistrictId)
               .Where(s => s > 0)
               .Select(s => s)
               .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
               .Subscribe(s => ((ICommand)Load)?.Execute(null))
               .DisposeWith(DeactivateWith);

            //距离排序
            this.WhenAnyValue(x => x.Filter.DistanceOrderBy)
              .Where(s => s > 0)
              .Select(s => s)
              .Skip(1)
              .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
              .Subscribe(s => ((ICommand)Load)?.Execute(null))
              .DisposeWith(DeactivateWith);

            //加载数据
            this.Load = ReactiveCommand.Create(async () =>
            {
                try
                {
                    System.Diagnostics.Debug.Print("----------------------CurrentCustomerPageViewModel----------------------------->");

                    //重载时排它
                    ItemTreshold = 1;

                    try
                    {
                        if (Terminals != null && Terminals.Any())
                            Terminals?.Clear();
                    }
                    catch (Exception) { }

                    PageCounter = 0;
                    itemTreshold = true;

                    DataVewEnable = false;
                    NullViewEnable = true;

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
                        1.5,
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

                    itemTreshold = false;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                finally  
                {
                    DataVewEnable = true;
                    NullViewEnable = false;
                }
            });

            //以增量方式加载数据
            this.ItemTresholdReachedCommand = ReactiveCommand.Create(async () =>
           {
               if (ItemTreshold == -1 || itemTreshold) return;

               try
               {
                   itemTreshold = true;
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
                               1.5,
                               pageNumber,
                               pageSize);

                           var series = tuple.Item2;
                           if (series != null && series.Any())
                           {
                               try
                               {
                                   foreach (var s in series)
                                   {
                                       if (!(this.Terminals?.Select(s => s.Id).Contains(s.Id) ?? true))
                                       {
                                           this.Terminals?.Add(s);
                                       }
                                   }
                               }
                               catch (Exception) { }
                               itemTreshold = false;
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


            //线路选择
            this.LineSelected = ReactiveCommand.Create<object>(async e => await SelectUserLine((data) =>
            {
                if (data != null)
                {
                    Filter.LineId = data.Id;
                    Filter.LineName = data.Name;
                    if (Filter.LineId > 0)
                    {
                        var dts = data.Terminals;
                        if (dts != null && dts.Any())
                        {
                            var series = dts.OrderByDescending(t => t.Id);

                            series?.ToList().ForEach(s =>
                            {
                                s.RankName = string.IsNullOrEmpty(s.RankName) ? "A级" : s.RankName;
                                s.Distance = MapHelper.CalculateDistance(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0, s.Location_Lat ?? 0, s.Location_Lng ?? 0);
                            });

                            if (series.Count() > 0)
                            {
                                Terminals = new AsyncObservableCollection<TerminalModel>(series);
                                MessageBus.Current.SendMessage(Terminals, Constants.TRACKTERMINALS_KEY);
                            }
                        }
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
                })
                .DisposeWith(DeactivateWith);


            //绑定页面菜单
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                 //整单备注
                { MenuEnum.NEARCUSTOMER, (m,vm) => {
                   if (this != null)
                            {
                                Title = "附近客户";
                                Filter.LineId = 0;
                                ((ICommand)RefreshCommand)?.Execute(null);
                            }
                } }, 
                //清空单据
                { MenuEnum.LINESELECT, (m,vm) => {
                     if (this != null)
                            {
                                ((ICommand)LineSelected)?.Execute(null);
                            }
                } }
            });

            this.BindBusyCommand(Load);
        }


        public void RefreshTerminals()
        {
            try
            {
                ((ICommand)Load)?.Execute(null);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        public override void OnAppearing()
        {
            base.OnAppearing();

            //控制显示菜单
            _popupMenu?.Show(18, 19);
            ((ICommand)Load)?.Execute(null);
        }
    }
}
