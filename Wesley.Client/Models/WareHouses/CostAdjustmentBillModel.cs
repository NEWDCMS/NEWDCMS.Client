using System;
using System.Collections.Generic;

namespace Wesley.Client.Models.WareHouses
{


    /// <summary>
    /// 用于表示成本调价单列表
    /// </summary>
    public partial class CostAdjustmentBillListModel
    {

        public IList<CostAdjustmentBillModel> Items { get; set; }

        //("操作员", "操作员
        public int OperatorId { get; set; }
        public string OperatorName { get; set; }
        public SelectList Operators { get; set; }

        //("单据编号", "单据编号
        public string BillNumber { get; set; }

        //("状态(审核)", "状态(审核)
        public bool? AuditedStatus { get; set; }

        //("打印状态", "打印状态
        public bool? PrintedStatus { get; set; }

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
    /// 用于表示成本调价单
    /// </summary>
    public class CostAdjustmentBillModel : EntityBase
    {
        public CostAdjustmentBillModel()
        {
            Items = new List<CostAdjustmentItemModel>();
        }
        public IList<CostAdjustmentItemModel> Items { get; set; }

        //("单据编号", "单据编号
        public string BillNumber { get; set; }
        public string BillBarCode { get; set; }

        //("调价日期", "调价日期
        public DateTime AdjustmentDate { get; set; }
        //("调价日期显示", "调价日期显示
        public string AdjustmentDateView { get; set; }

        //("是否按最小单位调价", "是否按最小单位调价
        public bool AdjustmentByMinUnit { get; set; }

        //("备注", "备注
        public string Remark { get; set; }

        //("制单人", "制单人
        public int MakeUserId { get; set; }
        public string MakeUserName { get; set; }

        //("审核人", "审核人
        public int? AuditedUserId { get; set; }
        public string AuditedUserName { get; set; }

        //("状态(审核)", " 状态(审核)
        public bool AuditedStatus { get; set; }
        //("状态(审核)显示", " 状态(审核)显示
        public string AuditedStatusName { get; set; }

        //("审核时间", "审核时间
        public DateTime? AuditedDate { get; set; }
        //("审核时间显示", "审核时间显示
        public string AuditedDateName { get; set; }

        //("红冲人", "红冲人
        public int? ReversedUserId { get; set; }

        //("红冲状态", "红冲状态
        public bool ReversedStatus { get; set; }

        //("红冲时间", "红冲时间
        public DateTime? ReversedDate { get; set; }

        //("打印数", "打印数
        public int? PrintNum { get; set; }

        public DateTime CreatedOnUtc { get; set; }

    }

    /// <summary>
    /// 成本调价单项目
    /// </summary>

    public class CostAdjustmentItemModel : ProductBaseModel
    {

        //("成本调价单", "成本调价单
        public int CostAdjustmentBillId { get; set; }

        //("数量", "数量
        public int Quantity { get; set; }

        //("调整前价格", "调整前价格
        public decimal? AdjustmentPriceBefore { get; set; }

        //("调整后价格", "调整后价格
        public decimal? AdjustedPrice { get; set; }

        //("创建时间", "创建时间
        public DateTime CreatedOnUtc { get; set; }

    }

    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class CostAdjustmentUpdateModel
    {
        public CostAdjustmentUpdateModel()
        {
            Items = new List<CostAdjustmentItemModel>();
        }

        //("调价日期", "调价日期
        public DateTime AdjustmentDate { get; set; }

        //("是否按最小单位调价", "是否按最小单位调价
        public bool AdjustmentByMinUnit { get; set; }

        //("备注", "备注
        public string Remark { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        public List<CostAdjustmentItemModel> Items { get; set; }

    }
}
