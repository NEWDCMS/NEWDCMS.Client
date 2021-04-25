using System.Collections.Generic;

namespace Wesley.Client.Models.Finances
{
    /// <summary>
    /// 用于表示预付款单据
    /// </summary>
    public class AdvancePaymentBillModel : AbstractBill
    {
        public string BillBarCode { get; set; }
        public int Draweer { get; set; }
        public string DraweerName { get; set; }
        public SelectList Draweers { get; set; }
        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public SelectList Manufacturers { get; set; }
        public int PaymentType { get; set; }
        public string AuditedUserName { get; set; }
        public int? PrintNum { get; set; }
        public int? AccountingOptionId { get; set; }
        public string AccountingOptionName { get; set; }
        public SelectList AccountingOptions { get; set; }
        public decimal? AdvanceAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public IList<AccountMaping> AdvancePaymentBillAccountings { get; set; } = new List<AccountMaping>();

    }


    /// <summary>
    /// 提交或者编辑
    /// </summary>
    public class AdvancePaymenUpdateModel
    {
        public int Draweer { get; set; }
        public int ManufacturerId { get; set; }
        public decimal? AdvanceAmount { get; set; }
        public string Remark { get; set; }
        public int AccountingOptionId { get; set; }
        public List<AccountMaping> Accounting { get; set; } = new List<AccountMaping>();

    }

}
