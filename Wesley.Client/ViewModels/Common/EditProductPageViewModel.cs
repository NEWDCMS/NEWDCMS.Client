using Wesley.Client.CustomViews;
using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Products;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Wesley.Client.ViewModels
{
    public class EditProductPageViewModel : ViewModelBase
    {
        /// <summary>
        /// 原单据商品数量
        /// </summary>
        public int OldProductQuantity { get; set; }
        public IReactiveCommand TextChangedCommand { get; set; }
        public IReactiveCommand DeleteCommand { get; set; }
        public ReactiveCommand<int, Unit> UnitSelected { get; set; }
        public ReactiveCommand<int, Unit> PriceSelected { get; set; }

        public ReactiveCommand<string, Unit> EntryUnfocused { get; }

        public EditProductPageViewModel(INavigationService navigationService, IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "修改商品信息";
            _navigationService = navigationService;
            _dialogService = dialogService;

            //保存
            this.SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (this.OldProductQuantity < Product.Quantity && Product.IsDispatchProduct)
                {
                    this.Alert("已调度商品，数量不能大于原来单据商品量。");
                    return;
                }
                await _navigationService.GoBackAsync(("UpdateProduct", Product));

            });

            //重算
            this.EntryUnfocused = ReactiveCommand.Create<string>((t) =>
            {
                CalcPrice();
            });

            //删除商品
            this.DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await _navigationService.GoBackAsync(("DelProduct", Product));
            });

            //单位选择
            this.UnitSelected = ReactiveCommand.CreateFromTask<int>(async (r) =>
            {
                try
                {
                    int porductId = r;
                    var product = this.Product;
                    var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("单位选择", "", (() =>
                    {
                        var popDatas = new List<PopData>();
                        if (product.Units != null)
                        {
                            var unt = product.Units.ToList();

                            if (unt[0].Key != "SMALL")
                            {
                                popDatas.Add(new PopData()
                                {
                                    Id = unt[0].Value,
                                    Column = unt[0].Key,
                                    Column1 = "SMALL",
                                    Column1Enable = false,
                                    Data = unt[0].Value,
                                    Selected = this.Product.UnitAlias == "SMALL"
                                });
                            }

                            if (unt[1].Key != "STROK")
                            {
                                popDatas.Add(new PopData()
                                {
                                    Id = unt[1].Value,
                                    Column = unt[1].Key,
                                    Column1 = "STROK",
                                    Column1Enable = false,
                                    Data = unt[1].Value,
                                    Selected = this.Product.UnitAlias == "STROK"
                                });
                            }

                            if (unt[2].Key != "BIG")
                            {
                                popDatas.Add(new PopData()
                                {
                                    Id = unt[2].Value,
                                    Column = unt[2].Key,
                                    Column1 = "BIG",
                                    Column1Enable = false,
                                    Data = unt[2].Value,
                                    Selected = this.Product.UnitAlias == "BIG"
                                });
                            }
                        }

                        return Task.FromResult(popDatas);
                    }));

                    if (result != null)
                    {
                        Product.UnitId = result.Data;
                        Product.UnitName = result.Column;
                        product.UnitAlias = result.Column1;

                        //重算
                        CalcPrice();
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });

            //单价选择
            this.PriceSelected = ReactiveCommand.CreateFromTask<int>(async (r) =>
            {
                try
                {
                    int porductId = r;
                    var product = this.Product;
                    var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择价格", "", (() =>
                    {
                        var popDatas = new List<PopData>();

                        if (product != null)
                        {
                            var tierprices = product.ProductTierPrices;
                            if (tierprices != null)
                            {
                                //默认条件
                                int pId = 0;
                                int pTId = 0;
                                var pArry = Settings.DefaultPricePlan.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                                if (pArry.Count > 0)
                                {
                                    pId = pArry[0];
                                    pTId = pArry[1];
                                }

                                if (product.UnitAlias == "SMALL")
                                {
                                    #region

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
                                else if (product.UnitAlias == "STROK")
                                {
                                    #region

                                    switch (pTId)
                                    {
                                        case (int)PriceType.ProductCost:
                                            {
                                                var price = tierprices.FirstOrDefault(s => s.PriceTypeId == 0);
                                                popDatas.Add(new PopData()
                                                {
                                                    Id = (int)PriceType.ProductCost,
                                                    Column = "进价",
                                                    Column1 = price?.StrokeUnitPrice.ToString(),
                                                    Column1Enable = true,
                                                    Data = price?.StrokeUnitPrice ?? 0
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
                                                    Column1 = price?.StrokeUnitPrice.ToString(),
                                                    Column1Enable = true,
                                                    Data = price?.StrokeUnitPrice ?? 0
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
                                                    Column1 = price?.StrokeUnitPrice.ToString(),
                                                    Column1Enable = true,
                                                    Data = price?.StrokeUnitPrice ?? 0
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
                                                    Column1 = price?.StrokeUnitPrice.ToString(),
                                                    Column1Enable = true,
                                                    Data = price?.StrokeUnitPrice ?? 0
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
                                                    Column1 = price?.StrokeUnitPrice.ToString(),
                                                    Column1Enable = true,
                                                    Data = price?.StrokeUnitPrice ?? 0
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
                                                        Column1 = price?.StrokeUnitPrice.ToString(),
                                                        Column1Enable = true,
                                                        Data = price?.StrokeUnitPrice ?? 0
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
                                                        Column1 = price?.StrokeUnitPrice.ToString(),
                                                        Column1Enable = true,
                                                        Data = price?.StrokeUnitPrice ?? 0
                                                    });
                                                }
                                            }
                                            break;
                                    }

                                    #endregion
                                }
                                else if (product.UnitAlias == "BIG")
                                {
                                    #region

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
                        }

                        return Task.FromResult(popDatas);
                    }));


                    if (result != null)
                    {
                        Product.Price = result.Data;
                        Product.IsShowGiveEnabled = true;
                        //重算
                        CalcPrice();
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });
        }


        /// <summary>
        /// 动态计算
        /// </summary>
        private void CalcPrice()
        {

            if (Product.UnitAlias == "BIG")
            {
                var amount = Product.Price * Product.Quantity * Product.BigQuantity;
                Product.Amount = amount != 0 ? amount : 0;
            }
            else if (Product.UnitAlias == "STROK")
            {
                var amount = Product.Price * Product.Quantity * Product.StrokeQuantity;
                Product.Amount = amount != 0 ? amount : 0;
            }
            else
            {
                var amount = Product.Price * Product.Quantity;
                Product.Amount = amount != 0 ? amount : 0;
            }
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                //编辑商品回传
                if (parameters.ContainsKey("Product"))
                {
                    parameters.TryGetValue("Product", out ProductModel product);
                    if (product != null)
                    {
                        product.RemarkSelected2 = ReactiveCommand.Create<ProductModel>(async e =>
                        {
                            var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择备注", "", async () =>
                            {
                                var _settingService = App.Resolve<ISettingService>();
                                var result = await _settingService?.GetRemarkConfigListSetting();
                                var popDatas = result?.Select(s =>
                                {
                                    return new PopData
                                    {
                                        Id = s.Key,
                                        Column = s.Value,
                                        Selected = false
                                    };
                                })?.ToList();
                                return popDatas;
                            });

                            if (result != null)
                            {
                                //RemarkConfig.Id = result.Id;
                                product.Remark = result.Column;
                            }
                        });
                        Product = product;
                        this.OldProductQuantity = product.Quantity;
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
