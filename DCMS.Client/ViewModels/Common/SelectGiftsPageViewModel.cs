using DCMS.Client.Models.Campaigns;
using DCMS.Client.Models.Products;
using DCMS.Client.Pages.Common;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace DCMS.Client.ViewModels
{
    public class SelectGiftsPageViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private readonly ICampaignService _campaignService;
        [Reactive] public ObservableCollection<CampaignBuyGiveProductModelGroup> CampaignSeries { get; set; } = new ObservableCollection<CampaignBuyGiveProductModelGroup>();

        public SelectGiftsPageViewModel(INavigationService navigationService,
            IProductService productService,
            ICampaignService campaignService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "选择促销品";

            _productService = productService;
            _campaignService = campaignService;

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

            //Load
            this.Load = CampaignBuyGiveProductLoader.Load(async () =>
            {
                string name = "";
                int terminalId = Terminal?.Id ?? 0;
                int channelId = Terminal?.ChannelId ?? 0; //2375
                int wareHouseId = WareHouse?.Id ?? 0;
                int pagenumber = 0;
                int pageSize = 50;

                var campaigns = new List<CampaignBuyGiveProductModelGroup>();

                try
                {
                    var products = await _campaignService.GetAllCampaigns(name,
                        terminalId,
                        channelId,
                        wareHouseId,
                        pagenumber,
                        pageSize,
                        this.ForceRefresh,
                        new System.Threading.CancellationToken());

                    if (products != null)
                    {
                        foreach (var grop in products.GroupBy(s => s.CampaignId))
                        {
                            var first = grop.FirstOrDefault();
                            var items = new List<CampaignProduct>();

                            //购买
                            foreach (var buy in grop.ToList())
                            {
                                var cps = buy.CampaignBuyProducts.Select(s =>
                                {
                                    return new CampaignProduct()
                                    {
                                        Id = s.ProductId,
                                        ProductId = s.ProductId,
                                        ProductName = s.ProductName,
                                        Name = s.Name,
                                        UnitId = s.UnitId,
                                        Quantity = s.Quantity,
                                        QuantityCopy = s.Quantity,
                                        Price = s.Price,
                                        Amount = s.Amount,
                                        Remark = s.Remark,
                                        Subtotal = s.Subtotal,
                                        StockQty = s.StockQty,
                                        UnitConversion = s.UnitConversion,
                                        UnitName = s.UnitName,
                                        Units = s.Units,
                                        BigUnitId = s.BigUnitId,
                                        SmallUnitId = s.SmallUnitId,
                                        BigPriceUnit = new PriceUnit()
                                        {
                                            Amount = s.BigPriceUnit.Amount,
                                            Price = s.BigPriceUnit.Price,
                                            Quantity = s.BigPriceUnit.Quantity,
                                            Remark = s.BigPriceUnit.Remark,
                                            UnitId = s.BigUnitId ?? 0,
                                            UnitName = s.BigPriceUnit.UnitName
                                        },
                                        SmallPriceUnit = new PriceUnit()
                                        {
                                            Amount = s.SmallPriceUnit.Amount,
                                            Price = s.SmallPriceUnit.Price,
                                            Quantity = s.SmallPriceUnit.Quantity,
                                            Remark = s.SmallPriceUnit.Remark,
                                            UnitId = s.SmallUnitId ?? 0,
                                            UnitName = s.SmallPriceUnit.UnitName
                                        },
                                        //促销(售)
                                        TypeId = s.BuyProductTypeId,
                                        TypeName = s.BuyProductTypeName,
                                        CampaignId = first.CampaignId,
                                        CampaignName = first.CampaignName,
                                    };
                                }).ToList();
                                items.AddRange(cps);
                            }

                            //赠送
                            foreach (var give in grop.ToList())
                            {
                                var cps = give.CampaignGiveProducts.Select(s =>
                                {
                                    return new CampaignProduct()
                                    {
                                        Id = s.ProductId,
                                        ProductId = s.ProductId,
                                        ProductName = s.ProductName,
                                        Name = s.Name,
                                        UnitId = s.UnitId,
                                        Quantity = s.Quantity,
                                        QuantityCopy = s.Quantity,
                                        Price = s.Price,
                                        Amount = s.Amount,
                                        Remark = s.Remark,
                                        Subtotal = s.Subtotal,
                                        StockQty = s.StockQty,
                                        UnitConversion = s.UnitConversion,
                                        UnitName = s.UnitName,
                                        Units = s.Units,
                                        BigUnitId = s.BigUnitId,
                                        SmallUnitId = s.SmallUnitId,
                                        BigPriceUnit = new PriceUnit()
                                        {
                                            Amount = s.BigPriceUnit.Amount,
                                            Price = s.BigPriceUnit.Price,
                                            Quantity = s.BigPriceUnit.Quantity,
                                            Remark = s.BigPriceUnit.Remark,
                                            UnitId = s.BigUnitId ?? 0,
                                            UnitName = s.BigPriceUnit.UnitName
                                        },
                                        SmallPriceUnit = new PriceUnit()
                                        {
                                            Amount = s.SmallPriceUnit.Amount,
                                            Price = s.SmallPriceUnit.Price,
                                            Quantity = s.SmallPriceUnit.Quantity,
                                            Remark = s.SmallPriceUnit.Remark,
                                            UnitId = s.SmallUnitId ?? 0,
                                            UnitName = s.SmallPriceUnit.UnitName
                                        },
                                        //促销（赠）
                                        TypeId = s.GiveProductTypeId,
                                        TypeName = s.GiveProductTypeName,
                                        CampaignId = first.CampaignId,
                                        CampaignName = first.CampaignName,
                                    };
                                }).ToList();
                                items.AddRange(cps);
                            }

                            campaigns.Add(new CampaignBuyGiveProductModelGroup(grop.Key, first.CampaignName, false, items));
                        }

                        if (campaigns.Any())
                            this.CampaignSeries = new ObservableCollection<CampaignBuyGiveProductModelGroup>(campaigns);
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                return campaigns;
            });


            //保存选择商品
            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                var gifts = this.CampaignSeries.Where(s => s.Selected).ToList();
                if (!gifts.Any())
                {
                    this.Alert("请选择赠品！");
                    return;
                }

                //添加单据商品
                await this.NavigateAsync($"{nameof(AddGiftProductPage)}",
                   ("WareHouse", WareHouse),
                   ("Reference", ReferencePage),
                   ("CampaignProducts", gifts)
               );

            });

            this.BindBusyCommand(Load);
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                ((ICommand)Load)?.Execute(null);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
