using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wesley.Client.Models.WareHouses
{
    /// <summary>
    /// 用于表示调拨单
    /// </summary>
    public class AllocationBillModel : AbstractBill, IBCollection<AllocationItemModel>
    {

        [Reactive] public ObservableCollection<AllocationItemModel> Items { get; set; } = new ObservableCollection<AllocationItemModel>();

        [Reactive] public string BillBarCode { get; set; }

        //("出货仓库", "出货仓库
        public SelectList ShipmentWareHouses { get; set; }

        //("入货仓库", "入货仓库
        public SelectList IncomeWareHouses { get; set; }


        //("调拨日期（显示）", "调拨日期（显示）
        public string CreatedOnUtcView { get; set; }

        //("是否按最小单位调拨", "是否按最小单位调拨
        public bool AllocationByMinUnit { get; set; }
        [Reactive] public string AuditedUserName { get; set; }

        //("状态(审核)（显示）", " 状态(审核)（显示）
        public string AuditedStatusName { get; set; }

        //("审核时间（显示）", "审核时间（显示）
        public string AuditedDateName { get; set; }

        //("加载类型", "加载类型
        public int ModelLoadType { get; set; }

        //("加载数据", "加载数据
        public string ModelLoadData { get; set; }

    }

    /// <summary>
    /// 调拨单项目
    /// </summary>
    public class AllocationItemModel : ProductBaseModel
    {

        //("调拨单", "调拨单Id
        public int AllocationBillId { get; set; }

        //("数量", "数量
        [Reactive] public int Quantity { get; set; }

        //("批发价", "批发价
        public decimal? TradePrice { get; set; }

        //("批发金额", "批发金额
        public decimal? WholesaleAmount { get; set; }

        //("出库库存", "出库库存
        public int OutgoingStock { get; set; }

        //("入库库存", "入库库存
        public int WarehousingStock { get; set; }


        //("创建时间", "创建时间
        public DateTime CreatedOnUtc { get; set; }

        #region 商品信息
        //("小单位", "规格属性小单位
        public int SmallUnitId { get; set; }

        //("中单位", "规格属性中单位
        public int? StrokeUnitId { get; set; }

        //("大单位", "规格属性大单位
        public int? BigUnitId { get; set; }
        #endregion

        public bool IsManufactureDete { get; set; }

        /// <summary>
        /// 小计
        /// </summary>
        [Reactive]
        public decimal Subtotal { get; set; }

    }

    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class AllocationUpdateModel : EntityBase
    {
        public string BillNumber { get; set; }
        //("出货仓库", "出货仓库
        public int ShipmentWareHouseId { get; set; }

        //("入货仓库", "入货仓库
        public int IncomeWareHouseId { get; set; }

        //("调拨日期", "调拨日期
        public DateTime CreatedOnUtc { get; set; }

        //("是否按最小单位调拨", "是否按最小单位调拨
        public bool AllocationByMinUnit { get; set; }

        //("备注", "备注
        public string Remark { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        [Reactive] public IList<AllocationItemModel> Items { get; set; } = new ObservableCollection<AllocationItemModel>();

    }
}
