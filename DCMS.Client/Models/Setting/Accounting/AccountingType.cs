using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Reactive;


namespace Wesley.Client.Models.Settings
{

    /// <summary>
    /// 表示科目类别
    /// </summary>
    public class AccountingType : EntityBase
    {

        private IList<AccountingOption> _accountingOptions;

        /// <summary>
        /// 类别名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? DisplayOrder { get; set; }


        /// <summary>
        ///导航属性
        /// </summary>
        public IList<AccountingOption> AccountingOptions
        {
            get { return _accountingOptions ?? (_accountingOptions = new List<AccountingOption>()); }
            protected set { _accountingOptions = value; }
        }
    }





    /// <summary>
    /// 表示科目项目
    /// </summary>
    public class AccountingOption : EntityBase
    {
        public int AccountingTypeId { get; set; }
        public int? AccountCodeTypeId { get; set; }
        public int? ParentId { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? DisplayOrder { get; set; }
        public bool Enabled { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsLeaf { get; set; }
        public virtual AccountingType AccountingType { get; set; }
        public decimal Balance { get; set; }

    }


    /// <summary>
    /// 表示单据财务科目项目配置
    /// </summary>
    public class FinanceAccountingOptionSetting : EntityBase
    {
        /// <summary>
        /// 销售单(收款账户) 配置
        /// </summary>
        public IList<AccountingOption> SaleBillAccountingOptions { get; set; } = new List<AccountingOption>();

        /// <summary>
        /// 销售订单(收款账户) 配置
        /// </summary>
        public IList<AccountingOption> SaleReservationBillAccountingOptions { get; set; } = new List<AccountingOption>();

        /// <summary>
        /// 退货单(收款账户) 配置
        /// </summary>
        public IList<AccountingOption> ReturnBillAccountingOptions { get; set; } = new List<AccountingOption>();

        /// <summary>
        /// 退货订单(收款账户) 配置
        /// </summary>
        public IList<AccountingOption> ReturnReservationBillAccountingOptions { get; set; } = new List<AccountingOption>();

        /// <summary>
        /// 收款单(收款账户) 配置
        /// </summary>
        public IList<AccountingOption> ReceiptAccountingOptions { get; set; } = new List<AccountingOption>();

        /// <summary>
        /// 付款单(付款账户) 配置
        /// </summary>
        public IList<AccountingOption> PaymentAccountingOptions { get; set; } = new List<AccountingOption>();

        /// <summary>
        /// 预收款单(收款账户) 配置
        /// </summary>
        public IList<AccountingOption> AdvanceReceiptAccountingOptions { get; set; } = new List<AccountingOption>();

        /// <summary>
        /// 预付款单(付款账户) 配置
        /// </summary>
        public IList<AccountingOption> AdvancePaymentAccountingOptions { get; set; } = new List<AccountingOption>();

        /// <summary>
        /// 采购单(付款账户) 配置
        /// </summary>
        public IList<AccountingOption> PurchaseBillAccountingOptions { get; set; } = new List<AccountingOption>();

        /// <summary>
        /// 采购退货单(付款账户) 配置
        /// </summary>
        public IList<AccountingOption> PurchaseReturnBillAccountingOptions { get; set; } = new List<AccountingOption>();

        /// <summary>
        /// 费用支出（支出账户） 配置
        /// </summary>
        public IList<AccountingOption> CostExpenditureAccountingOptions { get; set; } = new List<AccountingOption>();

        /// <summary>
        /// 财务收入（收款账户）  配置
        /// </summary>
        public IList<AccountingOption> FinancialIncomeAccountingOptions { get; set; } = new List<AccountingOption>();

        /// <summary>
        /// 收款对账（会计科目）
        /// </summary>
        public IList<AccountingOption> FinanceReceiveAccountingOptions { get; set; } = new List<AccountingOption>();
    }



    public partial class AccountingOptionModel : EntityBase
    {
        public int AccountingTypeId { get; set; }
        public string AccountingTypeName { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public int Number { get; set; }
        public SelectList PartentAccounts { get; set; }
        public int AccountCodeTypeId { get; set; }
        public string AccountCodeTypeName { get; set; }
        public SelectList AccountCodeTypes { get; set; }
        public bool IsEndPoint { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int DisplayOrder { get; set; }
        public bool Enabled { get; set; }
        public bool IsDefault { get; set; }
        [Reactive] public bool Selected { get; set; }
        public bool IsLeaf { get; set; }
        public bool IsCustom { get; set; }
        public decimal Balance { get; set; }
        public ReactiveCommand<int, Unit> SelectedCommand { get; set; }
    }
}
