using Wesley.Client.Models.Census;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{
    public class SelectUserPageViewModel : ViewModelBaseCutom
    {
        public DateTime SelectDateTime { get; set; } = DateTime.Now;


        public new ReactiveCommand<CollectionView, Unit> ItemSelectedCommand { get; }
        [Reactive] public BusinessVisitList Selecter { get; set; }


        public SelectUserPageViewModel(INavigationService navigationService,
               IProductService productService,
               ITerminalService terminalService,
               IUserService userService,
               IWareHousesService wareHousesService,
               IAccountingService accountingService,
                 IDialogService dialogService
            ) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "选择业务员";
            _navigationService = navigationService;
            _dialogService = dialogService;
            _terminalService = terminalService;


            //搜索
            this.WhenAnyValue(x => x.Filter.SerchKey)
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s)
                .Throttle(TimeSpan.FromSeconds(2), RxApp.MainThreadScheduler)
                .Subscribe(s =>
                {
                    ((ICommand)SerchCommand)?.Execute(s);
                }).DisposeWith(DeactivateWith);
            this.SerchCommand = ReactiveCommand.Create<string>(e =>
            {
                if (string.IsNullOrEmpty(Filter.SerchKey))
                {
                    this.Alert("请输入关键字！");
                    return;
                }
                ((ICommand)Load)?.Execute(null);
            });

            this.Load = BusinessVisitLoader.Load(async () =>
            {
                var pending = new List<BusinessVisitList>();
                var result = await _terminalService.GetAllUserVisitedListAsync(SelectDateTime, this.ForceRefresh, new System.Threading.CancellationToken());
                if (result != null)
                {
                    var lists = result.ToList();
                    int i = 0;
                    lists.ForEach(u => { u.ColumnIndex = i++; });

                    if (!string.IsNullOrEmpty(Filter.SerchKey))
                        lists = lists.Where(u => u.BusinessUserName.Contains(Filter.SerchKey)).ToList();

                    pending = lists;
                }

                if (pending != null && pending.Count > 0)
                {
                    BusinessUsers = new ObservableRangeCollection<BusinessVisitList>(pending);
                }

                return await Task.FromResult(pending);
            });



            //选择
            this.ItemSelectedCommand = ReactiveCommand.Create<CollectionView>(async e =>
            {
                if (e.SelectedItem != null)
                {
                    await _navigationService.GoBackAsync(("BusinessVisitUser", Selecter));
                }
            });

            this.BindBusyCommand(Load);

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                if (parameters.ContainsKey("SelectDateTime"))
                {
                    parameters.TryGetValue<DateTime>("SelectDateTime", out DateTime SelectDateTime);
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
            ((ICommand)Load)?.Execute(null);
        }
    }
}
