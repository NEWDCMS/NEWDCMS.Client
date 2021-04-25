using Acr.UserDialogs;
using Wesley.Client.Models;
using Wesley.Client.Models.Products;
using Wesley.Client.Models.WareHouses;
using Wesley.Client.Pages.Archive;
using Wesley.Client.Pages.Bills;
using Wesley.Client.Pages.Common;
using Wesley.Client.Pages.Market;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{
    public class SelectProductPageViewModel : ViewModelBase
    {
        private readonly IProductService _productService;

        [Reactive] public bool ShowStockQty { get; set; } = true;
        [Reactive] public bool ShowGift { get; set; } = true;

        public EntityBase Bill { get; set; }
        public IReactiveCommand InitCatagory { get; }
        public IReactiveCommand SelectGiftsCommand { get; }
        [Reactive] public ProductModel Selecter { get; set; }
        public SelectProductPageViewModel(INavigationService navigationService,
            IProductService productService,


            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "选择商品";
            _navigationService = navigationService;
            _dialogService = dialogService;
            _productService = productService;

            //搜索
            this.WhenAnyValue(x => x.Filter.SerchKey)
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s)
                .Throttle(TimeSpan.FromSeconds(2), RxApp.MainThreadScheduler)
                .Subscribe(s =>
                {
                    ((ICommand)SerchCommand)?.Execute(s);
                }).DisposeWith(DestroyWith);
            this.SerchCommand = ReactiveCommand.Create<string>(e =>
            {
                if (string.IsNullOrEmpty(Filter.SerchKey))
                {
                    this.Alert("请输入关键字！");
                    return;
                }
                ((ICommand)Load)?.Execute(null);
            });

            //Load
            this.Load = ProductSeriesLoader.Load(async () =>
            {
                //重载时排它
                ItemTreshold = 1;

                var items = await GetProductsPage(0, PageSize);

                //清除列表
                ProductSeries.Clear();
                foreach (var item in items)
                {
                    if (ProductSeries.Count(s => s.ProductId == item.ProductId) == 0)
                    {
                        ProductSeries.Add(item);
                    }
                }

                return this.ProductSeries;
            });

            //以增量方式加载数据
            this.ItemTresholdReachedCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                using (var dig = UserDialogs.Instance.Loading("加载中..."))
                {
                    try
                    {
                        int pageIdex = ProductSeries.Count / (PageSize == 0 ? 1 : PageSize);
                        var items = await GetProductsPage(pageIdex, PageSize);
                        var previousLastItem = ProductSeries.Last();
                        foreach (var item in items)
                        {
                            if (ProductSeries.Count(s => s.ProductId == item.ProductId) == 0)
                            {
                                ProductSeries.Add(item);
                            }
                        }

                        if (items.Count() == 0 || items.Count() == ProductSeries.Count)
                        {
                            ItemTreshold = -1;
                            return this.ProductSeries;
                        }

                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                        ItemTreshold = -1;
                    }

                    return this.ProductSeries;
                }

            }, this.WhenAny(x => x.ProductSeries, x => x.GetValue().Count > 0));

            //初始类别
            this.InitCatagory = ReactiveCommand.Create(() =>
            {
                Sync.Run(async () =>
                {
                    var result = await _productService.GetAllCategoriesAsync(this.ForceRefresh, calToken: cts.Token);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (result != null)
                        {
                            BindCategories = new ObservableCollection<CategoryModel>(result.ToList());
                        }
                    });
                }, (ex) => { Crashes.TrackError(ex); });
            },
            this.WhenAny(x => x.BindCategories, x => x.GetValue().Count == 0));

            //菜单选择
            subProductCatagoryBus = MessageBus.Current.Listen<List<CategoryModel>>(string.Format(Constants.PRODUCT_MENU_KEY, this.GetType().FullName))
                .Subscribe(x =>
                {
                    Filter.CatagoryIds = x.Select(s => s.Id).ToArray();
                    ((ICommand)Load)?.Execute(null);
                });

            this.WhenAnyValue(x => x.ReferencePage)
                .Where(x => !string.IsNullOrEmpty(x))
                .Subscribe(x =>
            {
                this.ShowStockQty = !(x.Equals("InventoryOPBillPage") || x.Equals("InventoryBillPage"));
            });

            //选择礼品
            this.SelectGiftsCommand = ReactiveCommand.Create(async () =>
            {
                await this.NavigateAsync("SelectGiftsPage", ("WareHouse", WareHouse), ("Terminaler", this.Terminal));
            });

            //保存选择商品
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object>(async e =>
            {
                if (string.IsNullOrEmpty(ReferencePage))
                {
                    await _navigationService.GoBackAsync();
                    return;
                }

                var products = ProductSeries.Select(p => p).Where(p => p.Selected == true).ToList();
                if (products.Count == 0)
                {
                    this.Alert("请选择商品项目");
                    return;
                }

                if (ReferencePage == nameof(InventoryReportPage))
                {
                    //添加上报商品
                    await this.NavigateAsync(nameof(AddReportProductPage),
                        ("Reference", ReferencePage),
                        ("Products", products));
                }
                else if (ReferencePage == nameof(AllocationBillPage))
                {
                    //添加调拨商品
                    await this.NavigateAsync(nameof(AddAllocationProductPage),
                        ("Reference", ReferencePage),
                        ("Products", products));
                }
                else if (ReferencePage == nameof(BackStockBillPage))
                {
                    //添加回库调拨单商品
                    await this.NavigateAsync(nameof(AddBackStockBillPage),
                        ("Reference", ReferencePage),
                        ("Products", products));
                }
                else if (ReferencePage == nameof(TrackAllocationBillPage))
                {
                    //添加回库调拨单商品
                    await this.NavigateAsync(nameof(AddAllocationProductPage),
                        ("Reference", ReferencePage),
                        ("Products", products));
                }
                else if (ReferencePage == nameof(CostContractBillPage))
                {
                    //添加合同商品
                    await this.NavigateAsync(nameof(AddCostContractProductPage),
                        ("Reference", ReferencePage),
                        ("Products", products));
                }
                else if (ReferencePage == nameof(InventoryOPBillPage) || ReferencePage == nameof(InventoryBillPage))
                {
                    //追加商品
                    if (TempProductSeries.Any())
                    {
                        var temps = TempProductSeries.ToList();
                        foreach (var tp in temps)
                        {
                            if (products.Where(s => s.ProductId == tp.ProductId).Count() == 0)
                            {
                                products.Add(tp);
                            }
                        }
                    }

                    //添加盘点商品
                    await this.NavigateAsync(nameof(AddInventoryProductPage),
                        ("Reference", ReferencePage),
                        ("WareHouse", WareHouse),
                        ("Products", products));
                }
                else if (ReferencePage == nameof(FilterPage))
                {
                    //过滤
                    await _navigationService.GoBackAsync(
                        ("Filter", Filter),
                        ("Reference", ReferencePage),
                        ("Products", products));
                }
                else if (ReferencePage == nameof(PurchaseOrderBillPage))
                {
                    //添加采购商品
                    await this.NavigateAsync(nameof(AddPurchaseProductPage),
                        ("WareHouse", WareHouse),
                        ("Reference", ReferencePage),
                        ("Products", products)
                    );
                }
                else if (ReferencePage == nameof(ExchangeBillPage))
                {
                    //添加换货商品
                    await this.NavigateAsync(nameof(AddExchangeProductPage),
                        ("WareHouse", WareHouse),
                        ("Reference", ReferencePage),
                        ("Products", products)
                    );
                }
                else
                {
                    //添加单据商品
                    await this.NavigateAsync($"{nameof(AddProductPage)}",
                        ("WareHouse", WareHouse),
                        ("Reference", ReferencePage),
                        ("Products", products)
                    );
                }

            });

            //选择商品
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
             .Skip(1)
             .Where(x => x != null)
             .SubOnMainThread(item =>
            {
                if (!ReferencePage.Equals("PurchaseOrderBillPage")
                  && !ReferencePage.Equals("FilterPage")
                  && !ReferencePage.Equals(nameof(CostContractBillPage))
                  && !ReferencePage.Equals("InventoryOPBillPage"))
                {
                    if (!item.StockQty.HasValue || item.StockQty.Value == 0)
                    {
                        this.Alert("零库存商品无效！");
                        item.Selected = false;
                        return;
                    }

                    item.Selected = !item.Selected;
                }
                else if (ReferencePage.Equals("InventoryReportPage"))
                {
                    item.Selected = !item.Selected;
                }
                else
                {
                    item.Selected = !item.Selected;
                }

                Selecter = null;
            });

            this.InitCatagory.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.Load.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.ItemTresholdReachedCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        /// <summary>
        /// 分页获取商品
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IList<ProductModel>> GetProductsPage(int pageNumber, int pageSize)
        {
            string key = string.IsNullOrEmpty(Filter.SerchKey) ? "" : Filter.SerchKey;
            var catagoryids = Filter.CatagoryIds ?? null;

            bool usablequantity = true;

            //是否包含库存量商品 
            if (ReferencePage.Equals("PurchaseOrderBillPage")
                || ReferencePage.Equals("CostContractBillPage")
                || ReferencePage.Equals("InventoryBillPage")
                || ReferencePage.Equals("InventoryOPBillPage")
                || ReferencePage.Equals("InventoryBillPage"))
            {
                usablequantity = false;
            }

            if (!string.IsNullOrEmpty(key))
                if (Settings.WareHouseId != Filter.WareHouseId)
                {
                    Settings.WareHouseId = Filter.WareHouseId;
                }

            //检索商品
            var results = await _productService.GetProductsAsync(catagoryids,
                key,
                null,
                Filter.WareHouseId,
                pageNumber,
                pageSize,
                usablequantity, this.ForceRefresh, calToken: cts.Token);

            if (results != null && (results?.Data.Any() ?? false))
            {
                foreach (var p in results?.Data)
                {
                    p.BigUnitId = p.BigProductPrices.UnitId;
                    p.BigPriceUnit = new PriceUnit()
                    {
                        UnitId = p.bigOption.Id,
                        Amount = 0,
                        //默认绑定批发价
                        Price = p.BigProductPrices.TradePrice ?? 0,
                        Quantity = 0,
                        Remark = "",
                        UnitName = p.bigOption.Name
                    };

                    p.SmallUnitId = p.SmallProductPrices.UnitId;
                    p.SmallPriceUnit = new PriceUnit()
                    {
                        UnitId = p.smallOption.Id,
                        Amount = 0,
                        //默认绑定批发价
                        Price = p.SmallProductPrices.TradePrice ?? 0,
                        Quantity = 0,
                        Remark = "",
                        UnitName = p.smallOption.Name
                    };

                    var currentStock = p.StockQuantities.Where(w => w.WareHouseId == Filter.WareHouseId).FirstOrDefault();
                    p.StockQty = currentStock != null ? currentStock.UsableQuantity : p.StockQty;
                    if (p.StockQty == 0)
                    {
                        p.StockQty = p.StockQuantities?.Sum(s => s.UsableQuantity);
                    }

                    p.CurWareHouseName = Filter.WareHouseName;

                    p.IsShowStock = ShowStockQty;
                    p.IsShowGiveEnabled = !ReferencePage.Equals("PurchaseOrderBillPage");

                    //选择命令
                    p.SelectCommand = ReactiveCommand.Create<ProductModel>(e =>
                    {
                        if (e == null)
                            return;

                        if ((!e.StockQty.HasValue || e.StockQty.Value == 0) && !ReferencePage.Equals("InventoryOPBillPage"))
                        {
                            this.Alert("零库存商品无效！");
                            e.Selected = false;
                            return;
                        }
                        else
                        {
                            e.Selected = !e.Selected;
                        }
                    });

                }
            }

            return results?.Data?.OrderBy(s => s.StockQty).ToList();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            try
            {
                if (parameters.ContainsKey("Bill"))
                {
                    if (ReferencePage.Equals("AllocationBillPage"))
                    {
                        parameters.TryGetValue("Bill", out AllocationBillModel Bill);
                    }
                }

                if (ReferencePage == "InventoryOPBillPage")
                {
                    ShowStockQty = false;
                    ShowGift = false;
                }

                if (ReferencePage == "PurchaseOrderBillPage")
                {
                    ShowStockQty = true;
                    ShowGift = false;
                }

                //盘点单
                if (ReferencePage == "InventoryOPBillPage")
                {
                    ProductSeries.ToList().ForEach(p =>
                    {
                        TempProductSeries.Add(p);
                    });
                }

                //载入商品
                if (this.ProductSeries?.Count == 0 || ReferencePage == "InventoryOPBillPage")
                    ((ICommand)Load)?.Execute(null);

                //载入类别
                ((ICommand)InitCatagory)?.Execute(null);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
