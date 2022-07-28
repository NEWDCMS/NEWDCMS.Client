using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Wesley.Client.Models.WareHouses
{

    /// <summary>
    /// 用于表示盘点任务(部分)列表
    /// </summary>
    public partial class InventoryPartTaskBillListModel : EntityBase
    {
        public IList<InventoryPartTaskBillModel> Items { get; set; } = new List<InventoryPartTaskBillModel>();

        //("盘点人", "盘点人
        public int InventoryPerson { get; set; }
        [Reactive] public string InventoryPersonName { get; set; }
        public SelectList InventoryPersons { get; set; }

        //("盘点状态", "盘点状态(未盘点，进行总，已结束)
        public int? InventoryStatus { get; set; }
        public IEnumerable<SelectListItem> InventoryStatuss { get; set; }

        //("仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public string WareHouseName { get; set; }
        public SelectList WareHouses { get; set; }

        //("单据编号", "单据编号
        public string BillNumber { get; set; }

        //("状态(审核)", "状态(审核)
        public bool? AuditedStatus { get; set; }

        //("开始日期", "开始日期
        public DateTime? StartTime { get; set; }

        //("截止日期", "截止日期
        public DateTime? EndTime { get; set; }

        //(" 显示红冲的数据", " 显示红冲的数据
        public bool? ShowReverse { get; set; }

        //("按审核时间", " 按审核时间
        public bool? SortByAuditedTime { get; set; }

        //("备注", "备注
        public string Remark { get; set; }

    }



    /// <summary>
    /// 用于表示盘点任务(部分)
    /// </summary>
    public class InventoryPartTaskBillModel : AbstractBill
    {

        [Reactive] public IList<InventoryPartTaskItemModel> Items { get; set; } = new ObservableCollection<InventoryPartTaskItemModel>();

        public int ProductId { get; set; }
        [Reactive] public string ProductName { get; set; }

        [Reactive] public string BillBarCode { get; set; }

        //("盘点人", "盘点人
        [Reactive] public int InventoryPerson { get; set; }
        [Reactive] public string InventoryPersonName { get; set; }
        public SelectList InventoryPersons { get; set; }

        public SelectList WareHouses { get; set; }

        //("盘点时间", "盘点时间
        public DateTime InventoryDate { get; set; }
        //("盘点时间显示", "盘点时间显示
        [Reactive] public string InventoryDateView { get; set; }

        //("关联盈亏单", "关联盈亏单
        public int? InventoryProfitLossBillId { get; set; }

        //("审核时间显示", "审核时间显示
        public string AuditedDateName { get; set; }

        //("盘点状态", "盘点状态(未盘点，进行总，已结束)
        [Reactive] public int InventoryStatus { get; set; }
        [Reactive] public string InventoryStatusName { get; set; }

        public IEnumerable<SelectListItem> InventoryStatuss { get; set; }


        //合计
        [Reactive] public int SumCount { get; set; }
        //已盘
        [Reactive] public int CompletedCount { get; set; }
        //未盘
        [Reactive] public int NuCompletedCount { get; set; }

        [Reactive] public bool CompletedCommandEnable { get; set; } = true;

        public string TerminalPointCode { get; set; }

        public SelectList BusinessUsers { get; set; }

    }


    /// <summary>
    /// 盘点任务项目
    /// </summary>
    public class InventoryPartTaskItemModel : ProductBaseModel
    {

        //("盘点任务单", "盘点任务单
        public int InventoryPartTaskBillId { get; set; }

        //("数量", "数量
        [Reactive] public int Quantity { get; set; }

        /// <summary>
        /// 当前库存数量
        /// </summary>
        [Reactive] public int? CurrentStock { get; set; }

        /// <summary>
        /// 大单位数量
        /// </summary>
        [Reactive] public int? BigUnitQuantity { get; set; }

        /// <summary>
        /// 中单位数量
        /// </summary>
        [Reactive] public int? AmongUnitQuantity { get; set; }

        /// <summary>
        /// 小单位数量
        /// </summary>
        [Reactive] public int? SmallUnitQuantity { get; set; }

        /// <summary>
        /// 盘盈数量
        /// </summary>
        [Reactive] public int? VolumeQuantity { get; set; }

        /// <summary>
        /// 盘亏数量
        /// </summary>
        [Reactive] public int? LossesQuantity { get; set; }

        /// <summary>
        /// 盘盈批发金额
        /// </summary>
        [Reactive] public decimal? VolumeWholesaleAmount { get; set; }

        /// <summary>
        /// 盘亏批发金额
        /// </summary>
        [Reactive] public decimal? LossesWholesaleAmount { get; set; }

        /// <summary>
        /// 盘盈成本金额
        /// </summary>
        [Reactive] public decimal? VolumeCostAmount { get; set; }

        /// <summary>
        /// 盘亏成本金额
        /// </summary>
        [Reactive] public decimal? LossesCostAmount { get; set; }

        //("创建时间", "创建时间
        [Reactive] public DateTime CreatedOnUtc { get; set; }

        #region 商品信息
        //("小单位", "规格属性小单位
        [Reactive] public int SmallUnitId { get; set; }

        //("中单位", "规格属性中单位
        [Reactive] public int? StrokeUnitId { get; set; }

        //("大单位", "规格属性大单位
        [Reactive] public int? BigUnitId { get; set; }

        #endregion

        /// <summary>
        /// 当前库存描述
        /// </summary>
        [Reactive] public string CurrentStockName { get; set; }
        /// <summary>
        /// 盘点库存描述
        /// </summary>
        [Reactive] public string InventoryStockName { get; set; }
        /// <summary>
        /// 盈亏状态描述
        /// </summary>
        [Reactive] public string StatusName { get; set; }

    }

    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class InventoryPartTaskUpdateModel : Base
    {
        public string BillNumber { get; set; }

        //("盘点人", "盘点人
        [Reactive] public int InventoryPerson { get; set; }

        //("仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }

        //("盘点时间", "盘点时间
        [Reactive] public DateTime InventoryDate { get; set; }

        [Reactive] public string Remark { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        [Reactive] public IList<InventoryPartTaskItemModel> Items { get; set; } = new ObservableCollection<InventoryPartTaskItemModel>();

    }
}
