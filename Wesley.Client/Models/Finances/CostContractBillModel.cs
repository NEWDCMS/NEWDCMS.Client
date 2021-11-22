using Wesley.Client.Models.Settings;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Wesley.Client.Models.Finances
{

    /// 用于表示费用合同单据
    /// </summary>
    public class CostContractBillModel : AbstractBill, IBCollection<CostContractItemModel>
    {
        public string BillBarCode { get; set; }
        [Reactive] public int CustomerId { get; set; }
        [Reactive] public string CustomerName { get; set; }
        public string CustomerPointCode { get; set; }
        public int EmployeeId { get; set; }
        [Reactive] public string EmployeeName { get; set; }
        public SelectList Employees { get; set; }
        public int LeaderId { get; set; }
        public string LeaderName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int ContractType { get; set; }
        [Reactive] public int AccountingOptionId { get; set; }
        [Reactive] public string AccountingOptionName { get; set; }
        public List<AccountingOption> AccountingOptionSelects { get; set; } = new List<AccountingOption>();
        [Reactive] public string SaleRemark { get; set; }
        [Reactive] public string AuditedUserName { get; set; }
        [Reactive] public string AuditedStatusName { get; set; }
        public int? RejectUserId { get; set; }
        public bool RejectedStatus { get; set; }
        public DateTime? RejectedDate { get; set; }
        public int? AbandonedUserId { get; set; }
        public bool AbandonedStatus { get; set; }
        public DateTime? AbandonedDate { get; set; }
        [Reactive] public ObservableCollection<CostContractItemModel> Items { get; set; } = new ObservableCollection<CostContractItemModel>();
        [Reactive] public decimal? TotalAmount { get; set; }
        [Reactive] public int AllNum { get; set; }
        [Reactive] public int BigNum { get; set; }
        [Reactive] public int SmallNum { get; set; }

    }


    /// <summary>
    /// 用于表示费用合同单据项目
    /// </summary>
    public class CostContractItemModel : EntityBase
    {
        public int CostContractBillId { get; set; }
        public int ProductId { get; set; }
        [Reactive] public string ProductName { get; set; }
        public string ContractTypeName { get; set; }
        public int ContractType { get; set; }
        public int CType { get; set; }
        public string Name { get; set; }
        public int? UnitId { get; set; }
        [Reactive] public string UnitName { get; set; }
        public int? BigUnitId { get; set; }
        public int? SmallUnitId { get; set; }
        public int? BigUnitQuantity { get; set; }
        public int? SmallUnitQuantity { get; set; }

        public decimal? Jan { get; set; }
        public decimal? Feb { get; set; }
        public decimal? Mar { get; set; }
        public decimal? Apr { get; set; }
        public decimal? May { get; set; }
        public decimal? Jun { get; set; }
        public decimal? Jul { get; set; }
        public decimal? Aug { get; set; }
        public decimal? Sep { get; set; }
        public decimal? Oct { get; set; }
        public decimal? Nov { get; set; }
        public decimal? Dec { get; set; }

        /// <summary>
        /// 按月兑付时记录各月余额
        /// </summary>
        public decimal? Jan_Balance { get; set; }
        public decimal? Feb_Balance { get; set; }
        public decimal? Mar_Balance { get; set; }
        public decimal? Apr_Balance { get; set; }
        public decimal? May_Balance { get; set; }
        public decimal? Jun_Balance { get; set; }
        public decimal? Jul_Balance { get; set; }
        public decimal? Aug_Balance { get; set; }
        public decimal? Sep_Balance { get; set; }
        public decimal? Oct_Balance { get; set; }
        public decimal? Nov_Balance { get; set; }
        public decimal? Dec_Balance { get; set; }


        public decimal? Total_Balance { get; set; }
        [Reactive] public decimal? Total { get; set; }
        [Reactive] public int TotalQuantity { get; set; }
        [Reactive] public string Remark { get; set; }
        [Reactive] public DateTime CreatedOnUtc { get; set; }


        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal AvailableQuantityOrAmount { get; set; }
        public int GiveQuotaId { get; set; }
        public int GiveQuotaOptionId { get; set; }
        public int SaleProductTypeId { get; set; }
        public string SaleProductTypeName { get; set; }
        public int GiveTypeId { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
    }


    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class CostContractUpdateModel : Base
    {
        public string BillNumber { get; set; }
        public int CustomerId { get; set; }
        public int LeaderId { get; set; }
        public int EmployeeId { get; set; }
        public int ContractType { get; set; }
        public int AccountingOptionId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        [Reactive] public string Remark { get; set; }
        [Reactive] public string SaleRemark { get; set; }
        [Reactive] public ObservableCollection<CostContractItemModel> Items { get; set; } = new ObservableCollection<CostContractItemModel>();
    }


    /// <summary>
    /// 合同选择绑定
    /// </summary>
    public class CostContractBindingModel : EntityBase
    {
        [Reactive] public string BillNumber { get; set; }
        [Reactive] public int CustomerId { get; set; }
        [Reactive] public string CustomerName { get; set; }
        [Reactive] public decimal? Amount { get; set; }
        [Reactive] public decimal? Balance { get; set; }
        [Reactive] public int Year { get; set; }
        [Reactive] public int Month { get; set; }
        [Reactive] public string MonthName { get; set; }
        [Reactive] public int AccountingOptionId { get; set; }
        [Reactive] public int AccountCodeTypeId { get; set; }
        [Reactive] public string AccountingOptionName { get; set; }

    }

}
