using Acr.UserDialogs;
using Wesley.Client.CustomViews;
using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Finances;
using Wesley.Client.Models.Sales;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Pages;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{
    public class ReceiptBillPageViewModel : ViewModelBaseCutom<CashReceiptBillModel>
    {
        private readonly IReceiptCashService _receiptCashService;
        public ReactiveCommand<int, Unit> TextChangedCommand { get; }
        public ReactiveCommand<object, Unit> MorePaymentCommand { get; }
        public IReactiveCommand CrearFrom { get; set; }
        [Reactive] public CashReceiptItemModel Selecter { get; set; }
        [Reactive] public string ClearIcon { get; set; } = "fas-clock";
        //
        public ReceiptBillPageViewModel(INavigationService navigationService,
            IProductService productService,
            ITerminalService terminalService,
            IUserService userService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IReceiptCashService receiptCashService,
            IDialogService dialogService

            ) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "收款单";

            _receiptCashService = receiptCashService;

            InitBill();

            //验证
            var valid_IsVieweBill = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");
            var valid_TerminalId = this.ValidationRule(x => x.Bill.TerminalId, _isZero, "客户未指定");
            var valid_BusinessUserId = this.ValidationRule(x => x.Bill.BusinessUserId, _isZero, "业务员未指定");
            var valid_ProductCount = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "无收款单据信息");
            var valid_SelectesCount = this.ValidationRule(x => x.Bill.CashReceiptBillAccountings.Count, _isZero, "请选择支付方式");

            //更改时间
            this.WhenAnyValue(x => x.Filter.EndTime).Subscribe(x =>
            {
                if (x.HasValue)
                    ClearIcon = "fas-times-circle";
                else
                {
                    ClearIcon = "fas-clock";
                    ((ICommand)Load)?.Execute(null);
                }
            }).DisposeWith(DeactivateWith);

            //绑定数据
            this.Load = BillItemsLoader.Load(async () =>
            {
                if (Bill.Id == 0)
                {
                    var inits = await _receiptCashService.GetInitDataAsync(calToken: new System.Threading.CancellationToken());
                    if (inits != null)
                    {
                        Bill.CashReceiptBillAccountings = inits.CashReceiptBillAccountings;
                        if (this.PaymentMethods.Selectes.Count == 0)
                        {
                            var defaultPaymentMethods = inits.CashReceiptBillAccountings.Select(a =>
                            {
                                return new AccountingModel()
                                {
                                    Name = a.Name,
                                    AccountingOptionId = a.AccountingOptionId,
                                    CollectionAmount = 0,
                                    AccountCodeTypeId = a.AccountCodeTypeId,
                                    Number = a.AccountCodeTypeId
                                };
                            }).ToList();
                            this.PaymentMethods.Selectes = new ObservableCollection<AccountingModel>(defaultPaymentMethods); ;
                        }
                    }

                    var pending = new List<CashReceiptItemModel>();

                    int? terminalId = Filter.TerminalId;
                    int? businessUserId = Filter.BusinessUserId;

                    string billNumber = "";
                    string remark = "";
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;
                    int pageIndex = 0;
                    int pageSize = 20;

                    if (terminalId.HasValue && terminalId.Value != 0)
                    {
                        var result = await _receiptCashService.GetOwecashBillsAsync(Settings.UserId, terminalId, null, billNumber, remark, startTime, endTime, pageIndex, pageSize, calToken: new System.Threading.CancellationToken());

                        if (result != null)
                        {
                            var curBills = result?.Where(s => s?.BillTypeId != (int)BillTypeEnum.FinancialIncomeBill).ToList();
                            if (curBills != null && curBills.Count > 0)
                            {
                                foreach (var bill in curBills)
                                {
                                    pending.Add(new CashReceiptItemModel()
                                    {
                                        CashReceiptBillId = 0,
                                        BillId = bill.BillId,
                                        BillNumber = bill.BillNumber,
                                        BillTypeId = bill.BillTypeId,
                                        BillTypeName = bill.BillTypeName,
                                        BillLink = bill.BillLink,
                                        MakeBillDate = bill.MakeBillDate,

                                        //单据金额
                                        Amount = bill.Amount,
                                        //优惠
                                        DiscountAmount = bill.DiscountAmount,
                                        //已收金额
                                        PaymentedAmount = bill.PaymentedAmount,
                                        //欠款金额
                                        ArrearsAmount = bill.ArrearsAmount,

                                        //本次优惠金额
                                        DiscountAmountOnce = 0,
                                        //本次收款金额
                                        ReceivableAmountOnce = 0,
                                        //收款后尚欠金额 = 欠款金额
                                        AmountOwedAfterReceipt = bill.ArrearsAmount,
                                        //应收金额 = 欠款金额
                                        AmountReceivable = bill.ArrearsAmount,
                                        Remark = bill.Remark,
                                        //预览单据
                                        RedirectCommand = ReactiveCommand.Create<CashReceiptItemModel>(async e =>
                                        {
                                            //销售单
                                            if (e?.BillTypeId == (int)BillTypeEnum.SaleBill)
                                            {
                                                await this.NavigateAsync("SaleBillPage", ("BillId", e.BillId.ToString()));
                                            }
                                            //退货单
                                            else if (e?.BillTypeId == (int)BillTypeEnum.ReturnBill)
                                            {
                                                await this.NavigateAsync("ReturnBillPage", ("BillId", e.BillId.ToString()));
                                            }
                                            //预收款单
                                            else if (e?.BillTypeId == (int)BillTypeEnum.AdvanceReceiptBill)
                                            {
                                                await this.NavigateAsync("AdvanceReceiptBillPage", ("BillId", e.BillId.ToString()));
                                            }
                                            //费用支出
                                            else if (e?.BillTypeId == (int)BillTypeEnum.CostExpenditureBill)
                                            {
                                                await this.NavigateAsync("CostExpenditureBillPage", ("BillId", e.BillId.ToString()));
                                            }
                                            //其它收入
                                            else if (e?.BillTypeId == (int)BillTypeEnum.FinancialIncomeBill)
                                            {
                                                //移动端不支持（财务其它收入）收款
                                            }
                                        })
                                    });
                                }
                            }
                        }
                    }

                    if (pending != null && pending.Any())
                        Bill.Items = new ObservableRangeCollection<CashReceiptItemModel>(pending);
                }

                return Bill.Items;
            });

            //提交单据
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                return await this.Access(AccessGranularityEnum.ReceiptBillsSave, async () =>
                {
                    if (this.Bill.ReversedStatus)
                    {
                        _dialogService.ShortAlert("已红冲单据不能操作");
                        return Unit.Default;
                    }

                    if (this.Bill.AuditedStatus)
                    {
                        _dialogService.ShortAlert("已审核单据不能操作");
                        return Unit.Default;
                    }

                    //验证
                    var receivableAmount = Bill.TotalReceivableAmountOnce ?? 0;
                    if (receivableAmount == 0)
                    {
                        this.Alert("收款金额不能为空！"); return Unit.Default;
                    }
                    var totalReceiptAmount = Bill.CashReceiptBillAccountings?.Sum(s => s.CollectionAmount);
                    if (receivableAmount != totalReceiptAmount)
                    {
                        this.Alert("本次单据收款合计不等于总收款方式合计！"); return Unit.Default;
                    }

                    if (Bill.BusinessUserId == 0)
                        Bill.BusinessUserId = Settings.UserId;

                    var postData = new CashReceiptUpdateModel()
                    {
                        BillNumber = this.Bill.BillNumber,
                        CustomerId = Bill.TerminalId,
                        Payeer = Bill.BusinessUserId,
                        PreferentialAmount = Bill.TotalDiscountAmountOnce ?? 0,
                        OweCash = Bill.TotalAmountOwedAfterReceipt ?? 0,
                        ReceivableAmount = Bill.TotalReceivableAmountOnce ?? 0,
                        Remark = Bill.Remark,
                        Items = Bill.Items,
                        Accounting = Bill.CashReceiptBillAccountings,
                        //预收金额(收款中包含的预收部分)
                        AdvanceAmount = Bill.CashReceiptBillAccountings?.Where(s => s.AccountCodeTypeId == (int)AccountingCodeEnum.AdvancesReceived)
                           .FirstOrDefault()?.CollectionAmount ?? 0,
                        //预收款余额（客户余额）
                        AdvanceAmountBalance = this.TBalance.AdvanceAmountBalance
                    };

                    return await SubmitAsync(postData, Bill.Id, _receiptCashService.CreateOrUpdateAsync, (result) =>
                    {
                        Bill = new CashReceiptBillModel();
                    }, token: new System.Threading.CancellationToken());
                });
            },
            this.IsValid());

            //清除时间
            this.CrearFrom = ReactiveCommand.Create(() =>
           {
               Filter.StartTime = null;
               Filter.EndTime = null;
           });

            //支付方式
            this.MorePaymentCommand = ReactiveCommand.Create<object>(async e =>
           {
               await this.NavigateAsync("PaymentMethodPage",
                   ("PaymentMethods", this.PaymentMethods),
                   ("TBalance", this.TBalance),
                   ("BillType", this.BillType));
           });

            //更改
            this.TextChangedCommand = ReactiveCommand.Create<int>(e =>
           {
               UpdateUI(e);
           });

            //审核
            this.AuditingDataCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                //是否具有审核权限
                await this.Access(AccessGranularityEnum.ReceiptBillsApproved);
                await SubmitAsync(Bill.Id, _receiptCashService.AuditingAsync, async (result) =>
                {
                    //红冲审核水印
                    this.Bill.AuditedStatus = true;

                    var _conn = App.Resolve<ILiteDbService<MessageInfo>>();
                    var ms = await _conn.Table.FindByIdAsync(SelecterMessage.Id);
                    if (ms != null)
                    {
                        ms.IsRead = true;
                        await _conn.UpsertAsync(ms);
                    }

                }, token: new System.Threading.CancellationToken());
            }, this.WhenAny(x => x.Bill.Id, (x) => x.GetValue() > 0));

            //工具栏打印
            this.PrintCommand = ReactiveCommand.Create(async () =>
            {
                if (Bill.Items.Count == 0)
                {
                    Alert("请添加收款项目");
                    return;
                }
                Bill.BillType = BillTypeEnum.CashReceiptBill;
                await SelectPrint(Bill);
            });

            //绑定页面菜单
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                   //整单备注
                { MenuEnum.REMARK,(m,vm)=>{

                 AllRemak((result) =>
                     {
                         Bill.Remark = result;
                     }, Bill.Remark);

                } },
                //清空单据
                { MenuEnum.CLEAR,(m,vm)=>{

                 ClearBill<SaleBillModel, SaleItemModel>(null, DoClear);

                } },
                //打印
                { MenuEnum.PRINT,async (m,vm)=>{

                        this.Bill.BillType = BillTypeEnum.CashReceiptBill;
                         await SelectPrint(Bill);

                } },
                //历史单据
                { MenuEnum.HISTORY,async (m,vm)=>{
                        await SelectHistory();
                } },
                //审核
                { MenuEnum.SHENGHE,async (m,vm)=>{

                    ResultData result = null;
                         using (UserDialogs.Instance.Loading("审核中..."))
                         {
                             result = await _receiptCashService.AuditingAsync(Bill.Id);
                         }
                         if (result!=null&&result.Success)
                         {
                            //红冲审核水印
                            this.EnableOperation = true;
                            this.Bill.AuditedStatus = true;
                            await ShowConfirm(true, "审核成功", true, goReceipt: false);
                         }
                         else
                         {
                             await ShowConfirm(false, $"审核失败！{result?.Message}", false, goReceipt: false);
                         }

                } },
                //红冲
                { MenuEnum.HONGCHOU,async (m,vm)=>{

                    var remark = await CrossDiaglogKit.Current.GetInputTextAsync("红冲备注", "",Keyboard.Text);
                    if (!string.IsNullOrEmpty(remark))
                    {
                      bool result = false;
                        using (UserDialogs.Instance.Loading("红冲中..."))
                        {
                            result = await _receiptCashService.ReverseAsync(Bill.Id);
                        }

                        if (result)
                        {
                            //红冲审核水印
                            this.EnableOperation = true;
                            this.Bill.ReversedStatus = true;
                            await ShowConfirm(true, "红冲成功", true);
                        }
                        else
                        {
                            await ShowConfirm(false, "红冲失败！", false, goReceipt: false);
                        }
                    }
                } },
                //冲改
                { MenuEnum.CHOUGAI,async (m,vm)=>{
                    await _receiptCashService.ReverseAsync(Bill.Id);
                } }
            });

            this.BindBusyCommand(Load);
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                //支付方式更新
                if (parameters.ContainsKey("PaymentMethods"))
                {
                    #region //解析支付方式
                    parameters.TryGetValue("PaymentMethods", out PaymentMethodBaseModel paymentMethod);
                    if (paymentMethod != null)
                    {
                        PaymentMethods = paymentMethod;
                        //本次收款金额
                        decimal tmpCurrentCollectionAmount = paymentMethod.CurrentCollectionAmount;
                        //本次优惠金额
                        decimal tempCurrentPreferentialAmount = paymentMethod.PreferentialAmount;

                        //总应收款
                        decimal totalOwed = Bill.TotalArrearsAmountOnce ?? 0;
                        //收款后欠款金额
                        decimal currentTotalOwed = totalOwed - tmpCurrentCollectionAmount - tempCurrentPreferentialAmount;
                        //欠终端款
                        decimal currentTotalOwedTerminal = Bill.Items.Where(p => p.AmountReceivable < 0).Sum(p => Math.Abs(p.AmountReceivable ?? 0));

                        //备份欠终端款计算欠款抵扣
                        decimal tempCurrentTotalOwedTerminal = currentTotalOwedTerminal;
                        foreach (var b in Bill.Items)
                        {
                            if (b.AmountReceivable == 0)
                                continue;
                            if (b.AmountReceivable > 0)
                            {
                                b.ReceivableAmountOnce = 0;
                                b.DiscountAmountOnce = 0;
                                //计算优惠
                                if (b.AmountReceivable < tempCurrentPreferentialAmount)
                                {
                                    b.ReceivableAmountOnce = 0;
                                    b.DiscountAmountOnce = tempCurrentPreferentialAmount;
                                    tempCurrentPreferentialAmount -= b.AmountReceivable ?? 0;
                                    continue;
                                }
                                if (b.AmountReceivable == tempCurrentPreferentialAmount)
                                {
                                    b.ReceivableAmountOnce = 0;
                                    b.DiscountAmountOnce = tempCurrentPreferentialAmount;
                                    tempCurrentPreferentialAmount = 0;
                                    continue;
                                }
                                if (b.AmountReceivable > tempCurrentPreferentialAmount)
                                {
                                    b.DiscountAmountOnce = tempCurrentPreferentialAmount;
                                    tempCurrentPreferentialAmount = 0;
                                }

                                //计算实际收款
                                decimal laveAmountReceivable = (b.AmountReceivable ?? 0) - (b.DiscountAmountOnce ?? 0);
                                if (laveAmountReceivable == tmpCurrentCollectionAmount)
                                {
                                    b.ReceivableAmountOnce += laveAmountReceivable;
                                    tmpCurrentCollectionAmount = 0;
                                    continue;
                                }
                                if (laveAmountReceivable < tmpCurrentCollectionAmount)
                                {
                                    b.ReceivableAmountOnce += laveAmountReceivable;
                                    tmpCurrentCollectionAmount -= laveAmountReceivable;
                                    continue;
                                }
                                if (laveAmountReceivable > tmpCurrentCollectionAmount)
                                {
                                    b.ReceivableAmountOnce += tmpCurrentCollectionAmount;
                                    tmpCurrentCollectionAmount = 0;
                                }

                                //计算欠款抵扣
                                laveAmountReceivable -= (b.ReceivableAmountOnce ?? 0);
                                if (laveAmountReceivable == currentTotalOwedTerminal)
                                {
                                    b.ReceivableAmountOnce += currentTotalOwedTerminal;
                                    currentTotalOwedTerminal = 0;
                                    break;
                                }
                                if (laveAmountReceivable < currentTotalOwedTerminal)
                                {
                                    b.ReceivableAmountOnce += laveAmountReceivable;
                                    currentTotalOwedTerminal -= laveAmountReceivable;
                                    continue;
                                }
                                if (laveAmountReceivable > currentTotalOwedTerminal)
                                {
                                    b.ReceivableAmountOnce += currentTotalOwedTerminal;
                                    currentTotalOwedTerminal = 0;
                                    break;
                                }
                            }
                        }
                        tempCurrentTotalOwedTerminal -= currentTotalOwedTerminal;
                        foreach (var b in Bill.Items)
                        {
                            if (b.AmountReceivable == 0)
                                continue;
                            if (b.AmountReceivable < 0)
                            {
                                b.ReceivableAmountOnce = 0;
                                b.DiscountAmountOnce = 0;
                                if (b.AmountReceivable > tempCurrentPreferentialAmount)
                                {
                                    b.ReceivableAmountOnce = 0;
                                    b.DiscountAmountOnce = b.AmountReceivable;
                                    tempCurrentPreferentialAmount -= b.AmountReceivable ?? 0;
                                    continue;
                                }
                                if (b.AmountReceivable == tempCurrentPreferentialAmount)
                                {
                                    b.ReceivableAmountOnce = 0;
                                    b.DiscountAmountOnce = tempCurrentPreferentialAmount;
                                    tempCurrentPreferentialAmount = 0;
                                    continue;
                                }
                                if (b.AmountReceivable < tempCurrentPreferentialAmount)
                                {
                                    b.DiscountAmountOnce = tempCurrentPreferentialAmount;
                                    tempCurrentPreferentialAmount = 0;
                                }
                                //优惠后
                                decimal laveAmountReceivable = (b.AmountReceivable ?? 0) - (b.DiscountAmountOnce ?? 0);
                                if (laveAmountReceivable == -tempCurrentTotalOwedTerminal)
                                {
                                    b.ReceivableAmountOnce += laveAmountReceivable;
                                    tempCurrentTotalOwedTerminal = 0;
                                    continue;
                                }
                                if (laveAmountReceivable > -tempCurrentTotalOwedTerminal)
                                {
                                    b.ReceivableAmountOnce += laveAmountReceivable;
                                    tempCurrentTotalOwedTerminal -= laveAmountReceivable;
                                    continue;
                                }
                                if (laveAmountReceivable < -tempCurrentTotalOwedTerminal)
                                {
                                    b.ReceivableAmountOnce += (-tempCurrentTotalOwedTerminal);
                                    tempCurrentTotalOwedTerminal = 0;
                                }
                                //抵扣后
                                laveAmountReceivable -= (b.ReceivableAmountOnce ?? 0);
                                if (laveAmountReceivable == tmpCurrentCollectionAmount)
                                {
                                    b.ReceivableAmountOnce += laveAmountReceivable;
                                    tmpCurrentCollectionAmount = 0;
                                    break;
                                }
                                if (laveAmountReceivable < tmpCurrentCollectionAmount && tmpCurrentCollectionAmount != 0)
                                {
                                    b.ReceivableAmountOnce += laveAmountReceivable;
                                    tmpCurrentCollectionAmount -= laveAmountReceivable;
                                    continue;
                                }
                                if (laveAmountReceivable > tmpCurrentCollectionAmount && tmpCurrentCollectionAmount != 0)
                                {
                                    b.ReceivableAmountOnce += tmpCurrentCollectionAmount;
                                    tmpCurrentCollectionAmount = 0;
                                    break;
                                }

                            }
                        }


                        if (paymentMethod.Selectes != null)
                        {
                            var accountings = paymentMethod.Selectes.Select(a =>
                            {
                                return new AccountMaping()
                                {
                                    Name = a.Name,
                                    AccountingOptionId = a.AccountingOptionId,
                                    CollectionAmount = a.CollectionAmount,
                                    AccountCodeTypeId = a.AccountCodeTypeId,
                                    Number = a.Number,
                                };
                            });
                            //收款账户映射
                            Bill.CashReceiptBillAccountings = new ObservableCollection<AccountMaping>(accountings);
                        }
                    }
                    #endregion 
                    UpdateUI(-1);
                    //返回不执行 load 
                    return;
                }

                //选择客户
                if (parameters.ContainsKey("Terminaler"))
                {
                    parameters.TryGetValue("Terminaler", out TerminalModel terminaler);
                    Bill.TerminalId = terminaler != null ? terminaler.Id : 0;
                    Bill.TerminalName = terminaler != null ? terminaler.Name : "";
                    //获取余额
                    this.TBalance = await _terminalService.GetTerminalBalance(Bill.TerminalId);
                    //获取欠款
                    ((ICommand)Load)?.Execute(null);
                }

                //预览单据
                CashReceiptBillModel bill = null;

                if (parameters.ContainsKey("Bill"))
                {
                    parameters.TryGetValue("Bill", out bill);
                    parameters.TryGetValue("IsSubmitBill", out bool isSubmitBill);
                    this.Bill.IsSubmitBill = isSubmitBill;
                }

                if (parameters.ContainsKey("BillId"))
                {
                    parameters.TryGetValue("BillId", out int billId);
                    this.BillId = billId;
                    bill = await _receiptCashService.GetBillAsync(billId);
                }

                if (bill != null)
                {
                    Bill = bill;
                    Bill.TerminalName = bill.CustomerName;
                    Bill.BusinessUserName = bill.PayeerName;
                    this.PaymentMethods = this.ToPaymentMethod(Bill, bill.CashReceiptBillAccountings);

                    if (!bill.AuditedStatus)
                    {
                        //控制显示菜单
                        _popupMenu?.Show(34);
                    }
                }

                ((ICommand)Load)?.Execute(null);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 更新计算
        /// </summary>
        /// <param name="billId"></param>
        public void UpdateUI(int billId)
        {
            try
            {
                if (billId > 0)
                {
                    //计算当前尚欠金额
                    var item = Bill.Items?.Where(s => s.BillId == billId).FirstOrDefault();
                    if (item != null)
                        item.AmountOwedAfterReceipt = item?.ArrearsAmount - (item?.ReceivableAmountOnce ?? 0) - (item?.DiscountAmountOnce ?? 0);
                }
                else if (-1 == billId && null != Bill && null != Bill.Items)
                {
                    foreach (var item in Bill.Items)
                    {
                        item.AmountOwedAfterReceipt = item?.ArrearsAmount - (item?.ReceivableAmountOnce ?? 0) - (item?.DiscountAmountOnce ?? 0);
                    }
                }

                //本次单据应收合计
                Bill.TotalArrearsAmountOnce = Bill.Items?.Select(c => c.ArrearsAmount).Sum();
                //本次单据尚欠合计
                Bill.TotalAmountOwedAfterReceipt = Bill.Items?.Select(c => c.AmountOwedAfterReceipt).Sum();

                //本次单据优惠合计
                if (this.PaymentMethods.PreferentialAmount != Bill.Items?.Select(c => c.DiscountAmountOnce).Sum())
                    Bill.TotalDiscountAmountOnce = Bill.Items?.Select(c => c.DiscountAmountOnce).Sum();
                else
                    Bill.TotalDiscountAmountOnce = this.PaymentMethods.PreferentialAmount;


                //本次单据收款合计
                if (this.PaymentMethods.CurrentCollectionAmount != Bill.Items?.Select(c => c.ReceivableAmountOnce).Sum())
                    Bill.TotalReceivableAmountOnce = Bill.Items?.Select(c => c.ReceivableAmountOnce).Sum();
                else
                    Bill.TotalReceivableAmountOnce = this.PaymentMethods.CurrentCollectionAmount;


                //单据收款 ReceivableAmount
                Bill.ReceivableAmount = Bill.TotalArrearsAmountOnce ?? 0;
                //单据优惠 PreferentialAmount
                Bill.PreferentialAmount = Bill.TotalDiscountAmountOnce ?? 0;
                //单据欠款 OweCash
                Bill.OweCash = Bill.ReceivableAmount - Bill.PreferentialAmount - Bill.CashReceiptBillAccountings.Sum(s => s.CollectionAmount);

                //更新收款方式
                this.PaymentMethods.SubAmount = Bill.TotalArrearsAmountOnce ?? 0;
                this.PaymentMethods.PreferentialAmount = Bill.TotalDiscountAmountOnce ?? 0;
                this.PaymentMethods.OweCash = Bill.TotalAmountOwedAfterReceipt ?? 0;

                //保证收款合计等于总的本次收款
                var first = PaymentMethods.Selectes.FirstOrDefault();
                if (first != null)
                {
                    first.CollectionAmount = Bill.TotalReceivableAmountOnce ?? 0;
                }

                //更新收款账户
                Bill.Accountings = PaymentMethods.Selectes.Select(a => { return $"{a.Name}：{string.Format("{0:F2}", a.CollectionAmount)}"; }).ToArray();
                var accountings = PaymentMethods.Selectes.Select(a =>
                {
                    return new AccountMaping()
                    {
                        Name = a.Name,
                        AccountingOptionId = a.AccountingOptionId,
                        CollectionAmount = a.CollectionAmount,
                        AccountCodeTypeId = a.AccountCodeTypeId,
                        Number = a.Number
                    };
                });
                Bill.CashReceiptBillAccountings = new ObservableCollection<AccountMaping>(accountings);

                //红冲审核水印
                if (this.Bill.Id > 0 && this.Bill.AuditedStatus)
                    this.Bill.AuditedStatus = !this.Bill.ReversedStatus;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }
        private void InitBill()
        {
            this.BillType = BillTypeEnum.CashReceiptBill;
            this.Bill = new CashReceiptBillModel()
            {
                BillTypeId = (int)this.BillType,
                MakeUserId = Settings.UserId,
                MakeUserName = Settings.UserRealName,
                CreatedOnUtc = DateTime.Now,
                BillNumber = CommonHelper.GetBillNumber(CommonHelper.GetEnumDescription(BillType).Split(',')[1], Settings.StoreId)
            };

            if (this.Bill.Id == 0)
            {
                this.Bill.BusinessUserId = Settings.UserId;
                this.Bill.BusinessUserName = Settings.UserRealName;
            }

        }
        private void DoClear()
        {
            Filter.StartTime = null;
            Filter.EndTime = null;
            InitBill();
        }
        public override void OnAppearing()
        {
            base.OnAppearing();

            //控制显示菜单
            _popupMenu?.Show(3, 4, 5);
        }
    }
}
