using DCMS.Client.Enums;
using DCMS.Client.Models;
using DCMS.Client.Models.Products;
using DCMS.Client.Pages;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace DCMS.Client.ViewModels
{
    public class AddExchangeProductPageViewModel : ViewModelBase
    {
        public ReactiveCommand<ProductModel, Unit> BigEntryUnfocused { get; }
        public ReactiveCommand<ProductModel, Unit> SmallEntryUnfocused { get; }

        public ReactiveCommand<int, Unit> SmallPriceSelected { get; }
        public ReactiveCommand<int, Unit> BigPriceSelected { get; }

        public ReactiveCommand<int, Unit> RemoveCommend { get; set; }
        public ReactiveCommand<int, Unit> CopyCommend { get; set; }

        [Reactive] public ProductModel SelectedItem { get; set; } = new ProductModel();
        public AddExchangeProductPageViewModel(INavigationService navigationService,
              IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "添加换货商品";

            this.BigEntryUnfocused = ReactiveCommand.Create<ProductModel>((p) =>
           {
               CalcChanged(p, 0);
           });

            this.SmallEntryUnfocused = ReactiveCommand.Create<ProductModel>((p) =>
            {
                CalcChanged(p, 1);
            });

            this.SmallPriceSelected = ReactiveCommand.Create<int>((pid) =>
            {
                CalcSmallPrice(pid);
            });

            this.BigPriceSelected = ReactiveCommand.Create<int>((pid) =>
            {
                CalcBigPrice(pid);
            });

            //保存
            this.SaveCommand = ReactiveCommand.Create(async () =>
           {
               try
               {
                   bool vaildQuantity = false;
                   bool vaildBigPrice = false;
                   bool vaildSmallPrice = false;
                   bool vaildBigQuantity = false;
                   bool vaildSmallQuantity = false;
                   bool vaildGiftQuantity = false;

                   if (!string.IsNullOrEmpty(ReferencePage))
                   {
                       //数量
                       ProductSeries.ToList().ForEach(p =>
                      {
                          vaildQuantity = (p.BigPriceUnit.Quantity == 0 || p.SmallPriceUnit.Quantity == 0);
                          if (vaildQuantity)
                              return;
                      });

                       //赠送
                       ProductSeries.ToList().ForEach(p =>
                      {
                          vaildGiftQuantity = (p.GiveProduct.BigUnitQuantity == 0 && p.GiveProduct.SmallUnitQuantity == 0);
                          if (vaildGiftQuantity)
                              return;
                      });

                       //大单位
                       ProductSeries.ToList().ForEach(p =>
                      {
                          vaildBigPrice = p.BigPriceUnit.Quantity > 0 && (p.BigPriceUnit?.Price == null || p.BigPriceUnit?.Price < 0);
                          if (vaildBigPrice)
                              return;
                      });
                       ProductSeries.ToList().ForEach(p =>
                       {
                           vaildBigQuantity = (p.BigPriceUnit.Quantity <= 0);
                           if (vaildBigQuantity)
                               return;
                       });


                       //小单位
                       ProductSeries.ToList().ForEach(p =>
                      {
                          vaildSmallPrice = p.SmallPriceUnit.Quantity > 0 && (p.SmallPriceUnit?.Price == null || p.SmallPriceUnit?.Price < 0);
                          if (vaildSmallPrice)
                              return;
                      });
                       ProductSeries.ToList().ForEach(p =>
                       {
                           vaildSmallQuantity = (p.SmallPriceUnit.Quantity <= 0);
                           if (vaildSmallQuantity)
                               return;
                       });

                       if (vaildGiftQuantity)
                       {
                           if (vaildBigQuantity && vaildSmallQuantity)
                           {
                               this.Alert("商品数量不能为空");
                               return;
                           }
                       }

                       if (vaildBigQuantity && !vaildBigPrice && !vaildQuantity)
                       {
                           this.Alert("商品大单位数量不能为空");
                           return;
                       }
                       if (vaildBigPrice && !vaildBigQuantity && !vaildQuantity)
                       {
                           this.Alert("商品大单位价格不能为空");
                           return;
                       }

                       if (vaildSmallQuantity && !vaildSmallPrice && !vaildQuantity)
                       {
                           this.Alert("商品小单位数量不能为空");
                           return;
                       }
                       if (vaildSmallPrice && !vaildSmallQuantity && !vaildQuantity)
                       {
                           this.Alert("商品小单位价格不能为空");
                           return;
                       }

                       var products = ProductSeries.Where(s => s.BigPriceUnit.Quantity > 0 && s.BigPriceUnit.Price >= 0 || s.SmallPriceUnit.Quantity > 0 && s.SmallPriceUnit.Price >= 0 || s.GiveProduct.BigUnitQuantity > 0 || s.GiveProduct.SmallUnitQuantity >= 0).ToList();

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

            //移除商品
            this.RemoveCommend = ReactiveCommand.Create<int>(async (pid) =>
            {
                try
                {
                    var porductId = pid;

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

            //拷贝商品
            this.CopyCommend = ReactiveCommand.Create<int>((pid) =>
            {
                try
                {
                    var porductId = pid;

                    var products = ProductSeries;
                    var product = ProductSeries.Where(p => p.Id == porductId).Select(p => p).FirstOrDefault();
                    if (product != null)
                    {
                        var newModel = new ProductModel
                        {
                            Id = product.Id,
                            ProductId = product.Id,
                            ProductName = product.ProductName,
                            Name = product.Name,
                            UnitId = product.UnitId,
                            Quantity = product.Quantity,
                            Price = product.Price,
                            Amount = product.Amount,
                            Remark = product.Remark,
                            Subtotal = product.Subtotal,
                            StockQty = product.StockQty,
                            UnitConversion = product.UnitConversion,
                            UnitName = product.UnitName,
                            Units = product.Units,
                            BigPriceUnit = new PriceUnit()
                            {
                                Amount = product.BigPriceUnit.Amount,
                                Price = product.BigPriceUnit.Price,
                                Quantity = product.BigPriceUnit.Quantity,
                                Remark = product.BigPriceUnit.Remark,
                                UnitId = product.BigPriceUnit.UnitId,
                                UnitName = product.BigPriceUnit.UnitName
                            },
                            SmallPriceUnit = new PriceUnit()
                            {
                                Amount = product.SmallPriceUnit.Amount,
                                Price = product.SmallPriceUnit.Price,
                                Quantity = product.SmallPriceUnit.Quantity,
                                Remark = product.SmallPriceUnit.Remark,
                                UnitId = product.SmallPriceUnit.UnitId,
                                UnitName = product.SmallPriceUnit.UnitName
                            }
                        };
                        newModel.BigPriceUnit.UnitId = product.BigUnitId ?? 0;
                        newModel.SmallPriceUnit.UnitId = product.SmallUnitId ?? 0;


                        products.Add(newModel);
                    }
                    ProductSeries = new ObservableCollection<ProductModel>(products);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });
        }


        /// <summary>
        /// 输入值更改时
        /// </summary>
        public void CalcChanged(ProductModel curProduct, int type)
        {
            try
            {
                var pid = curProduct?.Id ?? 0;
                if (curProduct != null)
                {
                    var bigPrice = curProduct?.BigPriceUnit?.Price ?? 0;
                    var smallPrice = curProduct?.SmallPriceUnit?.Price ?? 0;

                    if (type == 0)
                    {
                        var bp = curProduct?.BigQuantity ?? 0;
                        if (bp > 0)
                        {
                            var sp = bigPrice / bp;
                            curProduct.SmallPriceUnit.Price = sp;
                        }
                    }
                    else if (type == 1)
                    {
                        var bp = curProduct?.BigQuantity ?? 0;
                        var sp = smallPrice * bp;
                        curProduct.BigPriceUnit.Price = sp;
                    }
                }

                ProductSeries.ForEach(product =>
                {
                    if (product != null)
                    {
                        //采购单时
                        if (ReferencePage.Equals("PurchaseOrderBillPage"))
                        {
                            //大单位价格
                            product.BigPriceUnit.Amount = product.BigPriceUnit.Quantity * product.BigPriceUnit.Price;


                            if ((product.BigQuantity ?? 0) == 0) product.BigQuantity = 1;
                            var price = (product.BigQuantity ?? 0) != 0 ? (product.BigPriceUnit.Price / product.BigQuantity) : 0;
                            var amount = product.SmallPriceUnit.Quantity * product.SmallPriceUnit.Price;

                            //小单位价格
                            product.SmallPriceUnit.Price = price != 0 ? price ?? 0 : 0;
                            product.SmallPriceUnit.Amount = amount != 0 ? amount : 0;

                        }
                        else
                        {
                            var ba = product.BigPriceUnit.Quantity * product.BigPriceUnit.Price;
                            var sa = product.SmallPriceUnit.Quantity * product.SmallPriceUnit.Price;

                            //大单位价格
                            product.BigPriceUnit.Amount = ba != 0 ? ba : 0;
                            //小单位价格 
                            product.SmallPriceUnit.Amount = sa != 0 ? sa : 0;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        public async void CalcBigPrice(int porductId)
        {
            try
            {
                var product = ProductSeries.Where(p => p.Id == porductId).Select(p => p).FirstOrDefault();

                #region
                //var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择价格", "", (() =>
                //{
                //    var popDatas = new List<PopData>();
                //    if (product != null)
                //    {
                //        var tierprices = product.ProductTierPrices;
                //        if (tierprices != null)
                //        {
                //            #region

                //            //默认条件
                //            int pId = 0;
                //            int pTId = 0;
                //            var pArry = Settings.DefaultPricePlan.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                //            if (pArry.Count > 0)
                //            {
                //                pId = pArry[0];
                //                pTId = pArry[1];
                //            }

                //            switch (pTId)
                //            {
                //                case (int)PriceType.ProductCost:
                //                    {
                //                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 0);
                //                        popDatas.Add(new PopData()
                //                        {
                //                            Id = (int)PriceType.ProductCost,
                //                            Column = "进价",
                //                            Column1 = price?.BigUnitPrice.ToString(),
                //                            Column1Enable = true,
                //                            Data = price?.BigUnitPrice ?? 0
                //                        });
                //                    }
                //                    break;
                //                case (int)PriceType.CostPrice:
                //                    {
                //                        //成本价
                //                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 5);
                //                        popDatas.Add(new PopData()
                //                        {
                //                            Id = (int)PriceType.CostPrice,
                //                            Column = "成本价",
                //                            Column1 = price?.BigUnitPrice.ToString(),
                //                            Column1Enable = true,
                //                            Data = price?.BigUnitPrice ?? 0
                //                        });
                //                    }
                //                    break;
                //                case (int)PriceType.WholesalePrice:
                //                    {
                //                        //批发价格
                //                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 1);
                //                        popDatas.Add(new PopData()
                //                        {
                //                            Id = (int)PriceType.WholesalePrice,
                //                            Column = "批发价格",
                //                            Column1 = price?.BigUnitPrice.ToString(),
                //                            Column1Enable = true,
                //                            Data = price?.BigUnitPrice ?? 0
                //                        });
                //                    }
                //                    break;
                //                case (int)PriceType.RetailPrice:
                //                    {
                //                        //零售价格
                //                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 2);
                //                        popDatas.Add(new PopData()
                //                        {
                //                            Id = (int)PriceType.RetailPrice,
                //                            Column = "零售价格",
                //                            Column1 = price?.BigUnitPrice.ToString(),
                //                            Column1Enable = true,
                //                            Data = price?.BigUnitPrice ?? 0
                //                        });
                //                    }
                //                    break;
                //                case (int)PriceType.LowestPrice:
                //                    {
                //                        //最低售价
                //                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 3);
                //                        popDatas.Add(new PopData()
                //                        {
                //                            Id = (int)PriceType.LowestPrice,
                //                            Column = "最低售价",
                //                            Column1 = price?.BigUnitPrice.ToString(),
                //                            Column1Enable = true,
                //                            Data = price?.BigUnitPrice ?? 0
                //                        });
                //                    }
                //                    break;
                //                case (int)PriceType.CustomPlan:
                //                    {
                //                        //方案价格
                //                        int i = 0;
                //                        foreach (var price in tierprices.Where(s => new int[] { (int)PriceType.CustomPlan }.Contains(s.PriceTypeId)).ToList())
                //                        {
                //                            popDatas.Add(new PopData()
                //                            {
                //                                Id = (int)PriceType.CustomPlan + i,
                //                                Column = price?.PriceTypeName,
                //                                Column1 = price?.BigUnitPrice.ToString(),
                //                                Column1Enable = true,
                //                                Data = price?.BigUnitPrice ?? 0
                //                            });
                //                            i++;
                //                        }
                //                    }
                //                    break;
                //                case (int)PriceType.LastedPrice:
                //                    {
                //                        //上次价格
                //                        var price = product.ProductTierPrices.Where(s => s.PriceTypeId == (int)PriceType.LastedPrice).FirstOrDefault();
                //                        if (price != null)
                //                        {
                //                            popDatas.Add(new PopData()
                //                            {
                //                                Id = (int)PriceType.CustomPlan,
                //                                Column = "上次价格",
                //                                Column1 = price?.BigUnitPrice.ToString(),
                //                                Column1Enable = true,
                //                                Data = price?.BigUnitPrice ?? 0
                //                            });
                //                        }
                //                    }
                //                    break;
                //            }

                //            #endregion
                //        }
                //    }
                //    return Task.FromResult(popDatas);
                //}));
                //if (result != null)
                //{
                //    product.BigPriceUnit.Price = (decimal)(result?.Data ?? 0);
                //    product.BigPriceUnit.Amount = (decimal)(result?.Data ?? 0) * product?.BigPriceUnit?.Quantity ?? 0;
                //}
                #endregion

                var _dialogView = new PopRadioButtonPage("选择价格", "", () =>
                {
                    var popDatas = new List<PopData>();
                    if (product != null)
                    {
                        var tierprices = product.ProductTierPrices;
                        if (tierprices != null)
                        {
                            #region

                            //默认条件
                            int pId = 0;
                            int pTId = 0;
                            var pArry = Settings.DefaultPricePlan.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                            if (pArry.Count > 0)
                            {
                                pId = pArry[0];
                                pTId = pArry[1];
                            }

                            switch (pTId)
                            {
                                case (int)PriceType.ProductCost:
                                    {
                                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 0);
                                        popDatas.Add(new PopData()
                                        {
                                            Id = (int)PriceType.ProductCost,
                                            Column = "进价",
                                            Column1 = price?.BigUnitPrice.ToString(),
                                            Column1Enable = true,
                                            Data = price?.BigUnitPrice ?? 0
                                        });
                                    }
                                    break;
                                case (int)PriceType.CostPrice:
                                    {
                                        //成本价
                                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 5);
                                        popDatas.Add(new PopData()
                                        {
                                            Id = (int)PriceType.CostPrice,
                                            Column = "成本价",
                                            Column1 = price?.BigUnitPrice.ToString(),
                                            Column1Enable = true,
                                            Data = price?.BigUnitPrice ?? 0
                                        });
                                    }
                                    break;
                                case (int)PriceType.WholesalePrice:
                                    {
                                        //批发价格
                                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 1);
                                        popDatas.Add(new PopData()
                                        {
                                            Id = (int)PriceType.WholesalePrice,
                                            Column = "批发价格",
                                            Column1 = price?.BigUnitPrice.ToString(),
                                            Column1Enable = true,
                                            Data = price?.BigUnitPrice ?? 0
                                        });
                                    }
                                    break;
                                case (int)PriceType.RetailPrice:
                                    {
                                        //零售价格
                                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 2);
                                        popDatas.Add(new PopData()
                                        {
                                            Id = (int)PriceType.RetailPrice,
                                            Column = "零售价格",
                                            Column1 = price?.BigUnitPrice.ToString(),
                                            Column1Enable = true,
                                            Data = price?.BigUnitPrice ?? 0
                                        });
                                    }
                                    break;
                                case (int)PriceType.LowestPrice:
                                    {
                                        //最低售价
                                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 3);
                                        popDatas.Add(new PopData()
                                        {
                                            Id = (int)PriceType.LowestPrice,
                                            Column = "最低售价",
                                            Column1 = price?.BigUnitPrice.ToString(),
                                            Column1Enable = true,
                                            Data = price?.BigUnitPrice ?? 0
                                        });
                                    }
                                    break;
                                case (int)PriceType.CustomPlan:
                                    {
                                        //方案价格
                                        int i = 0;
                                        foreach (var price in tierprices.Where(s => new int[] { (int)PriceType.CustomPlan }.Contains(s.PriceTypeId)).ToList())
                                        {
                                            popDatas.Add(new PopData()
                                            {
                                                Id = (int)PriceType.CustomPlan + i,
                                                Column = price?.PriceTypeName,
                                                Column1 = price?.BigUnitPrice.ToString(),
                                                Column1Enable = true,
                                                Data = price?.BigUnitPrice ?? 0
                                            });
                                            i++;
                                        }
                                    }
                                    break;
                                case (int)PriceType.LastedPrice:
                                    {
                                        //上次价格
                                        var price = product.ProductTierPrices.Where(s => s.PriceTypeId == (int)PriceType.LastedPrice).FirstOrDefault();
                                        if (price != null)
                                        {
                                            popDatas.Add(new PopData()
                                            {
                                                Id = (int)PriceType.CustomPlan,
                                                Column = "上次价格",
                                                Column1 = price?.BigUnitPrice.ToString(),
                                                Column1Enable = true,
                                                Data = price?.BigUnitPrice ?? 0
                                            });
                                        }
                                    }
                                    break;
                            }

                            #endregion
                        }
                    }
                    return Task.FromResult(popDatas);
                });
                _dialogView.Completed += (sender, result) =>
                {
                    if (result != null)
                    {
                        product.BigPriceUnit.Price = (decimal)(result?.Data ?? 0);
                        product.BigPriceUnit.Amount = (decimal)(result?.Data ?? 0) * product?.BigPriceUnit?.Quantity ?? 0;
                    }
                };
                await PopupNavigation.Instance.PushAsync(_dialogView);

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        public async void CalcSmallPrice(int porductId)
        {
            try
            {
                var product = ProductSeries.Where(p => p.Id == porductId).Select(p => p).FirstOrDefault();
                var _dialogView = new PopRadioButtonPage("选择价格", "", () =>
                {
                    var popDatas = new List<PopData>();
                    if (product != null)
                    {
                        var tierprices = product.ProductTierPrices;
                        if (tierprices != null)
                        {
                            #region

                            //默认条件
                            int pId = 0;
                            int pTId = 0;
                            var pArry = Settings.DefaultPricePlan.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                            if (pArry.Count > 0)
                            {
                                pId = pArry[0];
                                pTId = pArry[1];
                            }

                            switch (pTId)
                            {
                                case (int)PriceType.ProductCost:
                                    {
                                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 0);
                                        popDatas.Add(new PopData()
                                        {
                                            Id = (int)PriceType.ProductCost,
                                            Column = "进价",
                                            Column1 = price?.SmallUnitPrice.ToString(),
                                            Column1Enable = true,
                                            Data = price?.SmallUnitPrice ?? 0
                                        });
                                    }
                                    break;
                                case (int)PriceType.CostPrice:
                                    {
                                        //成本价
                                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 5);
                                        popDatas.Add(new PopData()
                                        {
                                            Id = (int)PriceType.CostPrice,
                                            Column = "成本价",
                                            Column1 = price?.SmallUnitPrice.ToString(),
                                            Column1Enable = true,
                                            Data = price?.SmallUnitPrice ?? 0
                                        });
                                    }
                                    break;
                                case (int)PriceType.WholesalePrice:
                                    {
                                        //批发价格
                                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 1);
                                        popDatas.Add(new PopData()
                                        {
                                            Id = (int)PriceType.WholesalePrice,
                                            Column = "批发价格",
                                            Column1 = price?.SmallUnitPrice.ToString(),
                                            Column1Enable = true,
                                            Data = price?.SmallUnitPrice ?? 0
                                        });
                                    }
                                    break;
                                case (int)PriceType.RetailPrice:
                                    {
                                        //零售价格
                                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 2);
                                        popDatas.Add(new PopData()
                                        {
                                            Id = (int)PriceType.RetailPrice,
                                            Column = "零售价格",
                                            Column1 = price?.SmallUnitPrice.ToString(),
                                            Column1Enable = true,
                                            Data = price?.SmallUnitPrice ?? 0
                                        });
                                    }
                                    break;
                                case (int)PriceType.LowestPrice:
                                    {
                                        //最低售价
                                        var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 3);
                                        popDatas.Add(new PopData()
                                        {
                                            Id = (int)PriceType.LowestPrice,
                                            Column = "最低售价",
                                            Column1 = price?.SmallUnitPrice.ToString(),
                                            Column1Enable = true,
                                            Data = price?.SmallUnitPrice ?? 0
                                        });
                                    }
                                    break;
                                case (int)PriceType.CustomPlan:
                                    {
                                        //方案价格
                                        int i = 0;
                                        foreach (var price in tierprices.Where(s => new int[] { (int)PriceType.CustomPlan }.Contains(s.PriceTypeId)).ToList())
                                        {
                                            popDatas.Add(new PopData()
                                            {
                                                Id = (int)PriceType.CustomPlan + i,
                                                Column = price?.PriceTypeName,
                                                Column1 = price?.SmallUnitPrice.ToString(),
                                                Column1Enable = true,
                                                Data = price?.SmallUnitPrice ?? 0
                                            });
                                            i++;
                                        }
                                    }
                                    break;
                                case (int)PriceType.LastedPrice:
                                    {
                                        //上次价格
                                        var price = product.ProductTierPrices.Where(s => s.PriceTypeId == (int)PriceType.LastedPrice).FirstOrDefault();
                                        if (price != null)
                                        {
                                            popDatas.Add(new PopData()
                                            {
                                                Id = (int)PriceType.CustomPlan,
                                                Column = "上次价格",
                                                Column1 = price?.SmallUnitPrice.ToString(),
                                                Column1Enable = true,
                                                Data = price?.SmallUnitPrice ?? 0
                                            });
                                        }
                                    }
                                    break;
                            }

                            #endregion
                        }
                    }
                    return Task.FromResult(popDatas);
                });
                _dialogView.Completed += (sender, result) =>
                {
                    try { 
                    if (result != null)
                    {
                        product.SmallPriceUnit.Price = (decimal)(result?.Data ?? 0);
                        product.SmallPriceUnit.Amount = (decimal)(result?.Data ?? 0) * product?.SmallPriceUnit?.Quantity ?? 0;
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };
                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            //获取选择的商品回传
            if (parameters.ContainsKey("Products"))
            {
                parameters.TryGetValue("Products", out List<ProductModel> products);

                //类型标识
                int pId = 0;
                //枚举标识
                int pTId = 0;
                var pArry = Settings.DefaultPricePlan.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                if (pArry.Count > 0)
                {
                    pId = pArry[0];
                    pTId = pArry[1];
                }

                //层次价格
                products.ForEach(product =>
                {
                    var sprice = product.SmallProductPrices;
                    var bprice = product.BigProductPrices;

                    switch (pTId)
                    {
                        case (int)PriceType.ProductCost:
                            {
                                //进价 
                                if (sprice != null)
                                    product.SmallPriceUnit.Price = sprice.PurchasePrice ?? 0;
                                if (bprice != null)
                                    product.BigPriceUnit.Price = bprice.PurchasePrice ?? 0;
                            }
                            break;
                        case (int)PriceType.CostPrice:
                            {
                                //成本价
                                if (sprice != null)
                                    product.SmallPriceUnit.Price = sprice.CostPrice ?? 0;
                                if (bprice != null)
                                    product.BigPriceUnit.Price = bprice.CostPrice ?? 0;
                            }
                            break;
                        case (int)PriceType.WholesalePrice:
                            {
                                //批发价格
                                if (sprice != null)
                                    product.SmallPriceUnit.Price = sprice.TradePrice ?? 0;
                                if (bprice != null)
                                    product.BigPriceUnit.Price = bprice.TradePrice ?? 0;
                            }
                            break;
                        case (int)PriceType.RetailPrice:
                            {
                                //零售价格
                                if (sprice != null)
                                    product.SmallPriceUnit.Price = sprice.RetailPrice ?? 0;
                                if (bprice != null)
                                    product.BigPriceUnit.Price = bprice.RetailPrice ?? 0;
                            }
                            break;
                        case (int)PriceType.LowestPrice:
                            {
                                //最低售价
                                if (sprice != null)
                                    product.SmallPriceUnit.Price = sprice.FloorPrice ?? 0;
                                if (bprice != null)
                                    product.BigPriceUnit.Price = bprice.FloorPrice ?? 0;
                            }
                            break;
                        case (int)PriceType.CustomPlan:
                            {
                                //方案价格
                                var price = product.ProductTierPrices.Where(s => s.PriceTypeId == pTId && s.PricesPlanId == pId).FirstOrDefault();
                                if (price != null)
                                {
                                    product.SmallPriceUnit.Price = price.SmallUnitPrice ?? 0;
                                    product.BigPriceUnit.Price = price.BigUnitPrice ?? 0;
                                }
                            }
                            break;
                        case (int)PriceType.LastedPrice:
                            {
                                //上次价格
                                var price = product.ProductTierPrices.Where(s => s.PriceTypeId == pTId).FirstOrDefault();
                                if (price != null)
                                {
                                    product.SmallPriceUnit.Price = price.SmallUnitPrice ?? 0;
                                    product.BigPriceUnit.Price = price.BigUnitPrice ?? 0;
                                }
                            }
                            break;
                    }
                });

                ProductSeries = new ObservableCollection<ProductModel>(products);
            }
        }

    }
}
