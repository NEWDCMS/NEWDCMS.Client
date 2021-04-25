using Wesley.Client.CustomViews;
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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{
    public class CostExpenditureBillPageViewModel : ViewModelBaseCutom<CostExpenditureBillModel>
    {
        private readonly ICostExpenditureService _costExpenditureService;
        private readonly ISaleBillService _saleBillService;

        public ReactiveCommand<object, Unit> AddCostCommand { get; }
        public ReactiveCommand<object, Unit> MorePaymentCommand { get; }
        [Reactive] public bool ShowSignInCommand { get; set; } = false;
        [Reactive] public CostExpenditureItemModel Selecter { get; set; }
        public ReactiveCommand<object, Unit> RefusedCommand { get; }
        public ReactiveCommand<object, Unit> ConfirmCommand { get; }


        public CostExpenditureBillPageViewModel(INavigationService navigationService,
                  IProductService productService,
                  IUserService userService,
                  ITerminalService terminalService,
                  IWareHousesService wareHousesService,
                  IAccountingService accountingService,
                  ICostExpenditureService costExpenditureService,
                  ISaleBillService saleBillService,
                  IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "费用支出";

            _costExpenditureService = costExpenditureService;
            _saleBillService = saleBillService;

            InitBill();
            //验证
            var valid_TerminalId = this.ValidationRule(x => x.Bill.TerminalId, _isZero, "客户未指定");
            var valid_ItemCount = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "请添加费用项目");
            var valid_SelectesCount = this.ValidationRule(x => x.PaymentMethods.Selectes.Count, _isZero, "请选择支付方式");
            var valid_IsVieweBill = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");

            //提交单据
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                await this.Access(AccessGranularityEnum.ExpenseExpenditureSave);
                var postData = new CostExpenditureUpdateModel()
                {
                    EmployeeId = Settings.UserId,
                    OweCash = Bill.OweCash,
                    Remark = Bill.Remark,
                    Items = Bill.Items,
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
                return await SubmitAsync(postData, 0, _costExpenditureService.CreateOrUpdateAsync, (result) =>
                {
                    Bill = new CostExpenditureBillModel();
                }, token: cts.Token);
            },
            this.IsValid());

            //存储记录
            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                if (!this.Bill.AuditedStatus && this.Bill.TerminalId != 0 && this.Bill.TerminalId != Settings.CostExpenditureBill?.TerminalId)
                {
                    var ok = await _dialogService.ShowConfirmAsync("你是否要保存单据？", "提示", "确定", "取消");
                    if (ok)
                    {
                        if (!this.Bill.AuditedStatus)
                            Settings.CostExpenditureBill = this.Bill;
                    }
                }
                await _navigationService.GoBackAsync();
            });

            //编辑费用
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
             .Skip(1)
             .Where(x => x != null)
             .SubOnMainThread(async x =>
            {
                await this.NavigateAsync("AddCostPage", ("Terminaler", this.Terminal), ("Selecter", x));
                this.Selecter = null;
            });

            //添加费用
            this.AddCostCommand = ReactiveCommand.Create<object>(async e =>
           {
               if (!valid_TerminalId.IsValid) { _dialogService.ShortAlert(valid_TerminalId.Message[0]); return; }
               if (this.Bill.AuditedStatus) { _dialogService.ShortAlert("已审核单据不能操作！"); return; }

               await this.NavigateAsync("AddCostPage", ("Terminaler", this.Terminal));
           });

            //更多支付方式
            this.MorePaymentCommand = ReactiveCommand.Create<object>(async e =>
           {
               if (!valid_TerminalId.IsValid) { _dialogService.ShortAlert("客户未指定！"); return; }
               this.PaymentMethods.PreferentialAmountShowFiled = false;
               await this.NavigateAsync("PaymentMethodPage", ("PaymentMethods", this.PaymentMethods), ("BillType", this.BillType));
           });

            //审核
            this.AuditingDataCommand = ReactiveCommand.Create<object>(async _ =>
            {
                //是否具有审核权限
                await this.Access(AccessGranularityEnum.ExpenseExpenditureApproved);
                await SubmitAsync(Bill.Id, _costExpenditureService.AuditingAsync, async (result) =>
                {
                    var db = Shiny.ShinyHost.Resolve<LocalDatabase>();
                    await db.SetPending(SelecterMessage.Id, true);
                });
            }, this.WhenAny(x => x.Bill.Id, (x) => x.GetValue() > 0));


            //拒签
            this.RefusedCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                try
                {
                    var ok = await _dialogService.ShowConfirmAsync("你确定要放弃签收吗？", "", "确定", "取消");
                    if (ok)
                    {
                        var postMData = new DeliverySignUpdateModel()
                        {
                            //不关联调拨单
                            DispatchItemId = 0,
                            DispatchBillId = 0,
                            StoreId = Settings.StoreId,
                            BillId = Bill.Id,
                            BillTypeId = (int)BillTypeEnum.CostExpenditureBill,
                            //
                            Latitude = GlobalSettings.Latitude,
                            Longitude = GlobalSettings.Longitude,
                            Signature = ""
                        };
                        await SubmitAsync(postMData, postMData.Id, _saleBillService.RefusedConfirmAsync, async (result) =>
                        {
                            await _navigationService.GoBackAsync();
                        });
                    }
                }
                catch (Exception)
                {
                    await _dialogService.ShowAlertAsync("拒签失败，服务器请求错误！", "", "取消");
                }
            });

            //签收
            this.ConfirmCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                try
                {
                    //如果签收距离>50 则计数
                    if ((Terminal.Distance ?? 0) > 50)
                    {
                        var tmp = Settings.Abnormal;
                        if (tmp != null)
                        {
                            tmp.Id = Terminal.Id;
                            tmp.Counter += 1;
                            Settings.Abnormal = tmp;
                        }
                        else
                        {
                            Settings.Abnormal = new AbnormalNum { Id = Terminal.Id, Counter = 1 };
                        }
                    }

                    var signature = await CrossDiaglogKit.Current.GetSignaturePadAsync("手写签名", "", Keyboard.Default, defaultValue: "", placeHolder: "请手写签名...");
                    if (!string.IsNullOrEmpty(signature))
                    {
                        var postMData = new DeliverySignUpdateModel()
                        {
                            //不关联调拨单
                            DispatchItemId = 0,
                            DispatchBillId = 0,
                            StoreId = Settings.StoreId,
                            BillId = Bill.Id,
                            BillTypeId = (int)BillTypeEnum.CostExpenditureBill,
                            //
                            Latitude = GlobalSettings.Latitude,
                            Longitude = GlobalSettings.Longitude,
                            Signature = signature
                        };

                        //如果终端异常签收大于5次，则拍照
                        if (Settings.Abnormal.Counter > 5)
                        {
                            await TakePhotograph((u, m) =>
                            {
                                var photo = new RetainPhoto
                                {
                                    DisplayPath = $"{GlobalSettings.FileCenterEndpoint}HRXHJS/document/image/" + u.Id + ""
                                };
                                postMData.RetainPhotos.Add(photo);
                            });
                        }

                        if (Settings.Abnormal.Counter > 5 && postMData.RetainPhotos.Count == 0)
                        {
                            await _dialogService.ShowAlertAsync("拒绝操作，请留存拍照！", "", "取消");
                            return;
                        }

                        await SubmitAsync(postMData, postMData.Id, _saleBillService.DeliverySignConfirmAsync, async (result) =>
                        {
                            await _navigationService.GoBackAsync();
                        });
                    }
                }
                catch (Exception)
                {
                    await _dialogService.ShowAlertAsync("签收失败，服务器请求错误！", "", "取消");
                }
            });

            //菜单选择
            this.SetMenus((x) =>
            {
                switch (x)
                {
                    case Enums.MenuEnum.REMARK: //整单备注
                        AllRemak((result) => { Bill.Remark = result; }, Bill.Remark);
                        break;
                    case Enums.MenuEnum.CLEAR: //清空单据
                        {
                            ClearBill<CostContractBillModel, CostContractItemModel>(null, DoClear);
                        }
                        break;

                }
            }, 3, 4);

            this.AddCostCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.MorePaymentCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.ExceptionsSubscribe();
        }

        private void InitBill(bool clear = false)
        {
            this.BillType = BillTypeEnum.CostExpenditureBill;
            var bill = Settings.CostExpenditureBill;
            if (bill != null && !clear)
            {
                this.Bill = bill;
                this.Bill.Items = new ObservableCollection<CostExpenditureItemModel>(bill?.Items);
                var defaultAccs = bill.CostExpenditureBillAccountings.Select(
                        s => new AccountingModel()
                        {
                            AccountCodeTypeId = s.AccountCodeTypeId,
                            Default = true,
                            Name = s.Name,
                            AccountingOptionId = s.AccountingOptionId,
                            AccountingOptionName = s.AccountingOptionName
                        }).ToList();
                this.PaymentMethods.Selectes = new ObservableCollection<AccountingModel>(defaultAccs);
                this.Selecter = bill?.Items?.FirstOrDefault();
            }
            else
            {
                this.Bill = new CostExpenditureBillModel()
                {
                    BillTypeId = (int)this.BillType,
                    MakeUserId = Settings.UserId,
                    MakeUserName = Settings.UserRealName,
                    CreatedOnUtc = DateTime.Now,
                    BillNumber = CommonHelper.GetBillNumber(CommonHelper.GetEnumDescription(BillType).Split(',')[1], Settings.StoreId)
                };
            }
        }
        private void DoClear()
        {
            InitBill(true);
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            try
            {
                //选择客户回传
                if (parameters.ContainsKey("Terminaler"))
                {
                    parameters.TryGetValue("Terminaler", out TerminalModel terminaler);
                    this.Terminal = terminaler;
                    Bill.TerminalId = terminaler != null ? terminaler.Id : 0;
                    Bill.TerminalName = terminaler != null ? terminaler.Name : "";
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

                //选择费用回传
                if (parameters.ContainsKey("CostExpenditure"))
                {
                    parameters.TryGetValue("CostExpenditure", out CostExpenditureItemModel ce);
                    if (ce != null)
                    {
                        if (ce.CostContractId > 0)
                        {
                            if (Bill.Items.Where(s => s.AccountingOptionId == ce.AccountingOptionId && s.CostContractId == ce.CostContractId).Count() > 0)
                            {
                                //_dialogService.ShortAlert("项目已经添加！"); return;
                                var item = Bill.Items.Where(s => s.AccountingOptionId == ce.AccountingOptionId && s.CostContractId == ce.CostContractId).FirstOrDefault();
                                item.CostContractId = ce.CostContractId;
                                item.AccountingOptionId = ce.AccountingOptionId;
                                item.AccountingOptionName = ce.AccountingOptionName;
                                item.Amount = ce.Amount;
                                return;
                            }
                        }
                        else
                        {
                            if (Bill.Items.Where(s => s.AccountingOptionId == ce.AccountingOptionId).Count() > 0)
                            {
                                //_dialogService.ShortAlert("项目已经添加！"); return;
                                var item = Bill.Items.Where(s => s.AccountingOptionId == ce.AccountingOptionId).FirstOrDefault();
                                item.CostContractId = ce.CostContractId;
                                item.AccountingOptionId = ce.AccountingOptionId;
                                item.AccountingOptionName = ce.AccountingOptionName;
                                item.Amount = ce.Amount;
                                return;
                            }
                        }

                        Bill.Items.Add(ce);
                        Bill.TotalAmount = Bill.Items.Select(s => s.Amount).Sum() ?? 0;
                        UpdateUI();
                    }
                }

                //移除回传
                if (parameters.ContainsKey("RemoveCostExpenditure"))
                {
                    parameters.TryGetValue("RemoveCostExpenditure", out CostExpenditureItemModel ce);
                    if (ce != null)
                    {
                        var cur = Bill.Items.Where(s => s.AccountingOptionId == ce.AccountingOptionId).FirstOrDefault();
                        if (ce.CostContractId > 0)
                        {
                            cur = Bill.Items.Where(s => s.AccountingOptionId == ce.AccountingOptionId && s.CostContractId == ce.CostContractId).FirstOrDefault();
                            if (cur != null)
                            {
                                Bill.Items.Remove(cur);
                            }
                        }
                        else
                        {
                            if (cur != null)
                            {
                                Bill.Items.Remove(cur);
                            }
                        }

                        Bill.TotalAmount = Bill.Items.Select(s => s.Amount).Sum() ?? 0;
                        UpdateUI();
                    }
                }

                CostExpenditureBillModel bill = null;
                if (parameters.ContainsKey("Bill"))
                {
                    parameters.TryGetValue("Bill", out bill);
                }

                if (parameters.ContainsKey("BillId"))
                {
                    parameters.TryGetValue("BillId", out int billId);
                    this.BillId = billId;
                    bill = await _costExpenditureService.GetBillAsync(billId);
                }

                if (bill != null)
                {
                    Bill = bill;

                    Bill.TerminalId = bill.CustomerId;
                    Bill.TerminalName = bill.CustomerName;

                    this.PaymentMethods = this.ToPaymentMethod(bill, Bill.CostExpenditureBillAccountings);

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

                    //来自费用签收单
                    if (ReferencePage.Equals("UnCostExpenditurePage"))
                    {
                        this.ShowAddProduct = false;
                        this.ShowSignInCommand = true;
                    }

                    UpdateUI();
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
                this.Bill.TotalAmount = decimal.Round(Bill.Items.Select(p => p.Amount ?? 0).Sum(), 2);

                //支付
                var totalAccountingAmount = PaymentMethods.Selectes.Sum(s => s.CollectionAmount);
                this.Bill.Accountings = PaymentMethods.Selectes.Select(a => { return $"{a.Name}: {string.Format("{0:F2}", a.CollectionAmount)}"; }).ToArray();

                //(欠款)待支付
                this.Bill.OweCash = this.Bill.TotalAmount - totalAccountingAmount;

                //用于传递
                this.PaymentMethods.SubAmount = this.Bill.TotalAmount;
                this.PaymentMethods.OweCash = this.Bill.OweCash;

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
