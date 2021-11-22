using Acr.UserDialogs;
using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Finances;
using Wesley.Client.Models.Purchases;
using Wesley.Client.Models.Sales;
using Wesley.Client.Models.WareHouses;
using Wesley.Client.Pages;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Wesley.Client.Models.Products;
using Wesley.Client.Models.Terminals;
using System.Collections.Generic;

namespace Wesley.Client.ViewModels
{
    public class SystemSettingPageViewModel : ViewModelBaseCutom
    {
        [Reactive] public SaleBillModel SaleBill { get; set; } = new SaleBillModel();
        [Reactive] public SaleReservationBillModel SaleReservationBill { get; set; } = new SaleReservationBillModel();
        [Reactive] public ReturnBillModel ReturnBill { get; set; } = new ReturnBillModel();
        [Reactive] public ReturnReservationBillModel ReturnReservationBill { get; set; } = new ReturnReservationBillModel();
        [Reactive] public ExchangeBillModel ExchangeBill { get; set; } = new ExchangeBillModel();
        [Reactive] public AllocationBillModel AllocationBill { get; set; } = new AllocationBillModel();
        [Reactive] public CashReceiptBillModel CashReceiptBill { get; set; } = new CashReceiptBillModel();
        [Reactive] public PurchaseBillModel PurchaseBill { get; set; } = new PurchaseBillModel();
        [Reactive] public InventoryPartTaskBillModel InventoryPartTaskBill { get; set; } = new InventoryPartTaskBillModel();
        [Reactive] public CostExpenditureBillModel CostExpenditureBill { get; set; } = new CostExpenditureBillModel();
        [Reactive] public CostContractBillModel CostContractBill { get; set; } = new CostContractBillModel();
        [Reactive] public AdvanceReceiptBillModel AdvanceReceiptBill { get; set; } = new AdvanceReceiptBillModel();
        [Reactive] public ObservableCollection<AbstractBill> PrintModules { get; set; } = new ObservableCollection<AbstractBill>();
        public ReactiveCommand<string, Unit> StockSelected { get; set; }
        public ReactiveCommand<int, Unit> Add { get; set; }
        public ReactiveCommand<int, Unit> Remove { get; set; }
        [Reactive] public bool IsAutoAsyncProducts { get; set; }
        [Reactive] public bool IsAutoAsyncTerminals { get; set; }

        private readonly ILiteDbService<TerminalModel>  _tliteDb;
        private readonly static object _lock = new object();


        public SystemSettingPageViewModel(INavigationService navigationService,
          IProductService productService,
          IUserService userService,
          ITerminalService terminalService,
          IWareHousesService wareHousesService,
          IAccountingService accountingService,
          ILiteDbService<TerminalModel> tliteDb,
          IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "系统设置";

            _tliteDb = tliteDb;

            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(() =>
            {
                try
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        //初始
                        this.SaleBill = Settings.SaleBill ?? new SaleBillModel();
                        this.SaleReservationBill = Settings.SaleReservationBill ?? new SaleReservationBillModel();
                        this.ReturnBill = Settings.ReturnBill ?? new ReturnBillModel();
                        this.ReturnReservationBill = Settings.ReturnReservationBill ?? new ReturnReservationBillModel();
                        this.ExchangeBill = Settings.ExchangeBill ?? new ExchangeBillModel();

                        this.AllocationBill = Settings.AllocationBill ?? new AllocationBillModel();
                        this.CashReceiptBill = Settings.CashReceiptBill ?? new CashReceiptBillModel();
                        this.PurchaseBill = Settings.PurchaseBill ?? new PurchaseBillModel();
                        this.InventoryPartTaskBill = Settings.InventoryPartTaskBill ?? new InventoryPartTaskBillModel();
                        this.CostExpenditureBill = Settings.CostExpenditureBill ?? new CostExpenditureBillModel();
                        this.CostContractBill = Settings.CostContractBill ?? new CostContractBillModel();
                        this.AdvanceReceiptBill = Settings.AdvanceReceiptBill ?? new AdvanceReceiptBillModel();


                        this.SaleBill.BillTypeName = "销售单";
                        this.SaleBill.BillType = BillTypeEnum.SaleBill;
                        this.SaleBill.BillTypeId = (int)BillTypeEnum.SaleBill;
                        this.SaleBill.Navigation = "SaleBillPage";

                        this.SaleReservationBill.BillTypeName = "销售订单";
                        this.SaleReservationBill.BillType = BillTypeEnum.SaleReservationBill;
                        this.SaleReservationBill.BillTypeId = (int)BillTypeEnum.SaleReservationBill;
                        this.SaleReservationBill.Navigation = "SaleOrderBillPage";

                        this.ReturnBill.BillTypeName = "退货单";
                        this.ReturnBill.BillType = BillTypeEnum.ReturnBill;
                        this.ReturnBill.BillTypeId = (int)BillTypeEnum.ReturnBill;
                        this.ReturnBill.Navigation = "ReturnBillPage";

                        this.ReturnReservationBill.BillTypeName = "退货订单";
                        this.ReturnReservationBill.BillType = BillTypeEnum.ReturnReservationBill;
                        this.ReturnReservationBill.BillTypeId = (int)BillTypeEnum.ReturnReservationBill;
                        this.ReturnReservationBill.Navigation = "ReturnOrderBillPage";

                        this.ExchangeBill.BillTypeName = "换货单";
                        this.ExchangeBill.BillType = BillTypeEnum.ExchangeBill;
                        this.ExchangeBill.BillTypeId = (int)BillTypeEnum.ExchangeBill;
                        this.ExchangeBill.Navigation = "ExchangeBillPage";

                        this.AllocationBill.BillTypeName = "调拨单";
                        this.AllocationBill.BillType = BillTypeEnum.AllocationBill;
                        this.AllocationBill.BillTypeId = (int)BillTypeEnum.AllocationBill;
                        this.AllocationBill.Navigation = "AllocationBillPage";

                        this.CashReceiptBill.BillTypeName = "收款单";
                        this.CashReceiptBill.BillType = BillTypeEnum.CashReceiptBill;
                        this.CashReceiptBill.BillTypeId = (int)BillTypeEnum.CashReceiptBill;
                        this.CashReceiptBill.Navigation = "ReceiptBillPage";

                        this.PurchaseBill.BillTypeName = "采购单";
                        this.PurchaseBill.BillType = BillTypeEnum.PurchaseBill;
                        this.PurchaseBill.BillTypeId = (int)BillTypeEnum.PurchaseBill;
                        this.PurchaseBill.Navigation = "PurchaseOrderBillPage";

                        this.InventoryPartTaskBill.BillTypeName = "盘点单";
                        this.InventoryPartTaskBill.BillType = BillTypeEnum.InventoryPartTaskBill;
                        this.InventoryPartTaskBill.BillTypeId = (int)BillTypeEnum.InventoryPartTaskBill;
                        this.InventoryPartTaskBill.Navigation = "InventoryOPBillPage";

                        this.CostExpenditureBill.BillTypeName = "费用支付单";
                        this.CostExpenditureBill.BillType = BillTypeEnum.CostExpenditureBill;
                        this.CostExpenditureBill.BillTypeId = (int)BillTypeEnum.CostExpenditureBill;
                        this.CostExpenditureBill.Navigation = "CostExpenditureBillPage";

                        this.CostContractBill.BillTypeName = "费用合同单";
                        this.CostContractBill.BillType = BillTypeEnum.CostContractBill;
                        this.CostContractBill.BillTypeId = (int)BillTypeEnum.CostContractBill;
                        this.CostContractBill.Navigation = "CostContractBillPage";

                        this.AdvanceReceiptBill.BillTypeName = "预收款单";
                        this.AdvanceReceiptBill.BillType = BillTypeEnum.AdvanceReceiptBill;
                        this.AdvanceReceiptBill.BillTypeId = (int)BillTypeEnum.AdvanceReceiptBill;
                        this.AdvanceReceiptBill.Navigation = "AdvanceReceiptBillPage";


                        this.PrintModules = new ObservableCollection<AbstractBill>()
                        {
                            this.SaleBill,
                            this.SaleReservationBill,
                            this.ReturnBill,
                            this.ReturnReservationBill,
                            this.ExchangeBill,
                            this.AllocationBill,
                            this.CashReceiptBill,
                            this.PurchaseBill,
                            this.InventoryPartTaskBill,
                            this.CostExpenditureBill,
                            this.CostContractBill,
                            this.AdvanceReceiptBill
                        };
                    });
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }));

            //选择默认仓库
            this.StockSelected = ReactiveCommand.CreateFromTask<string>((type) => Task.Run(() =>
           {
               if (string.IsNullOrEmpty(type))
               {
                   _dialogService.ShortAlert($"参数错误！");
                   return;
               }
                //注意：设置以Id订阅，Column 一定先于赋值
                var btype = (BillTypeEnum)int.Parse(type);
               switch (btype)
               {
                   case BillTypeEnum.SaleBill:
                       {
                           var sb = this.SaleBill;
                           GetSelectStock(btype, (result) =>
                           {
                               if (result != null && sb != null)
                               {
                                   if (result.Id > 0)
                                   {
                                       sb.WareHouseId = result.Id;
                                       sb.WareHouseName = result.Column;
                                       Settings.SaleBill = sb;
                                   }
                               }
                           });
                       }
                       break;
                   case BillTypeEnum.SaleReservationBill:
                       {
                           var sb = this.SaleReservationBill;
                           GetSelectStock(btype, (result) =>
                           {
                               if (result != null && sb != null)
                               {
                                   if (result.Id > 0)
                                   {
                                       sb.WareHouseId = result.Id;
                                       sb.WareHouseName = result.Column;
                                       Settings.SaleReservationBill = sb;
                                   }
                               }
                           });
                       }
                       break;
                   case BillTypeEnum.ReturnBill:
                       {
                           var sb = this.ReturnBill;
                           GetSelectStock(btype, (result) =>
                             {
                               if (result != null && sb != null)
                               {
                                   if (result.Id > 0)
                                   {
                                       sb.WareHouseId = result.Id;
                                       sb.WareHouseName = result.Column;
                                       Settings.ReturnBill = sb;
                                   }
                               }
                           });
                       }
                       break;
                   case BillTypeEnum.ReturnReservationBill:
                       {
                           var sb = this.ReturnReservationBill;
                           GetSelectStock(btype, (result) =>
                           {
                               if (result != null && sb != null)
                               {
                                   if (result.Id > 0)
                                   {
                                       sb.WareHouseId = result.Id;
                                       sb.WareHouseName = result.Column;
                                       Settings.ReturnReservationBill = sb;
                                   }
                               }
                           });

                       }
                       break;
                   case BillTypeEnum.ExchangeBill:
                       {
                           var sb = this.ExchangeBill;
                           GetSelectStock(btype, (result) =>
                           {
                               if (result != null && sb != null)
                               {
                                   if (result.Id > 0)
                                   {
                                       sb.WareHouseId = result.Id;
                                       sb.WareHouseName = result.Column;
                                       Settings.ExchangeBill = sb;
                                   }
                               }
                           });

                       }
                       break;
               }
           }));

            //增加次数
            this.Add = ReactiveCommand.CreateFromTask<int>((type) => Task.Run(() =>
           {
               if (type == 0)
               {
                   _dialogService.ShortAlert($"参数错误！");
                   return;
               }

               var btype = (BillTypeEnum)type;
               switch (btype)
               {
                   case BillTypeEnum.SaleBill:
                       {
                           var sb = this.SaleBill;
                           sb.PrintNum++;
                           if (sb.PrintNum >= 4) sb.PrintNum = 4;
                           Settings.SaleBill = sb;
                       }
                       break;
                   case BillTypeEnum.SaleReservationBill:
                       {
                           var sb = this.SaleReservationBill;
                           sb.PrintNum++;
                           if (sb.PrintNum >= 4) sb.PrintNum = 4;
                           Settings.SaleReservationBill = sb;
                       }
                       break;
                   case BillTypeEnum.ReturnBill:
                       {
                           var sb = this.ReturnBill;
                           sb.PrintNum++;
                           if (sb.PrintNum >= 4) sb.PrintNum = 4;
                           Settings.ReturnBill = sb;
                       }
                       break;
                   case BillTypeEnum.ReturnReservationBill:
                       {
                           var sb = this.ReturnReservationBill;
                           sb.PrintNum++;
                           if (sb.PrintNum >= 4) sb.PrintNum = 4;
                           Settings.ReturnReservationBill = sb;
                       }
                       break;
                   case BillTypeEnum.ExchangeBill:
                       {
                           var sb = this.ExchangeBill;
                           sb.PrintNum++;
                           if (sb.PrintNum >= 4) sb.PrintNum = 4;
                           Settings.ExchangeBill = sb;
                       }
                       break;
                   //
                   case BillTypeEnum.AllocationBill:
                       {
                           var sb = this.AllocationBill;
                           sb.PrintNum++;
                           if (sb.PrintNum >= 4) sb.PrintNum = 4;
                           Settings.AllocationBill = sb;
                       }
                       break;
                   case BillTypeEnum.CashReceiptBill:
                       {
                           var sb = this.CashReceiptBill;
                           sb.PrintNum++;
                           if (sb.PrintNum >= 4) sb.PrintNum = 4;
                           Settings.CashReceiptBill = sb;
                       }
                       break;
                   case BillTypeEnum.PurchaseBill:
                       {
                           var sb = this.PurchaseBill;
                           sb.PrintNum++;
                           if (sb.PrintNum >= 4) sb.PrintNum = 4;
                           Settings.PurchaseBill = sb;
                       }
                       break;
                   case BillTypeEnum.InventoryPartTaskBill:
                       {
                           var sb = this.InventoryPartTaskBill;
                           sb.PrintNum++;
                           if (sb.PrintNum >= 4) sb.PrintNum = 4;
                           Settings.InventoryPartTaskBill = sb;
                       }
                       break;
                   case BillTypeEnum.CostExpenditureBill:
                       {
                           var sb = this.CostExpenditureBill;
                           sb.PrintNum++;
                           if (sb.PrintNum >= 4) sb.PrintNum = 4;
                           Settings.CostExpenditureBill = sb;
                       }
                       break;
                   case BillTypeEnum.CostContractBill:
                       {
                           var sb = this.CostContractBill;
                           sb.PrintNum++;
                           if (sb.PrintNum >= 4) sb.PrintNum = 4;
                           Settings.CostContractBill = sb;
                       }
                       break;
                   case BillTypeEnum.AdvanceReceiptBill:
                       {
                           var sb = this.AdvanceReceiptBill;
                           sb.PrintNum++;
                           if (sb.PrintNum >= 4) sb.PrintNum = 4;
                           Settings.AdvanceReceiptBill = sb;
                       }
                       break;
               }
           }));

            //减少次数
            this.Remove = ReactiveCommand.CreateFromTask<int>((type) => Task.Run(() =>
           {
               if (type == 0)
               {
                   _dialogService.ShortAlert($"参数错误！");
                   return;
               }

               var btype = (BillTypeEnum)type;
               switch (btype)
               {
                   case BillTypeEnum.SaleBill:
                       {
                           var sb = this.SaleBill;
                           sb.PrintNum--;
                           if (sb.PrintNum < 0) sb.PrintNum = 0;
                           Settings.SaleBill = sb;
                       }
                       break;
                   case BillTypeEnum.SaleReservationBill:
                       {
                           var sb = this.SaleReservationBill;
                           sb.PrintNum--;
                           if (sb.PrintNum < 0) sb.PrintNum = 0;
                           Settings.SaleReservationBill = sb;
                       }
                       break;
                   case BillTypeEnum.ReturnBill:
                       {
                           var sb = this.ReturnBill;
                           sb.PrintNum--;
                           if (sb.PrintNum < 0) sb.PrintNum = 0;
                           Settings.ReturnBill = sb;
                       }
                       break;
                   case BillTypeEnum.ReturnReservationBill:
                       {
                           var sb = this.ReturnReservationBill;
                           sb.PrintNum--;
                           if (sb.PrintNum < 0) sb.PrintNum = 0;
                           Settings.ReturnReservationBill = sb;
                       }
                       break;
                   case BillTypeEnum.ExchangeBill:
                       {
                           var sb = this.ExchangeBill;
                           sb.PrintNum--;
                           if (sb.PrintNum < 0) sb.PrintNum = 0;
                           Settings.ExchangeBill = sb;
                       }
                       break;
                   //
                   case BillTypeEnum.AllocationBill:
                       {
                           var sb = this.AllocationBill;
                           sb.PrintNum--;
                           if (sb.PrintNum < 0) sb.PrintNum = 0;
                           Settings.AllocationBill = sb;
                       }
                       break;
                   case BillTypeEnum.CashReceiptBill:
                       {
                           var sb = this.CashReceiptBill;
                           sb.PrintNum--;
                           if (sb.PrintNum < 0) sb.PrintNum = 0;
                           Settings.CashReceiptBill = sb;
                       }
                       break;
                   case BillTypeEnum.PurchaseBill:
                       {
                           var sb = this.PurchaseBill;
                           sb.PrintNum--;
                           if (sb.PrintNum < 0) sb.PrintNum = 0;
                           Settings.PurchaseBill = sb;
                       }
                       break;
                   case BillTypeEnum.InventoryPartTaskBill:
                       {
                           var sb = this.InventoryPartTaskBill;
                           sb.PrintNum--;
                           if (sb.PrintNum < 0) sb.PrintNum = 0;
                           Settings.InventoryPartTaskBill = sb;
                       }
                       break;
                   case BillTypeEnum.CostExpenditureBill:
                       {
                           var sb = this.CostExpenditureBill;
                           sb.PrintNum--;
                           if (sb.PrintNum < 0) sb.PrintNum = 0;
                           Settings.CostExpenditureBill = sb;
                       }
                       break;
                   case BillTypeEnum.CostContractBill:
                       {
                           var sb = this.CostContractBill;
                           sb.PrintNum--;
                           if (sb.PrintNum < 0) sb.PrintNum = 0;
                           Settings.CostContractBill = sb;
                       }
                       break;
                   case BillTypeEnum.AdvanceReceiptBill:
                       {
                           var sb = this.AdvanceReceiptBill;
                           sb.PrintNum--;
                           if (sb.PrintNum < 0) sb.PrintNum = 0;
                           Settings.AdvanceReceiptBill = sb;
                       }
                       break;
               }
           }));

            //商品
            this.WhenAnyValue(x => x.IsAutoAsyncProducts)
            .Skip(1)
            .Subscribe(x => 
            {
                if (x)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        using (var dig = UserDialogs.Instance.Loading("同步商品..."))
                        {
                            await Task.Delay(3000);
                            //TODO....
                        }
                        this.IsAutoAsyncProducts = false;
                    });
                }

            }).DisposeWith(DeactivateWith);

            //同步终端
            this.WhenAnyValue(x => x.IsAutoAsyncTerminals)
            .Skip(1)
            .Subscribe(x => 
            {
                if (x)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        using (var dig = UserDialogs.Instance.Loading("同步终端..."))
                        {
                            await Task.Delay(3000);
                            //TODO....
                        }
                        this.IsAutoAsyncProducts = false;
                    });
                }
            }).DisposeWith(DeactivateWith);

            this.BindBusyCommand(Load);
        }
        public async void GetSelectStock(BillTypeEnum btype, Action<PopData> call)
        {
            var _dialogView = new PopRadioButtonPage("选择库存", "", async () =>
            {
                var results = await _wareHousesService.GetWareHousesAsync(btype, force: this.ForceRefresh);
                if (results != null && results.Any())
                {
                    var popDatas = results?.Select(s =>
                    {
                        return new PopData
                        {
                            Id = s?.Id ?? 0,
                            Column = s?.Name
                        };
                    })?.ToList();
                    return popDatas;
                }
                else
                {
                    return null;
                }
            });
            _dialogView.Completed += (sender, result) =>
            {
                try { 
                if (result != null)
                {
                    call?.Invoke(result);
                }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            };
            await PopupNavigation.Instance.PushAsync(_dialogView);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }

    }
}
