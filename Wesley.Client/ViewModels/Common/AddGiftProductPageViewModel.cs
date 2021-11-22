using Wesley.Client.Models.Campaigns;
using Wesley.Client.Models.Products;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Xamarin.Forms.Internals;

namespace Wesley.Client.ViewModels
{
    public class AddGiftProductPageViewModel : ViewModelBase
    {
        public ReactiveCommand<string, Unit> TextChangedCommand { get; set; }
        [Reactive] public ObservableCollection<CampaignBuyGiveProductModelGroup> CampaignSeries { get; set; } = new ObservableCollection<CampaignBuyGiveProductModelGroup>();

        public AddGiftProductPageViewModel(
            INavigationService navigationService,
              IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "添加促销商品";

            //更改组
            this.TextChangedCommand = ReactiveCommand.Create<string>((e) =>
            {
                CampaignSeries.ForEach(c =>
                {
                    if (c.Combination <= 0)
                        c.Combination = 1;

                    c.ForEach(p =>
                    {
                        p.Quantity = p.QuantityCopy * c.Combination;
                    });
                });
            });

            //保存
            this.SaveCommand = ReactiveCommand.Create(async () =>
           {
               try
               {
                   var gifts = this.CampaignSeries.ToList();
                   if (gifts.Any())
                   {
                       var products = new List<ProductModel>();
                       gifts.ForEach(g =>
                       {
                           g.ForEach(p =>
                           {
                               var product = new ProductModel()
                               {
                                   Id = p.Id,
                                   ProductId = p.Id,
                                   ProductName = p.ProductName,
                                   Name = p.Name,
                                   UnitId = p.UnitId,
                                   Quantity = p.Quantity,
                                   Price = p.Price,
                                   Amount = p.Amount,
                                   Remark = p.CampaignName,
                                   Subtotal = p.Subtotal,
                                   StockQty = p.StockQty,
                                   UnitConversion = p.UnitConversion,
                                   UnitName = p.UnitName,
                                   Units = p.Units,
                                   CurrentQuantity = p.CurrentQuantity,
                                   UsableQuantity = p.UsableQuantity,
                                   IsShowGiveEnabled = true,
                                   //
                                   TypeId = p.TypeId,
                                   QuantityCopy = p.QuantityCopy,
                                   CampaignId = p.CampaignId,
                                   CampaignName = p.CampaignName,
                               };

                               if (p.UnitId == p.BigPriceUnit.UnitId)
                               {
                                   product.BigPriceUnit = new PriceUnit()
                                   {
                                       Amount = 0,
                                       Price = p.TypeId == 2 ? 0 : p.Price ?? 0,
                                       Quantity = p.Quantity,
                                       Remark = p.CampaignName,
                                       UnitId = p.BigUnitId ?? 0,
                                       UnitName = p.BigPriceUnit.UnitName
                                   };
                               }

                               if (p.UnitId == p.SmallPriceUnit.UnitId)
                               {
                                   product.SmallPriceUnit = new PriceUnit()
                                   {
                                       Amount = 0,
                                       Price = p.TypeId == 2 ? 0 : p.Price ?? 0,
                                       Quantity = p.Quantity,
                                       Remark = p.CampaignName,
                                       UnitId = p.SmallUnitId ?? 0,
                                       UnitName = p.SmallPriceUnit.UnitName
                                   };
                               }

                               products.Add(product);
                           });
                       });
                       //转向引用页
                       await this.NavigateAsync($"../../../", ("ProductSeries", products));
                   }
               }
               catch (Exception ex)
               {
                   Crashes.TrackError(ex);
               }
           });
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            //获取选择的商品回传
            if (parameters.ContainsKey("CampaignProducts"))
            {
                parameters.TryGetValue("CampaignProducts", out List<CampaignBuyGiveProductModelGroup> products);
                if (products != null && products.Any())
                    CampaignSeries = new ObservableCollection<CampaignBuyGiveProductModelGroup>(products);
            }
        }

    }
}
