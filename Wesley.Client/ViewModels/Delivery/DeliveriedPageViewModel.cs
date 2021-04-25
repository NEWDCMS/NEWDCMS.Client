using Acr.UserDialogs;
using Wesley.Client.Enums;
using Wesley.Client.Models.Sales;
using Wesley.Client.Services;

using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    /// <summary>
    /// 已签收
    /// </summary>
    public class DeliveriedPageViewModel : ViewModelBase
    {
        private readonly ISaleBillService _saleBillService;
        [Reactive] public ObservableCollection<DeliverySignGroup> Bills { get; private set; } = new ObservableCollection<DeliverySignGroup>();
        public ReactiveCommand<DeliverySignModel, Unit> SelecterCommand { get; set; }
        [Reactive] public decimal? TotalAmount { get; set; }

        public DeliveriedPageViewModel(INavigationService navigationService,
            ISaleBillService saleBillService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "已签收(0)";

            _saleBillService = saleBillService;

            this.Load = BillsLoader.Load(async () =>
            {
                var pending = new List<DeliverySignModel>();

                var result = await _saleBillService.GetDeliveriedSignsAsync(Settings.UserId,
                    Filter.StartTime,
                    Filter.EndTime,
                    Filter.BusinessUserId,
                    Filter.TerminalId, force: this.ForceRefresh, calToken: cts.Token);

                if (result != null)
                {
                    pending = result.ToList();

                    this.Bills?.Clear();
                    var gDeliveries = pending.GroupBy(s => s.TerminalName).ToList();
                    foreach (var group in gDeliveries)
                    {
                        var gs = group.Select(s =>
                        {
                            var vs = s;
                            vs.IsLast = !(group.LastOrDefault()?.BillNumber == s.BillNumber);
                            return vs;
                        }).ToList();

                        var first = group.FirstOrDefault();
                        if (first != null)
                            Bills.Add(new DeliverySignGroup(first.TerminalName, first.Address, first.BossCall, first.Distance, gs));
                    }

                    TotalAmount = pending.Select(b => b.SumAmount).Sum();
                    Title = $"已签收({pending.Count})";
                }
                return pending;
            });

            this.SelecterCommand = ReactiveCommand.Create<DeliverySignModel>(async (item) =>
            {
                if (item != null)
                {
                    using (UserDialogs.Instance.Loading("稍等..."))
                    {
                        if (item.BillTypeId == (int)BillTypeEnum.ExchangeBill)
                        {
                            await this.NavigateAsync("ExchangeBillPage", ("Reference", this.PageName),
                                ("Bill", item.ExchangeBill));
                        }
                        else if (item.BillTypeId == (int)BillTypeEnum.SaleReservationBill)
                        {
                            await this.NavigateAsync("SaleBillPage", ("Reference", this.PageName),
                                 ("Bill", item.SaleBill));
                        }
                        else if (item.BillTypeId == (int)BillTypeEnum.ReturnReservationBill)
                        {
                            await this.NavigateAsync("ReturnBillPage", ("Reference", this.PageName),
                               ("Bill", item.ReturnBill));
                        }
                        else if (item.BillTypeId == (int)BillTypeEnum.CostExpenditureBill)
                        {
                            await this.NavigateAsync("CostExpenditureBillPage", ("Reference", this.PageName),
                               ("Bill", item.CostExpenditureBill));
                        }
                        else if (item.BillTypeId == (int)BillTypeEnum.SaleBill)
                        {
                            await this.NavigateAsync("SaleBillPage", ("Reference", this.PageName),
                               ("Bill", item.SaleBill));
                        }
                    }
                }
            });

            //菜单选择
            this.SetMenus((x) =>
            {
                string key = string.Format("Wesley.CLIENT.PAGES.MARKET.{0}_SELECTEDTAB_{1}", this.PageViewName.ToUpper(), 1);
                this.MenuBusKey = string.Format(Constants.MENU_KEY, key);
                this.HitFilterDate(x, () => { ((ICommand)Load)?.Execute(null); });

            }, 8, 9, 14);

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
