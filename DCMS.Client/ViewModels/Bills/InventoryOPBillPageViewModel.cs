using DCMS.Client.Enums;
using DCMS.Client.Models;
using DCMS.Client.Models.Products;
using DCMS.Client.Models.WareHouses;
using DCMS.Client.Pages;
using DCMS.Client.Services;
using DCMS.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using System.Reactive.Disposables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;

namespace DCMS.Client.ViewModels
{

    public class InventoryOPBillPageViewModel : ViewModelBaseCutom<InventoryPartTaskBillModel>
    {
        private readonly IInventoryService _inventoryService;
        public new ReactiveCommand<object, Unit> StockSelected { get; set; }
        public ReactiveCommand<object, Unit> DeleteCommand { get; }
        public ReactiveCommand<object, Unit> CancelCommand { get; }
        public ReactiveCommand<object, Unit> CompletedCommand { get; }
        public ReactiveCommand<object, Unit> ScanBarCommand { get; }
        [Reactive] public InventoryPartTaskItemModel Selecter { get; set; }

        public InventoryOPBillPageViewModel(INavigationService navigationService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IInventoryService inventoryService,
            IDialogService dialogService
            ) : base(navigationService,
                productService,
                terminalService,
                userService,
                wareHousesService,
                accountingService,
                dialogService)
        {
            Title = "盘点单";

            _inventoryService = inventoryService;


            this.BillType = BillTypeEnum.InventoryPartTaskBill;
            this.Bill = new InventoryPartTaskBillModel()
            {
                BillTypeId = (int)this.BillType,
                InventoryPerson = Settings.UserId,
                BillNumber = CommonHelper.GetBillNumber(CommonHelper.GetEnumDescription(BillType).Split(',')[1], Settings.StoreId)
            };

            //验证
            var valid_IsReversed = this.ValidationRule(x => x.Bill.ReversedStatus, _isBool, "已红冲单据不能操作");
            var valid_IsAudited = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");
            var valid_WareHouseId = this.ValidationRule(x => x.Bill.WareHouseId, _isZero, "仓库未指定");
            var valid_InventoryPerson = this.ValidationRule(x => x.Bill.InventoryPerson, _isZero, "盘点人未指定");
            var valid_IsVieweBill = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");
            var valid_ProductCount = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "请添加商品项目");

            //初始化 
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                var result = await _inventoryService.GetInventoryPartTaskBillAsync(Bill.Id, calToken: new System.Threading.CancellationToken());
                if (result != null)
                {
                    UpdateUI(result);
                }
            }));

            //删除选择
            this.DeleteCommand = ReactiveCommand.Create<object>(e =>
            {
                int productId = (int)e;
                var product = Bill.Items?.Select(p => p).Where(p => p.ProductId == productId).FirstOrDefault();
                if (product != null)
                {
                    Bill.Items?.Remove(product);
                    var lists = Bill.Items;
                    Bill.Items = new ObservableCollection<InventoryPartTaskItemModel>(lists);
                }
            });

            //添加商品
            this.AddProductCommand = ReactiveCommand.Create<object>(async e =>
           {
               if (!valid_WareHouseId.IsValid) { this.Alert(valid_WareHouseId.Message[0]); return; }
               if (!valid_InventoryPerson.IsValid) { this.Alert(valid_InventoryPerson.Message[0]); return; }
               if (!valid_IsVieweBill.IsValid) { this.Alert(valid_IsVieweBill.Message[0]); return; }

               await this.NavigateAsync("SelectProductPage", ("Reference", this.PageName),
                       ("WareHouse", WareHouse),
                       ("Products", ProductSeries.ToList()));
           });


            //选择仓库
            this.StockSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectStock(async (result) =>
                {
                    if (result != null && WareHouse != null && Bill != null)
                    {
                        WareHouse.Id = result.Id;
                        WareHouse.Name = result.Column;
                        Bill.WareHouseId = result.Id;
                        Bill.WareHouseName = result.Column;

                        var pendings = await _inventoryService.CheckInventoryAsync(result.Id, new System.Threading.CancellationToken());
                        if (pendings != null && pendings.Any())
                        {
                            WareHouse.Id = 0;
                            WareHouse.Name = "";
                            Bill.WareHouseId = 0;
                            Bill.WareHouseName = "";
                            this.Alert("库存正在盘点中，不能在生成盘点单！");
                            return;
                        }
                    }

                }, BillTypeEnum.InventoryAllTaskBill);
            });

            //保存盘点
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                return await this.Access(AccessGranularityEnum.InventoryAllSave, async () =>
                {
                    if (Bill.InventoryPerson == 0) { this.Alert("盘点人未指定！"); return Unit.Default; }
                    if (Bill.WareHouseId == 0) { this.Alert("仓库未指定！"); return Unit.Default; }
                    if (Bill.Items?.Count == 0) { this.Alert("请添加商品项目！"); return Unit.Default; }

                    var postMData = new InventoryPartTaskUpdateModel()
                    {
                        BillNumber = this.Bill.BillNumber,
                        InventoryPerson = Bill.InventoryPerson,
                        WareHouseId = Bill.WareHouseId,
                        InventoryDate = DateTime.Now,
                        Items = Bill.Items
                    };

                    var confirm = await _dialogService.ShowConfirmAsync("确认保存盘点吗？", okText: "确定", cancelText: "取消");
                    if (confirm)
                    {
                        return await SubmitAsync(postMData, Bill.Id, _inventoryService.CreateOrUpdateAsync, (result) =>
                        {
                            Bill.Id = result.Return;
                            Bill.InventoryPerson = postMData.InventoryPerson;
                            Bill.WareHouseId = postMData.WareHouseId;
                        }, false, token: new System.Threading.CancellationToken());
                    }
                    else
                    {
                        return Unit.Default;
                    }
                });
            },
            this.IsValid());

            //放弃盘点
            this.CancelCommand = ReactiveCommand.CreateFromTask<object>(async e =>
           {

               if (Bill.Id == 0)
               {
                   this.Alert("操作失败，你需要先保存盘点！");
                   return;
               }

               if (Bill.InventoryPerson == 0) { this.Alert("盘点人未指定！"); return; }
               if (Bill.WareHouseId == 0) { this.Alert("仓库未指定！"); return; }
               if (Bill.Items?.Count == 0) { this.Alert("请添加商品项目！"); return; }

               var confirm = await _dialogService.ShowConfirmAsync("确认放弃盘点吗？", okText: "确定", cancelText: "取消");
               if (confirm)
               {
                   await _inventoryService.CancelTakeInventoryAsync(Bill.Id, new System.Threading.CancellationToken());

               }
           });

            //完成确认
            this.CompletedCommand = ReactiveCommand.CreateFromTask<object>(async e =>
            {
                if (Bill.Id == 0)
                {
                    this.Alert("操作失败，你需要先保存盘点！");
                    return;
                }

                if (Bill.InventoryPerson == 0) { this.Alert("盘点人未指定！"); return; }
                if (Bill.WareHouseId == 0) { this.Alert("仓库未指定！"); return; }
                if (Bill.Items?.Count == 0) { this.Alert("请添加商品项目！"); return; }

                var confirm = await _dialogService.ShowConfirmAsync("确认完成盘点吗？", okText: "确定", cancelText: "取消");
                if (confirm)
                {

                    if (Bill.InventoryPerson == 0) { this.Alert("盘点人未指定！"); return; }
                    if (Bill.WareHouseId == 0) { this.Alert("仓库未指定！"); return; }
                    if (Bill.Items?.Count == 0) { this.Alert("请添加商品项目！"); return; }
                    var result = await _inventoryService.SetInventoryCompletedAsync(Bill.Id, new System.Threading.CancellationToken());
                    if (!result)
                    {
                        this.Alert("抱歉，系统错误，请反馈技术支持！");
                    }
                }
            });

            //编辑商品
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
             .Skip(1)
             .Where(x => x != null)
             .SubOnMainThread(async item =>
            {

                if (this.Bill.IsSubmitBill)
                {
                    _dialogService.ShortAlert("已提交的单据不能编辑");
                    return;
                }

                if (item != null)
                {
                    List<ProductModel> products = null;
                    var product = new ProductModel()
                    {
                        Name = item.ProductName,
                        ProductName = item.ProductName,
                        ProductId = item.ProductId,
                        BigPriceUnit = new PriceUnit() { UnitId = item.BigUnitId ?? 0, Quantity = item.BigQuantity ?? 0, UnitName = item.bigOption.Name },
                        SmallPriceUnit = new PriceUnit() { UnitId = item.SmallUnitId, Quantity = item.SmallUnitQuantity ?? 0, UnitName = item.smallOption.Name },
                        StockQty = item.StockQty,
                        UnitConversion = item.UnitConversion
                    };
                    products.Add(product);

                    await this.NavigateAsync("AddInventoryProductPage",
                        ("Reference", this.PageName),
                        ("WareHouse", WareHouse),
                        ("Products", products));
                }
            })
             .DisposeWith(DeactivateWith);

            //扫码商品
            this.ScanBarCommand = ReactiveCommand.Create<object>(async e => await this.NavigateAsync("ScanBarcodePage", ("action", "add")));

            //绑定页面菜单
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                //全部
                { Enums.MenuEnum.ALL,(m,vm)=> {
                  var lists = Bill.Items?.ToList();
                     Bill.Items = new ObservableCollection<InventoryPartTaskItemModel>(lists);
                } },
                //未盘点
                { MenuEnum.WART,(m,vm)=> {

                      var lists = Bill.Items?.Where(s => string.IsNullOrEmpty(s.StatusName)).ToList();
                     Bill.Items = new ObservableCollection<InventoryPartTaskItemModel>(lists);
                } },
                //已盘点
                { MenuEnum.YPD,(m,vm)=>{

            var lists = Bill.Items?.Where(s => !string.IsNullOrEmpty(s.StatusName)).ToList();
                    Bill.Items = new ObservableCollection<InventoryPartTaskItemModel>(lists);
                } },
                //整单备注
                { MenuEnum.REMARK,(m,vm)=>{

                 AllRemak((result) =>
                     {
                         Bill.Remark = result;
                     }, Bill.Remark);

                } },
                //历史单据
                { MenuEnum.HISTORY,async (m,vm)=>{
                        await SelectHistory();
                } }
            });



            this.BindBusyCommand(Load);
        }



        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            try
            {
                //选择商品序列
                if (parameters.ContainsKey("ProductSeries"))
                {
                    parameters.TryGetValue("ProductSeries", out List<ProductModel> productSeries);
                    if (productSeries != null && productSeries.Count > 0)
                    {
                        this.ProductSeries = new ObservableCollection<ProductModel>(productSeries);
                        productSeries.ForEach(p =>
                        {

                            int bigQ = 0;
                            int smaQ = 0;
                            int volumeQuantity = 0;
                            int lossesQuantity = 0;

                            if (p.StockQty.HasValue && p.StockQty > 0)
                            {
                                if ((p.BigQuantity ?? 0) == 0) p.BigQuantity = 1;
                                bigQ = (int)Math.Floor(Convert.ToDouble(p.StockQty) / Convert.ToDouble(p.BigQuantity ?? 0));
                                smaQ = (int)(Convert.ToDouble(p.StockQty) % Convert.ToDouble(p.BigQuantity ?? 0));
                            }

                            var statusName = "";
                            var sumQ = p.BigPriceUnit.Quantity * p.BigQuantity ?? 0 + p.SmallPriceUnit.Quantity;
                            if (p.StockQty > sumQ)
                            {
                                statusName = "盘亏";
                                lossesQuantity = p.StockQty ?? 0 - sumQ;
                            }
                            else if (p.StockQty == sumQ)
                            {
                                statusName = "盘平";
                            }
                            else if (p.StockQty < sumQ)
                            {
                                statusName = "盘盈";
                                volumeQuantity = Math.Abs(p.StockQty ?? 0 - sumQ);
                            }

                            var item = new InventoryPartTaskItemModel()
                            {
                                ProductId = p.ProductId,
                                ProductName = p.ProductName,
                                GUID = p.GUID,
                                CurrentStock = p.StockQty,
                                BigUnitQuantity = p.BigPriceUnit.Quantity,
                                AmongUnitQuantity = 0,
                                SmallUnitQuantity = p.SmallPriceUnit.Quantity,
                                VolumeQuantity = volumeQuantity,
                                LossesQuantity = lossesQuantity,
                                VolumeWholesaleAmount = volumeQuantity * p.SmallProductPrices.TradePrice,
                                LossesWholesaleAmount = lossesQuantity * p.SmallProductPrices.TradePrice,
                                VolumeCostAmount = volumeQuantity * p.SmallProductPrices.CostPrice,
                                LossesCostAmount = lossesQuantity * p.SmallProductPrices.CostPrice,

                                CurrentStockName = $"{bigQ} {p.BigPriceUnit.UnitName} {smaQ} {p.SmallPriceUnit.UnitName}",
                                InventoryStockName = $"{p.BigPriceUnit.Quantity} {p.BigPriceUnit.UnitName} {p.SmallPriceUnit.Quantity}{p.SmallPriceUnit.UnitName}",
                                StatusName = statusName
                            };

                            Bill.Items?.Add(item);
                        });
                    }
                }

                //单据预览
                if (parameters.ContainsKey("Bill"))
                {
                    parameters.TryGetValue("Bill", out InventoryPartTaskBillModel bill);
                    parameters.TryGetValue("IsSubmitBill", out bool isSubmitBill);
                    this.Bill.IsSubmitBill = isSubmitBill;
                    if (bill != null)
                    {
                        Bill = bill;
                        Bill.Items?.ToList().ForEach(p =>
                        {

                            int bigQ = 0;
                            int smaQ = 0;
                            int volumeQuantity = 0;
                            int lossesQuantity = 0;

                            if (p.StockQty.HasValue && p.StockQty > 0)
                            {
                                if ((p.BigQuantity ?? 0) == 0) p.BigQuantity = 1;
                                bigQ = (int)Math.Floor(Convert.ToDouble(p.StockQty) / Convert.ToDouble(p.BigQuantity ?? 0));
                                smaQ = (int)(Convert.ToDouble(p.StockQty) % Convert.ToDouble(p.BigQuantity ?? 0));
                            }

                            var statusName = "";
                            var sumQ = p.BigUnitQuantity * p.BigQuantity ?? 0 + p.SmallUnitQuantity;
                            if (p.StockQty > sumQ)
                            {
                                statusName = "盘亏";
                                lossesQuantity = p.CurrentStock ?? 0 - sumQ ?? 0;
                            }
                            else if (p.StockQty == sumQ)
                            {
                                statusName = "盘平";
                            }
                            else if (p.StockQty < sumQ)
                            {
                                statusName = "盘盈";
                                volumeQuantity = Math.Abs(p.CurrentStock ?? 0 - sumQ ?? 0);
                            }

                            p.CurrentStockName = $"{bigQ} {p.bigOption.Name} {smaQ} {p.smallOption.Name}";
                            p.InventoryStockName = $"{p.BigUnitQuantity} {p.bigOption.Name} {p.SmallUnitQuantity} {p.smallOption.Name}";
                            p.StatusName = statusName;

                        });


                    }
                }

                Bill.WareHouseId = WareHouse.Id;
                Bill.WareHouseName = WareHouse.Name;
                Bill.SumCount = Bill.Items?.Count ?? 0;
                Bill.CompletedCount = Bill.Items?.Where(s => !string.IsNullOrEmpty(s.StatusName)).Count() ?? 0;
                Bill.NuCompletedCount = Bill.Items?.Where(s => string.IsNullOrEmpty(s.StatusName)).Count() ?? 0;
                Bill.CompletedCommandEnable = Bill.InventoryStatus != 2;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="data"></param>
        private void UpdateUI(InventoryPartTaskBillModel data)
        {
            if (data != null)
            {
                var lists = data.Items.Select(p =>
                {
                    int bigQ = 0;
                    int smaQ = 0;
                    int volumeQuantity = 0;
                    int lossesQuantity = 0;

                    if (p.StockQty.HasValue && p.StockQty > 0)
                    {
                        if ((p.BigQuantity ?? 0) == 0) p.BigQuantity = 1;
                        bigQ = (int)Math.Floor(Convert.ToDouble(p.StockQty) / Convert.ToDouble(p.BigQuantity ?? 0));
                        smaQ = (int)(Convert.ToDouble(p.StockQty) % Convert.ToDouble(p.BigQuantity ?? 0));
                    }

                    var statusName = "";
                    var sumQ = p.BigUnitQuantity * p.BigQuantity ?? 0 + p.SmallUnitQuantity;
                    if (p.StockQty > sumQ)
                    {
                        statusName = "盘亏";
                        lossesQuantity = p.CurrentStock ?? 0 - sumQ ?? 0;
                    }
                    else if (p.StockQty == sumQ)
                    {
                        statusName = "盘平";
                    }
                    else if (p.StockQty < sumQ)
                    {
                        statusName = "盘盈";
                        volumeQuantity = Math.Abs(p.CurrentStock ?? 0 - sumQ ?? 0);
                    }

                    return new InventoryPartTaskItemModel()
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        GUID = p.GUID,
                        CurrentStock = p.StockQty,
                        BigUnitQuantity = p.BigUnitQuantity,
                        AmongUnitQuantity = 0,
                        SmallUnitQuantity = p.SmallUnitQuantity,
                        VolumeQuantity = volumeQuantity,
                        LossesQuantity = lossesQuantity,
                        VolumeWholesaleAmount = volumeQuantity * p.SmallProductPrices.TradePrice,
                        LossesWholesaleAmount = lossesQuantity * p.SmallProductPrices.TradePrice,
                        VolumeCostAmount = volumeQuantity * p.SmallProductPrices.CostPrice,
                        LossesCostAmount = lossesQuantity * p.SmallProductPrices.CostPrice,

                        CurrentStockName = $"{bigQ}{p.bigOption.Name}{smaQ}{p.smallOption.Name}",
                        InventoryStockName = $"{p.BigUnitQuantity}{p.bigOption.Name}{p.SmallUnitQuantity}{p.smallOption.Name}",
                        StatusName = statusName
                    };
                }).ToList();

                Bill.Items = new ObservableCollection<InventoryPartTaskItemModel>(lists);
                Bill.SumCount = Bill.Items?.Count ?? 0;
                Bill.CompletedCount = Bill.Items?.Where(s => !string.IsNullOrEmpty(s.StatusName)).Count() ?? 0;
                Bill.NuCompletedCount = Bill.Items?.Where(s => string.IsNullOrEmpty(s.StatusName)).Count() ?? 0;
                Bill.CompletedCommandEnable = Bill.InventoryStatus != 2;
            }
        }


        public override void OnAppearing()
        {
            base.OnAppearing();


            //控制显示菜单
            _popupMenu?.Show(25, 26, 27);

            if (!loaded)
            {
                loaded = true;
                ((ICommand)Load)?.Execute(null);
            }
        }

    }

}
