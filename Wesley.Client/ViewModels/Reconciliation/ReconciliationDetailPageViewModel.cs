using Wesley.Client.Enums;
using Wesley.Client.Models.Sales;
using Wesley.Client.Services;
using Wesley.Client.Services.Sales;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Wesley.Client.ViewModels
{
    public class ReconciliationDetailPageViewModel : ViewModelBaseCutom
    {
        private readonly IFinanceReceiveAccountService _financeReceiveAccountService;
        [Reactive] public IList<FinanceReceiveAccountBillModel> Bills { get; set; } = new ObservableCollection<FinanceReceiveAccountBillModel>();
        [Reactive] public FinanceReceiveAccountBillModel Selecter { get; set; }

        public ReconciliationDetailPageViewModel(INavigationService navigationService,
             IProductService productService,
             IUserService userService,
             ITerminalService terminalService,
             IWareHousesService wareHousesService,
             IAccountingService accountingService,
             IFinanceReceiveAccountService financeReceiveAccountService,


             IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "单据信息";

            _financeReceiveAccountService = financeReceiveAccountService;

            //选择单据
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
             .Skip(1)
             .Where(x => x != null)
             .SubOnMainThread(async x =>
            {
                switch (x.BillType)
                {
                    case (int)BillTypeEnum.SaleBill:
                        await this.NavigateAsync("SaleBillPage", ("BillId", x.BillId));
                        break;
                    case (int)BillTypeEnum.ReturnBill:
                        await this.NavigateAsync("ReturnBillPage", ("BillId", x.BillId));
                        break;
                    case (int)BillTypeEnum.CashReceiptBill:
                        await this.NavigateAsync("CashReceiptBillPage", ("BillId", x.BillId));
                        break;
                    case (int)BillTypeEnum.AdvanceReceiptBill:
                        await this.NavigateAsync("AdvanceReceiptBillPage", ("BillId", x.BillId));
                        break;
                    case (int)BillTypeEnum.CostExpenditureBill:
                        await this.NavigateAsync("CostExpenditureBillPage", ("BillId", x.BillId));
                        break;
                    default:
                        break;
                }
                Selecter = null;
            });

            //打印单据
            this.PrintCommand = ReactiveCommand.Create(() =>
            {
                Alert("请选择单据！");
            });

            //上交对账单
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async (e) =>
            {
                var bills = this.Bills.Where(s => s.Selected == true).ToList();
                if (bills.Count == 0)
                {
                    Alert("请选择单据！");
                    return Unit.Default;
                }

                var postData = new FinanceReceiveAccountBillSubmitModel()
                {
                    Items = bills
                };

                return await SubmitAsync(postData, 0, _financeReceiveAccountService.SubmitAccountStatementAsync, (result) =>
                {
                    //移除当前
                    bills.ForEach(b =>
                    {
                        this.Bills.Remove(b);
                    });
                });

            });

            this.ExceptionsSubscribe();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            switch (this.BillType)
            {
                case BillTypeEnum.SaleBill:
                case BillTypeEnum.ReturnBill:
                case BillTypeEnum.CashReceiptBill:
                    this.Title = "收欠款（明细）";
                    break;
                case BillTypeEnum.AdvanceReceiptBill:
                    this.Title = "收预收款（明细）";
                    break;
                case BillTypeEnum.CostExpenditureBill:
                    this.Title = "费用支出（明细）";
                    break;
                default:
                    break;
            }

            if (parameters.ContainsKey("Bills"))
            {
                parameters.TryGetValue("Bills", out ObservableCollection<FinanceReceiveAccountBillModel> bills);
                if (bills != null)
                {
                    this.Bills = bills;
                }
            }
        }
    }
}
