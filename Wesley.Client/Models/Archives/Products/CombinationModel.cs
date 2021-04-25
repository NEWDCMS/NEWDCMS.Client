using System.Collections.Generic;


namespace Wesley.Client.Models.Products
{

    public partial class CombinationListModel
    {
        public CombinationListModel()
        {

        }


        public IList<CombinationModel> Items { get; set; }
    }



    public partial class ProductCombinationListModel
    {

    }


    public partial class CombinationModel : EntityBase
    {
        public CombinationModel()
        {
            SubProducts = new List<ProductCombinationModel>();
        }
        public bool Enabled { get; set; }
        public int DisplayOrder { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }



        public IList<ProductCombinationModel> SubProducts { get; set; }

        public string JSONData { get; set; }

    }

    public class ProductCombinationModel : EntityBase
    {
        public int CombinationId { get; set; }


        //("商品名称", "商品名称
        public int ProductId { get; set; }
        public string ProductName { get; set; }


        //("数量", "数量
        public int Quantity { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }


        //("单位", "单位(规格属性项)
        public int UnitId { get; set; }
        public SelectList Units { get; set; }
        public string UnitName { get; set; }

        //("排序", "排序
        public int DisplayOrder { get; set; }

    }
}