using Acr.UserDialogs;
using DCMS.Client.CustomViews;
using DCMS.Client.Enums;
using DCMS.Client.Models;
using DCMS.Client.Models.Finances;
using DCMS.Client.Pages;
using DCMS.Client.Services;
using DCMS.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace DCMS.Client.ViewModels
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
            IDialogService dialogService
            ,
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
            var valid_IsReversed = this.ValidationRule(x => x.Bill.ReversedStatus, _isBool, "已红冲单据不能操作");
            var valid_IsAudited = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");
            var valid_TerminalId = this.ValidationRule(x => x.Bill.TerminalId, _isZero, "客户未指定");
            var valid_BusinessUserId = this.ValidationRule(x => x.Bill.AdvanceAmount, _isDZero, "预收金额未指定");
            var valid_SelectesCount = this.ValidationRule(x => x.PaymentMethods.Selectes.Count, _isZero, "请选择支付方式");

            //提交单据
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                if (this.Bill.ReversedStatus)
                {
                    _dialogService.ShortAlert("已红冲单据不能操作");
                    return Unit.Default;
                }

                return await this.Access(AccessGranularityEnum.ReceivablesBillsSave, async () =>
                {
                    var postData = new AdvanceReceiptUpdateModel()
                    {
                        BillNumber = this.Bill.BillNumber,
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
                    }, token: new System.Threading.CancellationToken());
                });

            },
            this.IsValid());

            //存储记录
            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                var c1 = this.Bill.TerminalId != 0 && this.Bill.TerminalId != (Settings.AdvanceReceiptBill?.TerminalId ?? 0);
                var c2 = this.Bill.AdvanceAmount != 0 && this.Bill.AdvanceAmount != (Settings.AdvanceReceiptBill?.AdvanceAmount ?? 0);
                if ((c1 || c2))
                {
                    if (!this.Bill.AuditedStatus && !this.Bill.IsSubmitBill)
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
                    //红冲审核水印
                    this.Bill.AuditedStatus = true;

                    var _conn = App.Resolve<ILiteDbService<MessageInfo>>();
                    var ms = await _conn.Table.FindByIdAsync(SelecterMessage.Id);
                    if (ms != null)
                    {
                        ms.IsRead = true;
                        await _conn.UpsertAsync(ms);
                    }
                });
            }, this.WhenAny(x => x.Bill.Id, (x) => x.GetValue() > 0));

            //绑定页面菜单
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                //清空单据
                { MenuEnum.CLEAR,(m,vm)=>
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

                } },
                //打印
                {
                    MenuEnum.PRINT,async (m,vm)=>{

                         Bill.BillType = BillTypeEnum.AdvanceReceiptBill;
                         await SelectPrint(Bill);

                } },
                //审核
                { MenuEnum.SHENGHE,async (m,vm)=>{

                    ResultData result = null;
                         using (UserDialogs.Instance.Loading("审核中..."))
                         {
                             result = await _advanceReceiptService.AuditingAsync(Bill.Id);
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
                { MenuEnum.HONGCHOU,async (m,vm) => {

                    var remark = await CrossDiaglogKit.Current.GetInputTextAsync("红冲备注", "",Keyboard.Text);
                    if (!string.IsNullOrEmpty(remark))
                    {
                        bool result = false;
                        using (UserDialogs.Instance.Loading("红冲中..."))
                        {
                             //红冲审核水印
                            this.EnableOperation = true;
                            this.Bill.ReversedStatus = true;
                            result = await _advanceReceiptService.ReverseAsync(Bill.Id,remark);
                        }
                        if (result)
                        {
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
                    await _advanceReceiptService.ReverseAsync(Bill.Id);
                } }
            });
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
                    parameters.TryGetValue("IsSubmitBill", out bool isSubmitBill);
                    this.Bill.IsSubmitBill = isSubmitBill;
                }

                if (parameters.ContainsKey("BillId"))
                {
                    parameters.TryGetValue("BillId", out int billId);
                    this.BillId = billId;
                    bill = await _advanceReceiptService.GetBillAsync(billId, calToken: new System.Threading.CancellationToken());
                }

                if (bill != null)
                {
                    Bill = bill;
                    Bill.TerminalId = bill.TerminalId;
                    Bill.TerminalName = bill.CustomerName;

                    this.PaymentMethods = this.ToPaymentMethod(Bill, bill.Items);

                    //控制显示菜单
                    _popupMenu?.Show(34);
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


                //红冲审核水印
                if (this.Bill.Id > 0 && this.Bill.AuditedStatus)
                    this.Bill.AuditedStatus = !this.Bill.ReversedStatus;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            //控制显示菜单
            _popupMenu?.Show(4, 5);
        }
    }
}
