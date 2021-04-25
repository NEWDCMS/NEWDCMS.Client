using Wesley.Client.Models.Products;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;

namespace Wesley.Client.Models.Campaigns
{
    public class CampaignBuyProductModel : ProductModel
    {
        public string ProductSKU { get; set; }
        public int BuyProductTypeId { get; set; }
        public string BuyProductTypeName { get; set; }
        public Dictionary<int, decimal> CostPrices { get; set; }
    }
    public class CampaignGiveProductModel : ProductModel
    {
        public string ProductSKU { get; set; }
        public int GiveProductTypeId { get; set; }
        public string GiveProductTypeName { get; set; }
        public Dictionary<int, decimal> CostPrices { get; set; }
    }

    public class CampaignProduct : ProductModel
    {

    }

    public class CampaignBuyGiveProductModel
    {
        public int CampaignId { get; set; } = 0;
        public string CampaignName { get; set; }
        public int? GiveTypeId { get; set; } = 0;
        public string BuyProductMessage { get; set; }
        public List<CampaignBuyProductModel> CampaignBuyProducts = new List<CampaignBuyProductModel>();
        public string GiveProductMessage { get; set; }
        public List<CampaignGiveProductModel> CampaignGiveProducts = new List<CampaignGiveProductModel>();
        public string CampaignLinkNumber { get; set; }
        public int SaleBuyQuantity { get; set; } = 1;
    }
    public class CampaignBuyGiveProductModelGroup : List<CampaignProduct>
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        [Reactive] public bool Selected { get; set; }
        [Reactive] public int Combination { get; set; } = 1;
        [Reactive] public string Remark { get; set; }

        public CampaignBuyGiveProductModelGroup(int campaignId, string campaignName, bool selected, List<CampaignProduct> items) : base(items)
        {
            CampaignId = campaignId;
            CampaignName = campaignName;
            Selected = selected;
        }
    }

}
