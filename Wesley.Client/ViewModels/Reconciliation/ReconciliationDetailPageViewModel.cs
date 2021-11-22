using Wesley.Client.Enums;
using Wesley.Client.Models.Sales;
using Wesley.Client.Services;
using Wesley.Client.Services.Sales;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;
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
        [Reactive] public IList<FinanceReceiveAccountBillModel> TempBills { get; set; } = new ObservableCollection<FinanceReceiveAccountBillModel>();
        [Reactive] public IList<FinanceReceiveAccountBillModel> Bills { get; set; } = new ObservableCollection<FinanceReceiveAccountBillModel>();
        [Reactive] public FinanceReceiveAccountBillModel Selecter { get; set; }

        [Reactive] public bool SelectedAll { get; set; }
        [Reactive] public decimal SumCount { get; set; }
        [Reactive] public string ConfirmText { get; set; }


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
            }).DisposeWith(DeactivateWith);

            //选择业务员
            this.WhenAnyValue(x => x.Filter.BusinessUserId)
             .Skip(1)
             .Where(x => x > 0)
             .Subscribe(x =>
            {
                var fillter = this.TempBills.Where(s => s.UserId == x).ToList();
                this.Bills = new ObservableCollection<FinanceReceiveAccountBillModel>(fillter);
            });

            //全选
            this.WhenAnyValue(x => x.SelectedAll)
                .Subscribe(x =>
                {
                    foreach (var b in this.Bills)
                    {
                        b.Selected = x;
                    }
                    CalcSum();
                    var count = this.Bills.Where(s => s.Selected).Count();
                    this.ConfirmText = $"确认上交({count})";

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
        }

        public void CheckChanged()
        {
            CalcSum();
            var count = this.Bills.Where(s => s.Selected).Count();
            this.ConfirmText = $"确认上交({count})";
        }


        public void CalcSum()
        {
            var bills = this.Bills.Where(s => s.Selected).ToList();
            decimal subCount = 0;
            foreach (var b in bills)
            {
                switch (b.BillType)
                {
                    case (int)BillTypeEnum.SaleBill:
                        subCount += b.SaleAmountSum;
                        break;
                    case (int)BillTypeEnum.ReturnBill:
                        subCount += b.ReturnAmountSum;
                        break;
                    case (int)BillTypeEnum.CashReceiptBill:
                        subCount += b.ReceiptCashOweCashAmountSum;
                        break;
                    case (int)BillTypeEnum.AdvanceReceiptBill:
                        subCount += b.AdvanceReceiptSum;
                        break;
                    case (int)BillTypeEnum.CostExpenditureBill:
                        subCount += b.CostExpenditureSum;
                        break;
                    default:
                        break;
                }
            }
            this.SumCount = subCount;
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
                    this.TempBills = bills;
                }
            }
        }
    }
}
