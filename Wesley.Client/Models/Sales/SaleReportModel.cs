using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
namespace Wesley.Client.Models.Sales
{

    /// <summary>
    /// 销售明细表
    /// </summary>
    public class SaleReportItemListModel : Base
    {

        public IList<SaleReportItem> Items { get; set; } = new List<SaleReportItem>();

        #region  用于条件检索

        //商品", "商品
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        //商品类别", "商品类别
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public SelectList Categories { get; set; }

        //品牌", "品牌
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public SelectList Brands { get; set; }

        //客户Id", "客户Id
        public int? TerminalId { get; set; }
        //客户", "客户
        public string TerminalName { get; set; }

        //单据编号", "单据编号
        public string BillNumber { get; set; }

        //销售类型", "销售类型
        public int? SaleTypeId { get; set; }
        public string SaleTypeName { get; set; }
        public SelectList SaleTypes { get; set; }

        //业务员", "业务员
        public int? BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }

        //仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public SelectList WareHouses { get; set; }

        //支付方式", "支付方式
        public int? PayTypeId { get; set; }
        public SelectList PayTypes { get; set; }

        //送货员", "送货员
        public int? DeliveryUserId { get; set; }
        public SelectList DeliveryUsers { get; set; }

        //客户等级", "客户等级
        public int? RankId { get; set; }
        public SelectList Ranks { get; set; }

        //备注", "备注
        public string Remark { get; set; }

        //客户渠道", "客户渠道
        public int? ChannelId { get; set; }
        public SelectList Channels { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        //费用合同兑现商品", " 费用合同兑现商品
        public bool CostContractProduct { get; set; }

        //客户片区", "客户片区
        public int? DistrictId { get; set; }
        public SelectList Districts { get; set; }


        #endregion


        #region 小计、合计

        /// <summary>
        /// 数量（当前页）
        /// </summary>
        public string PageSumQuantityConversion { get; set; }
        /// <summary>
        /// 数量（总）
        /// </summary>
        public string TotalSumQuantityConversion { get; set; }

        /// <summary>
        /// 金额（当前页）
        /// </summary>
        public decimal? PageSumAmount { get; set; }
        /// <summary>
        /// 金额（总）
        /// </summary>
        public decimal? TotalSumAmount { get; set; }

        /// <summary>
        /// 成本金额（当前页）
        /// </summary>
        public decimal? PageSumCostAmount { get; set; }
        /// <summary>
        /// 成本金额（总）
        /// </summary>
        public decimal? TotalSumCostAmount { get; set; }

        /// <summary>
        /// 利润（当前页）
        /// </summary>
        public decimal? PageSumProfit { get; set; }
        /// <summary>
        /// 利润（总）
        /// </summary>
        public decimal? TotalSumProfit { get; set; }

        /// <summary>
        /// 成本利润率（当前页）
        /// </summary>
        public decimal? PageSumCostProfitRate { get; set; }
        /// <summary>
        /// 成本利润率（总）
        /// </summary>
        public decimal? TotalSumCostProfitRate { get; set; }

        /// <summary>
        /// 变动差额（当前页）
        /// </summary>
        public decimal? PageSumChangeDifference { get; set; }
        /// <summary>
        /// 变动差额（总）
        /// </summary>
        public decimal? TotalSumChangeDifference { get; set; }


        #endregion

    }

    /// <summary>
    /// 销售汇总（按商品）
    /// </summary>
    public class SaleReportSummaryProductListModel : ReactiveObject
    {
        public IList<SaleReportSummaryProduct> Items { get; set; } = new List<SaleReportSummaryProduct>();

        #region  用于条件检索

        //商品", "商品
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        //商品类别", "商品类别
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public SelectList Categories { get; set; }

        //业务员", "业务员
        public int? BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }

        //仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public SelectList WareHouses { get; set; }

        //客户Id", "客户Id
        public int? TerminalId { get; set; }
        //客户", "客户
        public string TerminalName { get; set; }

        //送货员", "送货员
        public int? DeliveryUserId { get; set; }
        public SelectList DeliveryUsers { get; set; }

        //客户等级", "客户等级
        public int? RankId { get; set; }
        public SelectList Ranks { get; set; }

        //备注", "备注
        public string Remark { get; set; }

        //支付方式", "支付方式
        public int? PayTypeId { get; set; }
        public SelectList PayTypes { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        //费用合同兑现商品", " 费用合同兑现商品
        public bool CostContractProduct { get; set; }

        //客户渠道", "客户渠道
        public int? ChannelId { get; set; }
        public SelectList Channels { get; set; }

        //客户片区", "客户片区
        public int? DistrictId { get; set; }
        public SelectList Districts { get; set; }

        #endregion


        #region 总计

        /// <summary>
        /// 销售数量（总）
        /// </summary>
        public string TotalSumSaleQuantityConversion { get; set; }

        /// <summary>
        /// 销售金额（总）
        /// </summary>
        public decimal? TotalSumSaleAmount { get; set; }

        /// <summary>
        /// 赠送数量（总）
        /// </summary>
        public string TotalSumGiftQuantityConversion { get; set; }

        /// <summary>
        /// 退货数量（总）
        /// </summary>
        public string TotalSumReturnQuantityConversion { get; set; }

        /// <summary>
        /// 退货金额（总）
        /// </summary>
        public decimal? TotalSumReturnAmount { get; set; }

        /// <summary>
        /// 净销售量（总）
        /// </summary>
        public string TotalSumNetQuantityConversion { get; set; }

        /// <summary>
        /// 销售净额（总）
        /// </summary>
        public decimal? TotalSumNetAmount { get; set; }

        /// <summary>
        /// 成本金额（总）
        /// </summary>
        public decimal? TotalSumCostAmount { get; set; }

        /// <summary>
        /// 利润（总）
        /// </summary>
        public decimal? TotalSumProfit { get; set; }

        /// <summary>
        /// 成本利润率（总）
        /// </summary>
        public decimal? TotalSumCostProfitRate { get; set; }


        #endregion


    }

    /// <summary>
    /// 销售汇总（按客户）
    /// </summary>
    public class SaleReportSummaryCustomerListModel : Base
    {

        public IList<SaleReportSummaryCustomer> Items { get; set; } = new List<SaleReportSummaryCustomer>();
        public List<string> DynamicColumns { get; set; } = new List<string>();
        public IList<SaleReportSumStatisticalType> TotalDynamicDatas { get; set; } = new List<SaleReportSumStatisticalType>();

        #region  用于条件检索

        //客户Id", "客户Id
        public int? TerminalId { get; set; }
        //客户", "客户
        public string TerminalName { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        //品牌", "品牌
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public SelectList Brands { get; set; }

        //商品", "商品
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        //商品类别", "商品类别
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public SelectList Categories { get; set; }

        //客户片区", "客户片区
        public int? DistrictId { get; set; }
        public SelectList Districts { get; set; }

        //客户渠道", "客户渠道
        public int? ChannelId { get; set; }
        public SelectList Channels { get; set; }

        //客户等级", "客户等级
        public int? RankId { get; set; }
        public SelectList Ranks { get; set; }

        //整单备注", "整单备注
        public string Remark { get; set; }

        //业务员", "业务员
        public int? BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }

        //仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public SelectList WareHouses { get; set; }


        #endregion


        #region 合计

        /// <summary>
        /// 销售数量
        /// </summary>
        public int? TotalSumSaleSmallQuantity { get; set; }

        /// <summary>
        /// 退货数量
        /// </summary>
        public int? TotalSumReturnSmallQuantity { get; set; }

        /// <summary>
        /// 净销售量 = 销售数量 - 退货数量
        /// </summary>
        public int? TotalSumNetSmallQuantity { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal? TotalSumSaleAmount { get; set; }

        /// <summary>
        /// 退货金额
        /// </summary>
        public decimal? TotalSumReturnAmount { get; set; }

        /// <summary>
        /// 销售净额 = 销售金额 - 退货金额
        /// </summary>
        public decimal? TotalSumNetAmount { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal? TotalSumDiscountAmount { get; set; }

        /// <summary>
        /// 成本金额?
        /// </summary>
        public decimal? TotalSumCostAmount { get; set; }

        /// <summary>
        /// 利润
        /// </summary>
        public decimal? TotalSumProfit { get; set; }

        /// <summary>
        /// 成本利润率
        /// </summary>
        public decimal? TotalSumCostProfitRate { get; set; }


        #endregion


    }


    /// <summary>
    /// 销售汇总（按业务员）
    /// </summary>
    public class SaleReportSummaryBusinessUserListModel
    {
        public IList<SaleReportSummaryCustomer> Items { get; set; } = new List<SaleReportSummaryCustomer>();
        public List<string> DynamicColumns { get; set; } = new List<string>();
        public IList<SaleReportSumStatisticalType> TotalDynamicDatas { get; set; } = new List<SaleReportSumStatisticalType>();


        #region  用于条件检索

        //业务员", "业务员
        public int? BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        //品牌", "品牌
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public SelectList Brands { get; set; }

        #endregion

        #region 合计

        /// <summary>
        /// 销售数量
        /// </summary>
        public int? TotalSumSaleSmallQuantity { get; set; }

        /// <summary>
        /// 退货数量
        /// </summary>
        public int? TotalSumReturnSmallQuantity { get; set; }

        /// <summary>
        /// 净销售量 = 销售数量 - 退货数量
        /// </summary>
        public int? TotalSumNetSmallQuantity { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal? TotalSumSaleAmount { get; set; }

        /// <summary>
        /// 退货金额
        /// </summary>
        public decimal? TotalSumReturnAmount { get; set; }

        /// <summary>
        /// 销售净额 = 销售金额 - 退货金额
        /// </summary>
        public decimal? TotalSumNetAmount { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal? TotalSumDiscountAmount { get; set; }

        /// <summary>
        /// 成本金额?
        /// </summary>
        public decimal? TotalSumCostAmount { get; set; }

        /// <summary>
        /// 利润
        /// </summary>
        public decimal? TotalSumProfit { get; set; }

        /// <summary>
        /// 成本利润率
        /// </summary>
        public decimal? TotalSumCostProfitRate { get; set; }

        #endregion

    }


    /// <summary>
    /// 销售汇总（按客户商品）
    /// </summary>
    public class SaleReportSummaryCustomerProductListModel : Base
    {

        public IList<SaleReportSummaryCustomerProduct> Items { get; set; } = new List<SaleReportSummaryCustomerProduct>();

        #region  用于条件检索

        //仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public SelectList WareHouses { get; set; }

        //商品", "商品
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        //商品类别", "商品类别
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public SelectList Categories { get; set; }

        //品牌", "品牌
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public SelectList Brands { get; set; }

        //客户渠道", "客户渠道
        public int? ChannelId { get; set; }
        public SelectList Channels { get; set; }

        //客户等级", "客户等级
        public int? RankId { get; set; }
        public SelectList Ranks { get; set; }

        //业务员", "业务员
        public int? BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }

        //送货员", "送货员
        public int? DeliveryUserId { get; set; }
        public SelectList DeliveryUsers { get; set; }

        //客户Id", "客户Id
        public int? TerminalId { get; set; }
        //客户", "客户
        public string TerminalName { get; set; }

        //备注", "备注
        public string Remark { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        #endregion


        #region 总计

        /// <summary>
        /// 销售数量（总）
        /// </summary>
        public string TotalSumSaleQuantityConversion { get; set; }

        /// <summary>
        /// 销售金额（总）
        /// </summary>
        public decimal? TotalSumSaleAmount { get; set; }

        /// <summary>
        /// 退货数量（总）
        /// </summary>
        public string TotalSumReturnQuantityConversion { get; set; }

        /// <summary>
        /// 退货金额（总）
        /// </summary>
        public decimal? TotalSumReturnAmount { get; set; }

        /// <summary>
        /// 还货数量（总）
        /// </summary>
        public string TotalSumRepaymentQuantityConversion { get; set; }

        /// <summary>
        /// 还货金额（总）
        /// </summary>
        public decimal? TotalSumRepaymentAmount { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public string TotalSumQuantityConversion { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal? TotalSumAmount { get; set; }

        /// <summary>
        /// 成本金额（总）
        /// </summary>
        public decimal? TotalSumCostAmount { get; set; }

        /// <summary>
        /// 利润（总）
        /// </summary>
        public decimal? TotalSumProfit { get; set; }

        /// <summary>
        /// 成本利润率（总）
        /// </summary>
        public decimal? TotalSumCostProfitRate { get; set; }


        #endregion


    }

    /// <summary>
    /// 销售汇总（按仓库）
    /// </summary>
    public class SaleReportSummaryWareHouseListModel : Base
    {
        public IList<SaleReportSummaryWareHouse> Items { get; set; } = new List<SaleReportSummaryWareHouse>();
        public List<string> DynamicColumns { get; set; } = new List<string>();
        public IList<SaleReportSumStatisticalType> TotalDynamicDatas { get; set; } = new List<SaleReportSumStatisticalType>();

        #region  用于条件检索

        //仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public SelectList WareHouses { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        #endregion

        #region 合计

        /// <summary>
        /// 销售数量
        /// </summary>
        public int? TotalSumSaleSmallQuantity { get; set; }

        /// <summary>
        /// 退货数量
        /// </summary>
        public int? TotalSumReturnSmallQuantity { get; set; }

        /// <summary>
        /// 净销售量 = 销售数量 - 退货数量
        /// </summary>
        public int? TotalSumNetSmallQuantity { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal? TotalSumSaleAmount { get; set; }

        /// <summary>
        /// 退货金额
        /// </summary>
        public decimal? TotalSumReturnAmount { get; set; }

        /// <summary>
        /// 销售净额 = 销售金额 - 退货金额
        /// </summary>
        public decimal? TotalSumNetAmount { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal? TotalSumDiscountAmount { get; set; }

        /// <summary>
        /// 成本金额?
        /// </summary>
        public decimal? TotalSumCostAmount { get; set; }

        /// <summary>
        /// 利润
        /// </summary>
        public decimal? TotalSumProfit { get; set; }

        /// <summary>
        /// 成本利润率
        /// </summary>
        public decimal? TotalSumCostProfitRate { get; set; }

        #endregion

    }

    /// <summary>
    /// 销售汇总（按品牌）
    /// </summary>
    public class SaleReportSummaryBrandListModel
    {
        public IList<SaleReportSummaryBrand> Items { get; set; } = new List<SaleReportSummaryBrand>();
        public List<string> DynamicColumns { get; set; } = new List<string>();
        public IList<SaleReportSumStatisticalType> TotalDynamicDatas { get; set; } = new List<SaleReportSumStatisticalType>();

        #region  用于条件检索

        //品牌", "品牌
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public SelectList Brands { get; set; }

        //客户片区", "客户片区
        public int? DistrictId { get; set; }
        public SelectList Districts { get; set; }

        //客户渠道", "客户渠道
        public int? ChannelId { get; set; }
        public SelectList Channels { get; set; }

        //业务员", "业务员
        public int? BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }

        //送货员", "送货员
        public int? DeliveryUserId { get; set; }
        public SelectList DeliveryUsers { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        #endregion

        #region 合计

        /// <summary>
        /// 销售数量
        /// </summary>
        public int? TotalSumSaleSmallQuantity { get; set; }

        /// <summary>
        /// 退货数量
        /// </summary>
        public int? TotalSumReturnSmallQuantity { get; set; }

        /// <summary>
        /// 净销售量 = 销售数量 - 退货数量
        /// </summary>
        public int? TotalSumNetSmallQuantity { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal? TotalSumSaleAmount { get; set; }

        /// <summary>
        /// 退货金额
        /// </summary>
        public decimal? TotalSumReturnAmount { get; set; }

        /// <summary>
        /// 销售净额 = 销售金额 - 退货金额
        /// </summary>
        public decimal? TotalSumNetAmount { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal? TotalSumDiscountAmount { get; set; }

        /// <summary>
        /// 成本金额?
        /// </summary>
        public decimal? TotalSumCostAmount { get; set; }

        /// <summary>
        /// 利润
        /// </summary>
        public decimal? TotalSumProfit { get; set; }

        /// <summary>
        /// 成本利润率
        /// </summary>
        public decimal? TotalSumCostProfitRate { get; set; }

        #endregion

        /// <summary>
        /// 图表数据
        /// </summary>
        public string Charts { get; set; }

    }

    /// <summary>
    /// 订单明细
    /// </summary>
    public class SaleReportOrderItemListModel : Base
    {
        public IList<SaleReportOrderItem> Items { get; set; } = new List<SaleReportOrderItem>();

        #region  用于条件检索

        //商品", "商品
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        //商品类别", "商品类别
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public SelectList Categories { get; set; }

        //品牌", "品牌
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public SelectList Brands { get; set; }

        //客户Id", "客户Id
        public int? TerminalId { get; set; }
        //客户", "客户
        public string TerminalName { get; set; }

        //单据编号", "单据编号
        public string BillNumber { get; set; }

        //客户等级", "客户等级
        public int? RankId { get; set; }
        public SelectList Ranks { get; set; }

        //业务员", "业务员
        public int? BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }

        //仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public SelectList WareHouses { get; set; }

        //销售类型", "销售类型
        public int? SaleTypeId { get; set; }
        public string SaleTypeName { get; set; }
        public SelectList SaleTypes { get; set; }

        //客户渠道", "客户渠道
        public int? ChannelId { get; set; }
        public SelectList Channels { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        //客户片区", "客户片区
        public int? DistrictId { get; set; }
        public SelectList Districts { get; set; }

        //备注", "备注
        public string Remark { get; set; }

        /// <summary>
        /// 费用合同兑现商品
        /// </summary>
        //费用合同兑现商品", "费用合同兑现商品
        public bool CostContractProduct { get; set; }
        /// <summary>
        /// 只展示占用库存商品
        /// </summary>
        //只展示占用库存商品", "只展示占用库存商品
        public bool OccupyStock { get; set; }

        #endregion

        #region 小计、合计

        /// <summary>
        /// 数量（当前页）
        /// </summary>
        public string PageSumQuantityConversion { get; set; }
        /// <summary>
        /// 数量（总）
        /// </summary>
        public string TotalSumQuantityConversion { get; set; }

        /// <summary>
        /// 金额（当前页）
        /// </summary>
        public decimal? PageSumAmount { get; set; }
        /// <summary>
        /// 金额（总）
        /// </summary>
        public decimal? TotalSumAmount { get; set; }

        /// <summary>
        /// 成本金额（当前页）
        /// </summary>
        public decimal? PageSumCostAmount { get; set; }
        /// <summary>
        /// 成本金额（总）
        /// </summary>
        public decimal? TotalSumCostAmount { get; set; }

        /// <summary>
        /// 利润（当前页）
        /// </summary>
        public decimal? PageSumProfit { get; set; }
        /// <summary>
        /// 利润（总）
        /// </summary>
        public decimal? TotalSumProfit { get; set; }

        /// <summary>
        /// 成本利润率（当前页）
        /// </summary>
        public decimal? PageSumCostProfitRate { get; set; }
        /// <summary>
        /// 成本利润率（总）
        /// </summary>
        public decimal? TotalSumCostProfitRate { get; set; }

        #endregion

    }


    /// <summary>
    /// 订单汇总（按商品）
    /// </summary>
    public class SaleReportSummaryOrderProductListModel : Base
    {

        public IList<SaleReportSummaryOrderProduct> Items { get; set; } = new List<SaleReportSummaryOrderProduct>();

        #region  用于条件检索

        //商品", "商品
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        //商品类别", "商品类别
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public SelectList Categories { get; set; }

        //业务员", "业务员
        public int? BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }

        //仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public SelectList WareHouses { get; set; }

        //客户片区", "客户片区
        public int? DistrictId { get; set; }
        public SelectList Districts { get; set; }

        //客户Id", "客户Id
        public int? TerminalId { get; set; }
        //客户", "客户
        public string TerminalName { get; set; }

        //客户渠道", "客户渠道
        public int? ChannelId { get; set; }
        public SelectList Channels { get; set; }

        //备注", "备注
        public string Remark { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }


        //送货员", "送货员
        public int? DeliveryUserId { get; set; }
        public SelectList DeliveryUsers { get; set; }

        //客户等级", "客户等级
        public int? RankId { get; set; }
        public SelectList Ranks { get; set; }

        /// <summary>
        /// 费用合同兑现商品 
        /// </summary>
        //费用合同兑现商品", "费用合同兑现商品
        public bool CostContractProduct { get; set; }


        #endregion


        #region 总计

        /// <summary>
        /// 订单数量（总）
        /// </summary>
        public string TotalSumSaleQuantityConversion { get; set; }

        /// <summary>
        /// 订单金额（总）
        /// </summary>
        public decimal? TotalSumSaleAmount { get; set; }

        /// <summary>
        /// 赠送数量（总）
        /// </summary>
        public string TotalSumGiftQuantityConversion { get; set; }

        /// <summary>
        /// 退货数量（总）
        /// </summary>
        public string TotalSumReturnQuantityConversion { get; set; }

        /// <summary>
        /// 退货金额（总）
        /// </summary>
        public decimal? TotalSumReturnAmount { get; set; }

        /// <summary>
        /// 净销售量（总）
        /// </summary>
        public string TotalSumNetQuantityConversion { get; set; }

        /// <summary>
        /// 销售净额（总）
        /// </summary>
        public decimal? TotalSumNetAmount { get; set; }

        /// <summary>
        /// 成本金额（总）
        /// </summary>
        public decimal? TotalSumCostAmount { get; set; }

        /// <summary>
        /// 利润（总）
        /// </summary>
        public decimal? TotalSumProfit { get; set; }

        /// <summary>
        /// 成本利润率（总）
        /// </summary>
        public decimal? TotalSumCostProfitRate { get; set; }


        #endregion


    }

    /// <summary>
    /// 费用合同明细表
    /// </summary>
    public class SaleReportCostContractItemListModel
    {

        public IList<SaleReportCostContractItem> Items { get; set; } = new List<SaleReportCostContractItem>();

        #region  用于条件检索

        //客户Id", "客户Id
        public int? TerminalId { get; set; }
        //客户", "客户
        public string TerminalName { get; set; }

        //商品", "商品
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        //业务员", "业务员
        public int? BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }

        //费用类别", "费用类别
        public int AccountingOptionId { get; set; }
        public string AccountingOptionName { get; set; }

        //单据编号", "单据编号
        public string BillNumber { get; set; }

        //商品类别", "商品类别
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public SelectList Categories { get; set; }

        //兑现方式
        //兑现方式", "兑现方式
        public int? CashTypeId { get; set; }
        public string CashTypeName { get; set; }
        public SelectList CashTypes { get; set; }

        //备注", "备注
        public string Remark { get; set; }

        //状态
        //状态", "状态
        public int? StatusTypeId { get; set; }
        public string StatusTypeName { get; set; }
        public SelectList StatusTypes { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        #endregion


        #region 小计、合计

        /// <summary>
        /// 数量（总）
        /// </summary>
        public string TotalSumQuantityConversion { get; set; }

        #endregion

    }

    /// <summary>
    /// 赠品汇总
    /// </summary>
    public class SaleReportSummaryGiveQuotaListModel
    {
        public IList<SaleReportSummaryGiveQuota> Items { get; set; } = new List<SaleReportSummaryGiveQuota>();

        #region  用于条件检索

        //商品", "商品
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        //客户Id", "客户Id
        public int? TerminalId { get; set; }
        //客户", "客户
        public string TerminalName { get; set; }

        //商品类别", "商品类别
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public SelectList Categories { get; set; }

        //备注", "备注
        public string Remark { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        //业务员", "业务员
        public int? BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }

        #endregion


        #region 小计、合计


        /// <summary>
        /// 普通赠品 数量（总）
        /// </summary>
        public string TotalSumNormalQuantityConversion { get; set; }

        /// <summary>
        /// 普通赠品 成本（总）
        /// </summary>
        public decimal? TotalSumNormalCostAmount { get; set; }

        /// <summary>
        /// 订货赠品 数量（总）
        /// </summary>
        public string TotalSumOrderQuantityConversion { get; set; }

        /// <summary>
        /// 订货赠品 成本（总）
        /// </summary>
        public decimal? TotalSumOrderCostAmount { get; set; }

        /// <summary>
        /// 促销赠品 数量（总）
        /// </summary>
        public string TotalSumCampaignQuantityConversion { get; set; }

        /// <summary>
        /// 促销赠品 成本（总）
        /// </summary>
        public decimal? TotalSumCampaignCostAmount { get; set; }

        /// <summary>
        /// 费用合同 数量（总）
        /// </summary>
        public string TotalSumCostContractQuantityConversion { get; set; }

        /// <summary>
        /// 费用合同 成本（总）
        /// </summary>
        public decimal? TotalSumCostContractCostAmount { get; set; }

        /// <summary>
        /// 总数量（总）
        /// </summary>
        public string TotalSumQuantityConversion { get; set; }

        /// <summary>
        /// 总成本金额（总）
        /// </summary>
        public decimal? TotalSumCostAmount { get; set; }

        #endregion

    }

    /// <summary>
    /// 热销排行榜
    /// </summary>
    public class SaleReportHotSaleListModel : Base
    {

        public IList<SaleReportHotSale> Items { get; set; } = new List<SaleReportHotSale>();

        #region  用于条件检索

        //商品", "商品
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        //仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public SelectList WareHouses { get; set; }

        //客户Id", "客户Id
        public int? TerminalId { get; set; }
        //客户", "客户
        public string TerminalName { get; set; }

        //品牌", "品牌
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public SelectList Brands { get; set; }

        //商品类别", "商品类别
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public SelectList Categories { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        //统计前
        public int? TopNumber { get; set; }

        #endregion

        #region 合计

        /// <summary>
        /// 销售数量
        /// </summary>
        public string TotalSumSaleQuantityConversion { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal? TotalSumSaleAmount { get; set; }

        /// <summary>
        /// 退货数量
        /// </summary>
        public string TotalSumReturnQuantityConversion { get; set; }

        /// <summary>
        /// 退货金额
        /// </summary>
        public decimal? TotalSumReturnAmount { get; set; }

        /// <summary>
        /// 净销售量 = 销售数量 - 退货数量
        /// </summary>
        public string TotalSumNetQuantityConversion { get; set; }

        /// <summary>
        /// 销售净额 = 销售金额 - 退货金额
        /// </summary>
        public decimal? TotalSumNetAmount { get; set; }

        /// <summary>
        /// 成本金额
        /// </summary>
        public decimal? TotalSumCostAmount { get; set; }

        /// <summary>
        /// 利润
        /// </summary>
        public decimal? TotalSumProfit { get; set; }

        /// <summary>
        /// 成本利润率
        /// </summary>
        public decimal? TotalSumCostProfitRate { get; set; }

        #endregion

        /// <summary>
        /// 图表数据
        /// </summary>
        public string Charts { get; set; }

    }

    /// <summary>
    /// 销量走势图
    /// </summary>
    public class SaleReportSaleQuantityTrendListModel
    {

        public IList<SaleReportSaleQuantityTrend> Items { get; set; } = new List<SaleReportSaleQuantityTrend>();

        #region  用于条件检索

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        //统计方式", "统计方式
        public int? GroupByTypeId { get; set; }
        public SelectList GroupByTypes { get; set; }

        #endregion

        #region 合计

        #endregion

        /// <summary>
        /// 图表数据
        /// </summary>
        public string Charts { get; set; }

    }

    /// <summary>
    /// 销售商品成本利润
    /// </summary>
    public class SaleReportProductCostProfitListModel : Base
    {
        public IList<SaleReportProductCostProfit> Items { get; set; } = new List<SaleReportProductCostProfit>();

        #region  用于条件检索

        //商品", "商品
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        //商品类别", "商品类别
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public SelectList Categories { get; set; }

        //品牌", "品牌
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public SelectList Brands { get; set; }

        //客户渠道", "客户渠道
        public int? ChannelId { get; set; }
        public SelectList Channels { get; set; }

        //客户Id", "客户Id
        public int? TerminalId { get; set; }
        //客户", "客户
        public string TerminalName { get; set; }

        //业务员", "业务员
        public int? BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }

        //仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public SelectList WareHouses { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        #endregion


        #region 总计

        /// <summary>
        /// 销售数量（总）
        /// </summary>
        public string TotalSumSaleQuantityConversion { get; set; }

        /// <summary>
        /// 销售金额（总）
        /// </summary>
        public decimal? TotalSumSaleAmount { get; set; }

        /// <summary>
        /// 退货数量（总）
        /// </summary>
        public string TotalSumReturnQuantityConversion { get; set; }

        /// <summary>
        /// 退货金额（总）
        /// </summary>
        public decimal? TotalSumReturnAmount { get; set; }

        /// <summary>
        /// 净销售量（总）
        /// </summary>
        public string TotalSumNetQuantityConversion { get; set; }

        /// <summary>
        /// 销售净额（总）
        /// </summary>
        public decimal? TotalSumNetAmount { get; set; }

        /// <summary>
        /// 成本金额（总）
        /// </summary>
        public decimal? TotalSumCostAmount { get; set; }

        /// <summary>
        /// 利润（总）
        /// </summary>
        public decimal? TotalSumProfit { get; set; }

        /// <summary>
        /// 销售利润率（总）
        /// </summary>
        public decimal? TotalSumSaleProfitRate { get; set; }

        /// <summary>
        /// 成本利润率（总）
        /// </summary>
        public decimal? TotalSumCostProfitRate { get; set; }


        #endregion


    }

    /// <summary>
    /// 统计类别汇总
    /// </summary>
    public class SaleReportSumStatisticalType
    {
        /// <summary>
        /// 统计类别Id
        /// </summary>
        public int StatisticalTypeId { get; set; }

        /// <summary>
        /// 净销售量 = 销售数量 - 退货数量 (最小单位数量)
        /// </summary>
        public int? NetSmallQuantity { get; set; }

        /// <summary>
        /// 销售净额 = 销售金额 - 退货金额
        /// </summary>
        public decimal? NetAmount { get; set; }

        /// <summary>
        /// 成本金额
        /// </summary>
        public decimal? CostAmount { get; set; }

        /// <summary>
        /// 利润
        /// </summary>
        public decimal? Profit { get; set; }

        /// <summary>
        /// 成本利润率
        /// </summary>
        public decimal? CostProfitRate { get; set; }

    }

}