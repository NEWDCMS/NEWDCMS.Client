using Wesley.Client.Models.Finances;
using Wesley.Client.Models.Terminals;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wesley.Client.Models.Sales
{
    /// <summary>
    /// 销售单
    /// </summary>
    public class SaleBillModel : AbstractBill, IBCollection<SaleItemModel>
    {
        [Reactive] public string BillBarCode { get; set; }
        public int? SaleReservationBillId { get; set; }
        public string TerminalPointCode { get; set; }
        public SelectList BusinessUsers { get; set; }
        public SelectList DeliveryUsers { get; set; }
        public SelectList ParentList { get; set; }
        public SelectList Areas { get; set; }
        public SelectList WareHouses { get; set; }
        public DateTime? TransactionDate { get; set; }
        public bool IsMinUnitSale { get; set; }
        public string DefaultAmountId { get; set; } = Wesley.Client.Settings.DefaultPricePlan;
        public string DefaultAmountName { get; set; }
        public List<SelectListItem> SaleDefaultAmounts { get; set; } = new List<SelectListItem>();
        public decimal AdvanceReceiptAmount { get; set; }

        [Reactive] public decimal ReceivableAmount { get; set; }

        [Reactive] public decimal PreferentialEndAmount { get; set; }
        [Reactive] public decimal OweCashTotal { get; set; }

        [Reactive] public decimal PrepaidAmount { get; set; }
        public decimal SumCostPrice { get; set; }
        public decimal SumCostAmount { get; set; }
        public decimal SumProfit { get; set; }
        public decimal SumCostProfitRate { get; set; }
        public DateTime? PrintDate { get; set; }
        public int CollectionAccount { get; set; }
        public decimal? CollectionAmount { get; set; }
        public string AuditedUserName { get; set; }

        public string ReversedUserName { get; set; }

        public int PrintNum { get; set; }
        public bool Receipted { get; set; }
        public bool SignStatus { get; set; }
        public string SignSecretKey { get; set; }
        public string SignToken { get; set; }
        [Reactive] public ObservableCollection<SaleItemModel> Items { get; set; } = new ObservableCollection<SaleItemModel>();
        [Reactive] public ObservableCollection<AccountMaping> SaleBillAccountings { get; set; } = new ObservableCollection<AccountMaping>();
        [Reactive] public string[] Accountings { get; set; } = new string[] { };
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string SaleReservationBillNumber { get; set; }
        public bool SaleReservationChangePrice { get; set; }
        public int PayTypeId { get; set; }
        public bool? HandInStatus { get; set; }
        public DateTime? HandInDate { get; set; }
        public bool IsShowCreateDate { get; set; }
        public int VariablePriceCommodity { get; set; }
        public int AccuracyRounding { get; set; }
        public int AllowSelectionDateRange { get; set; }
        public bool AllowAdvancePaymentsNegative { get; set; }
        public bool APPShowOrderStock { get; set; }
        public bool SaleChangePrice { get; set; }


        #region 终端、员工欠款
        public TerminalBalance TBalance { get; set; } = new TerminalBalance();
        public decimal? UserMaxAmount { get; set; }
        public decimal? UserUsedAmount { get; set; }
        public decimal? UserAvailableAmount { get; set; }

        #endregion
    }


    /// <summary>
    /// 销售订单明细
    /// </summary>
    public class SaleItemModel : ProductBaseModel
    {
        public int SaleBillId { get; set; }
        public string BillNumber { get; set; }
        [Reactive] public int Quantity { get; set; }
        [Reactive] public decimal Price { get; set; }
        [Reactive] public decimal Amount { get; set; }
        public decimal TaxRate { get; set; }
        public int SanStockQty { get; set; }
        public int OrderStockQty { get; set; }
        public string ReserveBillNumber { get; set; }
        public int RemainderQty { get; set; }
        public decimal CostPrice { get; set; }
        public decimal CostAmount { get; set; }
        public decimal Profit { get; set; }
        public decimal CostProfitRate { get; set; }
        public DateTime CreatedOnUtc { get; set; }

        #region 商品信息

        public int? SmallUnitId { get; set; }
        public int? StrokeUnitId { get; set; }
        public int? BigUnitId { get; set; }
        #endregion

        [Reactive] public decimal Subtotal { get; set; }
        public bool IsGifts { get; set; } = false;
        public int? BigGiftQuantity { get; set; }
        public int? SmallGiftQuantity { get; set; }
        public bool IsManufactureDete { get; set; }
        public DateTime? ManufactureDete { get; set; }
        public IList<string> ProductTimes { get; set; } = new List<string>();

        #region 赠品信息

        public int? SaleProductTypeId { get; set; }
        public string SaleProductTypeName { get; set; }
        public int? GiveTypeId { get; set; }
        public int? CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int? CampaignBuyProductId { get; set; }
        public int? CampaignGiveProductId { get; set; }
        public string CampaignLinkNumber { get; set; }
        public int? CostContractId { get; set; }
        public int? CostContractItemId { get; set; }
        public int? CostContractMonth { get; set; }

        #endregion

        public decimal TaxPrice { get; set; }
        public decimal ContainTaxPrice { get; set; }
        public decimal TaxPriceAmount { get; set; }
    }


    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class SaleBillUpdateModel : BaseBalance
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
        [Reactive] public List<SaleItemModel> Items { get; set; } = new List<SaleItemModel>();
        [Reactive] public List<AccountMaping> Accounting { get; set; } = new List<AccountMaping>();

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

        /// <summary>
        ///留存照片
        /// </summary>
        public List<RetainPhoto> Photos { get; set; } = new List<RetainPhoto>();
    }


    /// <summary>
    /// 用于表示送货签收记录
    /// </summary>
    public class DeliverySignModel : EntityBase
    {
        /// <summary>
        /// 签收状态：0 待签收，1签收，2拒收
        /// </summary>
        public int SignStatus { get; set; }
        public int DispatchBillId { get; set; }

        public int BillTypeId { get; set; }
        public int BillId { get; set; }
        public string BillNumber { get; set; }
        public int ToBillId { get; set; }
        public string ToBillNumber { get; set; }
        public decimal SumAmount { get; set; }
        [Reactive] public int TerminalId { get; set; }
        public string TerminalName { get; set; }
        [Reactive] public int BusinessUserId { get; set; }
        public string BusinessUserName { get; set; }
        [Reactive] public int DeliveryUserId { get; set; }
        public string DeliveryUserName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int SignUserId { get; set; }
        public string SignUserName { get; set; }
        public DateTime? SignedDate { get; set; }

        public IList<RetainPhoto> RetainPhotos { get; set; } = new List<RetainPhoto>();


        public ExchangeBillModel ExchangeBill { get; set; }
        public SaleBillModel SaleBill { get; set; }
        public ReturnBillModel ReturnBill { get; set; }
        public CostExpenditureBillModel CostExpenditureBill { get; set; }


        public Double Distance { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 老板电话
        /// </summary>
        public string BossCall { get; set; }

        public bool IsLast { get; set; } = true;
    }



    public class DeliverySignGroup : List<DeliverySignModel>
    {
        public string TerminalName { get; set; }
        public string Address { get; set; }
        public string BossCall { get; set; }
        public Double Distance { get; set; }

        public DeliverySignGroup(string terminalName, string adress, string bossCall, double distance, List<DeliverySignModel> bills) : base(bills)
        {
            TerminalName = terminalName;
            Address = adress;
            BossCall = bossCall;
            Distance = distance;
        }
    }

    /// <summary>
    /// 用于表示送货签收
    /// </summary>
    public class DeliverySignUpdateModel : EntityBase
    {
        /// <summary>
        /// 留存凭证照片
        /// </summary>
        public List<RetainPhoto> RetainPhotos { get; set; } = new List<RetainPhoto>();

        /// <summary>
        /// 调拨单（销售订单、退货订单，换货单）适用
        /// </summary>
        public int DispatchBillId { get; set; }
        /// <summary>
        /// 调拨明细（销售订单、退货订单，换货单）适用
        /// </summary>
        public int DispatchItemId { get; set; }

        /// <summary>
        /// 终端纬度坐标
        /// </summary>
        public double? Latitude { get; set; } = 0;

        /// <summary>
        /// 终端经度坐标
        /// </summary>
        public double? Longitude { get; set; } = 0;

        /// <summary>
        /// 单据类型
        /// </summary>
        public int BillTypeId { get; set; }
        /// <summary>
        /// 单据ID
        /// </summary>
        public int BillId { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; }
    }


    /// <summary>
    /// 留存凭证照片
    /// </summary>
    public class RetainPhoto : EntityBase
    {
        public string DisplayPath { get; set; }

    }
}
