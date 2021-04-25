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
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    /// <summary>
    /// 选择终端客户
    /// </summary>
    public class SelectCustomerPageViewModel : ViewModelBaseCutom
    {
        [Reactive] public TerminalModel Selecter { get; set; }

        public SelectCustomerPageViewModel(INavigationService navigationService,
           IProductService productService,
           ITerminalService terminalService,
           IUserService userService,
           IWareHousesService wareHousesService,
           IAccountingService accountingService,


           IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {

            Title = "选择客户";

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


            //Load
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
                .DisposeWith(DestroyWith);

            this.ItemTresholdReachedCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

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
