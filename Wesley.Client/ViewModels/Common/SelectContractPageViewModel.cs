using Acr.UserDialogs;
using Wesley.Client.Models;
using Wesley.Client.Models.Finances;
using Wesley.Client.Models.Terminals;
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
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{
    public class SelectContractPageViewModel : ViewModelBaseCutom
    {
        private readonly ICostContractService _costContractService;
        [Reactive] public ObservableCollection<CostContractBindingModel> CostContracts { get; set; } = new ObservableCollection<CostContractBindingModel>();

        public new ReactiveCommand<CollectionView, Unit> ItemSelectedCommand { get; }
        [Reactive] public CostContractBindingModel Selecter { get; set; }

        public SelectContractPageViewModel(INavigationService navigationService,
           IProductService productService,
           ITerminalService terminalService,
           IUserService userService,
           IWareHousesService wareHousesService,
           IAccountingService accountingService,
           ICostContractService costContractService,
           IDialogService dialogService
            ) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {

            Title = "选择费用合同";

            _costContractService = costContractService;

            this.Load = CostContractLoader.Load(async () =>
            {
                //重载时排它
                ItemTreshold = 1;
                try
                {
                    //清除列表
                    CostContracts.Clear();

                    var items = await GetDataPages(0, PageSize);
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            if (CostContracts.Count(s => (s.BillNumber == item.BillNumber) && (s.Month == item.Month)) == 0)
                            {
                                CostContracts.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                this.CostContracts = new ObservableRangeCollection<CostContractBindingModel>(CostContracts);
                return CostContracts;
            });

            //以增量方式加载数据
            this.ItemTresholdReachedCommand = ReactiveCommand.Create(async () =>
            {
                using (var dig = UserDialogs.Instance.Loading("加载中..."))
                {
                    try
                    {

                        int pageIdex = CostContracts.Count / (PageSize == 0 ? 1 : PageSize);
                        var items = await GetDataPages(pageIdex, PageSize);
                        var previousLastItem = Terminals.Last();
                        if (items != null)
                        {
                            foreach (var item in items)
                            {
                                if (CostContracts.Count(s => (s.BillNumber == item.BillNumber) && (s.Month == item.Month)) == 0)
                                {
                                    CostContracts.Add(item);
                                }
                            }

                            if (items.Count() == 0 || items.Count() == CostContracts.Count)
                            {
                                ItemTreshold = -1;
                                return this.CostContracts;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                        ItemTreshold = -1;
                    }


                    this.CostContracts = new ObservableRangeCollection<CostContractBindingModel>(CostContracts);
                    return this.CostContracts;
                }

            });

            //选择
            this.ItemSelectedCommand = ReactiveCommand.Create<CollectionView>(async e =>
            {
                if (e.SelectedItem != null)
                {
                    Selecter.AccountingOptionId = this.Accounting.AccountingOptionId;
                    Selecter.AccountingOptionName = this.Accounting.AccountingOptionName;
                    await _navigationService.GoBackAsync(("CostContract", Selecter));
                }
            });

            this.BindBusyCommand(Load);

        }

        public async Task<IList<CostContractBindingModel>> GetDataPages(int pageNumber, int pageSize)
        {
            var costContracts = new List<CostContractBindingModel>();

            var result = await _costContractService.GetCostContractsBindingAsync(
                Settings.UserId,
                Filter.AccountOptionId,
                null,
                DateTime.Now.Year,
                DateTime.Now.Month,
                null,
                null,
                null,
                pageNumber,
                PageSize, this.ForceRefresh, new System.Threading.CancellationToken());

            if (result != null)
            {
                costContracts = result?.ToList();
            }

            return costContracts;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            try
            {
                //选择客户回传
                if (parameters.ContainsKey("Terminaler"))
                {
                    parameters.TryGetValue("Terminaler", out TerminalModel terminaler);
                    Filter.TerminalId = terminaler != null ? terminaler.Id : 0;
                }

                if (parameters.ContainsKey("Accounting"))
                {
                    parameters.TryGetValue("Accounting", out AccountingModel temp);
                    Filter.AccountOptionId = temp != null ? temp.AccountingOptionId : 0;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public override void OnAppearing()
        {
            if (this.CostContracts?.Count == 0)
                ((ICommand)Load)?.Execute(null);
        }
    }
}
