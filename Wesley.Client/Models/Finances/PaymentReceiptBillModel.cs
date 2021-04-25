using Wesley.Client.Enums;
using System;
using System.Collections.Generic;

namespace Wesley.Client.Models.Finances
{
    /// <summary>
    /// 用于表示付款单据
    /// </summary>
    public class PaymentReceiptBillModel : AbstractBill
    {

        public string BillBarCode { get; set; }

        public int Draweer { get; set; }
        public SelectList Draweers { get; set; }
        public string DraweerName { get; set; }
        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public string AuditedUserName { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? AmountOwedAfterReceipt { get; set; }
        public int? PrintNum { get; set; }
        public int CollectionAccount { get; set; }
        public decimal? CollectionAmount { get; set; }


        #region //以下用于隐藏域

        /// <summary>
        /// 单据总金额
        /// </summary>
        public decimal? TotalAmount { get; set; }


        /// <summary>
        /// 单据当前总优惠金额
        /// </summary>
        public decimal? TotalDiscountAmount { get; set; }


        /// <summary>
        /// 单据当前总已付金额
        /// </summary>
        public decimal? TotalPaymentedAmount { get; set; }


        /// <summary>
        /// 单据当前总欠款金额
        /// </summary>
        public decimal? TotalArrearsAmount { get; set; }


        /// <summary>
        /// 单据当前总本次优惠金额
        /// </summary>
        public decimal? TotalDiscountAmountOnce { get; set; }

        /// <summary>
        /// 单据当前总本次付款金额
        /// </summary>
        public decimal? TotalReceivableAmountOnce { get; set; }

        /// <summary>
        /// 单据当前总付款后尚欠金额
        /// </summary>
        public decimal? TotalAmountOwedAfterReceipt { get; set; }

        #endregion

        public decimal? BeforArrearsAmount { get; set; }
        public IList<AccountMaping> PaymentReceiptBillAccountings { get; set; } = new List<AccountMaping>();
    }


    /// <summary>
    /// 用于表示付款单据项目
    /// </summary>
    public class PaymentReceiptItemModel : EntityBase
    {

        //收款单", "收款单Id")]
        public int PaymentReceiptBillId { get; set; }


        //单据编号", "单据编号")]
        public string BillNumber { get; set; }


        //单据类型", "单据类型")]
        public int BillTypeId { get; set; }
        public string BillTypeName { get; set; }
        public BillTypeEnum BillTypeEnum
        {
            get { return (BillTypeEnum)BillTypeId; }
            set { BillTypeId = (int)value; }
        }
        public string BillLink { get; set; }



        //开单日期", "开单日期")]
        public DateTime MakeBillDate { get; set; }

        //单据金额", "单据金额")]
        public decimal? Amount { get; set; }

        //优惠金额", "优惠金额")]
        public decimal? DiscountAmount { get; set; }

        //已付金额", "已付金额")]
        public decimal? PaymentedAmount { get; set; }


        //尚欠金额", "尚欠金额")]
        public decimal? ArrearsAmount { get; set; }

        //本次优惠金额", "本次优惠金额")]
        public decimal? DiscountAmountOnce { get; set; }


        //本次付款金额", "本次付款金额")]
        public decimal? ReceivableAmountOnce { get; set; }


        //付款后尚欠金额", "付款后尚欠金额")]
        public decimal? AmountOwedAfterReceipt { get; set; }

        //备注", "备注")]
        public string Remark { get; set; }

        //创建日期", "创建日期")]
        public DateTime CreatedOnUtc { get; set; }


    }


    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class PaymentReceiptUpdateModel
    {
        public int Draweer { get; set; }
        public int ManufacturerId { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? AmountOwedAfterReceipt { get; set; }
        public string Remark { get; set; }
        public List<PaymentReceiptItemModel> Items { get; set; } = new List<PaymentReceiptItemModel>();
        public List<AccountMaping> Accounting { get; set; } = new List<AccountMaping>();

    }

}
