using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Wesley.Client.Models.Finances
{

    /// <summary>
    /// 用于表示预收款单据
    /// </summary>
    public class AdvanceReceiptBillModel : AbstractBill
    {
        public string BillBarCode { get; set; }
        public int? Payeer { get; set; }
        [Reactive] public string PayeerName { get; set; }
        public SelectList Payeers { get; set; }
        [Reactive] public string AuditedUserName { get; set; }
        public int? AccountingOptionId { get; set; }
        [Reactive] public string AccountingOptionName { get; set; }
        public SelectList AccountingOptions { get; set; }
        [Reactive] public decimal? AdvanceAmount { get; set; }
        [Reactive] public decimal? DiscountAmount { get; set; }

        [Reactive] public int CustomerId { get; set; }
        [Reactive] public string CustomerName { get; set; }

        public int? PrintNum { get; set; }
        public bool? HandInStatus { get; set; }
        public DateTime? HandInDate { get; set; }
        [Reactive] public ObservableCollection<AccountMaping> Items { get; set; } = new ObservableCollection<AccountMaping>();
        [Reactive] public string[] Accountings { get; set; } = new string[] { };
    }


    public class AdvanceReceiptUpdateModel : Base
    {
        public int CustomerId { get; set; } = 0;
        public int? Payeer { get; set; } = 0;
        public decimal? AdvanceAmount { get; set; } = 0;
        public decimal? DiscountAmount { get; set; } = 0;
        public decimal OweCash { get; set; } = 0;
        public string Remark { get; set; }
        public int AccountingOptionId { get; set; } = 0;
        public int? Operation { get; set; }
        public List<AccountMaping> Accounting { get; set; }

    }
}
