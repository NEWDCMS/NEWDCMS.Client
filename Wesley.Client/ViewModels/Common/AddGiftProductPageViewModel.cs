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
              IDialogService dialogService) : base(navigationService, dialogService)
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
                           g.ForEach(s =>
                           {
                               var product = new ProductModel()
                               {
                                   Id = s.Id,
                                   ProductId = s.Id,
                                   ProductName = s.ProductName,
                                   Name = s.Name,
                                   UnitId = s.UnitId,
                                   Quantity = s.Quantity,
                                   Price = s.Price,
                                   Amount = s.Amount,
                                   Remark = s.CampaignName,
                                   Subtotal = s.Subtotal,
                                   StockQty = s.StockQty,
                                   UnitConversion = s.UnitConversion,
                                   UnitName = s.UnitName,
                                   Units = s.Units,
                                   CurrentQuantity = s.CurrentQuantity,
                                   UsableQuantity = s.UsableQuantity,
                                   IsShowGiveEnabled = true,
                                   //
                                   TypeId = s.TypeId,
                                   QuantityCopy = s.QuantityCopy,
                                   CampaignId = s.CampaignId,
                                   CampaignName = s.CampaignName,
                               };

                               if (s.BigUnitId == product.UnitId)
                               {
                                   product.BigPriceUnit = new PriceUnit()
                                   {
                                       Amount = 0,
                                       Price = s.TypeId == 2 ? 0 : s.Price,
                                       Quantity = s.Quantity,
                                       Remark = s.CampaignName,
                                       UnitId = s.UnitId,
                                       UnitName = s.UnitName
                                   };
                               }

                               if (s.SmallUnitId == product.UnitId)
                               {
                                   product.SmallPriceUnit = new PriceUnit()
                                   {
                                       Amount = 0,
                                       Price = s.TypeId == 2 ? 0 : s.Price,
                                       Quantity = s.Quantity,
                                       Remark = s.CampaignName,
                                       UnitId = s.UnitId,
                                       UnitName = s.UnitName
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
                CampaignSeries = new ObservableCollection<CampaignBuyGiveProductModelGroup>(products);
            }
        }

    }
}
