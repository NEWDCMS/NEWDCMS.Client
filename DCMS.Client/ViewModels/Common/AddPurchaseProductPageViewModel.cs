using DCMS.Client.Models.Configuration;
using DCMS.Client.Models.Products;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
namespace DCMS.Client.ViewModels
{
    public class AddPurchaseProductPageViewModel : ViewModelBase
    {
        private readonly IPurchaseBillService _purchaseBillService;

        [Reactive] public ProductModel SelectedItem { get; set; } = new ProductModel();
        public AddPurchaseProductPageViewModel(
            INavigationService navigationService,
            IPurchaseBillService purchaseBillService,
              IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "添加采购商品";

            _navigationService = navigationService;
            _dialogService = dialogService;
            _purchaseBillService = purchaseBillService;
        }

        /// <summary>
        /// 保存
        /// </summary>
        private DelegateCommand<object> _saveCommand;
        public new DelegateCommand<object> SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new DelegateCommand<object>(async (e) =>
                   {
                       try
                       {
                           bool vaild = false;

                           if (!string.IsNullOrEmpty(ReferencePage))
                           {
                               //数量
                               ProductSeries.ToList().ForEach(p =>
                              {
                                  vaild = (p.BigPriceUnit.Quantity == 0 && p.SmallPriceUnit.Quantity == 0);
                                  if (vaild)
                                      return;
                              });
                               if (vaild)
                               {
                                   this.Alert("商品数量不能为空");
                                   return;
                               }

                               //大单位
                               ProductSeries.ToList().ForEach(p =>
                              {
                                  vaild = (p.BigPriceUnit.Quantity > 0 && p.BigPriceUnit.Price < 0);
                                  if (vaild)
                                      return;
                              });
                               if (vaild)
                               {
                                   this.Alert("商品大单位价格不能为空");
                                   return;
                               }
                               ProductSeries.ToList().ForEach(p =>
                               {
                                   vaild = (p.BigPriceUnit.Quantity <= 0 && p.BigPriceUnit.Price > 0);
                                   if (vaild)
                                       return;
                               });
                               if (vaild)
                               {
                                   this.Alert("商品大单位数量不能为空");
                                   return;
                               }

                               //小单位
                               ProductSeries.ToList().ForEach(p =>
                              {
                                  vaild = (p.SmallPriceUnit.Quantity > 0 && p.SmallPriceUnit.Price < 0);
                                  if (vaild)
                                      return;
                              });
                               if (vaild)
                               {
                                   this.Alert("商品小单位价格不能为空");
                                   return;
                               }
                               ProductSeries.ToList().ForEach(p =>
                               {
                                   vaild = (p.SmallPriceUnit.Quantity < 0 && p.SmallPriceUnit.Price > 0);
                                   if (vaild)
                                       return;
                               });
                               if (vaild)
                               {
                                   this.Alert("商品小单位数量不能为空");
                                   return;
                               }

                               var products = ProductSeries.Where(s => s.BigPriceUnit.Quantity > 0 && s.BigPriceUnit.Price >= 0 || s.SmallPriceUnit.Quantity > 0 && s.SmallPriceUnit.Price >= 0).ToList();

                               //转向引用页
                               var redirectPage = $"../../";
                               await this.NavigateAsync(redirectPage, ("ProductSeries", products));
                           }
                       }
                       catch (Exception ex)
                       {
                           Crashes.TrackError(ex);
                       }
                   });
                }
                return _saveCommand;
            }
        }


        /// <summary>
        /// 移除商品
        /// </summary>
        private DelegateCommand<object> _removeCommend;
        public DelegateCommand<object> RemoveCommend
        {
            get
            {
                if (_removeCommend == null)
                {
                    _removeCommend = new DelegateCommand<object>(async (e) =>
                   {
                       try
                       {
                           var porductId = (int)e;

                           var products = ProductSeries;
                           var product = ProductSeries.Where(p => p.Id == porductId).Select(p => p).FirstOrDefault();
                           if (product != null)
                           {
                               products.Remove(product);
                           }

                           ProductSeries = new ObservableCollection<ProductModel>(products);

                           if (products.Count == 0)
                           {
                               await _navigationService.GoBackAsync();
                           }
                       }
                       catch (Exception ex)
                       {
                           Crashes.TrackError(ex);
                       }
                   });
                }
                return _removeCommend;
            }
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            //获取选择的商品回传
            if (parameters.ContainsKey("Products"))
            {
                parameters.TryGetValue<List<ProductModel>>("Products", out List<ProductModel> products);

                //默认条件
                int pId = 0;
                int pTId = 0;
                var pArry = Settings.DefaultPricePlan.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                if (pArry.Count > 0)
                {
                    pId = pArry[0];
                    pTId = pArry[1];
                }

                products.ForEach(async product =>
                {
                    var sprice = product.SmallProductPrices;
                    var bprice = product.BigProductPrices;

                    //默认进价
                    if (sprice != null)
                        product.SmallPriceUnit.Price = sprice.PurchasePrice ?? 0;
                    if (bprice != null)
                        product.BigPriceUnit.Price = bprice.PurchasePrice ?? 0;

                    //上次进价
                    if (JsonConvert.DeserializeObject<CompanySettingModel>(Settings.CompanySetting).DefaultPurchasePrice == 0)
                    {
                        var lastprices = await _purchaseBillService.AsyncPurchaseItemByProductIdForAsync(product.ProductId, false);
                        if (lastprices != null && lastprices.Count > 0)
                        {
                            if (lastprices.Count(l => l.UnitId == product.BigUnitId) > 0)
                                product.BigPriceUnit.Price = lastprices.FirstOrDefault(l => l.UnitId == (product.BigUnitId ?? 0))?.Price ?? 0;

                            if (lastprices.Count(l => l.UnitId == product.SmallUnitId) > 0)
                                product.SmallPriceUnit.Price = lastprices.FirstOrDefault(l => l.UnitId == (product.SmallUnitId ?? 0))?.Price ?? 0;
                        }
                    }
                    else //预设进价
                    {
                        product.BigPriceUnit.Price = product.BigProductPrices.TradePrice ?? 0;
                        product.SmallPriceUnit.Price = product.SmallProductPrices.TradePrice ?? 0;
                    }

                });

                ProductSeries = new ObservableCollection<ProductModel>(products);
            }
        }

    }
}
