
using Wesley.Client.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;

namespace Wesley.Client.Models.Finances
{
    /// <summary>
    /// 用于表示收款单据
    /// </summary>
    public class CashReceiptBillModel : AbstractBill, IBCollection<CashReceiptItemModel>
    {

        public int? Payeer { get; set; } = 0;
        public string PayeerName { get; set; }
        public string CustomerName { get; set; }
        public int CollectionAccount { get; set; } = 0;
        public decimal? CollectionAmount { get; set; } = 0;

        [Reactive] public decimal ReceivableAmount { get; set; }

        [Reactive] public ObservableCollection<CashReceiptItemModel> Items { get; set; } = new ObservableCollection<CashReceiptItemModel>();

        #region //以下用于隐藏域

        /// <summary>
        /// 单据总金额
        /// </summary>
        [Reactive] public decimal? TotalArrearsAmountOnce { get; set; } = 0;

        /// <summary>
        /// 单据当前总本次优惠金额
        /// </summary>
        [Reactive] public decimal? TotalDiscountAmountOnce { get; set; } = 0;

        /// <summary>
        /// 单据当前总本次收款金额
        /// </summary>
        [Reactive] public decimal? TotalReceivableAmountOnce { get; set; } = 0;

        /// <summary>
        /// 单据当前总收款后尚欠金额
        /// </summary>
        [Reactive] public decimal? TotalAmountOwedAfterReceipt { get; set; } = 0;


        #endregion

        [Reactive] public decimal? BeforArrearsAmount { get; set; } = 0;
        [Reactive] public ObservableCollection<AccountMaping> CashReceiptBillAccountings { get; set; } = new ObservableCollection<AccountMaping>();
        [Reactive] public string[] Accountings { get; set; } = new string[] { };
        public bool AllowAdvancePaymentsNegative { get; set; }
    }


    /// <summary>
    /// 用于表示收款单据项目
    /// </summary>
    public class CashReceiptItemModel : EntityBase
    {
        public int CashReceiptBillId { get; set; }
        public int BillId { get; set; }
        [Reactive] public string BillNumber { get; set; }
        public int BillTypeId { get; set; }
        [Reactive] public string BillTypeName { get; set; }
        public BillTypeEnum BillTypeEnum
        {
            get { return (BillTypeEnum)BillTypeId; }
            set { BillTypeId = (int)value; }
        }
        public string BillLink { get; set; }
        [Reactive] public DateTime MakeBillDate { get; set; }
        [Reactive] public decimal? Amount { get; set; }
        [Reactive] public decimal? DiscountAmount { get; set; }
        [Reactive] public decimal? PaymentedAmount { get; set; }
        [Reactive] public decimal? ArrearsAmount { get; set; }
        [Reactive] public decimal? DiscountAmountOnce { get; set; }
        [Reactive] public decimal? ReceivableAmountOnce { get; set; }
        [Reactive] public decimal? AmountOwedAfterReceipt { get; set; }
        [Reactive] public decimal? AmountReceivable { get; set; }
        [Reactive] public string Remark { get; set; }
        [Reactive] public DateTime CreatedOnUtc { get; set; }
        public IReactiveCommand RedirectCommand { get; set; }
    }

    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class CashReceiptUpdateModel : BaseBalance
    {
        public int CustomerId { get; set; }
        public int? Payeer { get; set; }
        public string Remark { get; set; }
        public int? Operation { get; set; }
        public decimal OweCash { get; set; }
        public decimal ReceivableAmount { get; set; }
        public decimal PreferentialAmount { get; set; }
        [Reactive] public ObservableCollection<CashReceiptItemModel> Items { get; set; } = new ObservableCollection<CashReceiptItemModel>();
        [Reactive] public ObservableCollection<AccountMaping> Accounting { get; set; } = new ObservableCollection<AccountMaping>();

    }

    /// <summary>
    /// 用于单据汇总（用于单据的收付款汇总）
    /// </summary>
    public class BillSummaryModel : EntityBase
    {

        public int BillId { get; set; } = 0;
        public string BillNumber { get; set; }
        public string BillTypeName { get; set; }
        public int BillTypeId { get; set; } = 0;
        public string BillLink { get; set; }
        public int CustomerId { get; set; } = 0;
        public string CustomerName { get; set; }
        public string CustomerPointCode { get; set; }
        public int DistrictId { get; set; } = 0;
        public DateTime MakeBillDate { get; set; }
        public decimal? Amount { get; set; } = 0;
        public decimal? DiscountAmount { get; set; } = 0;
        public decimal? PaymentedAmount { get; set; } = 0;
        public decimal? ArrearsAmount { get; set; } = 0;
        public string Remark { get; set; }
    }

    /// <summary>
    /// 表示终端应收款分组合计
    /// </summary>
    public class AmountReceivableGroupModel
    {
        public int CustomerId { get; set; } = 0;
        public string CustomerName { get; set; }
        public string CustomerPointCode { get; set; }
        public decimal Amount { get; set; }
    }
}
