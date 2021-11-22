using Wesley.Client.Models.Products;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Navigation;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace Wesley.Client.ViewModels
{
    public class AddInventoryProductPageViewModel : ViewModelBase
    {
        public int ItemId { get; set; }

        public AddInventoryProductPageViewModel(INavigationService navigationService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "添加盘点商品";

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

                               if (vaild) { this.Alert("商品数量不能为空，操作失败！"); return; }

                               if (WareHouse == null || WareHouse.Id == 0)
                               {
                                   this.Alert("仓库未指定，操作失败！"); return;
                               }

                               //转向盘点单
                               await this.NavigateAsync("InventoryOPBillPage",
                                  ("ItemId", ItemId),
                                  ("ProductSeries", ProductSeries.ToList()),
                                  ("WareHouse", WareHouse));
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
            try
            {
                if (parameters.ContainsKey("ItemId"))
                {
                    parameters.TryGetValue<int>("ItemId", out int ItemId);
                }
                else
                {
                    ItemId = 0;
                }

                if (parameters.ContainsKey("Products"))
                {
                    parameters.TryGetValue("Products", out List<ProductModel> products);
                    if (products != null)
                    {
                        ProductSeries = new ObservableCollection<ProductModel>(products);
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
