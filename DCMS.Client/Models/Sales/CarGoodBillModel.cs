using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;

namespace Wesley.Client.Models.Sales
{

    public partial class CarGoodBillListModel
    {

        //客户Id", "客户Id
        public string TerminalId { get; set; }

        //客户", "客户
        public string TerminalName { get; set; }

        //单据编号", "单据编号
        public string BillNumber { get; set; }


        //业务员", "业务员
        public int? BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }


        //部门", "部门名称
        public int? DepartmentId { get; set; }
        public SelectList ParentList { get; set; }


        //仓库", "仓库
        public int? WareHouseId { get; set; }
        public SelectList WareHouses { get; set; }

        //送货员", "送货员
        public int? DeliveryUserId { get; set; }
        public SelectList DeliveryUsers { get; set; }


        //片区", "片区
        public int? DistrictId { get; set; }
        public SelectList Districts { get; set; }


        //备注", "备注
        public string Remark { get; set; }


        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        /// <summary>
        /// 默认售价下拉
        /// </summary>
        public IEnumerable<SelectListItem> DefaultAmounts { get; set; }


        //状态", "状态
        public int? AuditingStatus { get; set; }
        //public SaleReservationBillStatus SaleReservationBillStatus { get; set; }


        //过滤", "过滤
        public int[] SaleReservationBillFilterSelectedIds { get; set; }
        public IEnumerable<SelectListItem> SaleReservationBillFilters { get; set; }

    }
    /// <summary>
    /// 
    /// 车辆对货单
    /// </summary>
    public class CarGoodBillModel : EntityBase
    {
        //继承基类的Id为 销售单、退货单 单Id

        /// <summary>
        /// 单明细表Id
        /// </summary>
        public int DetailId { get; set; }

        /// <summary>
        /// 销售订单、退货订单 订单Id
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// 订单明细表Id
        /// </summary>
        public int ReservationDetailId { get; set; }

        /// <summary>
        /// 送货员Id
        /// </summary>
        public int? DeliveryUserId { get; set; }

        /// <summary>
        /// 送货员名称
        /// </summary>
        public string DeliveryUserName { get; set; }

        /// <summary>
        /// 单据类型Id
        /// </summary>
        public int BillType { get; set; }

        /// <summary>
        /// 单据类型名称
        /// </summary>
        public string BillTypeName { get; set; }

        /// <summary>
        /// 订单单据单号
        /// </summary>
        public string BillNumberReservation { get; set; }

        /// <summary>
        /// 单据单号
        /// </summary>
        public string BillNumber { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        [Reactive] public int TerminalId { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string TerminalName { get; set; }

        /// <summary>
        /// 转单时间
        /// </summary>
        public DateTime? ChangeDate { get; set; }

        /// <summary>
        /// 仓库Id
        /// </summary>
        [Reactive] public int WareHouseId { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WareHouseName { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 销售订单数量
        /// </summary>
        public int SaleReservationBillQty { get; set; }

        /// <summary>
        /// 销售单数量
        /// </summary>
        public int SaleBillQty { get; set; }

        /// <summary>
        /// 退货订单数量
        /// </summary>
        public int ReturnReservationBillQty { get; set; }

        /// <summary>
        /// 退货单数量
        /// </summary>
        public int ReturnBillQty { get; set; }

        /// <summary>
        /// 拒收
        /// </summary>
        public int RefuseQty { get; set; }

        /// <summary>
        /// 退货
        /// </summary>
        public int ReturnRealQty { get; set; }

        /// <summary>
        /// 送货员单选
        /// </summary>
        public SelectList DeliveryUsers { get; set; }


    }

    public class CarGoodDetailModel : EntityBase
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 拒收
        /// </summary>
        public int RefuseQty { get; set; }

        /// <summary>
        /// 退货
        /// </summary>
        public int ReturnRealQty { get; set; }

        /// <summary>
        /// 合计
        /// </summary>
        public int Total { get; set; }

    }

}
