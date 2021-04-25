using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
namespace Wesley.Client.Models.Purchases
{

    #region 采购明细表
    /// <summary>
    /// 采购明细表
    /// </summary>
    public class PurchaseReportItemListModel : Base
    {
        public PurchaseReportItemListModel()
        {

            Items = new List<PurchaseReportItem>();
        }


        public IList<PurchaseReportItem> Items { get; set; } = new List<PurchaseReportItem>();

        #region  用于条件检索

        //("商品", "商品
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        //("商品类别", "商品类别
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public SelectList Categories { get; set; }

        //("供应商", "供应商
        public int ManufacturerId { get; set; }
        public SelectList Manufacturers { get; set; }

        //("仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public SelectList WareHouses { get; set; }

        //("单据编号", "单据编号
        public string BillNumber { get; set; }

        //("单据类型", "单据类型
        public int? PurchaseTypeId { get; set; }
        public string PurchaseTypeName { get; set; }
        public SelectList PurchaseTypes { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        //("明细备注", "明细备注
        public string Remark { get; set; }

        #endregion

        #region 小计、合计

        /// <summary>
        /// 金额（当前页）
        /// </summary>
        public decimal? PageSumAmount { get; set; }
        /// <summary>
        /// 金额（总）
        /// </summary>
        public decimal? TotalSumAmount { get; set; }

        #endregion

    }
    #endregion

    #region 采购汇总（按商品）

    /// <summary>
    /// 采购汇总（按商品）
    /// </summary>
    public class PurchaseReportSummaryProductListModel : Base
    {

        public IList<PurchaseReportSummaryProduct> Items { get; set; } = new List<PurchaseReportSummaryProduct>();

        #region  用于条件检索

        //("商品类别", "商品类别
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public SelectList Categories { get; set; }

        //("商品", "商品
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        //("供应商", "供应商
        public int ManufacturerId { get; set; }
        public SelectList Manufacturers { get; set; }

        //("仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public SelectList WareHouses { get; set; }

        //开始时间

        public DateTime? StartTime { get; set; }

        //结束时间

        public DateTime? EndTime { get; set; }

        #endregion

        #region 总计

        /// <summary>
        /// 采购数量（总）
        /// </summary>
        public string TotalSumPurchaseQuantityConversion { get; set; }

        /// <summary>
        /// 采购金额（总）
        /// </summary>
        public decimal? TotalSumPurchaseAmount { get; set; }

        /// <summary>
        /// 赠送数量（总）
        /// </summary>
        public string TotalSumGiftQuantityConversion { get; set; }

        /// <summary>
        /// 退购数量（总）
        /// </summary>
        public string TotalSumPurchaseReturnQuantityConversion { get; set; }

        /// <summary>
        /// 退购金额（总）
        /// </summary>
        public decimal? TotalSumPurchaseReturnAmount { get; set; }

        /// <summary>
        /// 数量小计（总）
        /// </summary>
        public string TotalSumQuantityConversion { get; set; }

        /// <summary>
        /// 金额小计（总）
        /// </summary>
        public decimal? TotalSumAmount { get; set; }

        #endregion

    }

    #endregion

    #region 采购汇总（按供应商）

    /// <summary>
    /// 采购汇总（按供应商）
    /// </summary>
    public class PurchaseReportSummaryManufacturerListModel
    {
        public PurchaseReportSummaryManufacturerListModel()
        {

            Items = new List<PurchaseReportSummaryManufacturer>();
            DynamicColumns = new List<string>();
            TotalDynamicDatas = new List<PurchaseReportSumStatisticalType>();

        }


        public IList<PurchaseReportSummaryManufacturer> Items { get; set; }
        public List<string> DynamicColumns { get; set; }
        public IList<PurchaseReportSumStatisticalType> TotalDynamicDatas { get; set; }

        #region  用于条件检索

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        //("供应商", "供应商
        public int ManufacturerId { get; set; }
        public SelectList Manufacturers { get; set; }

        #endregion


        #region 合计

        /// <summary>
        /// 采购数量
        /// </summary>
        public int? TotalSumPurchaseSmallQuantity { get; set; }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? TotalSumOrderAmount { get; set; }


        #endregion

    }

    #endregion

    /// <summary>
    /// 统计类别汇总
    /// </summary>
    public class PurchaseReportSumStatisticalType
    {
        /// <summary>
        /// 统计类别Id
        /// </summary>
        public int StatisticalTypeId { get; set; }

        /// <summary>
        /// 采购数量
        /// </summary>
        public int? PurchaseSmallQuantity { get; set; }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? OrderAmount { get; set; }

    }




    #region 采购明细表
    /// <summary>
    /// 采购明细表
    /// </summary>
    public class PurchaseReportItem
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
        /// 单据类型Id
        /// </summary>
        public int? BillTypeId { get; set; }

        /// <summary>
        /// 单据类型名称
        /// </summary>
        public string BillTypeName { get; set; }

        /// <summary>
        /// 供应商Id
        /// </summary>
        public int ManufacturerId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime? TransactionDate { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditedDate { get; set; }

        /// <summary>
        /// 仓库Id
        /// </summary>
        public int? WareHouseId { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WareHouseName { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public int? ProductId { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string ProductSKU { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 条形码（小）
        /// </summary>
        public string SmallBarCode { get; set; }
        /// <summary>
        /// 条形码（中）
        /// </summary>
        public string StrokeBarCode { get; set; }
        /// <summary>
        /// 条形码（大）
        /// </summary>
        public string BigBarCode { get; set; }

        /// <summary>
        /// 条形码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 单位Id（小）
        /// </summary>
        public int? SmallUnitId { get; set; }
        /// <summary>
        /// 单位名称（小）
        /// </summary>
        public string SmallUnitName { get; set; }
        /// <summary>
        /// 单位Id（中）
        /// </summary>
        public int? StrokeUnitId { get; set; }
        /// <summary>
        /// 单位名称（中）
        /// </summary>
        public string StrokeUnitName { get; set; }
        /// <summary>
        /// 单位Id（大）
        /// </summary>
        public int? BigUintId { get; set; }
        /// <summary>
        /// 单位名称（大）
        /// </summary>
        public string BigUnitName { get; set; }

        /// <summary>
        /// 大转小
        /// </summary>
        public int? BigQuantity { get; set; }
        /// <summary>
        /// 中转小
        /// </summary>
        public int? StrokeQuantity { get; set; }

        /// <summary>
        /// 采购数量(小)
        /// </summary>
        public int? PurchaseSmallQuantity { get; set; }
        /// <summary>
        /// 采购数量(中)
        /// </summary>
        public int? PurchaseStrokeQuantity { get; set; }
        /// <summary>
        /// 采购数量(大)
        /// </summary>
        public int? PurchaseBigQuantity { get; set; }

        /// <summary>
        /// 单位换算
        /// </summary>
        public string UnitConversion { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// 单位Id
        /// </summary>
        public int? UnitId { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
    #endregion

    #region 采购汇总（按商品）
    /// <summary>
    /// 采购汇总（按商品）
    /// </summary>
    public class PurchaseReportSummaryProduct
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public int? ProductId { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 条形码（小）
        /// </summary>
        public string SmallBarCode { get; set; }
        /// <summary>
        /// 条形码（中）
        /// </summary>
        public string StrokeBarCode { get; set; }
        /// <summary>
        /// 条形码（大）
        /// </summary>
        public string BigBarCode { get; set; }

        /// <summary>
        /// 单位Id（小）
        /// </summary>
        public int? SmallUnitId { get; set; }
        /// <summary>
        /// 单位名称（小）
        /// </summary>
        public string SmallUnitName { get; set; }
        /// <summary>
        /// 单位Id（中）
        /// </summary>
        public int? StrokeUnitId { get; set; }
        /// <summary>
        /// 单位名称（中）
        /// </summary>
        public string StrokeUnitName { get; set; }
        /// <summary>
        /// 单位Id（大）
        /// </summary>
        public int? BigUintId { get; set; }
        /// <summary>
        /// 单位名称（大）
        /// </summary>
        public string BigUnitName { get; set; }

        /// <summary>
        /// 大转小
        /// </summary>
        public int? BigQuantity { get; set; }
        /// <summary>
        /// 中转小
        /// </summary>
        public int? StrokeQuantity { get; set; }

        /// <summary>
        /// 采购数量(小)
        /// </summary>
        public int? PurchaseSmallQuantity { get; set; }
        /// <summary>
        /// 采购数量(中)
        /// </summary>
        public int? PurchaseStrokeQuantity { get; set; }
        /// <summary>
        /// 采购数量(大)
        /// </summary>
        public int? PurchaseBigQuantity { get; set; }
        /// <summary>
        /// 采购数量(数量转换)
        /// </summary>
        public string PurchaseQuantityConversion { get; set; }

        /// <summary>
        /// 采购金额
        /// </summary>
        public decimal? PurchaseAmount { get; set; }

        /// <summary>
        /// 赠送数量(小)
        /// </summary>
        public int? GiftSmallQuantity { get; set; }
        /// <summary>
        /// 赠送数量(中)
        /// </summary>
        public int? GiftStrokeQuantity { get; set; }
        /// <summary>
        /// 赠送数量(大)
        /// </summary>
        public int? GiftBigQuantity { get; set; }
        /// <summary>
        /// 赠送数量(数量转换)
        /// </summary>
        public string GiftQuantityConversion { get; set; }

        /// <summary>
        /// 退购数量(小)
        /// </summary>
        public int? PurchaseReturnSmallQuantity { get; set; }
        /// <summary>
        /// 退购数量(中)
        /// </summary>
        public int? PurchaseReturnStrokeQuantity { get; set; }
        /// <summary>
        /// 退购数量(大)
        /// </summary>
        public int? PurchaseReturnBigQuantity { get; set; }
        /// <summary>
        /// 退购数量(数量转换)
        /// </summary>
        public string PurchaseReturnQuantityConversion { get; set; }

        /// <summary>
        /// 退购金额
        /// </summary>
        public decimal? PurchaseReturnAmount { get; set; }

        /// <summary>
        /// 数量小计（小）
        /// </summary>
        public int? SumSmallQuantity { get; set; }
        /// <summary>
        /// 数量小计（中）
        /// </summary>
        public int? SumStrokeQuantity { get; set; }
        /// <summary>
        /// 数量小计（大）
        /// </summary>
        public int? SumBigQuantity { get; set; }
        /// <summary>
        /// 数量小计(数量转换)
        /// </summary>
        public string SumQuantityConversion { get; set; }

        /// <summary>
        /// 金额小计
        /// </summary>
        public decimal? SumAmount { get; set; }

    }
    #endregion

    #region 采购汇总（按供应商）

    public class PurchaseReportSummaryManufacturerQuery
    {
        /// <summary>
        /// 供应商Id
        /// </summary>
        public int? ManufacturerId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 统计类别Id
        /// </summary>
        public int? StatisticalTypeId { get; set; }

        /// <summary>
        /// 条形码（小）
        /// </summary>
        public string SmallBarCode { get; set; }
        /// <summary>
        /// 条形码（中）
        /// </summary>
        public string StrokeBarCode { get; set; }
        /// <summary>
        /// 条形码（大）
        /// </summary>
        public string BigBarCode { get; set; }

        /// <summary>
        /// 单位Id（小）
        /// </summary>
        public int? SmallUnitId { get; set; }
        /// <summary>
        /// 单位名称（小）
        /// </summary>
        public string SmallUnitName { get; set; }
        /// <summary>
        /// 单位Id（中）
        /// </summary>
        public int? StrokeUnitId { get; set; }
        /// <summary>
        /// 单位名称（中）
        /// </summary>
        public string StrokeUnitName { get; set; }
        /// <summary>
        /// 单位Id（大）
        /// </summary>
        public int? BigUintId { get; set; }
        /// <summary>
        /// 单位名称（大）
        /// </summary>
        public string BigUnitName { get; set; }

        /// <summary>
        /// 大转小
        /// </summary>
        public int? BigQuantity { get; set; }
        /// <summary>
        /// 中转小
        /// </summary>
        public int? StrokeQuantity { get; set; }

        /// <summary>
        /// 采购数量
        /// </summary>
        public int? PurchaseQuantity { get; set; }

        /// <summary>
        /// 采购单位
        /// </summary>
        public int? PurchaseUnitId { get; set; }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? PurchaseAmount { get; set; }

    }

    /// <summary>
    /// 采购汇总（按供应商）
    /// </summary>
    public class PurchaseReportSummaryManufacturer
    {
        /// <summary>
        /// 供应商Id
        /// </summary>
        public int? ManufacturerId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string ManufacturerName { get; set; }

        private ICollection<PurchaseReportStatisticalType> _purchaseReportStatisticalTypes;
        /// <summary>
        /// (导航)商品类别
        /// </summary>
        public virtual ICollection<PurchaseReportStatisticalType> PurchaseReportStatisticalTypes
        {
            get { return _purchaseReportStatisticalTypes ?? (_purchaseReportStatisticalTypes = new List<PurchaseReportStatisticalType>()); }
            protected set { _purchaseReportStatisticalTypes = value; }
        }

        /// <summary>
        /// 采购数量
        /// </summary>
        public int? SumPurchaseSmallQuantity { get; set; }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? SumOrderAmount { get; set; }

    }
    #endregion


    /// <summary>
    /// 统计类别
    /// </summary>
    public class PurchaseReportStatisticalType
    {
        /// <summary>
        /// 统计类别Id
        /// </summary>
        public int StatisticalTypeId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? SmallQuantity { get; set; }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? OrderAmount { get; set; }


    }


}