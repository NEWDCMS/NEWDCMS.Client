using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
namespace Wesley.Client.Models.WareHouses
{

    /// <summary>
    /// 用于表示盘点盈亏单列表
    /// </summary>
    public partial class InventoryProfitLossBillListModel : Base
    {


        public IList<InventoryProfitLossBillModel> Items { get; set; }

        //("经办人", "经办人
        public int ChargePerson { get; set; }
        public string ChargePersonName { get; set; }
        public SelectList ChargePersons { get; set; }



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
    /// 用于表示盘点盈亏单
    /// </summary>
    public class InventoryProfitLossBillModel : EntityBase
    {
        public InventoryProfitLossBillModel()
        {
            Items = new List<InventoryProfitLossItemModel>();
        }
        public IList<InventoryProfitLossItemModel> Items { get; set; }


        //("单据编号", "单据编号
        public string BillNumber { get; set; }
        public string BillBarCode { get; set; }



        //("经办人", "经办人
        public int ChargePerson { get; set; }
        public string ChargePersonName { get; set; }
        public SelectList ChargePersons { get; set; }


        //("仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public string WareHouseName { get; set; }
        public SelectList WareHouses { get; set; }


        //("盘点时间", "盘点时间
        public DateTime InventoryDate { get; set; }
        //("盘点日期（显示）", "盘点日期（显示）
        public string InventoryDateView { get; set; }


        //("是否按最小单位盘点", "是否按最小单位盘点
        public bool InventoryByMinUnit { get; set; }


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
        //("状态(审核)（显示）", " 状态(审核)（显示）
        public string AuditedStatusName { get; set; }


        //("审核时间", "审核时间
        public DateTime? AuditedDate { get; set; }
        //("调拨日期（显示）", "调拨日期（显示）
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
    /// 盘点盈亏单项目
    /// </summary>

    public class InventoryProfitLossItemModel : ProductBaseModel
    {


        //("盘点盈亏单", "盘点盈亏单
        public int InventoryProfitLossBillId { get; set; }


        //("数量", "数量
        public int Quantity { get; set; }


        //("成本价", "成本价
        public decimal? CostPrice { get; set; }

        //("成本金额", "成本金额
        public decimal? CostAmount { get; set; }


        ////("库存", "库存
        //public int Stock { get; set; }

        //("创建时间", "创建时间
        public DateTime CreatedOnUtc { get; set; }

    }




    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class InventoryProfitLossUpdateModel : Base
    {
        public InventoryProfitLossUpdateModel()
        {
            Items = new List<InventoryProfitLossItemModel>();
        }


        //("经办人", "经办人
        public int ChargePerson { get; set; }


        //("仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }


        //("盘点时间", "盘点时间
        public DateTime InventoryDate { get; set; }


        //("是否按最小单位盘点", "是否按最小单位盘点
        public bool InventoryByMinUnit { get; set; }


        //("备注", "备注
        public string Remark { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        public List<InventoryProfitLossItemModel> Items { get; set; }

    }
}
