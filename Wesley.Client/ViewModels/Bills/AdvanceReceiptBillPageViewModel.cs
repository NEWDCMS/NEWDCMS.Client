using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Finances;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class AdvanceReceiptBillPageViewModel : ViewModelBaseCutom<AdvanceReceiptBillModel>
    {
        private readonly IAdvanceReceiptService _advanceReceiptService;

        public ReactiveCommand<object, Unit> AccountingSelected { get; }
        public ReactiveCommand<object, Unit> MorePaymentCommand { get; }
        public IReactiveCommand TextChangedCommand { get; set; }


        public AdvanceReceiptBillPageViewModel(INavigationService navigationService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IDialogService dialogService,
            IAdvanceReceiptService advanceReceiptService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "预收款单据";

            _advanceReceiptService = advanceReceiptService;

            InitBill();

            //选择预收款科目
            this.AccountingSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectAccounting((data) =>
                 {
                     Bill.AccountingOptionId = data.AccountingOptionId;
                     Bill.AccountingOptionName = data.Name;
                 }, (int)this.BillType, true);
            });

            //选择支付方式
            this.MorePaymentCommand = ReactiveCommand.Create<object>(e =>
           {
               SelectPaymentMethods(("PaymentMethods", PaymentMethods), ("BillType", this.BillType), ("Reference", PageName));
           });

            //验证
            var valid_TerminalId = this.ValidationRule(x => x.Bill.TerminalId, _isZero, "客户未指定");
            var valid_BusinessUserId = this.ValidationRule(x => x.Bill.AdvanceAmount, _isDZero, "预收金额未指定");
            var valid_SelectesCount = this.ValidationRule(x => x.PaymentMethods.Selectes.Count, _isZero, "请选择支付方式");
            var valid_IsVieweBill = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");

            //提交单据
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                await this.Access(AccessGranularityEnum.ReceivablesBillsSave);

                var postData = new AdvanceReceiptUpdateModel()
                {
                    CustomerId = Bill.TerminalId,
                    Payeer = Settings.UserId,
                    AdvanceAmount = Bill.AdvanceAmount,
                    DiscountAmount = Bill.DiscountAmount,
                    OweCash = Bill.OweCash,
                    Remark = Bill.Remark,
                    AccountingOptionId = Bill.AccountingOptionId ?? 0,
                    Accounting = PaymentMethods.Selectes.Select(a =>
                    {
                        return new AccountMaping()
                        {
                            AccountingOptionId = a.AccountingOptionId,
                            CollectionAmount = a.CollectionAmount,
                            Name = a.Name,
                            BillId = 0,
                        };
                    }).ToList(),
                };

                return await SubmitAsync(postData, Bill.Id, _advanceReceiptService.CreateOrUpdateAsync, (result) =>
                {
                    Bill = new AdvanceReceiptBillModel();
                }, token: cts.Token);
            },
            this.IsValid());

            //存储记录
            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                var c1 = this.Bill.TerminalId != 0 && this.Bill.TerminalId != (Settings.AdvanceReceiptBill?.TerminalId ?? 0);
                var c2 = this.Bill.AdvanceAmount != 0 && this.Bill.AdvanceAmount != (Settings.AdvanceReceiptBill?.AdvanceAmount ?? 0);
                if ((c1 || c2))
                {
                    if (!this.Bill.AuditedStatus)
                    {
                        var ok = await _dialogService.ShowConfirmAsync("你是否要保存单据？", "提示", "确定", "取消");
                        if (ok)
                        {
                            Settings.AdvanceReceiptBill = this.Bill;
                        }
                        else
                        {
                            await _navigationService.GoBackAsync();
                        }
                    }
                }
                else
                {
                    await _navigationService.GoBackAsync();
                }
            });


            //更改输入
            this.TextChangedCommand = ReactiveCommand.Create<object>(e =>
            {
                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(x =>
                {
                    UpdateUI();
                });
            });

            //审核
            this.AuditingDataCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                //是否具有审核权限
                await this.Access(AccessGranularityEnum.ReceivablesBillsApproved);
                await SubmitAsync(Bill.Id, _advanceReceiptService.AuditingAsync, async (result) =>
                {
                    var db = Shiny.ShinyHost.Resolve<LocalDatabase>();
                    await db.SetPending(SelecterMessage.Id, true);
                });
            }, this.WhenAny(x => x.Bill.Id, (x) => x.GetValue() > 0));


            //菜单选择
            this.SetMenus(async (x) =>
            {
                switch (x)
                {
                    case Enums.MenuEnum.CLEAR://清空单据
                        {
                            ClearForm(() =>
                            {
                                Bill = new AdvanceReceiptBillModel()
                                {
                                    Payeer = Settings.UserId,
                                    CreatedOnUtc = DateTime.Now,
                                    BillNumber = CommonHelper.GetBillNumber(
                                      CommonHelper.GetEnumDescription(
                                          BillTypeEnum.AdvanceReceiptBill).Split(',')[1], Settings.StoreId)
                                };
                            });
                        }
                        break;
                    case Enums.MenuEnum.PRINT://打印
                        {
                            await SelectPrint(this.Bill);
                        }
                        break;
                }
            }, 4, 5);

            this.TextChangedCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.AccountingSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.MorePaymentCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.ExceptionsSubscribe();
        }


        private void InitBill(bool clear = false)
        {
            this.BillType = BillTypeEnum.AdvanceReceiptBill;
            var bill = Settings.AdvanceReceiptBill;
            if (bill != null && !clear)
            {
                this.Bill = bill;
            }
            else
            {
                this.Bill = new AdvanceReceiptBillModel()
                {
                    BillTypeId = (int)this.BillType,
                    Payeer = Settings.UserId,
                    CreatedOnUtc = DateTime.Now,
                    BillNumber = CommonHelper.GetBillNumber(CommonHelper.GetEnumDescription(BillType).Split(',')[1], Settings.StoreId)
                };
            }
        }

        //private void DoClear()
        //{
        //    InitBill(true);
        //}

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                //选择客户
                if (parameters.ContainsKey("Terminaler"))
                {
                    Bill.TerminalId = this.Terminal.Id;
                    Bill.TerminalName = this.Terminal.Name;
                }

                //支付方式更新
                if (parameters.ContainsKey("PaymentMethods"))
                {
                    parameters.TryGetValue("PaymentMethods", out PaymentMethodBaseModel paymentMethod);

                    if (paymentMethod != null)
                    {
                        this.PaymentMethods = paymentMethod;
                        UpdateUI();
                    }
                }

                //单据预览
                AdvanceReceiptBillModel bill = null;
                if (parameters.ContainsKey("Bill"))
                {
                    parameters.TryGetValue("Bill", out bill);
                }

                if (parameters.ContainsKey("BillId"))
                {
                    parameters.TryGetValue("BillId", out int billId);
                    this.BillId = billId;
                    bill = await _advanceReceiptService.GetBillAsync(billId, calToken: cts.Token);
                }

                if (bill != null)
                {
                    Bill = bill;
                    Bill.TerminalId = bill.TerminalId;
                    Bill.TerminalName = bill.CustomerName;

                    this.PaymentMethods = this.ToPaymentMethod(Bill, bill.Items);

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
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void UpdateUI()
        {
            try
            {
                //合计
                this.Bill.SumAmount = decimal.Round(Bill.AdvanceAmount ?? 0, 2);

                //惠
                this.Bill.DiscountAmount = PaymentMethods.PreferentialAmount;

                //支付
                var totalAccountingAmount = PaymentMethods.Selectes.Sum(s => s.CollectionAmount);
                this.Bill.Accountings = PaymentMethods.Selectes.Select(a => { return $"{a.Name}: {string.Format("{0:F2}", a.CollectionAmount)}"; }).ToArray();

                //(欠款)
                this.Bill.OweCash = (this.Bill.SumAmount - (this.Bill.DiscountAmount ?? 0) - totalAccountingAmount);

                //用于传递
                this.PaymentMethods.SubAmount = this.Bill.SumAmount;
                this.PaymentMethods.OweCash = this.Bill.OweCash;
                this.PaymentMethods.PreferentialAmount = this.Bill.DiscountAmount ?? 0;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

    }
}
