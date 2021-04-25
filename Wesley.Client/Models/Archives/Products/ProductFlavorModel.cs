using System.Collections.Generic;

namespace Wesley.Client.Models.Products
{

    public partial class ProductFlavorListModel
    {
        public ProductFlavorListModel()
        {

        }


        public IList<ProductFlavorModel> Items { get; set; }
    }


    //typeof(ProductFlavorValidator))]
    public partial class ProductFlavorModel : EntityBase
    {
        //("商品", "商品
        public int ProductId { get; set; }

        //("口味", "口味
        public string Name { get; set; }

        //("小单位条码", "小单位条码
        public string SmallUnitBarCode { get; set; }

        //("中单位条码", "中单位条码
        public string StrokeUnitBarCode { get; set; }

        //("大单位条码", "大单位条码
        public string BigUnitBarCode { get; set; }
    }
}