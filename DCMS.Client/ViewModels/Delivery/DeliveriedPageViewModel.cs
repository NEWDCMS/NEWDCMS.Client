using Acr.UserDialogs;
using DCMS.Client.Enums;
using DCMS.Client.Models.Sales;
using DCMS.Client.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace DCMS.Client.ViewModels
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
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "已签收(0)";

            _saleBillService = saleBillService;

            this.Load = BillsLoader.Load(async () =>
            {
                var pending = new List<DeliverySignModel>();

                try
                {
                    var result = await _saleBillService.GetDeliveriedSignsAsync(Settings.UserId,
                        Filter.StartTime,
                        Filter.EndTime,
                        Filter.BusinessUserId,
                        Filter.TerminalId,
                        force: this.ForceRefresh,
                         calToken: new System.Threading.CancellationToken());

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
                }
                catch (System.Exception) { }

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
            this.SubscribeMenus((x) =>
            {
                //获取当前UTC时间
                DateTime dtime = DateTime.Now;

                switch (x)
                {
                    case MenuEnum.TODAY:
                        {
                            Filter.StartTime = DateTime.Parse(dtime.ToString("yyyy-MM-dd 00:00:00"));
                            Filter.EndTime = dtime;
                            ((ICommand)Load)?.Execute(null);
                        }
                        break;
                    case MenuEnum.YESTDAY:
                        {
                            Filter.StartTime = dtime.AddDays(-1);
                            Filter.EndTime = dtime;
                            ((ICommand)Load)?.Execute(null);
                        }
                        break;
                    case MenuEnum.OTHER:
                        {
                            SelectDateRang();
                            ((ICommand)Load)?.Execute(null);
                        }
                        break;
                }

            }, string.Format(Constants.MENU_DEV_KEY, 3));


            this.BindBusyCommand(Load);

        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }

    }
}
