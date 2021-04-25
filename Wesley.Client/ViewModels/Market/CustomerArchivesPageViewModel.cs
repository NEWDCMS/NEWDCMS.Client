using Acr.UserDialogs;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Services;
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
    public class CustomerArchivesPageViewModel : ViewModelBaseCutom
    {
        public ReactiveCommand<object, Unit> BusinessSelected { get; }
        [Reactive] public TerminalModel Selecter { get; set; }

        public ReactiveCommand<TerminalModel, Unit> OpenNavigationToCommand { get; }

        public CustomerArchivesPageViewModel(INavigationService navigationService,
               IProductService productService,
               IUserService userService,
               ITerminalService terminalService,
               IWareHousesService wareHousesService,
               IAccountingService accountingService,
                 IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "客户档案";

            Filter.BusinessUserName = Settings.UserRealName;
            Filter.BusinessUserId = Settings.UserId;


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


            //加载数据
            this.Load = TerminalsLoader.Load(async () =>
            {
                //重载时排它
                ItemTreshold = 1;
                try
                {
                    //清除列表
                    Terminals.Clear();
                    var items = await _terminalService.GetTerminalsPage(Filter, LineTier, 0, PageSize, this.ForceRefresh, calToken: cts.Token);
                    foreach (var item in items)
                    {
                        if (Terminals.Count(s => s.Id == item.Id) == 0)
                        {
                            Terminals.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                if (Terminals.Count > 0)
                    this.Terminals = new ObservableRangeCollection<TerminalModel>(Terminals);

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
                            var items = await _terminalService.GetTerminalsPage(Filter, LineTier, pageIdex, PageSize, this.ForceRefresh, calToken: cts.Token);
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
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                            ItemTreshold = -1;
                        }
                    }
                }
            }, this.WhenAny(x => x.Terminals, x => x.GetValue().Count > 0));


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

            //编辑客户
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
            .Skip(1)
            .Where(x => x != null)
            .SubOnMainThread(async item =>
           {
               await this.NavigateAsync("AddCustomerPage", ("Terminaler", item), ("Edit", true));
               this.Selecter = null;
           });

            //添加客户
            this.AddCommand = ReactiveCommand.Create<object>(async e => await this.NavigateAsync("AddCustomerPage", ("Edit", false)));

            //员工选择
            this.BusinessSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectUser((data) =>
                 {
                     Filter.BusinessUserId = data.Id;
                     Filter.BusinessUserName = data.Column;
                     ((ICommand)Load)?.Execute(null);
                 }, Enums.UserRoleType.Employees);
            });

            this.ItemTresholdReachedCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.BusinessSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
