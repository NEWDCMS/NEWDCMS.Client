using System.Collections.Generic;


namespace Wesley.Client.Models.Products
{


    public partial class ProductTierPricePlanListModel
    {
        public ProductTierPricePlanListModel()
        {

        }

        //("关键字", "搜索关键字
        public string Key { get; set; }

        public string Name { get; set; }


        public IList<ProductTierPricePlanModel> Items { get; set; }
    }
    public partial class ProductTierPricePlanModel : EntityBase
    {
        public string StoreName { get; set; }

        //("方案名", "方案名
        public string Name { get; set; }
    }


    public partial class ProductTierPriceModel : EntityBase
    {

        //("商品", "商品
        public int ProductId { get; set; }


        //("方案", "自定义方案
        public int PricesPlanId { get; set; }


        //("表示价格类型", "表示价格类型
        public int PriceTypeId { get; set; }
        public string PriceType { get; set; }
        public string PriceTypeName { get; set; }


        public decimal? SmallUnitPrice { get; set; } = 0;
        public int SmallUnitId { get; set; } = 0;

        public decimal? StrokeUnitPrice { get; set; } = 0;
        public int StrokeUnitId { get; set; } = 0;


        public decimal? BigUnitPrice { get; set; } = 0;
        public int BigUnitId { get; set; } = 0;

    }

}