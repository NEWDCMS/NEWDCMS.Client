using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Products;
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

namespace Wesley.Client.ViewModels
{
    public class PaymentMethodPageViewModel : ViewModelBase
    {
        private readonly IAccountingService _accountingService;
        public ReactiveCommand<object, Unit> MorePaymentCommand { get; }
        public IReactiveCommand TextChangedCommand { get; set; }
        [Reactive] public double CustomColViewHeight { get; set; }

        public PaymentMethodPageViewModel(INavigationService navigationService,
            IAccountingService accountingService,


            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "请输入支付金额";

            _navigationService = navigationService;
            _dialogService = dialogService;
            _accountingService = accountingService;


            //加载单据默认收付款方式
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    var defaultAcc = await _accountingService.GetDefaultAccountingAsync((int)BillType, this.ForceRefresh);

                    //绑定Accounts
                    var options = new List<AccountingModel>();
                    switch (BillType)
                    {
                        case BillTypeEnum.SaleBill:
                            {

                                options.Add(new AccountingModel()
                                {
                                    AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                                    Selected = true,
                                    CollectionAmount = 0,
                                    Name = defaultAcc.Item1?.Name,
                                    AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                                });

                            }
                            break;
                        case BillTypeEnum.SaleReservationBill:
                            {
                                PaymentMethods.OweCashName = "待支付：";
                                options.Add(new AccountingModel()
                                {
                                    AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                                    Selected = true,
                                    CollectionAmount = 0,
                                    Name = defaultAcc.Item1?.Name,
                                    AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                                });
                            }
                            break;
                        case BillTypeEnum.ReturnBill:
                            {
                                options.Add(new AccountingModel()
                                {
                                    AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                                    Selected = true,
                                    CollectionAmount = 0,
                                    Name = defaultAcc.Item1?.Name,
                                    AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                                });
                            }
                            break;
                        case BillTypeEnum.ReturnReservationBill:
                            {
                                options.Add(new AccountingModel()
                                {
                                    AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                                    Selected = true,
                                    CollectionAmount = 0,
                                    Name = defaultAcc.Item1?.Name,
                                    AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                                });
                            }
                            break;
                        case BillTypeEnum.CashReceiptBill:
                            {
                                options.Add(new AccountingModel()
                                {
                                    AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                                    Selected = true,
                                    CollectionAmount = 0,
                                    Name = defaultAcc.Item1?.Name,
                                    AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                                });
                            }
                            break;
                        case BillTypeEnum.PaymentReceiptBill:
                            {
                                options.Add(new AccountingModel()
                                {
                                    AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                                    Selected = true,
                                    CollectionAmount = 0,
                                    Name = defaultAcc.Item1?.Name,
                                    AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                                });
                            }
                            break;
                        case BillTypeEnum.AdvanceReceiptBill:
                            {
                                options.Add(new AccountingModel()
                                {
                                    AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                                    Selected = true,
                                    CollectionAmount = 0,
                                    Name = defaultAcc.Item1?.Name,
                                    AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                                });
                            }
                            break;
                        case BillTypeEnum.AdvancePaymentBill:
                            {
                                options.Add(new AccountingModel()
                                {
                                    AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                                    Selected = true,
                                    CollectionAmount = 0,
                                    Name = defaultAcc.Item1?.Name,
                                    AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                                });
                            }
                            break;
                        case BillTypeEnum.PurchaseBill:
                            {
                                options.Add(new AccountingModel()
                                {
                                    AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                                    Selected = true,
                                    CollectionAmount = 0,
                                    Name = defaultAcc.Item1?.Name,
                                    AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                                });
                            }
                            break;
                        case BillTypeEnum.PurchaseReturnBill:
                            {
                                options.Add(new AccountingModel()
                                {
                                    AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                                    Selected = true,
                                    CollectionAmount = 0,
                                    Name = defaultAcc.Item1?.Name,
                                    AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                                });
                            }
                            break;
                        case BillTypeEnum.CostExpenditureBill:
                            {
                                options.Add(new AccountingModel()
                                {
                                    AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                                    Selected = true,
                                    CollectionAmount = 0,
                                    Name = defaultAcc.Item1?.Name,
                                    AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                                });
                            }
                            break;
                        case BillTypeEnum.FinancialIncomeBill:
                            {
                                options.Add(new AccountingModel()
                                {
                                    AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                                    Selected = true,
                                    CollectionAmount = 0,
                                    Name = defaultAcc.Item1?.Name,
                                    AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                                });
                            }
                            break;
                    }

                    //初始
                    if (this.PaymentMethods.Selectes?.Count > 0)
                    {
                        foreach (var p in this.PaymentMethods.Selectes.ToList())
                        {
                            var cup = options.Where(s => s.AccountCodeTypeId == p.AccountCodeTypeId).FirstOrDefault();
                            if (cup == null)
                            {
                                this.PaymentMethods.Selectes.Add(p);
                            }
                        }
                        options = this.PaymentMethods.Selectes.Distinct().ToList();
                    }
                    else
                    {
                        this.PaymentMethods.Selectes = new ObservableCollection<AccountingModel>(options);
                    }

                    //动态行高度
                    this.CustomColViewHeight = 40.7 * options.Count();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }));

            //更多支付
            this.MorePaymentCommand = ReactiveCommand.Create<object>(async e =>
           {
               var selectes = this.PaymentMethods.Selectes.ToList();
               await this.NavigateAsync("MorePaymentPage", ("Selectes", selectes), ("BillType", BillType));
           });

            //保存支付
            this.SaveCommand = ReactiveCommand.CreateFromTask<object>(async e =>
            {
                if (PaymentMethods.CurrentCollectionAmount != PaymentMethods.Selectes.Distinct().Sum(s => s.CollectionAmount))
                {
                    this.Alert("各收款账户合计金额不等于本次收款金额！");
                    return;
                }

                switch (BillType)
                {
                    case BillTypeEnum.SaleBill:
                    case BillTypeEnum.SaleReservationBill:
                    case BillTypeEnum.ReturnBill:
                    case BillTypeEnum.ReturnReservationBill:
                        {
                            var prepaidAmount = PaymentMethods.Selectes.Where(a => (int)AccountingCodeEnum.AdvancesReceived == a.AccountCodeTypeId).FirstOrDefault();
                            if ((prepaidAmount?.CollectionAmount ?? 0) > TBalance.AdvanceAmountBalance)
                            {
                                this.Alert("可用预收款余额不足！");
                                return;
                            }
                        }
                        break;
                    case BillTypeEnum.CashReceiptBill:
                        break;
                    case BillTypeEnum.PaymentReceiptBill:
                        break;
                    case BillTypeEnum.AdvanceReceiptBill:
                        break;
                    case BillTypeEnum.PurchaseBill:
                    case BillTypeEnum.PurchaseReturnBill:
                        {
                            var prepaidAmount = PaymentMethods.Selectes.Where(a => (int)AccountingCodeEnum.Imprest == a.AccountCodeTypeId).FirstOrDefault();
                            if ((prepaidAmount?.CollectionAmount ?? 0) > MBalance.AdvanceAmountBalance)
                            {
                                this.Alert("可用预付款余额不足！");
                                return;
                            }
                        }
                        break;
                    case BillTypeEnum.CostExpenditureBill:
                        break;
                    case BillTypeEnum.FinancialIncomeBill:
                        break;
                }

                PaymentMethods.Selectes = new ObservableCollection<AccountingModel>(PaymentMethods.Selectes.Distinct().ToList()); //去重
                //返回到单据源
                await _navigationService.GoBackAsync(("PaymentMethods", PaymentMethods));
            });

            //更改输入
            this.TextChangedCommand = ReactiveCommand.Create<object>(e =>
            {
                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(x =>
                {
                    //动态行
                    var allAmount = PaymentMethods.Selectes.Distinct().Sum(s => s.CollectionAmount);

                    //本次收款
                    PaymentMethods.CurrentCollectionAmount = allAmount;

                    //欠款(待收/付款)
                    PaymentMethods.OweCash = PaymentMethods.SubAmount - PaymentMethods.PreferentialAmount - allAmount;
                });
            });

            this.TextChangedCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.MorePaymentCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.SaveCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            //单据来源
            if (parameters.ContainsKey("BillType"))
            {
                parameters.TryGetValue("BillType", out BillTypeEnum billType);
                BillType = billType;
                //根据单据类型触发
                ((ICommand)Load)?.Execute(null);
            }

            //单据支付方式回传
            if (parameters.ContainsKey("PaymentMethods"))
            {
                parameters.TryGetValue("PaymentMethods", out PaymentMethodBaseModel paymentmethods);
                if (paymentmethods != null)
                {
                    this.PaymentMethods.SubAmount = paymentmethods.SubAmount;
                    this.PaymentMethods.CurrentCollectionAmount = paymentmethods.CurrentCollectionAmount;
                    this.PaymentMethods.PreferentialAmount = paymentmethods.PreferentialAmount;
                    this.PaymentMethods.PreferentialAmountShowFiled = paymentmethods.PreferentialAmountShowFiled;
                    this.PaymentMethods.OweCash = paymentmethods.OweCash;
                    this.PaymentMethods.OweCashShowFiled = paymentmethods.OweCashShowFiled;
                    this.PaymentMethods.OweCashName = paymentmethods.OweCashName;

                    foreach (var p in this.PaymentMethods.Selectes)
                    {
                        var cup = paymentmethods
                            .Selectes
                            .Where(s => s.AccountingOptionId == p.AccountingOptionId).FirstOrDefault();

                        if (cup == null)
                        {
                            this.PaymentMethods.Selectes.Add(p);
                        }
                    }

                    if (this.PaymentMethods.Selectes.Count == 0)
                    {
                        this.PaymentMethods.Selectes = paymentmethods.Selectes;
                    }

                    this.CustomColViewHeight = 40.7 * this.PaymentMethods.Selectes.Count();
                }
            }

            //终端账户余额
            if (parameters.ContainsKey("TBalance"))
            {
                parameters.TryGetValue("TBalance", out TerminalBalance terminalBalance);
                if (terminalBalance != null)
                {
                    this.TBalance = terminalBalance;
                    this.ShowTBalance = true;
                    this.ShowMBalance = false;
                }
            }

            //供应商账户余额
            if (parameters.ContainsKey("MBalance"))
            {
                parameters.TryGetValue("MBalance", out ManufacturerBalance manufacturerBalance);
                if (manufacturerBalance != null)
                {
                    this.MBalance = manufacturerBalance;
                    this.ShowMBalance = true;
                    this.ShowTBalance = false;
                }
            }

            //MorePaymentPage 回传
            if (parameters.ContainsKey("Selectes"))
            {
                parameters.TryGetValue("Selectes", out List<AccountingModel> paymentmethods);
                if (paymentmethods != null)
                {
                    this.PaymentMethods.Selectes = new ObservableCollection<AccountingModel>(paymentmethods);
                    this.CustomColViewHeight = 40.7 * paymentmethods.Count();
                }
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
