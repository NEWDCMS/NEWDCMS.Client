using Acr.UserDialogs;
using Wesley.Client.Models.Products;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;


namespace Wesley.Client.ViewModels
{
    public class ProductArchivesPageViewModel : ViewModelBaseCutom
    {

        public new ReactiveCommand<CollectionView, Unit> ItemSelectedCommand { get; }
        public ReactiveCommand<object, Unit> CatagorySelected { get; }
        [Reactive] public ProductModel Selecter { get; set; }


        public ProductArchivesPageViewModel(INavigationService navigationService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
              IDialogService dialogService
            ) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "商品档案";

            _navigationService = navigationService;
            _dialogService = dialogService;
            _terminalService = terminalService;
            _productService = productService;
            _userService = userService;
            _wareHousesService = wareHousesService;
            _accountingService = accountingService;


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
            this.Load = ProductSeriesLoader.Load(async () =>
            {
                //重载时排它
                ItemTreshold = 1;

                var items = await GetProductsPage(0, PageSize);

                //清除列表
                ProductSeries?.Clear();

                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        if (ProductSeries.Count(s => s.ProductId == item.ProductId) == 0)
                        {
                            ProductSeries.Add(item);
                        }
                    }
                }

                if (ProductSeries.Count > 0)
                    this.ProductSeries = new System.Collections.ObjectModel.ObservableCollection<ProductModel>(ProductSeries);

                return ProductSeries;
            });
            //以增量方式加载数据
            this.ItemTresholdReachedCommand = ReactiveCommand.Create(async () =>
            {
                int pageIdex = ProductSeries.Count / PageSize;
                using (var dig = UserDialogs.Instance.Loading("加载中..."))
                {
                    try
                    {
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
                                //return this.ProductSeries;
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


            //选择商品
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
             .Skip(1)
             .Where(x => x != null)
             .SubOnMainThread(async item =>
             {
                 await this.NavigateAsync("AddProductArchivePage", ("Product", item));
                 Selecter = null;
             }).DisposeWith(DeactivateWith);

            this.AddCommand = ReactiveCommand.Create<object>(async e => await this.NavigateAsync("AddProductArchivePage"));

            this.CatagorySelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectCatagory((data) =>
                 {
                     Filter.CatagoryId = data.Id;
                     Filter.CatagoryName = data.Name;
                     Filter.CatagoryIds = new int[] { data.Id };
                 });
            });

            this.BindBusyCommand(Load);


            this.Load.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.SerchCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.ItemTresholdReachedCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.AddCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.CatagorySelected.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);

        }

        /// 分页获取商品
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IList<ProductModel>> GetProductsPage(int pageNumber, int pageSize)
        {
            var pending = new List<ProductModel>();
            string key = string.IsNullOrEmpty(Filter.SerchKey) ? "" : Filter.SerchKey;
            var catagoryids = Filter.CatagoryIds;

            var result = await _productService.GetProductsAsync(catagoryids,
                key,
                null,
                Filter.WareHouseId,
                pageNumber,
                pageSize,
                false,
                this.ForceRefresh, new System.Threading.CancellationToken());

            if (result != null && result.Data != null)
            {
                foreach (var p in result.Data)
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
                }
                pending = result.Data;
            }

            return pending;
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }

    }
}
