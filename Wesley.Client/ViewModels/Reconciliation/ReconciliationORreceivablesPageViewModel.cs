using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Sales;
using Wesley.Client.Pages;
using Wesley.Client.Services;
using Wesley.Client.Services.Sales;
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
             IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {

            Title = "收款对账";

            _financeReceiveAccountService = financeReceiveAccountService;

            //默认
            //Filter.BusinessUserName = Settings.UserRealName;
            //Filter.BusinessUserId = Settings.UserId;

            //载入单据
            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    //筛选
                    DateTime? start = !Filter.StartTime.HasValue ? DateTime.Now.AddMonths(-1) : Filter.StartTime;
                    DateTime? end = !Filter.EndTime.HasValue ? DateTime.Now.AddDays(1) : Filter.EndTime;
                    int? businessUserId = Filter.BusinessUserId;

                    //收款方式
                    var accounts = await _accountingService.GetDefaultAccountingAsync((int)BillTypeEnum.FinanceReceiveAccount,
                        this.ForceRefresh,
                        new System.Threading.CancellationToken());

                    var payments = accounts?.Item3?.Where(s => s != null).Select(s =>
                    {
                        return new AccountMaping
                        {
                            AccountingOptionId = s.Id,
                            CollectionAmount = 0,
                            Name = s.Name
                        };
                    })?.ToList();

                    //获取汇总
                    var summeries = await _financeReceiveAccountService.GetFinanceReceiveAccounts(start,
                        end,
                        businessUserId,
                        0,
                        0,
                        "",
                        force: this.ForceRefresh,
                         calToken: new System.Threading.CancellationToken());

                    var sales = summeries?.Where(s => s.BillType == (int)BillTypeEnum.SaleBill)?.ToList();
                    var returns = summeries?.Where(s => s.BillType == (int)BillTypeEnum.ReturnBill)?.ToList();
                    var cashReceipts = summeries?.Where(s => s.BillType == (int)BillTypeEnum.CashReceiptBill)?.ToList();
                    var advanceReceipts = summeries?.Where(s => s.BillType == (int)BillTypeEnum.AdvanceReceiptBill)?.ToList();
                    var costExpenditures = summeries?.Where(s => s.BillType == (int)BillTypeEnum.CostExpenditureBill)?.ToList();

                    if (sales != null && sales.Any())
                        sales.ForEach(s =>
                        {
                            s.BillTypeId = (int)BillTypeEnum.SaleBill;
                            s.BType = BillTypeEnum.SaleBill;
                        });

                    if (returns != null && returns.Any())
                        returns.ForEach(s =>
                    {
                        s.BillTypeId = (int)BillTypeEnum.ReturnBill;
                        s.BType = BillTypeEnum.ReturnBill;
                    });

                    if (cashReceipts != null && cashReceipts.Any())
                        cashReceipts.ForEach(s =>
                    {
                        s.BillTypeId = (int)BillTypeEnum.CashReceiptBill;
                        s.BType = BillTypeEnum.CashReceiptBill;
                    });

                    if (advanceReceipts != null && advanceReceipts.Any())
                        advanceReceipts.ForEach(s =>
                    {
                        s.BillTypeId = (int)BillTypeEnum.AdvanceReceiptBill;
                        s.BType = BillTypeEnum.AdvanceReceiptBill;
                    });

                    if (costExpenditures != null && costExpenditures.Any())
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

                        TotalSaleAmountSum = sales?.Sum(s => s.SaleAmountSum) ?? 0,
                        TotalSaleAmount = sales?.Sum(s => s.SaleAmount) ?? 0,
                        TotalSaleAdvanceReceiptAmount = sales?.Sum(s => s.SaleAdvanceReceiptAmount) ?? 0,
                        TotalSaleOweCashAmount = sales?.Sum(s => s.SaleOweCashAmount) ?? 0,
                        SaleBillCount = sales?.Count() ?? 0,
                    };

                    if (sales != null && sales.Any())
                        sale.SaleBills = new ObservableCollection<FinanceReceiveAccountBillModel>(sales);

                    //退货款
                    var @return = new FRABViewModel()
                    {
                        BType = BillTypeEnum.ReturnBill,
                        BillType = (int)BillTypeEnum.ReturnBill,
                        BillTypeName = "退货款",

                        TotalReturnAmountSum = returns?.Sum(s => s.ReturnAmountSum) ?? 0,
                        TotalReturnAmount = returns?.Sum(s => s.ReturnAmount) ?? 0,
                        TotalReturnAdvanceReceiptAmount = returns?.Sum(s => s.ReturnAdvanceReceiptAmount) ?? 0,
                        TotalReturnOweCashAmount = returns?.Sum(s => s.ReturnOweCashAmount) ?? 0,
                        ReturnBillCount = returns?.Count() ?? 0,
                    };

                    if (returns != null && returns.Any())
                        @return.ReturnBills = new ObservableCollection<FinanceReceiveAccountBillModel>(returns);

                    //收欠款
                    var cashReceipt = new FRABViewModel()
                    {
                        BType = BillTypeEnum.CashReceiptBill,
                        BillType = (int)BillTypeEnum.CashReceiptBill,
                        BillTypeName = "收欠款",

                        TotalReceiptCashOweCashAmountSum = cashReceipts?.Sum(s => s.ReceiptCashOweCashAmountSum) ?? 0,
                        TotalReceiptCashReceivableAmount = cashReceipts?.Sum(s => s.ReceiptCashReceivableAmount) ?? 0,
                        TotalReceiptCashAdvanceReceiptAmount = cashReceipts?.Sum(s => s.ReceiptCashAdvanceReceiptAmount) ?? 0,
                        ReceiptCashOweCashBillCount = cashReceipts?.Count() ?? 0,
                    };

                    if (cashReceipts != null && cashReceipts.Any())
                        cashReceipt.ReceiptCashOweCashBills = new ObservableCollection<FinanceReceiveAccountBillModel>(cashReceipts);

                    //收预收款
                    var advanceReceipt = new FRABViewModel()
                    {
                        BType = BillTypeEnum.AdvanceReceiptBill,
                        BillType = (int)BillTypeEnum.AdvanceReceiptBill,
                        BillTypeName = "收预收款",

                        TotalAdvanceReceiptSum = advanceReceipts?.Sum(s => s.AdvanceReceiptSum) ?? 0,
                        TotalAdvanceReceiptAmount = advanceReceipts?.Sum(s => s.AdvanceReceiptAmount) ?? 0,
                        TotalAdvanceReceiptOweCashAmount = advanceReceipts?.Sum(s => s.AdvanceReceiptOweCashAmount) ?? 0,
                        AdvanceReceiptBillCount = advanceReceipts?.Count() ?? 0,
                    };

                    if (advanceReceipts != null && advanceReceipts.Any())
                        advanceReceipt.AdvanceReceiptBills = new ObservableCollection<FinanceReceiveAccountBillModel>(advanceReceipts);


                    //费用支出
                    var costExpenditure = new FRABViewModel()
                    {
                        BType = BillTypeEnum.CostExpenditureBill,
                        BillType = (int)BillTypeEnum.CostExpenditureBill,
                        BillTypeName = "费用支出",

                        TotalCostExpenditureSum = costExpenditures?.Sum(s => s.CostExpenditureSum) ?? 0,
                        TotalCostExpenditureAmount = costExpenditures?.Sum(s => s.CostExpenditureAmount) ?? 0,
                        TotalCostExpenditureOweCashAmount = costExpenditures?.Sum(s => s.CostExpenditureOweCashAmount) ?? 0,
                        CostExpenditureBillCount = costExpenditures?.Count() ?? 0,
                    };

                    if (costExpenditures != null && costExpenditures.Any())
                        costExpenditure.CostExpenditureBills = new ObservableCollection<FinanceReceiveAccountBillModel>(costExpenditures);


                    //追加
                    this.Bills.Clear();
                    this.Bills.Add(sale);
                    this.Bills.Add(@return);
                    this.Bills.Add(cashReceipt);
                    this.Bills.Add(advanceReceipt);
                    this.Bills.Add(costExpenditure);


                    //合计收款金额
                    if (payments != null && payments.Any())
                    {
                        var allAccounts = new List<AccountMaping>();

                        if (summeries != null && summeries.Any())
                            foreach (var bill in summeries)
                            {
                                allAccounts.AddRange(bill.Accounts);
                            }

                        payments.ForEach(pay =>
                        {
                            pay.CollectionAmount = allAccounts.Where(s => s.AccountingOptionId == pay.AccountingOptionId).Sum(s => s.CollectionAmount);
                        });

                        this.HeightRequest = (payments?.Count() ?? 1) * 40.7;
                        this.Payments = new ObservableCollection<AccountMaping>(payments);
                    }

                    //合计商品
                    var spc = new List<AccountProductModel>();
                    var gpc = new List<AccountProductModel>();
                    var rpc = new List<AccountProductModel>();

                    foreach (var s in summeries)
                    {
                        foreach (var p in s.SaleProducts)
                        {
                            if (!spc.Select(s => s.ProductId).Contains(p.ProductId))
                            {
                                spc.Add(p);
                            }
                        }

                        foreach (var p in s.GiftProducts)
                        {
                            if (!gpc.Select(s => s.ProductId).Contains(p.ProductId))
                            {
                                gpc.Add(p);
                            }
                        }

                        foreach (var p in s.ReturnProducts)
                        {
                            if (!rpc.Select(s => s.ProductId).Contains(p.ProductId))
                            {
                                rpc.Add(p);
                            }
                        }
                    }

                    this.SaleProductCount = spc.Count();
                    this.GiftProductCount = gpc.Count(); 
                    this.ReturnProductCount = rpc.Count();
                    this.SaleProducts.Clear();
                    this.GiftProducts.Clear();
                    this.ReturnProducts.Clear();

                    if (summeries != null && summeries.Any())
                        foreach (var sp in summeries)
                        {
                            this.GiftProducts.AddRange(sp.GiftProducts);
                            this.SaleProducts.AddRange(sp.SaleProducts);
                            this.ReturnProducts.AddRange(sp.ReturnProducts);
                        }

                    //总计
                    this.TotalCount = this.Bills.Sum(s => s.SaleBillCount) + this.Bills.Sum(s => s.ReturnBillCount) + this.Bills.Sum(s => s.ReceiptCashOweCashBillCount) + this.Bills.Sum(s => s.AdvanceReceiptBillCount) + this.Bills.Sum(s => s.CostExpenditureBillCount);
                    this.TotalAmount = this.Bills.Sum(s => s.TotalSaleAmountSum) + this.Bills.Sum(s => s.TotalReturnAmountSum) + this.Bills.Sum(s => s.TotalReceiptCashOweCashAmountSum) + this.Bills.Sum(s => s.TotalAdvanceReceiptSum) + this.Bills.Sum(s => s.TotalCostExpenditureSum);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
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
                        {
                            if ((x?.SaleBills?.Count ?? 0) > 0)
                                await this.NavigateAsync("ReconciliationDetailPage", ("Bills", x?.SaleBills), ("BillType", x.BillType));

                        }
                        break;
                    case BillTypeEnum.ReturnBill:
                        {
                            if ((x?.ReturnBills?.Count ?? 0) > 0)
                                await this.NavigateAsync("ReconciliationDetailPage", ("Bills", x?.ReturnBills), ("BillType", x.BillType));
                        }
                        break;
                    case BillTypeEnum.CashReceiptBill:
                        {
                            if ((x?.ReceiptCashOweCashBills?.Count ?? 0) > 0)
                                await this.NavigateAsync("ReconciliationDetailPage", ("Bills", x?.ReceiptCashOweCashBills), ("BillType", x.BillType));
                        }
                        break;
                    case BillTypeEnum.AdvanceReceiptBill:
                        {
                            if ((x?.AdvanceReceiptBills?.Count ?? 0) > 0)
                                await this.NavigateAsync("ReconciliationDetailPage", ("Bills", x?.AdvanceReceiptBills), ("BillType", x.BillType));
                        }
                        break;
                    case BillTypeEnum.CostExpenditureBill:
                        {
                            if ((x?.CostExpenditureBills?.Count ?? 0) > 0)
                                await this.NavigateAsync("ReconciliationDetailPage", ("Bills", x?.CostExpenditureBills), ("BillType", x.BillType));
                        }
                        break;
                    default:
                        break;
                }
            })
             .DisposeWith(DeactivateWith);

            //预览商品
            this.ViewProducts = ReactiveCommand.Create<string>(async e =>
           {
               try
               {
                   if (e.Equals("SaleProducts"))
                   {
                       if ((this.SaleProducts?.Count ?? 0) > 0)
                           await this.NavigateAsync("ReconciliationProductsPage", ("ReconciliationProducts", this.SaleProducts), ("Title", "销售商品"));
                   }
                   else if (e.Equals("GiftProducts"))
                   {
                       if ((this.GiftProducts?.Count ?? 0) > 0)
                           await this.NavigateAsync("ReconciliationProductsPage", ("ReconciliationProducts", this.GiftProducts), ("Title", "赠送商品"));
                   }
                   else if (e.Equals("ReturnProducts"))
                   {
                       if ((this.ReturnProducts?.Count ?? 0) > 0)
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
               .Skip(1)
               .Where(s => s > 0)
               .Select(s => s)
               .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
               .Subscribe(s => ((ICommand)Load)?.Execute(null))
               .DisposeWith(DeactivateWith);


            //绑定页面菜单
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                 //TODAY
                { MenuEnum.TODAY, (m,vm) =>
                {
                            Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                            Filter.EndTime = DateTime.Now;
                            ((ICommand)Load)?.Execute(null);
                } }, 
                //YESTDAY
                { MenuEnum.YESTDAY, (m,vm) =>
                {
                            Filter.StartTime = DateTime.Now.AddDays(-1);
                            Filter.EndTime = DateTime.Now;
                            ((ICommand)Load)?.Execute(null);
                } },
                 //OTHER
                { MenuEnum.OTHER, (m,vm) =>
                {
                            SelectDateRang();
                            ((ICommand)Load)?.Execute(null);
                } },
                 //SUBMIT30
                { MenuEnum.SUBMIT30, (m,vm) =>
                {
                            Filter.StartTime = DateTime.Now.AddMonths(-1);
                            Filter.EndTime = DateTime.Now;
                            ((ICommand)Load)?.Execute(null);
                } }
            });

            this.BindBusyCommand(Load);
        }


        public override void OnAppearing()
        {
            base.OnAppearing();

            //控制显示菜单
            _popupMenu?.Show(8, 9, 14, 30);

            ((ICommand)Load)?.Execute(null);
        }
    }
}
