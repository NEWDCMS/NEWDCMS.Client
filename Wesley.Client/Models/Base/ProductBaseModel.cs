using Wesley.Client.Models.Products;
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
        public int? StockQty { get; set; } = 0;

        /// <summary>
        /// 可用库存数量
        /// </summary>
        public int? UsableQuantity { get; set; } = 0;
        public string UsableQuantityConversion;

        /// <summary>
        /// 预占库存数量
        /// </summary>
        public int? CurrentQuantity { get; set; } = 0;
        public string CurrentQuantityConversion;

        /// <summary>
        ///预占库存数量 
        /// </summary>
        public int? OrderQuantity { get; set; } = 0;
        public string OrderQuantityConversion;

        /// <summary>
        /// 锁定库存数量
        /// </summary>
        public int? LockQuantity { get; set; } = 0;
        public string LockQuantityConversion;

        /// <summary>
        /// 是否调度商品
        /// </summary>
        [Reactive] public bool IsDispatchProduct { get; set; }

    }

}
