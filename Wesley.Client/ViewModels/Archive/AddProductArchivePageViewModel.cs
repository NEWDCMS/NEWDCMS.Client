using Wesley.Client.Enums;
using Wesley.Client.Models.Products;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace Wesley.Client.ViewModels
{
    public class AddProductArchivePageViewModel : ViewModelBaseCutom
    {

        public ReactiveCommand<object, Unit> TextChangedCommend { get; }
        public ReactiveCommand<object, Unit> CatagorySelected { get; }
        public new ReactiveCommand<object, Unit> BrandSelected { get; }
        public ReactiveCommand<object, Unit> BigUnitSelected { get; }
        public ReactiveCommand<object, Unit> StrokeUnitSelected { get; }
        public ReactiveCommand<object, Unit> SmallUnitSelected { get; }
        public ReactiveCommand<object, Unit> ScanCode { get; }


        public AddProductArchivePageViewModel(INavigationService navigationService,
            IProductService productService,
            ITerminalService terminalService,
            IUserService userService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
             IDialogService dialogService) : base(navigationService,
                productService, terminalService, userService, wareHousesService,
                accountingService, dialogService)
        {

            Title = "添加商品档案";


            //验证
            var valid_TerminalId = this.ValidationRule(x => x.Product.Name, _isDefined, "商品名称未指定");
            var valid_BusinessUserId = this.ValidationRule(x => x.Product.MnemonicCode, _isDefined, "助记码未指定");
            var valid_WareHouseId = this.ValidationRule(x => x.Product.CategoryId, _isZero, "商品类别未指定");
            var valid_ProductCount = this.ValidationRule(x => x.Product.BrandId, _isZero, "品牌未指定");
            var valid_SelectesCount = this.ValidationRule(x => x.Product.SmallProductPrices.UnitId, _isZero, "小单位未指定");

            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                await this.Access(AccessGranularityEnum.ProductArchivesSave);

                var product = new ProductModel
                {
                    Id = Product.Id,
                    ProductId = Product.ProductId,

                    StoreId = Settings.StoreId,
                    //商品名称
                    ProductName = Product.Name,
                    Name = Product.Name,
                    //助记码
                    //product.MnemonicCode = CommonHelper.GenerateStrchar(5);
                    //商品编号
                    ProductCode = Product.ProductCode,
                    //商品类别
                    CategoryId = Product.CategoryId,
                    CategoryName = Product.CategoryName,
                    //商品品牌
                    BrandId = Product.BrandId,
                    BrandName = Product.BrandName,
                    //大单位换算数
                    BigQuantity = Product.BigQuantity,
                    //中单位换算数
                    StrokeQuantity = Product.StrokeQuantity,

                    //小单位
                    SmallUnitId = Product.SmallProductPrices.UnitId,
                    //中单位
                    StrokeUnitId = Product.StrokeProductPrices.UnitId,
                    //大单位
                    BigUnitId = Product.BigProductPrices.UnitId,

                    //小单位条码
                    SmallBarCode = Product.SmallBarCode,
                    //中单位条码
                    StrokeBarCode = Product.StrokeBarCode,
                    //大单位条码
                    BigBarCode = Product.BigBarCode,
                    //状态
                    Status = true
                };
                //档案价格
                var unitPrices = new Dictionary<string, string>
                            {
                                //小单位
                                { "Small_UnitId", Product.SmallProductPrices.UnitId.ToString() },
                                { "Small_TradePrice", (Product.SmallProductPrices.TradePrice ?? 0).ToString() },
                                { "Small_RetailPrice", (Product.SmallProductPrices.RetailPrice ?? 0).ToString() },
                                { "Small_FloorPrice", (Product.SmallProductPrices.FloorPrice ?? 0).ToString() },
                                { "Small_PurchasePrice", (Product.SmallProductPrices.PurchasePrice ?? 0).ToString() },
                                { "Small_CostPrice", (Product.SmallProductPrices.CostPrice ?? 0).ToString() },
                                { "Small_SALE1", (Product.SmallProductPrices.SALE1 ?? 0).ToString() },
                                { "Small_SALE2", (Product.SmallProductPrices.SALE2 ?? 0).ToString() },
                                { "Small_SALE3", (Product.SmallProductPrices.SALE3 ?? 0).ToString() },

                                //中单位
                                { "Stroke_UnitId", Product.StrokeProductPrices.UnitId.ToString() },
                                { "Stroke_TradePrice", (Product.StrokeProductPrices.TradePrice ?? 0).ToString() },
                                { "Stroke_RetailPrice", (Product.StrokeProductPrices.RetailPrice ?? 0).ToString() },
                                { "Stroke_FloorPrice", (Product.StrokeProductPrices.FloorPrice ?? 0).ToString() },
                                { "Stroke_PurchasePrice", (Product.StrokeProductPrices.PurchasePrice ?? 0).ToString() },
                                { "Stroke_CostPrice", (Product.StrokeProductPrices.CostPrice ?? 0).ToString() },
                                { "Stroke_SALE1", (Product.StrokeProductPrices.SALE1 ?? 0).ToString() },
                                { "Stroke_SALE2", (Product.StrokeProductPrices.SALE2 ?? 0).ToString() },
                                { "Stroke_SALE3", (Product.StrokeProductPrices.SALE3 ?? 0).ToString() },

                                //大单位
                                { "Big_UnitId", Product.BigProductPrices.UnitId.ToString() },
                                { "Big_TradePrice", (Product.BigProductPrices.TradePrice ?? 0).ToString() },
                                { "Big_RetailPrice", (Product.BigProductPrices.RetailPrice ?? 0).ToString() },
                                { "Big_FloorPrice", (Product.BigProductPrices.FloorPrice ?? 0).ToString() },
                                { "Big_PurchasePrice", (Product.BigProductPrices.PurchasePrice ?? 0).ToString() },
                                { "Big_CostPrice", (Product.BigProductPrices.CostPrice ?? 0).ToString() },
                                { "Big_SALE1", (Product.BigProductPrices.SALE1 ?? 0).ToString() },
                                { "Big_SALE2", (Product.BigProductPrices.SALE2 ?? 0).ToString() },
                                { "Big_SALE3", (Product.BigProductPrices.SALE3 ?? 0).ToString() }
                            };
                product.UnitPriceDicts = unitPrices;

                return await SubmitAsync(product, 0, _productService.CreateOrUpdateProductAsync, (result) =>
               {
                   Product = new ProductModel();
               }, token: cts.Token);

            },
            this.IsValid());

            this.TextChangedCommend = ReactiveCommand.Create<object>(e =>
            {
                //计算中单位单价
                var strokeTradePrice = Product.StrokeQuantity * Product.SmallProductPrices.TradePrice;
                //计算中单位采购价
                var strokePurchasePrice = Product.StrokeQuantity * Product.SmallProductPrices.PurchasePrice;

                //计算大单位单价
                var bigTradePrice = Product.BigQuantity * Product.SmallProductPrices.TradePrice;
                //计算大单位采购价
                var bigPurchasePrice = Product.BigQuantity * Product.SmallProductPrices.PurchasePrice;

                if (strokeTradePrice != 0)
                {
                    Product.StrokeProductPrices.TradePrice = strokeTradePrice;
                }

                if (strokePurchasePrice != 0)
                {
                    Product.StrokeProductPrices.PurchasePrice = strokePurchasePrice;
                }

                if (bigTradePrice != 0)
                {
                    Product.BigProductPrices.TradePrice = bigTradePrice;
                }

                if (bigPurchasePrice != 0)
                {
                    Product.BigProductPrices.PurchasePrice = bigPurchasePrice;
                }
            });
            this.CatagorySelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectCatagory((data) =>
                 {
                     Product.CategoryId = data.Id;
                     Product.CategoryName = data.Name;
                 });
            });
            this.BrandSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectBrand((data) =>
                {
                    Product.BrandId = data.Id;
                    Product.BrandName = data.Name;
                });
            });
            this.BigUnitSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectSpecification((data) =>
                 {
                     Product.BigProductPrices.UnitId = data.Id;
                     Product.BigProductPrices.UnitName = data.Name;
                 }, 2);
            });
            this.StrokeUnitSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectSpecification((data) =>
                 {
                     Product.StrokeProductPrices.UnitId = data.Id;
                     Product.StrokeProductPrices.UnitName = data.Name;
                 }, 1);
            });
            this.SmallUnitSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectSpecification((data) =>
                 {
                     Product.SmallProductPrices.UnitId = data.Id;
                     Product.SmallProductPrices.UnitName = data.Name;
                 }, 0);
            });
            this.ScanCode = ReactiveCommand.Create<object>(async e => await this.NavigateAsync("ScanBarcodePage"));

            this.TextChangedCommend.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.CatagorySelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.BrandSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.BigUnitSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.ScanCode.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.StrokeUnitSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.SmallUnitSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.ExceptionsSubscribe();
        }



        /// <summary>
        /// 完成后接收
        /// </summary>
        /// <param name="parameters"></param>
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            try
            {
                if (parameters.ContainsKey("Product"))
                {
                    parameters.TryGetValue<ProductModel>("Product", out ProductModel product);

                    if (product != null)
                    {
                        if (product.Id != 0 || product.ProductId != 0)
                        {
                            Title = "编辑商品档案";
                        }

                        Product = product;
                        //小单位
                        Product.SmallProductPrices.UnitId = product.SmallUnitId ?? 0;
                        Product.SmallProductPrices.UnitName = product.smallOption.Name;
                        //中单位
                        Product.StrokeProductPrices.UnitId = product.StrokeUnitId ?? 0;
                        Product.StrokeProductPrices.UnitName = product.strokeOption.Name;
                        //大单位
                        Product.BigProductPrices.UnitId = product.BigUnitId ?? 0;
                        Product.BigProductPrices.UnitName = product.bigOption.Name;
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
