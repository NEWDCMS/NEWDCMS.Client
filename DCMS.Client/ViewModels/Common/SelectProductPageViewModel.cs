using Acr.UserDialogs;
using DCMS.Client.Models;
using DCMS.Client.Models.Products;
using DCMS.Client.Models.WareHouses;
using DCMS.Client.Pages.Archive;
using DCMS.Client.Pages.Bills;
using DCMS.Client.Pages.Common;
using DCMS.Client.Pages.Market;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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

namespace DCMS.Client.ViewModels
{
    public class SelectProductPageViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        [Reactive] public bool DataVewEnable { get; set; }
        [Reactive] public bool NullViewEnable { get; set; } = true;
        [Reactive] public bool UsableQuantity { get; set; } = true;
        [Reactive] public bool ShowStockQty { get; set; } = true;
        [Reactive] public bool ShowGift { get; set; } = true;

        public EntityBase Bill { get; set; }
        public IReactiveCommand InitCatagory { get; }
        public IReactiveCommand SelectGiftsCommand { get; }

        [Reactive] public bool UsableFavorite { get; set; } = false;
        public ReactiveCommand<ProductModel, Unit> FavoriteCommand { get; }

        [Reactive] public CategoryModel Selecter { get; set; }

        public SelectProductPageViewModel(INavigationService navigationService,
            IProductService productService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "选择商品";

            _navigationService = navigationService;
            _dialogService = dialogService;
            _productService = productService;

            //只显库存商品
            this.WhenAnyValue(x => x.UsableQuantity)
               .Skip(1)
               .Subscribe(s =>
               {
                   ((ICommand)Load)?.Execute(null);
                   this.ForceRefresh = true;
               }).DisposeWith(DeactivateWith);

            //我的收藏
            this.WhenAnyValue(x => x.UsableFavorite)
               .Skip(1)
               .Subscribe(s =>
               {
                   ((ICommand)Load)?.Execute(null);
                   this.ForceRefresh = true;
               }).DisposeWith(DeactivateWith);

            //搜索
            this.WhenAnyValue(x => x.Filter.SerchKey)
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s)
                .Throttle(TimeSpan.FromSeconds(2), RxApp.MainThreadScheduler)
                .Subscribe(s =>
                {
                    ((ICommand)SerchCommand)?.Execute(s);
                }).DisposeWith(DeactivateWith);

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
            this.Load = ReactiveCommand.Create(async () =>
            {
                try
                {
                    ItemTreshold = 1;
                    DataVewEnable = false;
                    NullViewEnable = true;

                    var items = await GetProductsPage(0, 50);
                    if (items != null && items.Any())
                    {
                        //清除列表
                        ProductSeries?.Clear();
                        foreach (var item in items)
                        {
                            if (ProductSeries.Count(s => s.ProductId == item.ProductId) == 0)
                            {
                                ProductSeries.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex) { Crashes.TrackError(ex); }
                finally
                {
                    DataVewEnable = true;
                    NullViewEnable = false;
                }
            });

            //以增量方式加载数据
            this.ItemTresholdReachedCommand = ReactiveCommand.Create(async () =>
            {
                using (var dig = UserDialogs.Instance.Loading("加载中..."))
                {
                    try
                    {
                        int pageIdex = ProductSeries.Count / 50;
                        var items = await GetProductsPage(pageIdex, PageSize);
                        var previousLastItem = ProductSeries.Last();
                        if (items != null)
                        {
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
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                        ItemTreshold = -1;
                    }
                }
            }, this.WhenAny(x => x.ProductSeries, x => x.GetValue().Count > 0));


            //初始类别
            this.InitCatagory = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    var result = await _productService.GetAllCategoriesAsync(true, new System.Threading.CancellationToken());
                    if (result != null && result.Any())
                    {
                        var categories = result.ToList();
                        if (categories != null && categories.Any())
                        {
                            foreach (var op in categories)
                            {
                                op.SelectedCommand = ReactiveCommand.Create<int>(r =>
                                {
                                    op.Selected = !op.Selected;
                                });
                            }
                            this.BindCategories = new ObservableCollection<CategoryModel>(categories);
                        }
                    }
                }
                catch (Exception ex) { Crashes.TrackError(ex); }
            });
            //选择类别
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
             .Skip(1)
             .Where(x => x != null)
             .SubOnMainThread(x =>
             {
                 if (x != null)
                 {
                     Filter.CatagoryIds = new int[] { x.Id };
                     this.ForceRefresh = true;

                     foreach (var op in this.BindCategories)
                     {
                         op.Selected = false;
                     }

                     x.Selected = !x.Selected;

                    ((ICommand)Load)?.Execute(null);
                 }
                 Selecter = null;
             })
             .DisposeWith(DeactivateWith);


            this.WhenAnyValue(x => x.ReferencePage)
                .Where(x => !string.IsNullOrEmpty(x))
                .Subscribe(x =>
            {
                this.ShowStockQty = !(x.Equals("InventoryOPBillPage") || x.Equals("InventoryBillPage"));

            }).DisposeWith(DeactivateWith);

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
                    if (this.Bill == null)
                    {
                        this.Alert("单据不能关联，请确保参数");
                        return;
                    }

                    //添加调拨商品
                    await this.NavigateAsync(nameof(AddAllocationProductPage),
                        ("Bill", Bill),
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
            //收藏
            this.FavoriteCommand = ReactiveCommand.Create<ProductModel>(p =>
            {
                try
                {
                    var ps = Settings.FavoriteProducts;
                    this.Filter.SerchKey = "";
                    if (!p.Favorited)
                    {
                        if (!ps.Select(s => s.ProductId).Contains(p.ProductId))
                        {
                            p.Favorited = true;
                            ps.Add(p);
                            Settings.FavoriteProducts = ps;
                            _dialogService.ShortAlert("收藏成功！");
                        }
                    }
                    else
                    {
                        var cur = ps.Where(s => s.ProductId == p.ProductId).FirstOrDefault();
                        if (cur != null)
                        {
                            cur.Favorited = false;
                            p.Favorited = false;
                            ps.Remove(cur);
                            Settings.FavoriteProducts = ps;
                            _dialogService.ShortAlert("收藏已移除！");
                        }
                    }

                    if (UsableFavorite)
                    {
                        if (ProductSeries != null && ProductSeries.Any())
                        {
                            var reload = ProductSeries.Where(s => s.Favorited == true).ToList();
                            if (reload != null && reload.Any())
                                this.ProductSeries = new ObservableCollection<ProductModel>(reload);
                            else
                            {
                                this.UsableFavorite = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });

            this.BindBusyCommand(Load);

            this.Load.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.ItemTresholdReachedCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.InitCatagory.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.SelectGiftsCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.SubmitDataCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.FavoriteCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);

        }

        /// <summary>
        /// 分页获取商品
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IList<ProductModel>> GetProductsPage(int pageNumber, int pageSize)
        {
            var lists = new List<ProductModel>();
            try
            {
                Filter ??= new FilterModel();
                string key = string.IsNullOrEmpty(Filter.SerchKey) ? "" : Filter.SerchKey;
                var catagoryids = Filter.CatagoryIds ?? null;
                //bool usablequantity = true;

                //是否包含库存量商品  
                if (ReferencePage.Equals("PurchaseOrderBillPage")
                    || ReferencePage.Equals("CostContractBillPage")
                    || ReferencePage.Equals("ReturnBillPage")
                    || ReferencePage.Equals("ReturnOrderBillPage")
                    || ReferencePage.Equals("InventoryBillPage")
                    || ReferencePage.Equals("InventoryOPBillPage")
                    || ReferencePage.Equals("InventoryBillPage"))
                {
                    this.UsableQuantity = false;
                }

                if (Settings.WareHouseId != Filter.WareHouseId)
                    Settings.WareHouseId = Filter.WareHouseId;

                if (ReferencePage.Equals("ReturnBillPage") || ReferencePage.Equals("ReturnOrderBillPage"))
                {
                    Filter.WareHouseId = 0;
                    this.ForceRefresh = true;
                }
                //检索商品
                var results = await _productService.GetProductsAsync(catagoryids,
                    key,
                    Terminal.Id,
                    Filter.WareHouseId,
                    pageNumber,
                    pageSize,
                    this.UsableQuantity,
                    this.ForceRefresh,
                    new System.Threading.CancellationToken());

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

                        //当前仓库名称
                        p.CurWareHouseName = Filter.WareHouseName;
                        p.ShowCurWareHouseName = Filter.WareHouseId > 0;

                        //库存量转化
                        p.UsableQuantityConversion = p.FormatQuantity(p.UsableQuantity ?? 0);
                        p.CurrentQuantityConversion = p.FormatQuantity(p.CurrentQuantity ?? 0);
                        p.OrderQuantityConversion = p.FormatQuantity(p.OrderQuantity ?? 0);
                        p.LockQuantityConversion = p.FormatQuantity(p.LockQuantity ?? 0);


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

                    lists = results?.Data?.OrderBy(s => s.StockQty).ToList();

                    if (this.UsableQuantity)
                    {
                        lists = results?.Data?.Where(s => s.UsableQuantity > 0)
                            .OrderBy(s => s.StockQty)
                            .ToList();
                    }
                    else
                    {
                        lists = results?.Data?.OrderBy(s => s.StockQty)
                                .ToList();
                    }

                    var ps = Settings.FavoriteProducts;

                    //标记收藏状态
                    lists.ForEach(p =>
                    {
                        var f = ps.Select(s => s.Id).Contains(p.Id);
                        p.Favorited = f;
                    });

                    //过滤
                    if (this.UsableFavorite)
                    {
                        lists = lists.Where(s => s.Favorited == true).ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return lists;
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
                        this.ShowGift = false;
                        parameters.TryGetValue("Bill", out AllocationBillModel bill);
                        if (bill != null)
                            this.Bill = bill;
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
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            //载入类别
            ((ICommand)InitCatagory)?.Execute(null);

            //载入商品
            if (this.ProductSeries?.Count == 0 || ReferencePage == "InventoryOPBillPage")
                ((ICommand)Load)?.Execute(null);
        }
    }
}
