using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;

namespace Wesley.Client.Models.Report
{

    //资金报表

    #region 客户往来账

    /// <summary>
    /// 客户往来账
    /// </summary>
    public class FundReportCustomerAccount
    {
        /// <summary>
        /// 单据Id
        /// </summary>
        public int? BillId { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        public string BillNumber { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public int? TerminalId { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string TerminalName { get; set; }

        /// <summary>
        /// 客户编码
        /// </summary>
        public string TerminalCode { get; set; }

        /// <summary>
        /// 单据类型Id
        /// </summary>
        public int? BillTypeId { get; set; }

        /// <summary>
        /// 单价类型名称
        /// </summary>
        public string BillTypeName { get; set; }

        /// <summary>
        /// 发生日期
        /// </summary>
        public DateTime? TransactionDate { get; set; }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? BillAmount { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal? PreferentialAmount { get; set; }

        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal? CashReceiptAmount { get; set; }

        /// <summary>
        /// 应收款减（（（单据金额-优惠金额）-收款金额）<0）
        /// </summary>
        public decimal? ReceivableAmountSubtract { get; set; }

        /// <summary>
        /// 应收款加（（（单据金额-优惠金额）-收款金额）>0）
        /// </summary>
        public decimal? ReceivableAmountAdd { get; set; }

        /// <summary>
        /// 应收款余额
        /// </summary>
        public decimal? ReceivableAmountOverage { get; set; }

        /// <summary>
        /// 预收款减
        /// </summary>
        public decimal? AdvancePaymentAmountSubtract { get; set; }

        /// <summary>
        /// 预收款加
        /// </summary>
        public decimal? AdvancePaymentAmountAdd { get; set; }

        /// <summary>
        /// 预收款余额
        /// </summary>
        public decimal? AdvancePaymentAmountOverage { get; set; }

        /// <summary>
        /// 订货款余额
        /// </summary>
        public decimal? SubscribeCashAmountOverage { get; set; }

        /// <summary>
        /// 往来账余额=预收款余额-应收款余额
        /// </summary>
        public decimal? AccountAmountOverage { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


    }

    #endregion

    #region 客户应收款

    /// <summary>
    /// 客户应收款
    /// </summary>
    public class FundReportCustomerReceiptCash
    {

        /// <summary>
        /// 客户Id
        /// </summary>
        public int? TerminalId { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string TerminalName { get; set; }

        /// <summary>
        /// 客户编码
        /// </summary>
        public string TerminalCode { get; set; }


        /// <summary>
        /// 累计欠款
        /// </summary>
        public decimal? OweCase { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal? SaleAmount { get; set; }

        /// <summary>
        /// 退货金额
        /// </summary>
        public decimal? ReturnAmount { get; set; }

        /// <summary>
        /// 净销售额=(销售金额-退货金额)
        /// </summary>
        public decimal? NetAmount { get; set; }

        /// <summary>
        /// 首次欠款时间
        /// </summary>
        public DateTime? FirstOweCaseDate { get; set; }

        /// <summary>
        /// 末次欠款时间
        /// </summary>
        public DateTime? LastOweCaseDate { get; set; }

    }

    #endregion

    #region 供应商往来账

    /// <summary>
    /// 供应商来账
    /// </summary>
    public class FundReportManufacturerAccount
    {
        /// <summary>
        /// 单据Id
        /// </summary>
        public int? BillId { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        public string BillNumber { get; set; }

        /// <summary>
        /// 供应商Id
        /// </summary>
        public int ManufacturerId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// 单据类型Id
        /// </summary>
        public int? BillTypeId { get; set; }

        /// <summary>
        /// 单价类型名称
        /// </summary>
        public string BillTypeName { get; set; }

        /// <summary>
        /// 发生日期
        /// </summary>
        public DateTime? TransactionDate { get; set; }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? BillAmount { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal? PreferentialAmount { get; set; }

        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal? PayCashAmount { get; set; }

        /// <summary>
        /// 应付款减（（（单据金额-优惠金额）-收款金额）<0）
        /// </summary>
        public decimal? PayAmountSubtract { get; set; }

        /// <summary>
        /// 应付款加（（（单据金额-优惠金额）-收款金额）>0）
        /// </summary>
        public decimal? PayAmountAdd { get; set; }

        /// <summary>
        /// 应付款余额
        /// </summary>
        public decimal? PayAmountOverage { get; set; }

        /// <summary>
        /// 预付款减
        /// </summary>
        public decimal? AdvancePayAmountSubtract { get; set; }

        /// <summary>
        /// 预付款加
        /// </summary>
        public decimal? AdvancePayAmountAdd { get; set; }

        /// <summary>
        /// 预付款余额
        /// </summary>
        public decimal? AdvancePayAmountOverage { get; set; }

        /// <summary>
        /// 往来账余额=预付款余额-应付款余额
        /// </summary>
        public decimal? AccountAmountOverage { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


    }

    #endregion

    #region 供应商应付款

    /// <summary>
    /// 供应商应付款
    /// </summary>
    public class FundReportManufacturerPayCash
    {

        /// <summary>
        /// 供应商Id
        /// </summary>
        public int ManufacturerId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// 累计欠款
        /// </summary>
        public decimal? OweCase { get; set; }

        /// <summary>
        /// 首次欠款时间
        /// </summary>
        public DateTime? FirstOweCaseDate { get; set; }

        /// <summary>
        /// 末次欠款时间
        /// </summary>
        public DateTime? LastOweCaseDate { get; set; }

    }

    #endregion

    #region 预收款余额

    /// <summary>
    /// 预收款余额
    /// </summary>
    public class FundReportAdvanceReceiptOverage
    {

        /// <summary>
        /// 客户Id
        /// </summary>
        public int? TerminalId { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string TerminalName { get; set; }

        /// <summary>
        /// 客户编码
        /// </summary>
        public string TerminalCode { get; set; }

        /// <summary>
        /// 预收款
        /// </summary>
        public decimal? AdvanceReceiptAmount { get; set; }

        /// <summary>
        /// 预收款余额
        /// </summary>
        public decimal? AdvanceReceiptOverageAmount { get; set; }

        /// <summary>
        /// 应收款余额
        /// </summary>
        public decimal? ReceivableOverageAmount { get; set; }

        /// <summary>
        /// 余额=预收款金额-应收款余额
        /// </summary>
        public decimal? OverageAmount { get; set; }

    }

    #endregion

    #region 预付款余额

    /// <summary>
    /// 预付款余额
    /// </summary>
    public class FundReportAdvancePaymentOverage
    {

        /// <summary>
        /// 供应商Id
        /// </summary>
        public int ManufacturerId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// 预付款
        /// </summary>
        public decimal? AdvancePaymentAmount { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal? OverageAmount { get; set; }

    }

    #endregion


    /// <summary>
    /// 客户排行榜
    /// </summary>
    public class CustomerRanking : EntityBase
    {
        /// <summary>
        /// 客户Id
        /// </summary>
        public int? TerminalId { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string TerminalName { get; set; }

        /// <summary>
        /// 拜访数
        /// </summary>
        public int? VisitSum { get; set; }

        /// <summary>
        /// 销售
        /// </summary>
        public decimal? SaleAmount { get; set; }

        /// <summary>
        /// 销退
        /// </summary>
        public decimal? SaleReturnAmount { get; set; }

        /// <summary>
        /// 净额
        /// </summary>
        public decimal? NetAmount { get; set; }
    }


    /// <summary>
    /// 客户拜访活跃度排行榜
    /// </summary>
    public class CustomerActivityRanking : Base
    {
        /// <summary>
        /// 客户Id
        /// </summary>
        public int? TerminalId { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string TerminalName { get; set; }

        /// <summary>
        /// 拜访天数数
        /// </summary>
        public int? VisitDaySum { get; set; }

    }

    /// <summary>
    /// 业务员排行榜
    /// </summary>
    public class BusinessRanking : Base
    {

        /// <summary>
        /// 业务员
        /// </summary>
        public int BusinessUserId { get; set; }
        [Reactive]
        public string BusinessUserName { get; set; }

        /// <summary>
        /// 利润
        /// </summary>
        public decimal? Profit { get; set; }

        /// <summary>
        /// 销售
        /// </summary>
        public decimal? SaleAmount { get; set; }

        /// <summary>
        /// 销退
        /// </summary>
        public decimal? SaleReturnAmount { get; set; }

        /// <summary>
        /// 净额
        /// </summary>
        public decimal? NetAmount { get; set; }
    }


    /// <summary>
    /// 品牌销量汇总
    /// </summary>
    public class BrandRanking : Base
    {


        /// <summary>
        /// 品牌
        /// </summary>
        public int BrandId { get; set; }
        public string BrandName { get; set; }


        /// <summary>
        /// 利润
        /// </summary>
        public decimal? Profit { get; set; }

        /// <summary>
        /// 销售
        /// </summary>
        public decimal? SaleAmount { get; set; }

        /// <summary>
        /// 销退
        /// </summary>
        public decimal? SaleReturnAmount { get; set; }

        /// <summary>
        /// 净额
        /// </summary>
        public decimal? NetAmount { get; set; }

        /// <summary>
        /// 比例
        /// </summary>
        public double? Percentage { get; set; }
    }


    /// <summary>
    /// 销量走势图
    /// </summary>
    public class SaleTrending : Base
    {

        /// <summary>
        /// 日期类型
        /// </summary>
        public string DateType { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime SaleDate { get; set; }
        public string SaleDateName { get; set; } = "";

        /// <summary>
        /// 销售
        /// </summary>
        public decimal? SaleAmount { get; set; }

        /// <summary>
        /// 销退
        /// </summary>
        public decimal? SaleReturnAmount { get; set; }

        /// <summary>
        /// 净额
        /// </summary>
        public decimal? NetAmount { get; set; }

    }


    /// <summary>
    /// 热销排行榜
    /// </summary>
    public class HotSaleRanking : Base
    {
        /// <summary>
        /// 商品
        /// </summary>
        public int? ProductId { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public int? TerminalId { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public int? BusinessUserId { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public int? BrandId { get; set; }

        /// <summary>
        /// 商品类别
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>
        public int TotalSumSaleQuantity { get; set; }

        public string TotalSumSaleQuantityConversion { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal? TotalSumSaleAmount { get; set; }

        /// <summary>
        /// 退货数量
        /// </summary>
        public int TotalSumReturnQuantity { get; set; }

        public string TotalSumReturnQuantityConversion { get; set; }

        /// <summary>
        /// 退货金额
        /// </summary>
        public decimal? TotalSumReturnAmount { get; set; }

        /// <summary>
        /// 净销售量 = 销售数量 - 退货数量
        /// </summary>
        public decimal? TotalSumNetQuantity { get; set; }

        public string TotalSumNetQuantityConversion { get; set; }

        /// <summary>
        /// 销售净额 = 销售金额 - 退货金额
        /// </summary>
        public decimal? TotalSumNetAmount { get; set; }


        /// <summary>
        /// 退货率 = 退货数量  /  销售数量
        /// </summary>
        public double? ReturnRate { get; set; }

    }

    /// <summary>
    /// 滞销排行榜
    /// </summary>
    public class UnSaleRanking : Base
    {
        /// <summary>
        /// 商品
        /// </summary>
        public int? ProductId { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public int? TerminalId { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public int? BusinessUserId { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public int? BrandId { get; set; }

        /// <summary>
        /// 商品类别
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>
        public int TotalSumSaleQuantity { get; set; }

        public string TotalSumSaleQuantityConversion { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal? TotalSumSaleAmount { get; set; }

        /// <summary>
        /// 退货数量
        /// </summary>
        public int TotalSumReturnQuantity { get; set; }

        public string TotalSumReturnQuantityConversion { get; set; }

        /// <summary>
        /// 退货金额
        /// </summary>
        public decimal? TotalSumReturnAmount { get; set; }

        /// <summary>
        /// 净销售量 = 销售数量 - 退货数量
        /// </summary>
        public decimal? TotalSumNetQuantity { get; set; }

        public string TotalSumNetQuantityConversion { get; set; }

        /// <summary>
        /// 销售净额 = 销售金额 - 退货金额
        /// </summary>
        public decimal? TotalSumNetAmount { get; set; }

    }


    /// <summary>
    /// 销售商品成本利润排行榜
    /// </summary>
    public class CostProfitRanking : Base
    {

        /// <summary>
        /// 商品
        /// </summary>
        public int? ProductId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品类别
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public int? BrandId { get; set; }


        /// <summary>
        /// 客户Id
        /// </summary>
        public int? TerminalId { get; set; }


        /// <summary>
        /// 业务员
        /// </summary>
        public int? BusinessUserId { get; set; }


        /// <summary>
        /// 净销售量
        /// </summary>
        public int? TotalSumNetQuantity { get; set; }

        /// <summary>
        /// 净销售量单位转化 如：xx 箱 xx 瓶
        /// </summary>
        public string TotalSumNetQuantityConversion { get; set; }

        /// <summary>
        /// 销售净额
        /// </summary>
        public decimal? TotalSumNetAmount { get; set; }

        /// <summary>
        /// 利润
        /// </summary>
        public decimal? TotalSumProfit { get; set; }


    }


    /// <summary>
    /// 销售额分析
    /// </summary>
    public class SaleAnalysis : EntityBase
    {
        /// <summary>
        /// 业务员
        /// </summary>
        public int BusinessUserId { get; set; }
        public string BusinessUserName { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public int BrandId { get; set; }
        public string BrandName { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        /// <summary>
        /// 商品类别
        /// </summary>
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }


        /// <summary>
        /// 今日
        /// </summary>
        [Reactive] public Sale Today { get; set; } = new Sale();

        /// <summary>
        /// 今日上周同期
        /// </summary>
        [Reactive] public Sale LastWeekSame { get; set; } = new Sale();

        /// <summary>
        /// 昨天
        /// </summary>
        [Reactive] public Sale Yesterday { get; set; } = new Sale();
        /// <summary>
        /// 前天
        /// </summary>
        [Reactive] public Sale BeforeYesterday { get; set; } = new Sale();
        /// <summary>
        /// 上周
        /// </summary>
        [Reactive] public Sale LastWeek { get; set; } = new Sale();
        /// <summary>
        /// 本周
        /// </summary>
        [Reactive] public Sale ThisWeek { get; set; } = new Sale();
        /// <summary>
        /// 上月
        /// </summary>
        [Reactive] public Sale LastMonth { get; set; } = new Sale();
        /// <summary>
        /// 本月
        /// </summary>
        [Reactive] public Sale ThisMonth { get; set; } = new Sale();

        /// <summary>
        /// 本季
        /// </summary>
        [Reactive] public Sale ThisQuarter { get; set; } = new Sale();

        /// <summary>
        /// 本年
        /// </summary>
        [Reactive] public Sale ThisYear { get; set; } = new Sale();

    }


    /// <summary>
    /// 客户拜访排行分析
    /// </summary>
    public class CustomerVistAnalysis : EntityBase
    {
        /// <summary>
        /// 业务员
        /// </summary>
        public int BusinessUserId { get; set; }
        public string BusinessUserName { get; set; }


        /// <summary>
        /// 总客户数
        /// </summary>
        public int TotalCustomer { get; set; }

        public int TotalVist { get; set; }


        /// <summary>
        /// 今日
        /// </summary>
        public Vist Today { get; set; } = new Vist();

        /// <summary>
        /// 昨天
        /// </summary>
        public Vist Yesterday { get; set; } = new Vist();

        public Vist LastWeekSame { get; set; } = new Vist();


        /// <summary>
        /// 前天
        /// </summary>
        public Vist BeforeYesterday { get; set; } = new Vist();
        /// <summary>
        /// 上周
        /// </summary>
        public Vist LastWeek { get; set; } = new Vist();
        /// <summary>
        /// 本周
        /// </summary>
        public Vist ThisWeek { get; set; } = new Vist();
        /// <summary>
        /// 上月
        /// </summary>
        public Vist LastMonth { get; set; } = new Vist();
        /// <summary>
        /// 本月
        /// </summary>
        public Vist ThisMonth { get; set; } = new Vist();

        public Vist ThisYear { get; set; } = new Vist();


        /// <summary>
        /// 嵌套
        /// </summary>
        public class Vist
        {
            /// <summary>
            /// 拜访数
            /// </summary>
            public int? VistCount { get; set; }

            /// <summary>
            /// 拜访比例 = 拜访数/总客户数
            /// </summary>
            public double? Percentage { get; set; }

        }
    }


    /// <summary>
    /// 新增加客户分析
    /// </summary>
    public class NewCustomerAnalysis : EntityBase
    {
        /// <summary>
        /// 业务员
        /// </summary>
        public int BusinessUserId { get; set; }
        public string BusinessUserName { get; set; }


        /// <summary>
        /// 总客户数
        /// </summary>
        public int TotalCustomer { get; set; }


        /// <summary>
        /// 今日
        /// </summary>
        public Signin Today { get; set; } = new Signin();
        /// <summary>
        /// 今日上周同期签约客户
        /// </summary>
        public Signin LastWeekSame { get; set; } = new Signin();

        /// <summary>
        /// 昨天
        /// </summary>
        public Signin Yesterday { get; set; } = new Signin();
        /// <summary>
        /// 前天
        /// </summary>
        public Signin BeforeYesterday { get; set; } = new Signin();
        /// <summary>
        /// 上周
        /// </summary>
        public Signin LastWeek { get; set; } = new Signin();
        /// <summary>
        /// 本周
        /// </summary>
        public Signin ThisWeek { get; set; } = new Signin();
        /// <summary>
        /// 上月
        /// </summary>
        public Signin LastMonth { get; set; } = new Signin();
        /// <summary>
        /// 本月
        /// </summary>
        public Signin ThisMonth { get; set; } = new Signin();

        /// <summary>
        /// 本年
        /// </summary>
        public Signin ThisYear { get; set; } = new Signin();

        /// <summary>
        /// 嵌套
        /// </summary>
        public class Signin
        {
            /// <summary>
            /// 签约客户数
            /// </summary>
            public int? Count { get; set; }

            /// <summary>
            /// 签约客户的Id 列表
            /// </summary>
            public List<int> TerminalIds { get; set; } = new List<int>();
        }

        /// <summary>
        /// 月统计
        /// </summary>
        public Dictionary<string, double> ChartDatas { get; set; } = new Dictionary<string, double>();
    }


    /// <summary>
    /// 订单额分析
    /// </summary>
    public class OrderQuantityAnalysis : EntityBase
    {
        /// <summary>
        /// 业务员
        /// </summary>
        public int BusinessUserId { get; set; }
        public string BusinessUserName { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public int BrandId { get; set; }
        public string BrandName { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        /// <summary>
        /// 商品类别
        /// </summary>
        public int CatagoryId { get; set; }
        public string CatagoryName { get; set; }



        /// <summary>
        /// 今日
        /// </summary>
        public Sale Today { get; set; } = new Sale();

        /// <summary>
        /// 今日上周同期净额
        /// </summary>
        public Sale LastWeekSame { get; set; } = new Sale();

        /// <summary>
        /// 今日和上周同期净额差= SamePeriodLastWeek - Today
        /// </summary>
        public decimal? NetAmountBalance { get; set; } = 0;



        /// <summary>
        /// 昨天
        /// </summary>
        public Sale Yesterday { get; set; } = new Sale();
        /// <summary>
        /// 前天
        /// </summary>
        public Sale BeforeYesterday { get; set; } = new Sale();
        /// <summary>
        /// 上周
        /// </summary>
        public Sale LastWeek { get; set; } = new Sale();
        /// <summary>
        /// 本周
        /// </summary>
        public Sale ThisWeek { get; set; } = new Sale();
        /// <summary>
        /// 上月
        /// </summary>
        public Sale LastMonth { get; set; } = new Sale();
        /// <summary>
        /// 本月
        /// </summary>
        public Sale ThisMonth { get; set; } = new Sale();

        /// <summary>
        /// 本年
        /// </summary>
        public Sale ThisYear { get; set; } = new Sale();

        /// <summary>
        /// 本季
        /// </summary>
        public Sale ThisQuarter { get; set; } = new Sale();
    }

    /// <summary>
    /// 新增订单分析
    /// </summary>
    public class NewOrderAnalysis : EntityBase
    {
        /// <summary>
        /// 业务员
        /// </summary>
        public int BusinessUserId { get; set; }
        public string BusinessUserName { get; set; }


        /// <summary>
        /// 总客户数
        /// </summary>
        public int TotalOrders { get; set; }


        /// <summary>
        /// 今日
        /// </summary>
        public Order Today { get; set; } = new Order();
        /// <summary>
        /// 今日上周同期签约客户
        /// </summary>
        public Order LastWeekSame { get; set; } = new Order();

        /// <summary>
        /// 昨天
        /// </summary>
        public Order Yesterday { get; set; } = new Order();
        /// <summary>
        /// 前天
        /// </summary>
        public Order BeforeYesterday { get; set; } = new Order();
        /// <summary>
        /// 上周
        /// </summary>
        public Order LastWeek { get; set; } = new Order();
        /// <summary>
        /// 本周
        /// </summary>
        public Order ThisWeek { get; set; } = new Order();
        /// <summary>
        /// 上月
        /// </summary>
        public Order LastMonth { get; set; } = new Order();
        /// <summary>
        /// 本月
        /// </summary>
        public Order ThisMonth { get; set; } = new Order();

        /// <summary>
        /// 本年
        /// </summary>
        public Order ThisYear { get; set; } = new Order();

        /// <summary>
        /// 嵌套
        /// </summary>
        public class Order
        {
            /// <summary>
            /// 订单数
            /// </summary>
            public int? Count { get; set; }

            /// <summary>
            /// 单据的Id 列表
            /// </summary>
            public List<int> BillIds { get; set; } = new List<int>();
        }

        /// <summary>
        /// 月统计
        /// </summary>
        public Dictionary<string, double> ChartDatas { get; set; } = new Dictionary<string, double>();
    }


    /// <summary>
    /// 嵌套
    /// </summary>
    public class Sale : Base
    {

        /// <summary>
        /// 销售
        /// </summary>
        [Reactive] public decimal SaleAmount { get; set; }

        /// <summary>
        /// 销退
        /// </summary>
        [Reactive] public decimal SaleReturnAmount { get; set; }

        /// <summary>
        /// 净额
        /// </summary>
        [Reactive] public decimal NetAmount { get; set; }

    }


    public class DashboardReport : EntityBase
    {
        /// <summary>
        /// 今日销售额
        /// </summary>
        [Reactive] public decimal TodaySaleAmount { get; set; }

        /// <summary>
        /// 昨日销售额
        /// </summary>
        [Reactive] public decimal YesterdaySaleAmount { get; set; }

        /// <summary>
        /// 今日订单数
        /// </summary>
        [Reactive] public int TodayOrderQuantity { get; set; }

        /// <summary>
        /// 昨日订单数
        /// </summary>
        [Reactive] public int YesterdayOrderQuantity { get; set; }

        /// <summary>
        /// 今日新增客户
        /// </summary>
        [Reactive] public int TodayAddTerminalQuantity { get; set; }

        /// <summary>
        /// 昨日新增客户
        /// </summary>
        [Reactive] public int YesterdayAddTerminalQuantity { get; set; }

        /// <summary>
        /// 今日拜访客户
        /// </summary>
        [Reactive] public int TodayVisitQuantity { get; set; }

        /// <summary>
        /// 昨日拜访客户
        /// </summary>
        [Reactive] public int YesterdayVisitQuantity { get; set; }
    }
}
