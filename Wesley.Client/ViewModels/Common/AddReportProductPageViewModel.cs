using Wesley.Client.Models.Products;
using Wesley.Client.Models.WareHouses;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace Wesley.Client.ViewModels
{
    public class AddReportProductPageViewModel : ViewModelBase
    {
        [Reactive]
        public InventoryReportItemModel StoreReporting { get; set; } = new InventoryReportItemModel();
        [Reactive]
        public IList<InventoryReportStoreQuantityModel> ReportStoreQuantities { get; set; } = new ObservableCollection<InventoryReportStoreQuantityModel>();
        public List<InventoryReportStoreQuantityModel> StoreQuantities { get; set; } = new List<InventoryReportStoreQuantityModel>();

        public IReactiveCommand RemoveCommend { get; }
        public IReactiveCommand AddCommend { get; }

        public AddReportProductPageViewModel(INavigationService navigationService,
              IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "添加上报商品";

            this.RemoveCommend = ReactiveCommand.Create(() =>
            {
                if (StoreQuantities.Count > 0)
                {
                    StoreQuantities.RemoveAt(0);
                    ReportStoreQuantities = new ObservableCollection<InventoryReportStoreQuantityModel>(StoreQuantities);
                }
            });

            this.AddCommend = ReactiveCommand.Create(() =>
           {
               var product = new InventoryReportStoreQuantityModel()
               {
                   BigStoreQuantity = 0,
                   SmallStoreQuantity = 0,
                   BigUnitId = StoreReporting.BigUnitId,
                   SmallUnitId = StoreReporting.SmallUnitId,
                   BigUnitName = StoreReporting.BigUnitName,
                   SmallUnitName = StoreReporting.SmallUnitName,
                   ManufactureDete = DateTime.Now
               };
               StoreQuantities.Add(product);
               ReportStoreQuantities = new ObservableCollection<InventoryReportStoreQuantityModel>(StoreQuantities);
           });

            this.SaveCommand = ReactiveCommand.Create(async () =>
           {
               if (StoreReporting.ProductId == 0) { this.Alert("商品未指定！"); return; }
               if (StoreReporting.BigQuantity == 0 && StoreReporting.SmallQuantity == 0)
               { this.Alert("采购量必须指定一个！"); return; }

               StoreReporting.InventoryReportStoreQuantities = ReportStoreQuantities.ToList();

               if (StoreReporting.InventoryReportStoreQuantities.Count == 0)
               { this.Alert("库存量必须指定一个！"); return; }

               if (StoreReporting != null && !string.IsNullOrEmpty(ReferencePage))
               {
                   var redirectPage = $"../../";
                   await this.NavigateAsync(redirectPage, ("ProductSelect", StoreReporting));
               }
               else
               {
                   await _navigationService.GoBackAsync(("ProductSelect", StoreReporting));
               }
           });

            this.RemoveCommend.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.SaveCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.AddCommend.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.ExceptionsSubscribe();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                if (parameters.ContainsKey("InventoryReportItemModel"))
                {
                    parameters.TryGetValue<InventoryReportItemModel>("InventoryReportItemModel", out InventoryReportItemModel storeReporting);
                    if (storeReporting != null)
                    {
                        StoreReporting = storeReporting;
                        ReportStoreQuantities = storeReporting.InventoryReportStoreQuantities;
                    }
                }

                if (parameters.ContainsKey("Products"))
                {
                    parameters.TryGetValue<List<ProductModel>>("Products", out List<ProductModel> products);
                    if (products != null)
                    {
                        ProductSeries = new ObservableCollection<ProductModel>(products);
                    }
                }

                var product = ProductSeries.FirstOrDefault();
                if (product != null)
                {
                    StoreReporting = new InventoryReportItemModel()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        BigUnitId = product.BigPriceUnit.UnitId,
                        BigUnitName = product.BigPriceUnit.UnitName,
                        SmallUnitId = product.SmallPriceUnit.UnitId,
                        SmallUnitName = product.SmallPriceUnit.UnitName
                    };
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
