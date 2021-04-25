using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Sales;
using Wesley.Client.Services;
using Wesley.Client.Services.Sales;
using Wesley.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
namespace Wesley.Client.ViewModels
{
    public class ReconciliationORreceivablesPageViewModel : ViewModelBaseCutom
    {
        private readonly IFinanceReceiveAccountService _financeReceiveAccountService;
        [Reactive] public IList<FRABViewModel> Bills { get; set; } = new ObservableCollection<FRABViewModel>();
        [Reactive] public IList<AccountMaping> Payments { get; set; } = new ObservableCollection<AccountMaping>();
        [Reactive] public double HeightRequest { get; set; }

        [Reactive] public int TotalCount { get; set; }
        [Reactive] public decimal TotalAmount { get; set; }


        [Reactive] public int SaleProductCount { get; set; }
        public List<AccountProductModel> SaleProducts { get; set; } = new List<AccountProductModel>();

        [Reactive] public int GiftProductCount { get; set; }
        public List<AccountProductModel> GiftProducts { get; set; } = new List<AccountProductModel>();

        [Reactive] public int ReturnProductCount { get; set; }
        public List<AccountProductModel> ReturnProducts { get; set; } = new List<AccountProductModel>();

        [Reactive] public FRABViewModel Selecter { get; set; }

        public IReactiveCommand ViewProducts { get; set; }


        public ReconciliationORreceivablesPageViewModel(INavigationService navigationService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IFinanceReceiveAccountService financeReceiveAccountService,


            IDialogService dialogService) : base(navigationService,
                productService, terminalService, userService,
                wareHousesService, accountingService, dialogService)
        {

            Title = "收款对账";

            _financeReceiveAccountService = financeReceiveAccountService;

            //载入单据
            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {

                //筛选
                DateTime? start = !Filter.StartTime.HasValue ? DateTime.Now.AddMonths(-1) : Filter.StartTime;
                DateTime? end = !Filter.EndTime.HasValue ? DateTime.Now : Filter.EndTime;
                int? businessUserId = Filter.BusinessUserId;

                //收款方式
                var accounts = await _accountingService.GetDefaultAccountingAsync((int)BillTypeEnum.FinanceReceiveAccount, this.ForceRefresh, calToken: cts.Token);
                var payments = accounts?.Item3?.Select(s => { return new AccountMaping { AccountingOptionId = s.Id, CollectionAmount = 0, Name = s.Name }; }).ToList();

                //获取汇总
                var summeries = await _financeReceiveAccountService.GetFinanceReceiveAccounts(start, end, businessUserId, 0, 0, "", force: this.ForceRefresh, calToken: cts.Token);

                var sales = summeries?.Where(s => s.BillType == (int)BillTypeEnum.SaleBill).ToList();
                var returns = summeries?.Where(s => s.BillType == (int)BillTypeEnum.ReturnBill).ToList();
                var cashReceipts = summeries?.Where(s => s.BillType == (int)BillTypeEnum.CashReceiptBill).ToList();
                var advanceReceipts = summeries?.Where(s => s.BillType == (int)BillTypeEnum.AdvanceReceiptBill).ToList();
                var costExpenditures = summeries?.Where(s => s.BillType == (int)BillTypeEnum.CostExpenditureBill).ToList();

                sales.ForEach(s =>
                {
                    s.BillTypeId = (int)BillTypeEnum.SaleBill;
                    s.BType = BillTypeEnum.SaleBill;
                });
                returns.ForEach(s =>
                {
                    s.BillTypeId = (int)BillTypeEnum.ReturnBill;
                    s.BType = BillTypeEnum.ReturnBill;
                });
                cashReceipts.ForEach(s =>
                {
                    s.BillTypeId = (int)BillTypeEnum.CashReceiptBill;
                    s.BType = BillTypeEnum.CashReceiptBill;
                });
                advanceReceipts.ForEach(s =>
                {
                    s.BillTypeId = (int)BillTypeEnum.AdvanceReceiptBill;
                    s.BType = BillTypeEnum.AdvanceReceiptBill;
                });
                costExpenditures.ForEach(s =>
                {
                    s.BillTypeId = (int)BillTypeEnum.CostExpenditureBill;
                    s.BType = BillTypeEnum.CostExpenditureBill;
                });

                //销售收款
                var sale = new FRABViewModel()
                {

                    BType = BillTypeEnum.SaleBill,
                    BillType = (int)BillTypeEnum.SaleBill,
                    BillTypeName = "销售收款",

                    TotalSaleAmountSum = sales.Sum(s => s.SaleAmountSum),
                    TotalSaleAmount = sales.Sum(s => s.SaleAmount),
                    TotalSaleAdvanceReceiptAmount = sales.Sum(s => s.SaleAdvanceReceiptAmount),
                    TotalSaleOweCashAmount = sales.Sum(s => s.SaleOweCashAmount),
                    SaleBills = new ObservableCollection<FinanceReceiveAccountBillModel>(sales),
                    SaleBillCount = sales?.Count() ?? 0,
                };

                //退货款
                var @return = new FRABViewModel()
                {
                    BType = BillTypeEnum.ReturnBill,
                    BillType = (int)BillTypeEnum.ReturnBill,
                    BillTypeName = "退货款",

                    TotalReturnAmountSum = returns.Sum(s => s.ReturnAmountSum),
                    TotalReturnAmount = returns.Sum(s => s.ReturnAmount),
                    TotalReturnAdvanceReceiptAmount = returns.Sum(s => s.ReturnAdvanceReceiptAmount),
                    TotalReturnOweCashAmount = returns.Sum(s => s.ReturnOweCashAmount),
                    ReturnBills = new ObservableCollection<FinanceReceiveAccountBillModel>(returns),
                    ReturnBillCount = returns?.Count() ?? 0,
                };

                //收欠款
                var cashReceipt = new FRABViewModel()
                {
                    BType = BillTypeEnum.CashReceiptBill,
                    BillType = (int)BillTypeEnum.CashReceiptBill,
                    BillTypeName = "收欠款",

                    TotalReceiptCashOweCashAmountSum = cashReceipts.Sum(s => s.ReceiptCashOweCashAmountSum),
                    TotalReceiptCashReceivableAmount = cashReceipts.Sum(s => s.ReceiptCashReceivableAmount),
                    TotalReceiptCashAdvanceReceiptAmount = cashReceipts.Sum(s => s.ReceiptCashAdvanceReceiptAmount),
                    ReceiptCashOweCashBills = new ObservableCollection<FinanceReceiveAccountBillModel>(cashReceipts),
                    ReceiptCashOweCashBillCount = cashReceipts?.Count() ?? 0,
                };

                //收预收款
                var advanceReceipt = new FRABViewModel()
                {
                    BType = BillTypeEnum.AdvanceReceiptBill,
                    BillType = (int)BillTypeEnum.AdvanceReceiptBill,
                    BillTypeName = "收预收款",

                    TotalAdvanceReceiptSum = advanceReceipts.Sum(s => s.AdvanceReceiptSum),
                    TotalAdvanceReceiptAmount = advanceReceipts.Sum(s => s.AdvanceReceiptAmount),
                    TotalAdvanceReceiptOweCashAmount = advanceReceipts.Sum(s => s.AdvanceReceiptOweCashAmount),
                    AdvanceReceiptBills = new ObservableCollection<FinanceReceiveAccountBillModel>(advanceReceipts),
                    AdvanceReceiptBillCount = advanceReceipts?.Count() ?? 0,
                };

                //费用支出
                var costExpenditure = new FRABViewModel()
                {
                    BType = BillTypeEnum.CostExpenditureBill,
                    BillType = (int)BillTypeEnum.CostExpenditureBill,
                    BillTypeName = "费用支出",

                    TotalCostExpenditureSum = costExpenditures.Sum(s => s.CostExpenditureSum),
                    TotalCostExpenditureAmount = costExpenditures.Sum(s => s.CostExpenditureAmount),
                    TotalCostExpenditureOweCashAmount = costExpenditures.Sum(s => s.CostExpenditureOweCashAmount),
                    CostExpenditureBills = new ObservableCollection<FinanceReceiveAccountBillModel>(costExpenditures),
                    CostExpenditureBillCount = costExpenditures?.Count() ?? 0,
                };

                //追加
                this.Bills.Clear();
                this.Bills.Add(sale);
                this.Bills.Add(@return);
                this.Bills.Add(cashReceipt);
                this.Bills.Add(advanceReceipt);
                this.Bills.Add(costExpenditure);


                //合计收款金额
                if (payments.Any())
                {
                    var allAccounts = new List<AccountMaping>();
                    foreach (var bill in summeries)
                    {
                        allAccounts.AddRange(bill.Accounts);
                    }

                    payments.ForEach(pay =>
                    {
                        pay.CollectionAmount = allAccounts.Where(s => s.AccountingOptionId == pay.AccountingOptionId).Sum(s => s.CollectionAmount);
                    });

                    this.HeightRequest = payments.Count() * 40.7;
                    this.Payments = new ObservableCollection<AccountMaping>(payments);
                }

                //合计商品
                this.SaleProductCount = summeries?.Sum(s => s.SaleProductCount) ?? 0;
                this.GiftProductCount = summeries?.Sum(s => s.GiftProductCount) ?? 0;
                this.ReturnProductCount = summeries?.Sum(s => s.ReturnProductCount) ?? 0;
                this.SaleProducts.Clear();
                this.SaleProducts.Clear();
                this.ReturnProducts.Clear();
                foreach (var sp in summeries)
                {
                    this.GiftProducts.AddRange(sp.GiftProducts);
                    this.SaleProducts.AddRange(sp.SaleProducts);
                    this.ReturnProducts.AddRange(sp.ReturnProducts);
                }

                //总计
                this.TotalCount = this.Bills.Sum(s => s.SaleBillCount) + this.Bills.Sum(s => s.ReturnBillCount) + this.Bills.Sum(s => s.ReceiptCashOweCashBillCount) + this.Bills.Sum(s => s.AdvanceReceiptBillCount) + this.Bills.Sum(s => s.CostExpenditureBillCount);
                this.TotalAmount = this.Bills.Sum(s => s.TotalSaleAmountSum) + this.Bills.Sum(s => s.TotalReturnAmountSum) + this.Bills.Sum(s => s.TotalReceiptCashOweCashAmountSum) + this.Bills.Sum(s => s.TotalAdvanceReceiptSum) + this.Bills.Sum(s => s.TotalCostExpenditureSum);
            });

            //选择类型
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
             .Skip(1)
             .Where(x => x != null)
             .SubOnMainThread(async x =>
            {
                switch (x.BType)
                {
                    case BillTypeEnum.SaleBill:
                        await this.NavigateAsync("ReconciliationDetailPage", ("Bills", x?.SaleBills), ("BillType", x.BillType));
                        break;
                    case BillTypeEnum.ReturnBill:
                        await this.NavigateAsync("ReconciliationDetailPage", ("Bills", x?.ReturnBills), ("BillType", x.BillType));
                        break;
                    case BillTypeEnum.CashReceiptBill:
                        await this.NavigateAsync("ReconciliationDetailPage", ("Bills", x?.ReceiptCashOweCashBills), ("BillType", x.BillType));
                        break;
                    case BillTypeEnum.AdvanceReceiptBill:
                        await this.NavigateAsync("ReconciliationDetailPage", ("Bills", x?.AdvanceReceiptBills), ("BillType", x.BillType));
                        break;
                    case BillTypeEnum.CostExpenditureBill:
                        await this.NavigateAsync("ReconciliationDetailPage", ("Bills", x?.CostExpenditureBills), ("BillType", x.BillType));
                        break;
                    default:
                        break;
                }
            });

            //预览商品
            this.ViewProducts = ReactiveCommand.Create<string>(async e =>
           {
               try
               {
                   if (e.Equals("SaleProducts"))
                   {
                       await this.NavigateAsync("ReconciliationProductsPage", ("ReconciliationProducts", this.SaleProducts), ("Title", "销售商品"));
                   }
                   else if (e.Equals("GiftProducts"))
                   {
                       await this.NavigateAsync("ReconciliationProductsPage", ("ReconciliationProducts", this.GiftProducts), ("Title", "赠送商品"));
                   }
                   else if (e.Equals("ReturnProducts"))
                   {
                       await this.NavigateAsync("ReconciliationProductsPage", ("ReconciliationProducts", this.ReturnProducts), ("Title", "退货商品"));
                   }
               }
               catch (Exception ex)
               {
                   Crashes.TrackError(ex);
               }
           });


            //员工选择时
            this.WhenAnyValue(x => x.Filter.BusinessUserId)
               .Where(s => s > 0)
               .Select(s => s)
               .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
               .Subscribe(s => ((ICommand)Load)?.Execute(null))
               .DisposeWith(DestroyWith);

            //菜单选择
            this.SetMenus((x) =>
            {
                //获取当前UTC时间
                DateTime utcNow = DateTime.Now.ToUniversalTime();

                DateTime dtime = UtcHelper.ConvertDateTimeInt(utcNow);
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
                    case MenuEnum.SUBMIT30:
                        {
                            Filter.StartTime = dtime.AddMonths(-1);
                            Filter.EndTime = dtime;
                            ((ICommand)Load)?.Execute(null);
                        }
                        break;
                }
            }, 8, 9, 14, 30);

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
