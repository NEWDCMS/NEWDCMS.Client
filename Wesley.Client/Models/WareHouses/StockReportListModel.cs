using System;
using System.Collections.Generic;

namespace Wesley.Client.Models.WareHouses
{
    /// <summary>
    /// 用于分组
    /// </summary>
    public class StockCategoryGroup : List<StockReportProduct>
    {
        /// <summary>
        /// 类别
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 成本金额小计
        /// </summary>
        public decimal? SubCostAmount { get; set; }
        public StockCategoryGroup(string categoryName, List<StockReportProduct> bills) : base(bills)
        {
            CategoryName = categoryName;
        }
    }

    public class StockReportProduct
    {

        /// <summary>
        /// 商品Id
        /// </summary>
        public int? ProductId { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 条形码（小）
        /// </summary>
        public string SmallBarCode { get; set; }
        /// <summary>
        /// 条形码（中）
        /// </summary>
        public string StrokeBarCode { get; set; }
        /// <summary>
        /// 条形码（大）
        /// </summary>
        public string BigBarCode { get; set; }

        /// <summary>
        /// 单位Id（小）
        /// </summary>
        public int? SmallUnitId { get; set; }
        /// <summary>
        /// 单位名称（小）
        /// </summary>
        public string SmallUnitName { get; set; }
        /// <summary>
        /// 单位Id（中）
        /// </summary>
        public int? StrokeUnitId { get; set; }
        /// <summary>
        /// 单位名称（中）
        /// </summary>
        public string StrokeUnitName { get; set; }
        /// <summary>
        /// 单位Id（大）
        /// </summary>
        public int? BigUnitId { get; set; }
        /// <summary>
        /// 单位名称（大）
        /// </summary>
        public string BigUnitName { get; set; }

        /// <summary>
        /// 大单位转化量
        /// </summary>
        public int? BigQuantity { get; set; }
        /// <summary>
        /// 中单位转化量
        /// </summary>
        public int? StrokeQuantity { get; set; }

        /// <summary>
        /// 单位换算
        /// </summary>
        public string UnitConversion { get; set; }

        /// <summary>
        /// 商品类别Id
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 商品类别名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 商品品牌Id
        /// </summary>
        public int BrandId { get; set; }
        /// <summary>
        /// 商品品牌名称
        /// </summary>
        public string BrandName { get; set; }

        public int WareHouseId { get; set; }

        /// <summary>
        /// 现货库存数量
        /// </summary>
        public int CurrentQuantity { get; set; }
        /// <summary>
        /// 现货库存数量(数量转换)
        /// </summary>
        public string CurrentQuantityConversion { get; set; }
        public Tuple<int, int, int> CurrentQuantityPart { get; set; } = new Tuple<int, int, int>(0, 0, 0);


        /// <summary>
        /// 可用库存数量
        /// </summary>
        public int UsableQuantity { get; set; }
        /// <summary>
        /// 可用库存数量(数量转换)
        /// </summary>
        public string UsableQuantityConversion { get; set; }
        public Tuple<int, int, int> UsableQuantityPart { get; set; } = new Tuple<int, int, int>(0, 0, 0);


        /// <summary>
        /// 预占库存数量
        /// </summary>
        public int OrderQuantity { get; set; }
        /// <summary>
        /// 预占库存数量(数量转换)
        /// </summary>
        public string OrderQuantityConversion { get; set; }

        public Tuple<int, int, int> OrderQuantityPart { get; set; } = new Tuple<int, int, int>(0, 0, 0);

        /// <summary>
        /// 成本单价(元)
        /// </summary>
        public decimal CostPrice { get; set; }
        /// <summary>
        /// 成本金额(元)
        /// </summary>
        public decimal CostAmount { get; set; }

        /// <summary>
        /// 批发单价(元)
        /// </summary>
        public decimal TradePrice { get; set; }
        /// <summary>
        /// 批发金额(元)
        /// </summary>
        public decimal TradeAmount { get; set; }



        public class UnitQuantityPart
        {
            /// <summary>
            /// 小单位量
            /// </summary>
            public int Small { get; set; }
            /// <summary>
            /// 中单位量
            /// </summary>
            public int Stroke { get; set; }
            /// <summary>
            /// 大单位量
            /// </summary>
            public int Big { get; set; }

        }
    }
}