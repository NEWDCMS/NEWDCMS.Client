using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Wesley.Client.Models.Sales
{

    /// <summary>
    /// 退货订单
    /// </summary>
    public class ReturnReservationBillModel : AbstractBill, IBCollection<ReturnReservationItemModel>
    {
        [Reactive] public string BillBarCode { get; set; }
        public int ReturnBillId { get; set; }
        public string TerminalPointCode { get; set; }
        public SelectList BusinessUsers { get; set; }
        public SelectList DeliveryUsers { get; set; }
        public SelectList ParentList { get; set; }
        public SelectList Districts { get; set; }
        public SelectList WareHouses { get; set; }
        public int PayTypeId { get; set; }
        public string PayTypeName { get; set; }
        public DateTime? TransactionDate { get; set; }
        public bool IsMinUnitSale { get; set; }
        public string DefaultAmountId { get; set; } = Wesley.Client.Settings.DefaultPricePlan;
        public string DefaultAmountName { get; set; }
        public List<SelectListItem> ReturnReservationDefaultAmounts { get; set; } = new List<SelectListItem>();

        [Reactive] public decimal ReceivableAmount { get; set; }

        [Reactive] public decimal PreferentialEndAmount { get; set; }
        public decimal SubscribeAmount { get; set; }
        public decimal AdvanceCash { get; set; }
        [Reactive] public decimal OweCashTotal { get; set; }

        public decimal SumCostPrice { get; set; }
        public decimal SumCostAmount { get; set; }
        public decimal SumProfit { get; set; }
        public decimal SumCostProfitRate { get; set; }
        public string Status { get; set; }

        public int CollectionAccount { get; set; }
        public decimal CollectionAmount { get; set; }
        public string AuditedUserName { get; set; }

        [Reactive] public ObservableCollection<ReturnReservationItemModel> Items { get; set; } = new ObservableCollection<ReturnReservationItemModel>();
        [Reactive] public ObservableCollection<AccountMaping> ReturnReservationBillAccountings { get; set; } = new ObservableCollection<AccountMaping>();
        [Reactive] public string[] Accountings { get; set; } = new string[] { };
        public string ReturnBillNumber { get; set; }
        public bool IsShowCreateDate { get; set; }
        public int VariablePriceCommodity { get; set; }
        public int AccuracyRounding { get; set; }
        public int AllowSelectionDateRange { get; set; }
        public bool AllowAdvancePaymentsNegative { get; set; }
    }

    /// <summary>
    /// 退货订单明细
    /// </summary>
    public class ReturnReservationItemModel : ProductBaseModel
    {
        public int ReturnReservationBillId { get; set; }
        public string BillNumber { get; set; }
        [Reactive] public int Quantity { get; set; }
        [Reactive] public decimal Price { get; set; }
        [Reactive] public decimal Amount { get; set; }
        public string OrderCode { get; set; }
        public int RemainderQty { get; set; }
        public decimal CostPrice { get; set; }
        public decimal CostAmount { get; set; }
        public decimal Profit { get; set; }
        public decimal CostProfitRate { get; set; }
        public DateTime CreatedOnUtc { get; set; }

        #region 商品信息
        public int SmallUnitId { get; set; }
        public int? StrokeUnitId { get; set; }
        public int BigUnitId { get; set; }
        #endregion

        [Reactive] public decimal Subtotal { get; set; }
        public bool IsGifts { get; set; } = false;
        public int? BigGiftQuantity { get; set; }
        public int? SmallGiftQuantity { get; set; }
        public bool IsManufactureDete { get; set; }
        public DateTime? ManufactureDete { get; set; }
        public IList<string> ProductTimes { get; set; } = new List<string>();
        public decimal TaxRate { get; set; }
        public decimal TaxPrice { get; set; }
        public decimal ContainTaxPrice { get; set; }
        public decimal TaxPriceAmount { get; set; }
    }

    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class ReturnReservationBillUpdateModel : Base
    {
        public string BillNumber { get; set; }
        [Reactive] public int TerminalId { get; set; }
        [Reactive] public int BusinessUserId { get; set; }
        [Reactive] public int WareHouseId { get; set; }
        [Reactive] public int PayTypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsMinUnitSale { get; set; }
        public string Remark { get; set; }
        public string DefaultAmountId { get; set; }
        public decimal PreferentialAmount { get; set; }
        public decimal PreferentialEndAmount { get; set; }
        public decimal OweCash { get; set; }

        public IList<ReturnReservationItemModel> Items { get; set; } = new ObservableCollection<ReturnReservationItemModel>();
        public IList<AccountMaping> Accounting { get; set; } = new ObservableCollection<AccountMaping>();

    }

}
