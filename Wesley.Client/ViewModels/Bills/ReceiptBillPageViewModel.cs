using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Finances;
using Wesley.Client.Models.Sales;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using Shiny;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

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


            IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "收款单";

            _receiptCashService = receiptCashService;

            InitBill();
            //验证
            var valid_TerminalId = this.ValidationRule(x => x.Bill.TerminalId, _isZero, "客户未指定");
            var valid_BusinessUserId = this.ValidationRule(x => x.Bill.BusinessUserId, _isZero, "业务员未指定");
            var valid_ProductCount = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "无收款单据信息");
            var valid_SelectesCount = this.ValidationRule(x => x.Bill.CashReceiptBillAccountings.Count, _isZero, "请选择支付方式");
            var valid_IsVieweBill = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");

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
            }).DisposeWith(this.DeactivateWith);

            //绑定数据
            this.Load = BillItemsLoader.Load(async () =>
            {
                if (Bill.Id == 0)
                {
                    var results = await Sync.Run(() =>
                     {
                         var inits = _receiptCashService.GetInitDataAsync(calToken: cts.Token).Result;

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
                             var result = _receiptCashService.GetOwecashBillsAsync(Settings.UserId, terminalId, null, billNumber, remark, startTime, endTime, pageIndex, pageSize, calToken: cts.Token).Result;

                             if (result != null)
                             {

                                 foreach (var bill in result?.Where(s => s?.BillTypeId != (int)BillTypeEnum.FinancialIncomeBill).ToList())
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

                         return pending;

                     }, (ex) => { Crashes.TrackError(ex); });
                    Bill.Items = new ObservableRangeCollection<CashReceiptItemModel>(results);
                    return results;
                }
                return Bill.Items;
            });


            //提交单据
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                await this.Access(AccessGranularityEnum.ReceiptBillsSave);

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

                var postData = new CashReceiptUpdateModel()
                {
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
                }, token: cts.Token);
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
                    var db = Shiny.ShinyHost.Resolve<LocalDatabase>();
                    await db.SetPending(SelecterMessage.Id, true);
                }, token: cts.Token);
            }, this.WhenAny(x => x.Bill.Id, (x) => x.GetValue() > 0));

            //菜单选择
            this.SetMenus(async (x) =>
            {
                switch (x)
                {
                    case Enums.MenuEnum.REMARK: //整单备注
                        AllRemak((result) => { Bill.Remark = result; }, Bill.Remark);
                        break;
                    case Enums.MenuEnum.CLEAR://清空单据
                        {
                            ClearBill<SaleBillModel, SaleItemModel>(null, DoClear);
                        }
                        break;
                    case Enums.MenuEnum.PRINT: //打印
                        {
                            if (!valid_ProductCount.IsValid) { this.Alert(valid_IsVieweBill.Message[0]); return; }
                            await SelectPrint(this.Bill);
                        }
                        break;
                }
            }, 3, 4, 5);


            this.DeliverSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.MorePaymentCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.TextChangedCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
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

                        //获取当前经销商欠终端款总金额(费用支出，退货：欠终端)
                        decimal tmpCurrentCollectionAmount = paymentMethod.CurrentCollectionAmount;

                        if ((Bill.TotalArrearsAmountOnce ?? 0) > 0)
                        {
                            decimal owedAmountTotal = tmpCurrentCollectionAmount;
                            foreach (var b in Bill.Items)
                            {
                                if ((b.AmountReceivable ?? 0) < 0)
                                {
                                    b.ReceivableAmountOnce = b.AmountReceivable;
                                    //尚欠金额 = 0
                                    b.AmountOwedAfterReceipt = 0;
                                    owedAmountTotal += Math.Abs(b.AmountReceivable ?? 0);
                                }
                            }

                            foreach (var b in Bill.Items)
                            {
                                switch (b.BillTypeId)
                                {
                                    //预收款
                                    case (int)BillTypeEnum.AdvanceReceiptBill:
                                    //其它收入
                                    case (int)BillTypeEnum.FinancialIncomeBill:
                                    //销售单
                                    case (int)BillTypeEnum.SaleBill:
                                        {
                                            if (owedAmountTotal > 0 && owedAmountTotal >= b.AmountReceivable)
                                            {
                                                //本次收款 = 应收金额/尚欠金额
                                                b.ReceivableAmountOnce = b.AmountReceivable;
                                                owedAmountTotal = owedAmountTotal - b.AmountReceivable ?? 0;
                                            }
                                            else if (owedAmountTotal < b.AmountReceivable)
                                            {
                                                b.ReceivableAmountOnce = owedAmountTotal;
                                                owedAmountTotal -= owedAmountTotal;
                                            }
                                        }
                                        break;
                                }
                            }

                        }
                        else
                        {
                            decimal owedAmountTotal = tmpCurrentCollectionAmount;
                            foreach (var b in Bill.Items)
                            {
                                if ((b.AmountReceivable ?? 0) < 0)
                                {
                                    owedAmountTotal += Math.Abs(b.AmountReceivable ?? 0);
                                }
                            }

                            foreach (var b in Bill.Items)
                            {
                                switch (b.BillTypeId)
                                {
                                    //预收款
                                    case (int)BillTypeEnum.AdvanceReceiptBill:
                                    //其它收入
                                    case (int)BillTypeEnum.FinancialIncomeBill:
                                    //销售单
                                    case (int)BillTypeEnum.SaleBill:
                                        {
                                            if (owedAmountTotal > 0 && owedAmountTotal >= b.AmountReceivable)
                                            {
                                                //本次收款 = 应收金额/尚欠金额
                                                b.ReceivableAmountOnce = b.AmountReceivable;
                                                owedAmountTotal = owedAmountTotal - b.AmountReceivable ?? 0;
                                            }
                                            else if (owedAmountTotal < b.AmountReceivable)
                                            {
                                                b.ReceivableAmountOnce = owedAmountTotal;
                                            }
                                        }
                                        break;
                                }
                            }

                            foreach (var b in Bill.Items)
                            {
                                switch (b.BillTypeId)
                                {
                                    //退货单
                                    case (int)BillTypeEnum.ReturnBill:
                                    //费用支出单
                                    case (int)BillTypeEnum.CostExpenditureBill:
                                        {
                                            b.ReceivableAmountOnce = b.AmountReceivable + owedAmountTotal;
                                        }
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
                                    Number = a.Number
                                };
                            });
                            //收款账户映射
                            Bill.CashReceiptBillAccountings = new ObservableCollection<AccountMaping>(accountings);
                        }
                    }
                    #endregion 

                    UpdateUI(0);
                }

                //选择客户
                if (parameters.ContainsKey("Terminaler"))
                {
                    parameters.TryGetValue<TerminalModel>("Terminaler", out TerminalModel terminaler);
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

                    if (bill.AuditedStatus)
                    {
                    }
                    else
                    {
                        this.SetMenus((x) =>
                        {
                            switch (x)
                            {
                                case Enums.MenuEnum.SHENGHE://审核
                                    ((ICommand)AuditingDataCommand)?.Execute(null);
                                    break;
                            }
                        }, 34);
                    }
                       ((ICommand)Load)?.Execute(null);
                }
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
                    var item = Bill.Items.Where(s => s.BillId == billId).FirstOrDefault();
                    if (item != null)
                        item.AmountOwedAfterReceipt = item?.ArrearsAmount - (item?.ReceivableAmountOnce ?? 0) - (item?.DiscountAmountOnce ?? 0);
                }

                //本次单据应收合计
                Bill.TotalArrearsAmountOnce = Bill.Items.Select(c => c.ArrearsAmount).Sum();
                //本次单据尚欠合计
                Bill.TotalAmountOwedAfterReceipt = Bill.Items.Select(c => c.AmountOwedAfterReceipt).Sum();
                //本次单据优惠合计
                Bill.TotalDiscountAmountOnce = Bill.Items.Select(c => c.DiscountAmountOnce).Sum();


                //本次单据收款合计
                Bill.TotalReceivableAmountOnce = this.PaymentMethods.CurrentCollectionAmount;//  Bill.Items.Select(c => c.ReceivableAmountOnce).Sum();

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
        }
    }
}
