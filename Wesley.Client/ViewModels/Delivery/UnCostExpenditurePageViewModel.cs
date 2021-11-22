using Acr.UserDialogs;
using Wesley.Client.Enums;
using Wesley.Client.Models.Finances;
using Wesley.Client.Services;
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

namespace Wesley.Client.ViewModels
{

    /// <summary>
    /// 费用支出签收
    /// </summary>
    public class UnCostExpenditurePageViewModel : ViewModelBaseOrder<CostExpenditureBillModel>
    {
        [Reactive] public decimal? TotalAmount { get; set; }
        public ReactiveCommand<CostExpenditureBillModel, Unit> SelecterCommand { get; set; }

        public UnCostExpenditurePageViewModel(INavigationService navigationService,
            IGlobalService globalService,
            IDialogService dialogService,
            IAllocationService allocationService,
            IAdvanceReceiptService advanceReceiptService,
            IReceiptCashService receiptCashService,
            ICostContractService costContractService,
            ICostExpenditureService costExpenditureService,
            IInventoryService inventoryService,
            IPurchaseBillService purchaseBillService,
            IReturnReservationBillService returnReservationBillService,
            IReturnBillService returnBillService,
            ISaleReservationBillService saleReservationBillService,
            ISaleBillService saleBillService
            ) : base(navigationService, globalService, allocationService, advanceReceiptService, receiptCashService, costContractService, costExpenditureService, inventoryService, purchaseBillService, returnReservationBillService, returnBillService, saleReservationBillService, saleBillService, dialogService)
        {
            Title = "费用支出(0)";

            this.Bills = new ObservableCollection<CostExpenditureBillModel>();

            //载入未签收费用支出单据
            this.Load = ReactiveCommand.Create(async () =>
            {
                var pending = new List<CostExpenditureBillModel>();
                try
                {
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;
                    int? customerId = Filter.TerminalId;
                    int? employeeId = Filter.BusinessUserId;
                    int? makeuserId = Settings.UserId;

                    string billNumber = "";
                    //获取已审核
                    bool? auditedStatus = true;
                    bool? showReverse = null;
                    bool sortByAuditedTime = false;
                    int pagenumber = 0;
                    int pageSize = 20;

                    var result = await _costExpenditureService.GetCostExpendituresAsync(makeuserId,
                        customerId,
                        "",
                        employeeId,
                        billNumber,
                        auditedStatus,
                        startTime,
                        endTime,
                        showReverse,
                        sortByAuditedTime,
                        0,
                        pagenumber,
                        pageSize, this.ForceRefresh, new System.Threading.CancellationToken());

                    if (result != null)
                    {
                        pending = result?.Select(s =>
                        {
                            var sm = s;
                            sm.IsLast = !(result.LastOrDefault()?.BillNumber == s.BillNumber);
                            sm.SumAmount = s.Items?.Sum(i => i.Amount) ?? 0;

                            return sm;
                        }).ToList();

                        TotalAmount = pending.Select(b => b.SumAmount).Sum();
                        Title = $"费用支出({pending.Count})";

                        if (pending != null && pending.Any())
                            Bills = new ObservableCollection<CostExpenditureBillModel>(pending);
                    }
                }
                catch (System.Exception) { }
            });

            //费用支出单签收
            this.SelecterCommand = ReactiveCommand.CreateFromTask<CostExpenditureBillModel>(async (item) =>
            {
                if (item != null)
                {
                    using (UserDialogs.Instance.Loading("稍等..."))
                    {
                        await this.NavigateAsync("CostExpenditureBillPage",
                            ("Reference", this.PageName),
                            ("DispatchItemModel", null),
                            ("Bill", item));
                    }
                }
                item = null;
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

            }, string.Format(Constants.MENU_DEV_KEY, 1));


            this.BindBusyCommand(Load);

        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
