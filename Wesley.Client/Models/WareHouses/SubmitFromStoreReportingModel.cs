using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Wesley.Client.Models.WareHouses
{

    public class InventoryReportBillListModel : EntityBase
    {
        public InventoryReportBillListModel()
        {
            Lists = new List<InventoryReportBillModel>();
            DynamicColumns = new List<string>();
        }

        public IList<InventoryReportBillModel> Lists { get; set; }
        public List<string> DynamicColumns { get; set; }
        [Reactive] public int TerminalId { get; set; }
        public string TerminalName { get; set; }
        public string BillNumber { get; set; }
        public int? BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }
        [Reactive] public int DepartmentId { get; set; }
        public SelectList ParentList { get; set; }
        [Reactive] public int DistrictId { get; set; }
        public SelectList Districts { get; set; }
        public string Remark { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }


    /// <summary>
    /// 用于门店库存上报
    /// </summary>
    public class InventoryReportBillModel : EntityBase
    {

        [Reactive] public string BillNumber { get; set; }
        [Reactive] public string BillBarCode { get; set; }
        [Reactive] public int TerminalId { get; set; }
        [Reactive] public string TerminalName { get; set; }
        [Reactive] public string TerminalPointCode { get; set; }
        [Reactive] public int BusinessUserId { get; set; }
        [Reactive] public string BusinessUserName { get; set; }
        [Reactive] public int? ReversedUserId { get; set; }
        [Reactive] public string ReversedUserName { get; set; }
        [Reactive] public bool ReversedStatus { get; set; }
        [Reactive] public DateTime? ReversedDate { get; set; }
        [Reactive] public int Operation { get; set; }
        [Reactive] public DateTime CreatedOnUtc { get; set; }
        [Reactive] public string Remark { get; set; }
        [Reactive] public ObservableCollection<InventoryReportItemModel> Items { get; set; } = new ObservableCollection<InventoryReportItemModel>();

    }

    /// <summary>
    /// 门店上报商品明细
    /// </summary>
    public class InventoryReportItemModel : EntityBase
    {

        public int InventoryReportBillId { get; set; }



        /// <summary>
        /// 商品Id
        /// </summary>
        [Reactive] public int ProductId { get; set; }
        [Reactive] public string ProductName { get; set; }

        /// <summary>
        /// 采购大单位
        /// </summary>
        [Reactive] public int BigUnitId { get; set; }
        [Reactive] public string BigUnitName { get; set; }

        /// <summary>
        /// 采购大单位数量
        /// </summary>
        [Reactive] public int BigQuantity { get; set; }


        /// <summary>
        /// 采购小单位
        /// </summary>
        [Reactive] public int SmallUnitId { get; set; }
        [Reactive] public string SmallUnitName { get; set; }

        /// <summary>
        /// 采购小单位数量
        /// </summary>
        [Reactive] public int SmallQuantity { get; set; }


        [Reactive] public IList<InventoryReportStoreQuantityModel> InventoryReportStoreQuantities { get; set; } = new ObservableCollection<InventoryReportStoreQuantityModel>();

        /// <summary>
        /// 大单位库存量
        /// </summary>
        [Reactive] public int BigStoreQuantity { get; set; }

        /// <summary>
        /// 小单位库存量
        /// </summary>
        [Reactive] public int SmallStoreQuantity { get; set; }

    }

    public class InventoryReportStoreQuantityModel : EntityBase
    {
        /// <summary>
        /// 门店上报商品明细Id
        /// </summary>
        [Reactive] public int InventoryReportItemId { get; set; }


        /// <summary>
        /// 大单位库存量
        /// </summary>
        [Reactive] public int BigStoreQuantity { get; set; }

        /// <summary>
        /// 小单位库存量
        /// </summary>
        [Reactive] public int SmallStoreQuantity { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        [Reactive] public DateTime? ManufactureDete { get; set; }

        /// <summary>
        /// 采购大单位
        /// </summary>
        [Reactive] public int BigUnitId { get; set; }
        [Reactive] public string BigUnitName { get; set; }

        /// <summary>
        /// 采购小单位
        /// </summary>
        [Reactive] public int SmallUnitId { get; set; }
        [Reactive] public string SmallUnitName { get; set; }

    }

    public class InventoryReportSummaryListModel : EntityBase
    {
        public IList<InventoryReportSummaryModel> Items { get; set; } = new List<InventoryReportSummaryModel>();
    }

    /// <summary>
    /// 上报汇总表
    /// </summary>
    public class InventoryReportSummaryModel : AbstractBill
    {

        /// <summary>
        /// 商品
        /// </summary>
        public int ProductId { get; set; }
        [Reactive] public string ProductName { get; set; }
        public string ProductCode { get; set; }

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
        /// 单位Id（中）
        /// </summary>
        public int? StrokeUnitId { get; set; }
        /// <summary>
        /// 单位名称（中）
        /// </summary>
        public string StrokeUnitName { get; set; }

        /// <summary>
        /// 大转小
        /// </summary>
        public int? BigQuantity { get; set; }
        /// <summary>
        /// 中转小
        /// </summary>
        public int? StrokeQuantity { get; set; }

        /// <summary>
        /// 单位换算
        /// </summary>
        [Reactive] public string UnitConversion { get; set; }

        /// <summary>
        /// 大单位
        /// </summary>
        public int BigUnitId { get; set; }
        [Reactive] public string BigUnitName { get; set; }

        /// <summary>
        /// 小单位
        /// </summary>
        public int SmallUnitId { get; set; }
        [Reactive] public string SmallUnitName { get; set; }

        /// <summary>
        /// 期初时间
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// 期末时间
        /// </summary>
        public DateTime EndDate { get; set; }


        /// <summary>
        /// 期初库存量
        /// </summary>
        public int BeginStoreQuantity { get; set; }
        public string BeginStoreQuantityConversion { get; set; }
        /// <summary>
        /// 期末库存量
        /// </summary>
        public int EndStoreQuantity { get; set; }
        public string EndStoreQuantityConversion { get; set; }


        /// <summary>
        /// 采购量
        /// </summary>
        public int PurchaseQuantity { get; set; }
        public string PurchaseQuantityConversion { get; set; }

        /// <summary>
        /// 销售量
        /// </summary>
        public int SaleQuantity { get; set; }
        public string SaleQuantityConversion { get; set; }
    }


}