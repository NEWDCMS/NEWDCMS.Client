using Wesley.Infrastructure.Helpers;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;


namespace Wesley.Client.Models.Products
{
    public class MultiValue
    {
        public int Pid { get; set; }
        public int Type { get; set; }
    }

    /// <summary>
    /// ProductBaseModel
    /// </summary>
    [Serializable]
    public partial class ProductModel : ProductBaseModel
    {
        [Reactive] public bool Favorited { get; set; }
        [Reactive] public bool IsShowStock { get; set; } = true;
        [Reactive] public bool IsShowGiveEnabled { get; set; } = true;
        public int CombinationId { get; set; }
        [Reactive] public string Name { get; set; }
        public string MnemonicCode { get; set; } = CommonHelper.GenerateStrchar(5);
        [Reactive] public int CategoryId { get; set; }
        [Reactive] public string CategoryName { get; set; }
        [Reactive] public int BrandId { get; set; }
        [Reactive] public string BrandName { get; set; }
        public int? SmallUnitId { get; set; }
        [Reactive] public string SmallUnitName { get; set; }
        public SelectList SmallUnits { get; set; }
        public int? StrokeUnitId { get; set; }
        [Reactive] public string StrokeUnitName { get; set; }
        public SelectList StrokeUnits { get; set; }
        public int? BigUnitId { get; set; }
        [Reactive] public string BigUnitName { get; set; }

        public SelectList BigUnits { get; set; }
        public bool IsAdjustPrice { get; set; }
        public bool Status { get; set; }
        public bool IsManufactureDete { get; set; }
        public string ProductCode { get; set; }
        public string Specification { get; set; }
        public int? Number { get; set; }
        public string CountryOrigin { get; set; }
        public int? Supplier { get; set; }
        public string SupplierName { get; set; }
        public string OtherBarCode { get; set; }
        public string OtherBarCode1 { get; set; }
        public string OtherBarCode2 { get; set; }
        public int? StatisticalType { get; set; }
        public SelectList StatisticalTypes { get; set; }
        public int? ExpirationDays { get; set; }
        public int? AdventDays { get; set; }
        public string ProductImages { get; set; }
        public string SmallBarCode { get; set; }
        public string StrokeBarCode { get; set; }
        public string BigBarCode { get; set; }
        public decimal? Subtotal { get; set; }
        [Reactive] public decimal? Price { get; set; }
        [Reactive] public decimal? Amount { get; set; }
        [Reactive] public int Quantity { get; set; }


        [Reactive] public PriceUnit BigPriceUnit { get; set; } = new PriceUnit();
        [Reactive] public PriceUnit SmallPriceUnit { get; set; } = new PriceUnit();
        [Reactive] public GiveProduct GiveProduct { get; set; } = new GiveProduct();

        [Reactive] public Dictionary<string, string> UnitPriceDicts { get; set; } = new Dictionary<string, string>();

        [Reactive] public int ShipmentUsableQuantity { get; set; }
        [Reactive] public int IncomeUsableQuantity { get; set; }
        [Reactive] public int ShipmentCurrentQuantity { get; set; }
        [Reactive] public int IncomeCurrentQuantity { get; set; }

        [Reactive] public string ShipmentWareHouseName { get; set; }
        [Reactive] public string IncomeWareHouseName { get; set; }

        public DateTime? ManufactureDete { get; set; }
        [Reactive] public string ManufactureDateStr { get; set; }
        
        public bool IsShowCreateDate { get; set; }
        public int DisplayOrder { get; set; }
        public int CurrentStock { get; set; }
        public List<string> ProductTimes { get; set; }
        //是否赠品
        public bool IsGifts { get; set; } = false;
        //促销活动
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public int QuantityCopy { get; set; }
        public int CampaignId { get; set; } = 0;
        public string CampaignName { get; set; }

        [JsonIgnore]
        public ReactiveCommand<UProduct, Unit> RemarkSelected { get; set; }
        [JsonIgnore]
        public ReactiveCommand<ProductModel, Unit> RemarkSelected2 { get; set; }
    }


    [Serializable]
    public class StockQuantityModel : Base
    {
        /// <summary>
        /// 可用
        /// </summary>
        public int UsableQuantity { get; set; }
        /// <summary>
        /// 现货
        /// </summary>
        public int CurrentQuantity { get; set; }

        ///public int StockQuantity { get; set; }
        ///
        [Reactive] public int WareHouseId { get; set; }
    }


    [Serializable]
    public class ProductPriceModel : Base
    {
        public int ProductId { get; set; }
        [Reactive] public int UnitId { get; set; }
        [Reactive] public string UnitName { get; set; }
        [Reactive] public decimal? TradePrice { get; set; }
        [Reactive] public decimal? RetailPrice { get; set; }
        [Reactive] public decimal? FloorPrice { get; set; }
        [Reactive] public decimal? PurchasePrice { get; set; }
        [Reactive] public decimal? CostPrice { get; set; }
        [Reactive] public decimal? SALE1 { get; set; }
        [Reactive] public decimal? SALE2 { get; set; }
        [Reactive] public decimal? SALE3 { get; set; }
    }

    [Serializable]
    public class UnitPricesModel
    {
        public int UnitId { get; set; }
        public ProductPriceModel ProductPrice { get; set; } = new ProductPriceModel();
        public string UnitConversion { get; set; }
    }

    public class UProduct : Base
    {
        [Reactive] public string Remark { get; set; }
    }


    [Serializable]
    public class PriceUnit : UProduct
    {
        public int UnitId { get; set; }
        [Reactive] public int Quantity { get; set; }
        [Reactive] public decimal Price { get; set; }
        [Reactive] public decimal Amount { get; set; }
        [Reactive] public string UnitName { get; set; }
    }

    [Serializable]
    public class GiveProduct : UProduct
    {
        [Reactive] public int BigUnitQuantity { get; set; }
        [Reactive] public string BigUnitName { get; set; }
        [Reactive] public int SmallUnitQuantity { get; set; }
        [Reactive] public string SmallUnitName { get; set; }
    }
}