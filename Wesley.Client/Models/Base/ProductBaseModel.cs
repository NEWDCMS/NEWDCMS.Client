using Wesley.Client.Models.Products;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;

namespace Wesley.Client.Models
{

    [Serializable]
    public class ProductBaseModel : EntityBase
    {
        public int ProductId { get; set; }

        [JsonIgnore]
        public IReactiveCommand SelectCommand { get; set; }
        [Reactive] public bool Selected { get; set; }
        [Reactive] public string ProductName { get; set; }
        [Reactive] public string BarCode { get; set; }

        public int UnitId { get; set; }
        [Reactive] public Dictionary<string, int> Units { get; set; }
        [Reactive] public string UnitName { get; set; }
        [Reactive] public string UnitAlias { get; set; } = "SMALL";
        [Reactive] public string UnitConversion { get; set; }
        [Reactive] public int? BigQuantity { get; set; }
        [Reactive] public int? StrokeQuantity { get; set; }

        public SpecificationAttributeOptionModel smallOption { get; set; } = new SpecificationAttributeOptionModel();
        public SpecificationAttributeOptionModel strokeOption { get; set; } = new SpecificationAttributeOptionModel();
        public SpecificationAttributeOptionModel bigOption { get; set; } = new SpecificationAttributeOptionModel();

        [Reactive] public ProductPriceModel BigProductPrices { get; set; } = new ProductPriceModel();
        [Reactive] public ProductPriceModel StrokeProductPrices { get; set; } = new ProductPriceModel();
        [Reactive] public ProductPriceModel SmallProductPrices { get; set; } = new ProductPriceModel();


        [Reactive] public IList<UnitPricesModel> Prices { get; set; } = new List<UnitPricesModel>();
        [Reactive] public IList<ProductTierPriceModel> ProductTierPrices { get; set; } = new List<ProductTierPriceModel>();
        [Reactive] public IList<StockQuantityModel> StockQuantities { get; set; } = new List<StockQuantityModel>();

        [Reactive] public string Remark { get; set; }
        [Reactive] public string CurWareHouseName { get; set; }
        [Reactive] public bool ShowCurWareHouseName { get; set; } = true;
        public int? StockQty { get; set; } = 0;

        /// <summary>
        /// 可用库存数量
        /// </summary>
        public int? UsableQuantity { get; set; } = 0;
        [Reactive] public string UsableQuantityConversion { get; set; }

        /// <summary>
        /// 预占库存数量
        /// </summary>
        public int? CurrentQuantity { get; set; } = 0;
        [Reactive] public string CurrentQuantityConversion { get; set; }

        /// <summary>
        /// 预占库存数量 
        /// </summary>
        public int? OrderQuantity { get; set; } = 0;
        [Reactive] public string OrderQuantityConversion { get; set; }

        /// <summary>
        /// 锁定库存数量
        /// </summary>
        public int? LockQuantity { get; set; } = 0;
        [Reactive] public string LockQuantityConversion { get; set; }

        /// <summary>
        /// 是否调度商品
        /// </summary>
        [Reactive] public bool IsDispatchProduct { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortIndex { get; set; }

    }

    public static class ProductExt
    {

        /// <summary>
        /// 格式化转化量
        /// </summary>
        /// <param name="totalQuantity">总小单位量</param>
        /// <param name="mCQuantity">中单位转化量</param>
        /// <param name="bCQuantity">大单位转化量</param>
        /// <param name="sName">小单位</param>
        /// <param name="mName">中单位</param>
        /// <param name="bName">大单位</param>
        /// <returns></returns>
        public static string FormatQuantity(this ProductModel product, int totalQuantity)
        {
            try
            {
                int thisQuantity = totalQuantity;
                string result = string.Empty;

                var bigUnitName = product.BigUnitName;
                if (string.IsNullOrEmpty(bigUnitName))
                    bigUnitName = product.BigPriceUnit.UnitName;

                var bigQuantity = product.BigQuantity ?? 0;

                var strokeUnitName = product.StrokeUnitName;
                var strokeQuantity = product.StrokeQuantity ?? 0;

                var smallUnitName = product.SmallUnitName;
                if (string.IsNullOrEmpty(smallUnitName))
                    smallUnitName = product.SmallPriceUnit.UnitName;

                int big = 0;
                int stroke = 0;
                int small = 0;

                //大
                if (bigQuantity > 0)
                {
                    big = thisQuantity / bigQuantity;
                    thisQuantity = thisQuantity - big * bigQuantity;
                }
                //中
                if (bigQuantity > 0 && strokeQuantity > 0)
                {
                    stroke = thisQuantity / bigQuantity;
                    thisQuantity = thisQuantity - stroke * strokeQuantity;
                }

                //小
                small = thisQuantity;

                if (big > 0)
                {
                    result += big.ToString() + bigUnitName;
                }

                if (stroke > 0)
                {
                    result += stroke.ToString() + strokeUnitName;
                }

                if (small > 0)
                {
                    result += small.ToString() + smallUnitName;
                }

                return result;
            }
            catch
            {
                return "";
            }
        }
    }

}
