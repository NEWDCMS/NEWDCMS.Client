using Acr.UserDialogs;
using DCMS.Client.CustomViews;
using DCMS.Client.Enums;
using DCMS.Client.Models;
using DCMS.Client.Models.Products;
using DCMS.Client.Models.Purchases;
using DCMS.Client.Models.Settings;
using DCMS.Client.Pages;
using DCMS.Client.Services;
using DCMS.Infrastructure.Helpers;
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
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;


namespace DCMS.Client.ViewModels
{
    public class PurchaseOrderBillPageViewModel : ViewModelBaseCutom<PurchaseBillModel>
    {
        private readonly IManufacturerService _manufacturerService;
        private readonly IPurchaseBillService _purchaseBillService;

        private readonly IMicrophoneService _microphoneService;

        [Reactive] public PurchaseItemUpdateModel PostMData { get; set; } = new PurchaseItemUpdateModel();
        [Reactive] public AccountingOption AdvancePayment { get; set; } = new AccountingOption();
        [Reactive] public PurchaseItemModel Selecter { get; set; }


        public PurchaseOrderBillPageViewModel(INavigationService navigationService,
           IProductService productService,
           IUserService userService,
           ITerminalService terminalService,
           IWareHousesService wareHousesService,
           IAccountingService accountingService,
           IPurchaseBillService purchaseBillService,
           IManufacturerService manufacturerService,
           IMicrophoneService microphoneService,
             IDialogService dialogService
            ) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "采购单";

            _purchaseBillService = purchaseBillService;
            _manufacturerService = manufacturerService;

            _microphoneService = microphoneService;


            InitBill();


            //验证
            var valid_IsReversed = this.ValidationRule(x => x.Bill.ReversedStatus, _isBool, "已红冲单据不能操作");
            var valid_IsAudited = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");
            var valid_ManufacturerId = this.ValidationRule(x => x.Bill.ManufacturerId, _isZero, "供应商未指定");
            var valid_WareHouseId = this.ValidationRule(x => x.Bill.WareHouseId, _isZero, "仓库未指定");
            var valid_ProductCount = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "请添加商品项目");
            var valid_SelectesCount = this.ValidationRule(x => x.PaymentMethods.Selectes.Count, _isZero, "请选择支付方式");


            //初始化 
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                var whs = await _wareHousesService.GetWareHousesAsync(this.BillType, force: true);
                if (whs != null && whs.FirstOrDefault() != null)
                {
                    var wh = whs.FirstOrDefault();
                    if (Bill.WareHouseId == 0)
                    {
                        WareHouse = wh;
                        Bill.WareHouseId = wh.Id;
                    }

                    if (string.IsNullOrEmpty(Bill.WareHouseName))
                        Bill.WareHouseName = wh.Name;



                }

                var result = await _purchaseBillService.GetInitDataAsync(calToken: new System.Threading.CancellationToken());
                if (result != null)
                {
                    var defaultAccs = result.PurchaseBillAccountings.Select(s => new AccountingModel()
                    {
                        Default = true,
                        AccountingOptionId = s.AccountingOptionId,
                        AccountingOptionName = s.AccountingOptionName,
                        AccountCodeTypeId = s.AccountCodeTypeId,
                        Name = s.Name
                    }).ToList();

                    PaymentMethods.Selectes = new ObservableCollection<AccountingModel>(defaultAccs);
                }
            }));

            //提交单据
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                //await this.Access(AccessGranularityEnum.PurchaseBillsSave);

                return await this.Access(AccessGranularityEnum.PurchaseBillsSave, async () =>
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

                    var dateTime = UtcHelper.ConvertDateTimeInt(DateTime.Now.ToUniversalTime());

                    if (Bill.BusinessUserId == 0)
                        Bill.BusinessUserId = Settings.UserId;

                    var postMData = new PurchaseItemUpdateModel()
                    {
                        BillNumber = this.Bill.BillNumber,
                        //供应商
                        ManufacturerId = Bill.ManufacturerId,
                        //业务员
                        BusinessUserId = Bill.BusinessUserId,
                        //仓库
                        WareHouseId = Bill.WareHouseId,
                        //交易日期
                        //TransactionDate = DateTime.Now,
                        TransactionDate = dateTime,
                        //按最小单位采购
                        IsMinUnitPurchase = true,
                        //备注
                        Remark = Bill.Remark,
                        //优惠金额
                        PreferentialAmount = Bill.PreferentialAmount,
                        //优惠后金额
                        PreferentialEndAmount = Bill.SumAmount - Bill.PreferentialAmount,
                        //欠款金额
                        OweCash = Bill.OweCash,
                        //商品项目(保存量大于零的商品)
                        Items = Bill.Items?.Where(i => i.Quantity > 0).ToList(),
                        //收款账户
                        Accounting = PaymentMethods.Selectes.Select(a =>
                        {
                            return new AccountMaping()
                            {
                                AccountingOptionId = a.AccountingOptionId,
                                CollectionAmount = a.CollectionAmount,
                                Name = a.Name,
                                BillId = 0,
                            };
                        }).ToList()
                    };

                    return await SubmitAsync(postMData, Bill.Id, _purchaseBillService.CreateOrUpdateAsync, (result) =>
                    {
                        Bill = new PurchaseBillModel();
                    }, token: new System.Threading.CancellationToken());
                });
            },
            this.IsValid());

            //存储记录
            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                var c1 = this.Bill.ManufacturerId != 0 && this.Bill.ManufacturerId != (Settings.PurchaseBill?.ManufacturerId ?? 0);
                var c3 = this.Bill.WareHouseId != 0 && this.Bill.WareHouseId != (Settings.PurchaseBill?.WareHouseId ?? 0);
                var c4 = this.Bill.Items?.Count != (Settings.PurchaseBill?.Items?.Count ?? 0);
                if (!this.Bill.AuditedStatus && (c1 || c3 || c4))
                {
                    if (!this.Bill.AuditedStatus && !this.Bill.IsSubmitBill)
                    {
                        var ok = await _dialogService.ShowConfirmAsync("你是否要保存单据？", "提示", "确定", "取消");
                        if (ok)
                        {
                            Settings.PurchaseBill = this.Bill;
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

            //商品编辑
            this.ItemSelectedCommand = ReactiveCommand.Create<CollectionView>(async e =>
           {

               if (this.Bill.ReversedStatus)
               {
                   _dialogService.ShortAlert("已红冲单据不能操作");
                   return;
               }

               if (this.Bill.AuditedStatus)
               {
                   _dialogService.ShortAlert("已审核单据不能操作");
                   return;
               }

               if (this.Bill.IsSubmitBill)
               {
                   _dialogService.ShortAlert("已提交的单据不能编辑");
                   return;
               }

               if (e.SelectedItem != null)
               {
                   var item = Selecter;
                   if (item != null)
                   {
                       var product = ProductSeries.Select(p => p).Where(p => p.Id == item.ProductId).FirstOrDefault();
                       if (product != null)
                       {
                           product.UnitId = item.UnitId;
                           product.Quantity = item.Quantity;
                           product.Price = item.Price;
                           product.Amount = item.Amount;
                           product.Remark = item.Remark;
                           product.Subtotal = item.Subtotal;
                           product.UnitName = item.UnitName;
                           product.GUID = item.GUID;
                           if (item.BigUnitId > 0)
                           {
                               product.bigOption.Name = item.UnitName;
                               product.BigPriceUnit.Quantity = item.Quantity;
                               product.BigPriceUnit.UnitId = item.BigUnitId ?? 0;
                               product.BigPriceUnit.Amount = item.Amount;
                               product.BigPriceUnit.Remark = item.Remark;
                           }

                           if (item.SmallUnitId > 0)
                           {
                               product.bigOption.Name = item.UnitName;
                               product.SmallPriceUnit.Quantity = item.Quantity;
                               product.SmallPriceUnit.UnitId = item.SmallUnitId ?? 0;
                               product.SmallPriceUnit.Amount = item.Amount;
                               product.SmallPriceUnit.Remark = item.Remark;
                           }

                           await this.NavigateAsync("EditProductPage", ("Product", product), ("Reference", PageName), ("Item", item), ("WareHouse", WareHouse));
                       }
                   }
               }
           });

            //添加商品
            this.AddProductCommand = ReactiveCommand.Create<object>(async e =>
           {
               if (!valid_IsAudited.IsValid)
               {
                   _dialogService.ShortAlert("已审核单据不能操作！");
                   return;
               }

               if (!valid_ManufacturerId.IsValid)
               {
                   _dialogService.ShortAlert("请选择供应商！");
                   ((ICommand)ManufacturerSelected)?.Execute(null);
                   return;
               }

               if (!valid_WareHouseId.IsValid || WareHouse == null)
               {
                   _dialogService.ShortAlert("请选择仓库！");
                   ((ICommand)StockSelected)?.Execute(null);
                   return;
               }

               await this.NavigateAsync("SelectProductPage", ("Reference", this.PageName),
                       ("WareHouse", WareHouse),
                       ("ManufacturerId", Bill.ManufacturerId),
                       ("SerchKey", Filter.SerchKey));
           });

            //审核
            this.AuditingDataCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                //是否具有审核权限
                await this.Access(AccessGranularityEnum.PurchaseBillsApproved);
                await SubmitAsync(Bill.Id, _purchaseBillService.AuditingAsync, async (result) =>
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

            //启用麦克风
            this.WhenAnyValue(x => x.EnableMicrophone)
              .Subscribe(async x =>
              {
                  var micAccessGranted = await _microphoneService.GetPermissionsAsync();
                  if (!micAccessGranted)
                  {
                      this.Alert("请打开麦克风");
                  }
              }).DisposeWith(DeactivateWith);
            //匹配声音
            this.RecognitionCommand = ReactiveCommand.Create(() =>
            {
                if (!valid_ManufacturerId.IsValid)
                {
                    _dialogService.ShortAlert("请选择供应商！");
                    return;
                }

                if (!valid_WareHouseId.IsValid)
                {
                    _dialogService.ShortAlert("请选择仓库！");
                    return;
                }

                RecognitionSpeech((key) =>
                {
                    Filter.SerchKey = key;
                    ((ICommand)this.AddProductCommand)?.Execute(null);
                });
            });

            //切换语音助手
            this.SpeechCommand = ReactiveCommand.Create(() =>
            {
                if (IsFooterVisible)
                {
                    IsVisible = true;
                    IsExpanded = true;
                    IsFooterVisible = false;
                }
                else
                {
                    IsVisible = false;
                    IsExpanded = false;
                    IsFooterVisible = true;
                }
            });

            //工具栏打印
            this.PrintCommand = ReactiveCommand.Create(async () =>
            {
                if (Bill.Items.Count == 0)
                {
                    Alert("请添加商品项目");
                    return;
                }
                Bill.BillType = BillTypeEnum.PurchaseBill;
                await SelectPrint(Bill);
            });

            //绑定页面菜单
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                //支付方式
                { MenuEnum.PAY,(m,vm)=> {
                  if (Bill.SumAmount == 0) { this.Alert("请添加商品项目！"); return; }
                         PaymentMethodBaseModel payments = this.PaymentMethods;
                         SelectPaymentMethods(("PaymentMethods", payments),
                                     ("TBalance", this.TBalance),
                                     ("BillType", this.BillType),
                                     ("Reference", PageName));
                } },
                //欠款
                { MenuEnum.ARREARS,(m,vm)=> {

                        SetOweCash((result) =>
                         {
                             decimal.TryParse(result, out decimal oweCash);
                             PaymentMethods.OweCash = oweCash;
                             PaymentMethods.OweCashShowFiled = oweCash > 0;
                             PaymentMethods.OweCashShowFiled = oweCash > 0;
                             UpdateUI();
                         }, Bill.OweCash);
                } },
                //优惠
                { MenuEnum.DISCOUNT,(m,vm)=>{

                SetDiscount((result) =>
                         {
                             decimal.TryParse(result, out decimal preferentialAmount);
                             PaymentMethods.PreferentialAmount = preferentialAmount;
                             PaymentMethods.PreferentialAmountShowFiled = preferentialAmount > 0;
                             PaymentMethods.PreferentialAmountShowFiled = preferentialAmount > 0;
                             UpdateUI();
                         }, Bill.PreferentialAmount);
                } },
                //整单备注
                { MenuEnum.REMARK,(m,vm)=>{

                 AllRemak((result) =>
                     {
                         Bill.Remark = result;
                     }, Bill.Remark);

                } },
                //清空单据
                { MenuEnum.CLEAR,(m,vm)=>{

                 ClearBill<PurchaseBillModel, PurchaseItemModel>(Bill, DoClear);

                } },
                //打印
                { MenuEnum.PRINT,async (m,vm)=>{

                     if (Bill.Items.Count == 0)
                         {
                             Alert("请添加商品项目");
                             return;
                         }

                         Bill.BillType = BillTypeEnum.PurchaseBill;
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
                             result = await _purchaseBillService.AuditingAsync(Bill.Id);
                         }
                         if (null!=result&&result.Success)
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
                            result = await _purchaseBillService.ReverseAsync(Bill.Id);
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
                    await _purchaseBillService.ReverseAsync(Bill.Id);
                } }
            });

            this.BindBusyCommand(Load);

        }

        private void BillAddItem(PurchaseItemModel item)
        {
            if (null == Bill.Items?.FirstOrDefault(p => p.GUID == item.GUID))
            {
                item.SortIndex = Bill.Items?.Count ?? 0;
                Bill.Items?.Add(item);
            }
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                //选择供应商
                if (parameters.ContainsKey("Manufacturer"))
                {
                    parameters.TryGetValue<ManufacturerModel>("Manufacturer", out ManufacturerModel manufacturer);
                    Bill.ManufacturerId = manufacturer != null ? manufacturer.Id : 0;
                    Bill.ManufacturerName = manufacturer != null ? manufacturer.Name : "";
                    Sync.Run(async () =>
                    {
                        //获取余额
                        this.MBalance = await _manufacturerService.GetManufacturerBalance(Bill.ManufacturerId);
                    });
                }

                //删除商品
                if (parameters.ContainsKey("DelProduct"))
                {
                    parameters.TryGetValue<ProductModel>("DelProduct", out ProductModel p);
                    if (p != null)
                    {
                        var bigProduct = Bill.Items?.Where(b => b.GUID == p.GUID && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items?.Where(b => b.GUID == p.GUID && b.SmallUnitId == p.UnitId).FirstOrDefault();
                        if (bigProduct != null)
                        {
                            Bill.Items?.Remove(bigProduct);
                        }

                        if (smalProduct != null)
                        {
                            Bill.Items?.Remove(smalProduct);
                        }

                        UpdateUI();
                    }
                }

                //编辑商品更新
                if (parameters.ContainsKey("UpdateProduct"))
                {
                    parameters.TryGetValue<ProductModel>("UpdateProduct", out ProductModel p);
                    if (p != null)
                    {
                        var bigProduct = Bill.Items?.Where(b => b.GUID == p.GUID && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items?.Where(b => b.GUID == p.GUID && b.SmallUnitId == p.UnitId).FirstOrDefault();
                        if (bigProduct != null)
                        {
                            bigProduct.UnitName = p.UnitName;
                            bigProduct.Quantity = p.Quantity;
                            bigProduct.Price = p.Price ?? 0;
                            bigProduct.Amount = p.Amount ?? 0;
                            bigProduct.Remark = p.Remark;
                            bigProduct.BigUnitId = p.UnitId;
                            bigProduct.Subtotal = (p.Price ?? 0) * p.Quantity;
                        }
                        if (smalProduct != null)
                        {
                            smalProduct.UnitName = p.UnitName;
                            smalProduct.Quantity = p.Quantity;
                            smalProduct.Price = p.Price ?? 0;
                            smalProduct.Amount = p.Amount ?? 0;
                            smalProduct.Remark = p.Remark;
                            smalProduct.BigUnitId = p.UnitId;
                            smalProduct.Subtotal = (p.Price ?? 0) * p.Quantity;
                        }

                        if (Bill.Items?.Any() ?? false)
                        {
                            foreach (var ip in Bill.Items)
                            {
                                if (smalProduct != null && (ip.UnitId == smalProduct.SmallUnitId || ip.SmallUnitId == smalProduct.SmallUnitId))
                                {
                                    ip.UnitName = smalProduct.UnitName;
                                    ip.Quantity = smalProduct.Quantity;
                                    ip.Remark = smalProduct.Remark;
                                    ip.SmallUnitId = smalProduct.SmallUnitId;
                                    ip.Subtotal = smalProduct.Subtotal;
                                }
                                else if (bigProduct != null && (ip.UnitId == bigProduct.SmallUnitId || ip.BigUnitId == bigProduct.BigUnitId))
                                {
                                    ip.UnitName = bigProduct.UnitName;
                                    ip.Quantity = bigProduct.Quantity;
                                    ip.Remark = bigProduct.Remark;
                                    ip.BigUnitId = bigProduct.UnitId;
                                    ip.Subtotal = bigProduct.Subtotal;
                                }
                            }
                        }

                        UpdateUI();
                    }
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

                //选择商品序列
                if (parameters.ContainsKey("ProductSeries"))
                {
                    var productSeries = parameters.GetValue<List<ProductModel>>("ProductSeries");
                    if (productSeries != null && productSeries.Count > 0)
                    {
                        foreach (var p in productSeries)
                        {
                            var bigItem = new PurchaseItemModel()
                            {
                                Id = 0,
                                //GUID = p.GUID,
                                UnitId = p.BigPriceUnit.UnitId,
                                ProductId = p.Id,
                                ProductName = p.ProductName,
                                StoreId = Settings.StoreId,
                                PurchaseBillId = 0,
                                UnitName = p.bigOption.Name,
                                Quantity = p.BigPriceUnit.Quantity,
                                Price = p.BigPriceUnit.Price,
                                Amount = (p.BigPriceUnit.Quantity) * (p.BigPriceUnit.Price),
                                Remark = p.BigPriceUnit.Remark,
                                BigUnitId = p.BigPriceUnit.UnitId,
                                SmallUnitId = 0,
                                Subtotal = (p.BigPriceUnit.Price) * p.BigPriceUnit.Quantity
                            };

                            var smallItem = new PurchaseItemModel()
                            {
                                Id = 0,
                                //GUID = p.GUID,
                                UnitId = p.SmallPriceUnit.UnitId,
                                ProductId = p.Id,
                                ProductName = p.ProductName,
                                StoreId = Settings.StoreId,
                                PurchaseBillId = 0,
                                UnitName = p.smallOption.Name,
                                Quantity = p.SmallPriceUnit.Quantity,
                                Price = p.SmallPriceUnit.Price,
                                Amount = (p.SmallPriceUnit.Quantity) * (p.SmallPriceUnit.Price),
                                Remark = p.SmallPriceUnit.Remark,
                                SmallUnitId = p.SmallPriceUnit.UnitId,
                                BigUnitId = 0,
                                Subtotal = (p.SmallPriceUnit.Price) * p.SmallPriceUnit.Quantity
                            };

                            if (bigItem.Quantity > 0)
                            {
                                BillAddItem(bigItem);
                            }

                            if (smallItem.Quantity > 0)
                            {
                                BillAddItem(smallItem);
                            }
                            ProductSeries.Add(p);
                        }
                        //ProductSeries = new ObservableCollection<ProductModel>(productSeries);
                    }
                    UpdateUI();
                }

                //预览单据
                if (parameters.ContainsKey("Bill"))
                {
                    parameters.TryGetValue("Bill", out PurchaseBillModel bill);
                    parameters.TryGetValue("IsSubmitBill", out bool isSubmitBill);
                    this.Bill.IsSubmitBill = isSubmitBill;

                    if (bill != null)
                    {
                        this.loaded = true;
                        Bill = bill;
                        Bill.WareHouseId = bill.WareHouseId;
                        Bill.WareHouseName = bill.WareHouseName;

                        this.PaymentMethods = this.ToPaymentMethod(Bill, bill.PurchaseBillAccountings);
                        UpdateUI();
                    }
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
                this.Bill.SumAmount = decimal.Round(Bill.Items?.Select(p => p.Subtotal).Sum() ?? 0, 2);

                //惠
                this.Bill.PreferentialAmount = PaymentMethods.PreferentialAmount;

                //支付
                var totalAccountingAmount = PaymentMethods.Selectes.Sum(s => s.CollectionAmount);
                this.Bill.Accountings = PaymentMethods.Selectes.Select(a => { return $"{a.Name}: {string.Format("{0:F2}", a.CollectionAmount)}"; }).ToArray();

                //(欠款)待支付
                this.Bill.OweCash = this.Bill.SumAmount - this.Bill.PreferentialAmount - totalAccountingAmount;

                //用于传递
                this.PaymentMethods.SubAmount = this.Bill.SumAmount;
                this.PaymentMethods.OweCash = this.Bill.OweCash;
                this.PaymentMethods.PreferentialAmount = this.Bill.PreferentialAmount;

                //红冲审核水印
                if (this.Bill.Id > 0 && this.Bill.AuditedStatus)
                    this.Bill.AuditedStatus = !this.Bill.ReversedStatus;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void InitBill(bool clear = false)
        {
            this.BillType = BillTypeEnum.PurchaseBill;
            var bill = Settings.PurchaseBill;
            if (bill != null && !clear)
            {
                this.Bill = bill;
                this.Bill.Items = new ObservableCollection<PurchaseItemModel>(bill?.Items);
                var defaultAccs = bill.PurchaseBillAccountings.Select(
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
                this.Bill = new PurchaseBillModel()
                {
                    BusinessUserId = Settings.UserId,
                    BillNumber = CommonHelper.GetBillNumber(CommonHelper.GetEnumDescription(BillType).Split(',')[1], Settings.StoreId)
                };

                if (this.Bill.Id == 0)
                {
                    this.Bill.BusinessUserId = Settings.UserId;
                    this.Bill.BusinessUserName = Settings.UserRealName;
                }
            }
        }
        private void DoClear()
        {
            InitBill(true);
            ((ICommand)Load)?.Execute(null);
        }
        public override void OnAppearing()
        {
            base.OnAppearing();

            //控制显示菜单
            if (Bill.Id > 0)
            {
                AppendMenus(Bill);
            }
            else
            {
                //控制显示菜单
                _popupMenu?.Show(0, 1, 2, 3, 4, 5);
            }

            if (!loaded)
            {
                loaded = true;
                ((ICommand)Load)?.Execute(null);
            }
        }
    }
}
