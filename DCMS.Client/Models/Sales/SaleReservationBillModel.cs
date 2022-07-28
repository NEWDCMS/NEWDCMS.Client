using Wesley.Client.Models.Terminals;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wesley.Client.Models.Sales
{
    /// <summary>
    /// 销售订单
    /// </summary>
    public class SaleReservationBillModel : AbstractBill, IBCollection<SaleReservationItemModel>
    {
        [Reactive] public int PayTypeId { get; set; }
        [Reactive] public string PayTypeName { get; set; } = "现结";
        public DateTime? TransactionDate { get; set; }
        public string DefaultAmountId { get; set; } = Wesley.Client.Settings.DefaultPricePlan;
        public List<SelectListItem> SaleReservationBillDefaultAmounts { get; set; }

        [Reactive] public decimal ReceivableAmount { get; set; }

        public int CollectionAccount { get; set; }
        public decimal? CollectionAmount { get; set; }

        [Reactive] public ObservableCollection<SaleReservationItemModel> Items { get; set; } = new ObservableCollection<SaleReservationItemModel>();
        [Reactive] public IList<AccountMaping> SaleReservationBillAccountings { get; set; } = new List<AccountMaping>();
        [Reactive] public string[] Accountings { get; set; } = new string[] { };


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
    public class SaleReservationItemModel : ProductBaseModel
    {
        public int SaleReservationBillId { get; set; }
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
        [Reactive] public bool IsGifts { get; set; } = false;
        public int? BigGiftQuantity { get; set; }
        public int? SmallGiftQuantity { get; set; }
        public bool IsManufactureDete { get; set; }
        public DateTime? ManufactureDete { get; set; }
        public string ManufactureDateStr { get; set; }
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


        public decimal TaxRate { get; set; }
        public decimal TaxPrice { get; set; }
        public decimal ContainTaxPrice { get; set; }
        public decimal TaxPriceAmount { get; set; }
    }

    /// <summary>
    /// 项目保存或者编辑
    /// </summary>
    public class SaleReservationBillUpdateModel : BaseBalance
    {
        public string BillNumber { get; set; }
        public int TerminalId { get; set; } = 0;
        public int BusinessUserId { get; set; } = 0;
        public int DeliveryUserId { get; set; } = 0;
        public int WareHouseId { get; set; } = 0;
        public int PayTypeId { get; set; } = 0;
        public DateTime TransactionDate { get; set; }
        public bool IsMinUnitSale { get; set; }
        public string Remark { get; set; }
        public string DefaultAmountId { get; set; }
        public decimal PreferentialAmount { get; set; } = 0;
        public decimal PreferentialEndAmount { get; set; } = 0;
        public decimal OweCash { get; set; } = 0;
        public int? Operation { get; set; }
        public IList<SaleReservationItemModel> Items { get; set; } = new ObservableCollection<SaleReservationItemModel>();
        public IList<AccountMaping> Accounting { get; set; } = new ObservableCollection<AccountMaping>();
        /// <summary>
        /// 配送时间
        /// </summary>
        public DateTime DeliverDate { get; set; }
        public string AMTimeRange { get; set; }
        public string PMTimeRange { get; set; }
    }
}
