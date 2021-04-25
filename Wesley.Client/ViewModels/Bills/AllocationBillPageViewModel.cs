using Wesley.Client.Enums;
using Wesley.Client.Models.Products;
using Wesley.Client.Models.WareHouses;
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
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
namespace Wesley.Client.ViewModels
{
    public class AllocationBillPageViewModel : ViewModelBaseCutom<AllocationBillModel>
    {
        private readonly IMicrophoneService _microphoneService;
        [Reactive] public AllocationItemModel Selecter { get; set; }


        public AllocationBillPageViewModel(INavigationService navigationService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,


            IDialogService dialogService,
            IMicrophoneService microphoneService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "调拨单";
            _microphoneService = microphoneService;

            InitBill();

            //验证
            var valid_ShipmentWareHouseId = this.ValidationRule(x => x.Bill.ShipmentWareHouseId, _isZero, "出货仓库未指定");
            var valid_IncomeWareHouseId = this.ValidationRule(x => x.Bill.IncomeWareHouseId, _isZero, "入货仓库未指定");
            var valid_ProductCount = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "请添加商品项目");
            var valid_IsVieweBill = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");

            //添加商品
            this.AddProductCommand = ReactiveCommand.Create<object>(async e =>
           {
               if (this.Bill.AuditedStatus) { _dialogService.ShortAlert("已审核单据不能操作！"); return; }

               if (!valid_ShipmentWareHouseId.IsValid)
               {
                   _dialogService.ShortAlert(valid_ShipmentWareHouseId.Message[0]);
                   return;
               }

               if (!valid_IncomeWareHouseId.IsValid)
               {
                   _dialogService.ShortAlert(valid_IncomeWareHouseId.Message[0]);
                   return;
               }

               if (Bill.ShipmentWareHouseId == Bill.IncomeWareHouseId)
               {
                   _dialogService.ShortAlert("出入库不能相同！");
                   return;
               }

               //传入当前主库
               this.WareHouse.Id = Bill.ShipmentWareHouseId;
               this.WareHouse.Name = Bill.ShipmentWareHouseName;
               this.Filter.WareHouseId = Bill.ShipmentWareHouseId;
               this.Filter.WareHouseName = Bill.ShipmentWareHouseName;

               await this.NavigateAsync("SelectProductPage", ("Reference", this.PageName),
                         ("Bill", Bill),
                         ("Filter", Filter),
                         ("WareHouse", WareHouse),
                         ("SerchKey", Filter.SerchKey));
           });

            //编辑商品
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
                .Skip(1)
                .Where(x => x != null)
                .SubOnMainThread(async item =>
               {
                   if (item != null)
                   {
                       var product = ProductSeries.Select(p => p).Where(p => p.Id == item.ProductId).FirstOrDefault();
                       if (product != null)
                       {
                           product.UnitId = item.UnitId;
                           product.Quantity = item.Quantity;
                           product.Remark = item.Remark;
                           product.Subtotal = item.Subtotal;
                           product.UnitName = item.UnitName;

                           if (item.BigUnitId > 0)
                           {
                               product.bigOption.Name = item.UnitName;
                               product.BigPriceUnit.Quantity = item.Quantity;
                               product.BigPriceUnit.UnitId = item.BigUnitId ?? 0;
                               product.BigPriceUnit.Remark = item.Remark;
                           }

                           if (item.SmallUnitId > 0)
                           {
                               product.bigOption.Name = item.UnitName;
                               product.SmallPriceUnit.Quantity = item.Quantity;
                               product.SmallPriceUnit.UnitId = item.SmallUnitId;
                               product.SmallPriceUnit.Remark = item.Remark;
                           }

                           await this.NavigateAsync("EditAllocationProductPage", ("Product", product));
                       }
                   }

                   this.Selecter = null;
               }, ex => _dialogService.ShortAlert(ex.ToString()));

            //提交单据
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                await this.Access(AccessGranularityEnum.AllocationFormSave);

                if (!valid_ShipmentWareHouseId.IsValid) { this.Alert(valid_ShipmentWareHouseId.Message[0]); return Unit.Default; }
                if (!valid_IncomeWareHouseId.IsValid) { this.Alert(valid_IncomeWareHouseId.Message[0]); return Unit.Default; }
                if (!valid_ProductCount.IsValid) { this.Alert(valid_ProductCount.Message[0]); return Unit.Default; }

                var postMData = new AllocationUpdateModel()
                {
                    ShipmentWareHouseId = Bill.ShipmentWareHouseId,
                    IncomeWareHouseId = Bill.IncomeWareHouseId,
                    CreatedOnUtc = Bill.CreatedOnUtc,
                    AllocationByMinUnit = false,
                    Remark = Bill.Remark,
                    //商品项目
                    Items = Bill.Items.Where(i => i.Quantity > 0).ToList(),
                };

                return await SubmitAsync(postMData, Bill.Id, _wareHousesService.CreateOrUpdateAllocationbillAsync, (result) =>
                {
                    Bill = new AllocationBillModel();
                }, token: cts.Token);
            },
            this.IsValid());

            //存储记录
            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                if (!this.Bill.AuditedStatus && this.Bill.ShipmentWareHouseId != 0 && this.Bill.ShipmentWareHouseId != Settings.AllocationBill?.ShipmentWareHouseId)
                {
                    var ok = await _dialogService.ShowConfirmAsync("你是否要保存单据？", "提示", "确定", "取消");
                    if (ok)
                    {
                        if (!this.Bill.AuditedStatus)
                            Settings.AllocationBill = this.Bill;
                    }
                }
                await _navigationService.GoBackAsync();
            });

            //审核
            this.AuditingDataCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                //是否具有审核权限
                await this.Access(AccessGranularityEnum.AllocationFormApproved);
                await SubmitAsync(Bill.Id, _wareHousesService.AuditingAsync, async (result) =>
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
                   case Enums.MenuEnum.REMARK://整单备注
                       AllRemak((result) => { Bill.Remark = result; }, Bill.Remark);
                       break;
                   case Enums.MenuEnum.CLEAR://清空单据
                       {
                           ClearBill<AllocationBillModel, AllocationItemModel>(Bill, DoClear);
                       }
                       break;
                   case Enums.MenuEnum.PRINT://打印
                       {
                           if (!valid_ProductCount.IsValid) { this.Alert(valid_IsVieweBill.Message[0]); return; }
                           await SelectPrint(this.Bill);
                       }
                       break;
                   case Enums.MenuEnum.AXBH:
                       {
                           if (Bill.IncomeWareHouseId == 0)
                           {
                               _dialogService.ShortAlert("入仓为必选");
                               return;
                           }

                           await this.NavigateAsync("SelectAllocationProductPage", ("WareHouseId", Bill.IncomeWareHouseId));
                       }
                       break;
                   case Enums.MenuEnum.ATDB:
                       {
                           if (Bill.ShipmentWareHouseId == 0)
                           {
                               _dialogService.ShortAlert("出仓为必选");
                               return;
                           }

                           await this.NavigateAsync("SelectAllocationProductPage", ("WareHouseId", Bill.ShipmentWareHouseId));
                       }
                       break;
                   case Enums.MenuEnum.AKCDB:
                       {
                           if (Bill.ShipmentWareHouseId == 0)
                           {
                               _dialogService.ShortAlert("出仓为必选");
                               return;
                           }

                           await this.NavigateAsync("SelectAllocationProductPage", ("WareHouseId", Bill.ShipmentWareHouseId));
                       }
                       break;
               }
           }, 3, 4, 5, 22, 23, 24);


            //启用麦克风
            this.WhenAnyValue(x => x.EnableMicrophone)
              .Subscribe(async x =>
              {
                  var micAccessGranted = await _microphoneService.GetPermissionsAsync();
                  if (!micAccessGranted)
                  {
                      this.Alert("请打开麦克风");
                  }
              })
              .DisposeWith(this.DeactivateWith);
            //匹配声音
            this.RecognitionCommand = ReactiveCommand.Create(() =>
            {
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


            this.RecognitionCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.StockSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.AddProductCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.ExceptionsSubscribe();
        }

        private void InitBill(bool clear = false)
        {
            this.BillType = BillTypeEnum.AllocationBill;
            var bill = Settings.AllocationBill;
            if (bill != null && !clear)
            {
                this.Bill = bill;
                this.Bill.Items = new ObservableCollection<AllocationItemModel>(bill?.Items);
                this.Selecter = bill?.Items?.FirstOrDefault();
            }
            else
            {
                this.Bill = new AllocationBillModel()
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

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                //删除商品
                if (parameters.ContainsKey("DelProduct"))
                {
                    parameters.TryGetValue("DelProduct", out ProductModel p);
                    if (p != null)
                    {
                        var bigProduct = Bill.Items.Where(b => b.Id == p.Id && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items.Where(b => b.Id == p.Id && b.SmallUnitId == p.UnitId).FirstOrDefault();
                        if (bigProduct != null)
                        {
                            Bill.Items.Remove(bigProduct);
                        }

                        if (smalProduct != null)
                        {
                            Bill.Items.Remove(smalProduct);
                        }
                    }
                }

                //编辑商品更新
                if (parameters.ContainsKey("UpdateProduct"))
                {
                    parameters.TryGetValue("UpdateProduct", out ProductModel p);
                    if (p != null)
                    {
                        var bigProduct = Bill.Items.Where(b => b.Id == p.Id && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items.Where(b => b.Id == p.Id && b.SmallUnitId == p.UnitId).FirstOrDefault();
                        if (bigProduct != null)
                        {

                            bigProduct.UnitName = p.UnitName;
                            bigProduct.Quantity = p.Quantity;
                            bigProduct.Remark = p.Remark;
                            bigProduct.BigUnitId = p.UnitId;
                            bigProduct.Subtotal = p.Quantity * p.BigQuantity ?? 0;
                        }
                        if (smalProduct != null)
                        {
                            smalProduct.UnitName = p.UnitName;
                            smalProduct.Quantity = p.Quantity;
                            smalProduct.Remark = p.Remark;
                            smalProduct.BigUnitId = p.UnitId;
                            smalProduct.Subtotal = p.Quantity;
                        }
                    }
                }

                //选择商品序列
                if (parameters.ContainsKey("ProductSeries"))
                {
                    parameters.TryGetValue("ProductSeries", out ObservableCollection<ProductModel> productSeries);
                    this.ProductSeries = productSeries;

                    if (productSeries != null)
                    {
                        foreach (var p in productSeries)
                        {
                            var bigItem = new AllocationItemModel()
                            {
                                Id = p.Id,
                                UnitId = p.BigPriceUnit.UnitId,
                                ProductId = p.Id,
                                ProductName = p.ProductName,
                                UnitName = p.bigOption.Name,
                                TradePrice = p.BigPriceUnit.Price,
                                Quantity = p.BigPriceUnit.Quantity,
                                Remark = p.BigPriceUnit.Remark,
                                BigUnitId = p.BigPriceUnit.UnitId,
                                SmallUnitId = 0,
                                Subtotal = p.BigPriceUnit.Quantity * p.BigQuantity ?? 0
                            };

                            var smallItem = new AllocationItemModel()
                            {
                                Id = p.Id,
                                UnitId = p.SmallPriceUnit.UnitId,
                                ProductId = p.Id,
                                ProductName = p.ProductName,
                                UnitName = p.smallOption.Name,
                                TradePrice = p.SmallPriceUnit.Price,
                                Quantity = p.SmallPriceUnit.Quantity,
                                Remark = p.SmallPriceUnit.Remark,
                                SmallUnitId = p.SmallPriceUnit.UnitId,
                                BigUnitId = 0,
                                Subtotal = p.SmallPriceUnit.Quantity
                            };

                            Bill.Items.Add(bigItem);
                            Bill.Items.Add(smallItem);
                        }
                    }
                }

                if (parameters.ContainsKey("Bill"))
                {
                    parameters.TryGetValue("Bill", out AllocationBillModel bill);
                    if (bill != null)
                    {
                        this.loaded = true;
                        Bill = bill;
                        Bill.ShipmentWareHouseId = bill.ShipmentWareHouseId;
                        Bill.IncomeWareHouseId = bill.IncomeWareHouseId;

                        if (bill.AuditedStatus) { }
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
                    }
                }

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

    }
}
