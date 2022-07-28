using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;

namespace Wesley.Client.Models.Finances
{
    /// <summary>
    /// 用于表示费用支出单据
    /// </summary>
    public class CostExpenditureBillModel : AbstractBill
    {
        public string BillBarCode { get; set; }
        [Reactive] public int EmployeeId { get; set; }
        [Reactive] public string EmployeeName { get; set; }
        public SelectList Employees { get; set; }

        public int CustomerId { get; set; }
        [Reactive] public string CustomerName { get; set; }
        public string CustomerCode { get; set; }

        [Reactive] public int AccountingOptionId { get; set; }
        [Reactive] public string AccountingOptionName { get; set; }
        public DateTime? PayDate { get; set; }
        [Reactive] public string AuditedUserName { get; set; }
        [Reactive] public string AuditedStatusName { get; set; }

        public bool? HandInStatus { get; set; }
        public DateTime? HandInDate { get; set; }

        [Reactive] public decimal TotalAmount { get; set; }
        [Reactive] public ObservableCollection<AccountMaping> CostExpenditureBillAccountings { get; set; } = new ObservableCollection<AccountMaping>();
        [Reactive] public string[] Accountings { get; set; } = new string[] { };
        [Reactive] public ObservableCollection<CostExpenditureItemModel> Items { get; set; } = new ObservableCollection<CostExpenditureItemModel>();
    }

    /// <summary>
    /// 用于表示费用支出单据项目
    /// </summary>
    public class CostExpenditureItemModel : EntityBase
    {
        [Reactive] public int CostExpenditureBillId { get; set; }
        [Reactive] public int AccountingOptionId { get; set; }
        [Reactive] public string AccountingOptionName { get; set; }
        [Reactive] public int CustomerId { get; set; }
        [Reactive] public string CustomerName { get; set; }
        [Reactive] public int CostContractId { get; set; }
        [Reactive] public string CostContractName { get; set; }
        [Reactive] public decimal? Amount { get; set; } = 0;
        [Reactive] public int Month { get; set; } = 0;
        [Reactive] public string Remark { get; set; }
        [Reactive] public DateTime CreatedOnUtc { get; set; }
        [Reactive] public bool ShowBalance { get; set; }
        [Reactive] public decimal? Balance { get; set; } = 0;
        public ReactiveCommand<int, Unit> SelectedCommand { get; set; }
    }



    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class CostExpenditureUpdateModel : Base
    {
        public int CustomerId { get; set; }
        public string BillNumber { get; set; }
        [Reactive] public int EmployeeId { get; set; }
        [Reactive] public decimal OweCash { get; set; }
        [Reactive] public string Remark { get; set; }
        public IList<CostExpenditureItemModel> Items { get; set; } = new ObservableCollection<CostExpenditureItemModel>();
        public IList<AccountMaping> Accounting { get; set; } = new ObservableCollection<AccountMaping>();
    }

}
