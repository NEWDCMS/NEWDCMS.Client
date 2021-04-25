using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Wesley.Client.Models.Sales
{

    /// <summary>
    /// 退货单
    /// </summary>
    public class ReturnBillModel : AbstractBill, IBCollection<ReturnItemModel>
    {
        [Reactive] public string BillBarCode { get; set; }
        public int ReturnReservationBillId { get; set; }
        public string TerminalPointCode { get; set; }
        public SelectList BusinessUsers { get; set; }
        public SelectList DeliveryUsers { get; set; }
        public SelectList ParentList { get; set; }
        public SelectList Districts { get; set; }
        public SelectList WareHouses { get; set; }
        public int PayTypeId { get; set; }
        public string PayTypeName { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string DefaultAmountId { get; set; } = Wesley.Client.Settings.DefaultPricePlan;
        public string DefaultAmountName { get; set; }
        public List<SelectListItem> ReturnDefaultAmounts { get; set; }
        [Reactive] public decimal ReceivableAmount { get; set; }

        [Reactive] public decimal PreferentialEndAmount { get; set; }
        [Reactive] public decimal OweCashTotal { get; set; }

        public decimal SumCostPrice { get; set; }
        public decimal SumCostAmount { get; set; }
        public decimal SumProfit { get; set; }
        public decimal SumCostProfitRate { get; set; }
        public string Status { get; set; }

        public int CollectionAccount { get; set; }
        public decimal? CollectionAmount { get; set; }
        public string AuditedUserName { get; set; }
        public int PrintNum { get; set; }
        [Reactive] public ObservableCollection<ReturnItemModel> Items { get; set; } = new ObservableCollection<ReturnItemModel>();
        [Reactive] public ObservableCollection<AccountMaping> ReturnBillAccountings { get; set; } = new ObservableCollection<AccountMaping>();
        [Reactive] public string[] Accountings { get; set; } = new string[] { };
        public bool? HandInStatus { get; set; }
        public DateTime? HandInDate { get; set; }
        public bool Receipted { get; set; }
        public bool IsShowCreateDate { get; set; }
        public int VariablePriceCommodity { get; set; }
        public int AccuracyRounding { get; set; }
        public int AllowSelectionDateRange { get; set; }
        public bool AllowAdvancePaymentsNegative { get; set; }

    }

    /// <summary>
    /// 退货单明细
    /// </summary>
    public class ReturnItemModel : ProductBaseModel
    {
        public int ReturnBillId { get; set; }
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
        public int BigUnitId { get; set; }
        public bool IsManufactureDete { get; set; }
        public DateTime? ManufactureDete { get; set; }
        public IList<string> ProductTimes { get; set; } = new List<string>();

        #endregion


        [Reactive] public decimal Subtotal { get; set; }
        public bool IsGifts { get; set; } = false;
        public int? BigGiftQuantity { get; set; }
        public int? SmallGiftQuantity { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxPrice { get; set; }
        public decimal ContainTaxPrice { get; set; }
        public decimal TaxPriceAmount { get; set; }
    }


    /// <summary>
    ///  收款账户（退货单科目映射表）
    /// </summary>
    public class ReturnBillAccountingModel : EntityBase
    {

        public string Name { get; set; }
        public int AccountingOptionId { get; set; }
        public int ReturnId { get; set; }
        public decimal CollectionAmount { get; set; }
        public bool IsDefault { get; set; }

    }


    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class ReturnBillUpdateModel : Base
    {
        [Reactive] public int TerminalId { get; set; }
        [Reactive] public int BusinessUserId { get; set; }
        [Reactive] public int WareHouseId { get; set; }
        [Reactive] public int DeliveryUserId { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsMinUnitSale { get; set; }
        public string Remark { get; set; }
        public string DefaultAmountId { get; set; }
        public decimal PreferentialAmount { get; set; }
        public decimal PreferentialEndAmount { get; set; }
        public decimal OweCash { get; set; }
        public List<ReturnItemModel> Items { get; set; } = new List<ReturnItemModel>();
        public List<AccountMaping> Accounting { get; set; } = new List<AccountMaping>();

        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }
        public int DispatchItemId { get; set; }
        /// <summary>
        /// 终端纬度坐标
        /// </summary>
        public double? Latitude { get; set; } = 0;
        /// <summary>
        /// 终端经度坐标
        /// </summary>
        public double? Longitude { get; set; } = 0;

    }



}
