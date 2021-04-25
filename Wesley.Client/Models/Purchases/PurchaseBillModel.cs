using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wesley.Client.Models.Purchases
{
    /// <summary>
    /// 采购单
    /// </summary>
    public class PurchaseBillModel : AbstractBill, IBCollection<PurchaseItemModel>
    {
        [Reactive] public string BillBarCode { get; set; }
        [Reactive] public int ManufacturerId { get; set; }
        [Reactive] public string ManufacturerName { get; set; }
        public SelectList Manufacturers { get; set; }
        public SelectList BusinessUsers { get; set; }
        public SelectList WareHouses { get; set; }
        public DateTime? TransactionDate { get; set; }
        public bool IsMinUnitPurchase { get; set; }
        [Reactive] public decimal PayableAmount { get; set; }
        [Reactive] public decimal PreferentialEndAmount { get; set; }
        [Reactive] public decimal OweCashTotal { get; set; }
        [Reactive] public decimal PrepaidAmount { get; set; }
        public int CollectionAccount { get; set; }
        [Reactive] public decimal CollectionAmount { get; set; }
        public string AuditedUserName { get; set; }
        public int PrintNum { get; set; }
        public bool Paymented { get; set; }
        [Reactive] public ObservableCollection<PurchaseItemModel> Items { get; set; } = new ObservableCollection<PurchaseItemModel>();
        [Reactive] public IList<AccountMaping> PurchaseBillAccountings { get; set; } = new ObservableCollection<AccountMaping>();
        [Reactive] public string[] Accountings { get; set; } = new string[] { };
        public bool IsShowCreateDate { get; set; }
        public int DefaultPurchasePrice { get; set; }
        public string DefaultAmountId { get; set; } = Wesley.Client.Settings.DefaultPricePlan;
        public int AccuracyRounding { get; set; }
        public bool APPShowOrderStock { get; set; }

    }

    /// <summary>
    /// 采购订单明细
    /// </summary>
    public class PurchaseItemModel : ProductBaseModel
    {

        public int PurchaseBillId { get; set; }
        [Reactive] public int Quantity { get; set; }
        [Reactive] public decimal Price { get; set; }
        [Reactive] public decimal Amount { get; set; }
        [Reactive] public int RemainderQty { get; set; }
        public DateTime CreatedOnUtc { get; set; }


        public int? SmallUnitId { get; set; }
        public int? StrokeUnitId { get; set; }
        public int? BigUnitId { get; set; }


        public bool IsManufactureDete { get; set; }
        public DateTime? ManufactureDete { get; set; }
        [Reactive] public decimal Subtotal { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxPrice { get; set; }
        public decimal ContainTaxPrice { get; set; }
        public decimal TaxPriceAmount { get; set; }
    }


    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class PurchaseItemUpdateModel : Base
    {
        public int ManufacturerId { get; set; }
        [Reactive] public int BusinessUserId { get; set; }
        [Reactive] public int WareHouseId { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsMinUnitPurchase { get; set; }
        public string Remark { get; set; }
        public decimal PreferentialAmount { get; set; }
        public decimal PreferentialEndAmount { get; set; }
        public decimal OweCash { get; set; }
        public List<PurchaseItemModel> Items { get; set; } = new List<PurchaseItemModel>();
        public List<AccountMaping> Accounting { get; set; } = new List<AccountMaping>();

    }


}
