using System;
using System.Collections.Generic;

namespace Wesley.Client.Models.Finances
{

    /// <summary>
    /// 用于表示财务收入单据(其它收入)
    /// </summary>
    public class FinancialIncomeBillModel : AbstractBill
    {
        public FinancialIncomeBillModel()
        {
            FinancialIncomeBillAccountings = new List<AccountMaping>();
            Items = new List<FinancialIncomeItemModel>();
        }

        public string BillBarCode { get; set; }
        public int SalesmanId { get; set; }
        public string SalesmanName { get; set; }
        public SelectList Salesmans { get; set; }
        public int CustomerOrManufacturerId { get; set; }
        public string CustomerOrManufacturerName { get; set; }
        public int AccountingOptionId { get; set; }
        public string AccountingOptionName { get; set; }
        public string AuditedUserName { get; set; }
        public int? PrintNum { get; set; }
        public IList<FinancialIncomeItemModel> Items { get; set; }
        public IList<AccountMaping> FinancialIncomeBillAccountings { get; set; }

    }


    /// <summary>
    /// 用于表示财务收入单据项目
    /// </summary>
    public class FinancialIncomeItemModel : EntityBase
    {
        public int FinancialIncomeBillId { get; set; }
        public int AccountingOptionId { get; set; }
        public string AccountingOptionName { get; set; }
        public int CustomerOrManufacturerId { get; set; }
        public string CustomerOrManufacturerName { get; set; }
        public decimal? Amount { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }


    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class FinancialIncomeUpdateModel
    {
        public int SalesmanId { get; set; }
        public string Remark { get; set; }
        public List<FinancialIncomeItemModel> Items { get; set; } = new List<FinancialIncomeItemModel>();
        public List<AccountMaping> Accounting { get; set; } = new List<AccountMaping>();

    }

}
