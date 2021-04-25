using Wesley.Client.Enums;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Models.WareHouses;
using Wesley.Client.Pages.Market;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Wesley.Client.ViewModels
{
    public class InventoryReportPageViewModel : ViewModelBaseCutom
    {

        [Reactive] public InventoryReportBillModel Bill { get; set; } = new InventoryReportBillModel();
        public ReactiveCommand<object, Unit> AddProductCommand { get; }
        [Reactive] public InventoryReportItemModel Selecter { get; set; }

        public InventoryReportPageViewModel(INavigationService navigationService,
                 IProductService productService,
                 ITerminalService terminalService,
                 IUserService userService,
                 IWareHousesService wareHousesService,
                 IAccountingService accountingService,


                 IDialogService dialogService) : base(navigationService,
                     productService,
                     terminalService,
                     userService,
                     wareHousesService,
                     accountingService,


                     dialogService)
        {
            Title = "库存上报";

            this.Bill = new InventoryReportBillModel()
            {
                BusinessUserId = Settings.UserId,
                BillNumber = CommonHelper.GetBillNumber("KCSB", Settings.StoreId),
                CreatedOnUtc = DateTime.Now
            };

            //编辑项目
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
           .Skip(1)
           .Where(x => x != null)
           .SubOnMainThread(async item =>
          {
              await this.NavigateAsync("AddReportProductPage", ("InventoryReportItemModel", item));
              this.Selecter = null;
          });

            //添加商品
            this.AddProductCommand = ReactiveCommand.Create<object>(async e =>
           {
               if (Bill.TerminalId == 0)
               {
                   _dialogService.ShortAlert("请选择客户！");
                   return;
               }
               await this.NavigateAsync("SelectProductPage", ("Reference", $"{nameof(InventoryReportPage)}"), ("SerchKey", Filter.SerchKey));
           });

            //验证
            var valid_TerminalId = this.ValidationRule(x => x.Bill.TerminalId, _isZero, "客户未指定");
            var valid_BusinessUserId = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "商品未指定");


            //提交
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                await this.Access(AccessGranularityEnum.StockReportSave);

                Bill.StoreId = Settings.StoreId;
                Bill.BusinessUserId = Settings.UserId;
                Bill.ReversedUserId = 0;
                Bill.ReversedStatus = false;

                return await SubmitAsync(Bill, 0, _wareHousesService.CreateOrUpdateAsync, (result) =>
                 {
                     Bill = new InventoryReportBillModel();
                 }, token: cts.Token);
            },
            this.IsValid());

            //菜单选择
            this.SetMenus((x) =>
            {
                switch (x)
                {
                    case Enums.MenuEnum.CLEAR:
                        {
                            ClearForm(() =>
                            {
                                Bill.Items.Clear();
                            });
                        }
                        break;
                }
            }, 4);


            this.AddProductCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));


            this.ExceptionsSubscribe();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                //选择客户
                if (parameters.ContainsKey("Terminaler"))
                {
                    parameters.TryGetValue("Terminaler", out TerminalModel terminaler);
                    Bill.BusinessUserId = Bill.BusinessUserId;
                    Bill.BillNumber = Bill.BillNumber;
                    Bill.TerminalId = terminaler != null ? terminaler.Id : 0;
                    Bill.TerminalName = terminaler != null ? terminaler.Name : "";
                }

                //选择商品序列
                if (parameters.ContainsKey("ProductSelect"))
                {
                    parameters.TryGetValue("ProductSelect", out InventoryReportItemModel product);
                    if (product != null)
                    {
                        if (!Bill.Items.Select(p => p.ProductId).Contains(product.ProductId))
                        {
                            product.BigStoreQuantity = product.InventoryReportStoreQuantities.Select(b => b.BigStoreQuantity).Sum();
                            product.SmallStoreQuantity = product.InventoryReportStoreQuantities.Select(b => b.SmallStoreQuantity).Sum();
                            Bill.Items.Add(product);
                        }
                        else
                        {
                            var p = Bill.Items.Where(x => x.ProductId == product.ProductId).FirstOrDefault();
                            p.SmallQuantity = product.SmallQuantity;
                            p.BigQuantity = product.BigQuantity;
                            p.SmallStoreQuantity = product.InventoryReportStoreQuantities.Select(b => b.SmallStoreQuantity).Sum();
                            p.BigStoreQuantity = product.InventoryReportStoreQuantities.Select(b => b.BigStoreQuantity).Sum();
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
