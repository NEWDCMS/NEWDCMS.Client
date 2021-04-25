using Wesley.Client.Enums;
using Wesley.Client.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;

namespace Wesley.Client.Models.Sales
{

    /// <summary>
    /// 用于收款对账
    /// </summary>
    public class FinanceReceiveAccountBillModel : IBillTemplate
    {
        [Reactive] public bool Selected { get; set; }
        public ReactiveCommand<FinanceReceiveAccountBillModel, Unit> SelectedCommand { get; set; }


        public int TerminalId { get; set; }
        public string TerminalName { get; set; }

        public string BillNumber { get; set; }
        public string BillLink { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        public int BillType { get; set; }
        public int BillTypeId { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public int UserId { get; set; }
        public string UserName { get; set; }

        /// <summary>
        /// 上交状态
        /// </summary>
        public int HandInStatus { get; set; }
        public FinanceReceiveAccountStatus FinanceReceiveAccountStatus
        {
            get { return (FinanceReceiveAccountStatus)HandInStatus; }
            set { HandInStatus = (int)value; }
        }

        /// <summary>
        /// 待交金额
        /// </summary>
        public decimal PaidAmount { get; set; }
        /// <summary>
        /// 电子支付金额
        /// </summary>
        public decimal EPaymentAmount { get; set; }

        #region 销售收款 = 销售金额-预收款-欠款

        public decimal SaleAmountSum { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal SaleAdvanceReceiptAmount { get; set; }
        public decimal SaleOweCashAmount { get; set; }

        #endregion

        #region 退货款 =退款金额-预收款-欠款

        public decimal ReturnAmountSum { get; set; }
        public decimal ReturnAmount { get; set; }
        public decimal ReturnAdvanceReceiptAmount { get; set; }
        public decimal ReturnOweCashAmount { get; set; }

        #endregion

        #region 收欠款 =应收金额-预收款

        public decimal ReceiptCashOweCashAmountSum { get; set; }
        public decimal ReceiptCashReceivableAmount { get; set; }
        public decimal ReceiptCashAdvanceReceiptAmount { get; set; }

        #endregion

        #region 收预收款 =预收金额-欠款

        public decimal AdvanceReceiptSum { get; set; }
        public decimal AdvanceReceiptAmount { get; set; }
        public decimal AdvanceReceiptOweCashAmount { get; set; }

        #endregion

        #region 费用支出 = 支出金额-欠款

        public decimal CostExpenditureSum { get; set; }
        public decimal CostExpenditureAmount { get; set; }
        public decimal CostExpenditureOweCashAmount { get; set; }

        #endregion

        /// <summary>
        /// 单据ID
        /// </summary>
        public int BillId { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public List<AccountMaping> Accounts { get; set; } = new List<AccountMaping>();

        /// <summary>
        /// 优惠金额
        /// </summary>
        public int PreferentialAmount { get; set; }

        /// <summary>
        /// 销售商品(含赠)
        /// </summary>
        public int SaleProductCount { get; set; }
        public List<AccountProductModel> SaleProducts { get; set; } = new List<AccountProductModel>();

        /// <summary>
        /// 赠送商品
        /// </summary>
        public int GiftProductCount { get; set; }
        public List<AccountProductModel> GiftProducts { get; set; } = new List<AccountProductModel>();
        /// <summary>
        /// 退货商品
        /// </summary>
        public int ReturnProductCount { get; set; }
        public List<AccountProductModel> ReturnProducts { get; set; } = new List<AccountProductModel>();

        /// <summary>
        /// 上交单据的交易日期
        /// </summary>
        public DateTime HandInTransactionDate { get; set; }


        /// <summary>
        /// 单据时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

    }


    public class AccountProductModel
    {
        public int ProductId { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
        public string UnitName { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }

    public class AccountProductGroup : List<AccountProductModel>
    {
        public string CategoryName { get; private set; }

        public AccountProductGroup(string categoryName, List<AccountProductModel> products) : base(products)
        {
            CategoryName = categoryName;
        }
    }

    /// <summary>
    /// 表示收款对账视图绑定模型
    /// </summary>
    public class FRABViewModel : IBillTemplate
    {
        public int BillType { get; set; }
        /// <summary>
        /// 单据类型
        /// </summary>
        public string BillTypeName { get; set; }

        #region 销售收款 = 销售金额-预收款-欠款

        [Reactive] public decimal TotalSaleAmountSum { get; set; }
        [Reactive] public decimal TotalSaleAmount { get; set; }
        [Reactive] public decimal TotalSaleAdvanceReceiptAmount { get; set; }
        [Reactive] public decimal TotalSaleOweCashAmount { get; set; }
        [Reactive] public ObservableCollection<FinanceReceiveAccountBillModel> SaleBills { get; set; } = new ObservableCollection<FinanceReceiveAccountBillModel>();
        [Reactive] public int SaleBillCount { get; set; }

        #endregion

        #region 退货款 =退款金额-预收款-欠款

        [Reactive] public decimal TotalReturnAmountSum { get; set; }
        [Reactive] public decimal TotalReturnAmount { get; set; }
        [Reactive] public decimal TotalReturnAdvanceReceiptAmount { get; set; }
        [Reactive] public decimal TotalReturnOweCashAmount { get; set; }
        [Reactive] public ObservableCollection<FinanceReceiveAccountBillModel> ReturnBills { get; set; } = new ObservableCollection<FinanceReceiveAccountBillModel>();
        [Reactive] public int ReturnBillCount { get; set; }
        #endregion

        #region 收欠款 =应收金额-预收款

        [Reactive] public decimal TotalReceiptCashOweCashAmountSum { get; set; }
        [Reactive] public decimal TotalReceiptCashReceivableAmount { get; set; }
        [Reactive] public decimal TotalReceiptCashAdvanceReceiptAmount { get; set; }
        [Reactive] public ObservableCollection<FinanceReceiveAccountBillModel> ReceiptCashOweCashBills { get; set; } = new ObservableCollection<FinanceReceiveAccountBillModel>();
        [Reactive] public int ReceiptCashOweCashBillCount { get; set; }
        #endregion

        #region 收预收款 =预收金额-欠款

        [Reactive] public decimal TotalAdvanceReceiptSum { get; set; }
        [Reactive] public decimal TotalAdvanceReceiptAmount { get; set; }
        [Reactive] public decimal TotalAdvanceReceiptOweCashAmount { get; set; }
        [Reactive] public ObservableCollection<FinanceReceiveAccountBillModel> AdvanceReceiptBills { get; set; } = new ObservableCollection<FinanceReceiveAccountBillModel>();
        [Reactive] public int AdvanceReceiptBillCount { get; set; }

        #endregion

        #region 费用支出 = 支出金额-欠款

        [Reactive] public decimal TotalCostExpenditureSum { get; set; }
        [Reactive] public decimal TotalCostExpenditureAmount { get; set; }
        [Reactive] public decimal TotalCostExpenditureOweCashAmount { get; set; }
        [Reactive] public ObservableCollection<FinanceReceiveAccountBillModel> CostExpenditureBills { get; set; } = new ObservableCollection<FinanceReceiveAccountBillModel>();
        [Reactive] public int CostExpenditureBillCount { get; set; }

        #endregion
    }

    /// <summary>
    /// 收款对账上交
    /// </summary>
    public class FinanceReceiveAccountBillSubmitModel : Base
    {
        public IList<FinanceReceiveAccountBillModel> Items { get; set; } = new List<FinanceReceiveAccountBillModel>();
    }
}
