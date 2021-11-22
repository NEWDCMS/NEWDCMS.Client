using Wesley.Client.Models.Products;
using Wesley.Client.Models.WareHouses;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Wesley.Client.ViewModels
{
    public class AddBackStockBillPageViewModel : ViewModelBase
    {
        [Reactive] public ProductModel SelectedItem { get; set; } = new ProductModel();
        public AllocationBillModel Bill { get; set; }


        public AddBackStockBillPageViewModel(INavigationService navigationService, IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "添加回库调拨单商品";

            _navigationService = navigationService;
            _dialogService = dialogService;

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

                               ProductSeries.ToList().ForEach(p =>
                               {
                                   vaild = (p.BigPriceUnit.Quantity == 0 && p.SmallPriceUnit.Quantity == 0);
                               });

                               if (vaild)
                               {
                                   this.Alert("调拨数量不能为空");
                                   return;
                               }


                               //转向引用页
                               var redirectPage = $"../../";
                               await this.NavigateAsync(redirectPage, ("ProductSeries", ProductSeries));
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

        /// <summary>
        /// 拷贝商品
        /// </summary>
        private DelegateCommand<object> _copyCommend;
        public DelegateCommand<object> CopyCommend
        {
            get
            {
                if (_copyCommend == null)
                {
                    _copyCommend = new DelegateCommand<object>((e) =>
                   {
                       try
                       {
                           var porductId = (int)e;
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
                                   UnitName = product.UnitName,
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
                return _copyCommend;
            }
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                if (parameters.ContainsKey("Products"))
                {
                    parameters.TryGetValue("Products", out List<ProductModel> products);
                    if (products != null)
                    {
                        ProductSeries = new ObservableCollection<ProductModel>(products);
                    }
                }


                if (parameters.ContainsKey("Bill"))
                {
                    if (ReferencePage.Equals("AllocationBillPage"))
                    {
                        parameters.TryGetValue("Bill", out AllocationBillModel Bill);

                        if (Bill != null)
                        {
                            foreach (var p in ProductSeries.ToList())
                            {
                                var outs = p.StockQuantities.Where(q => q.WareHouseId == Bill.ShipmentWareHouseId).FirstOrDefault();
                                var ins = p.StockQuantities.Where(q => q.WareHouseId == Bill.IncomeWareHouseId).FirstOrDefault();

                                p.ShipmentUsableQuantity = outs != null ? outs.UsableQuantity : 0;
                                p.IncomeUsableQuantity = ins != null ? ins.UsableQuantity : 0;
                            }
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
