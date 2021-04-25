using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;

namespace Wesley.Client.Models.WareHouses
{

    /// <summary>
    /// 用于表示库存商品拆分单列表
    /// </summary>
    public partial class SplitProductBillListModel : ReactiveObject
    {

        public IList<SplitProductBillModel> Items { get; set; }

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
    /// 用于表示库存商品组合单
    /// </summary>
    public class SplitProductBillModel : EntityBase
    {

        public SplitProductBillModel()
        {
            Items = new List<SplitProductItemModel>();
        }
        public IList<SplitProductItemModel> Items { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public string SalesmanName { get; set; }

        //("单据编号", "单据编号
        public string BillNumber { get; set; }
        public string BillBarCode { get; set; }

        //("仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public string WareHouseName { get; set; }
        public SelectList WareHouses { get; set; }

        //("主商品", "主商品
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        //("主商品数量", "主商品数量
        public int? Quantity { get; set; }

        //("主商品成本", "主商品成本
        public decimal? ProductCost { get; set; }

        //("主商品成本金额", "主商品成本金额
        public decimal? ProductCostAmount { get; set; }

        //("备注", "备注
        public string Remark { get; set; }

        //("成本差额", "成本差额
        public decimal? CostDifference { get; set; }

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
    /// 库存商品组合单项目
    /// </summary>

    public class SplitProductItemModel : ProductBaseModel
    {

        //("库存商品组合单", "库存商品组合单
        public int SplitProductBillId { get; set; }

        //("子商品单位", "子商品单位
        public int? SubProductUnitId { get; set; }
        public string SubProductUnitName { get; set; }

        //("子商品数量", "子商品数量
        public int? SubProductQuantity { get; set; }

        //("数量", "数量
        public int? Quantity { get; set; }

        //("单位成本", "单位成本
        public decimal? CostPrice { get; set; }

        //("成本金额", "成本金额
        public decimal? CostAmount { get; set; }

        //("库存", "库存
        public int? Stock { get; set; }

        //("创建时间", "创建时间
        public DateTime CreatedOnUtc { get; set; }


    }

    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class SplitProductUpdateModel : Base
    {

        public SplitProductUpdateModel()
        {
            Items = new List<SplitProductItemModel>();
        }

        //("仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }

        //("主商品", "主商品
        public int ProductId { get; set; }

        //("主商品数量", "主商品数量
        public int? Quantity { get; set; }

        //("主商品成本", "主商品成本
        public decimal? ProductCost { get; set; }

        //("主商品成本金额", "主商品成本金额
        public decimal? ProductCostAmount { get; set; }

        //("备注", "备注
        public string Remark { get; set; }

        //("成本差额", "成本差额
        public decimal? CostDifference { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        public List<SplitProductItemModel> Items { get; set; }

    }
}
