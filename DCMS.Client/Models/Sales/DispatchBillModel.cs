using System;
using System.Collections.Generic;

namespace Wesley.Client.Models.Sales
{
    /// <summary>
    /// 装车调度单
    /// </summary>
    public class DispatchBillModel : EntityBase
    {
        public List<DispatchItemModel> Items { get; set; } = new List<DispatchItemModel>();
        public List<DispatchBillModel> Bills { get; set; } = new List<DispatchBillModel>();

        public string BillNumber { get; set; }
        public string BillBarCode { get; set; }
        public int BillId { get; set; }
        public int BillTypeId { get; set; } = 0;
        public int DeliveryUserId { get; set; } = 0;
        public SelectList DeliveryUsers { get; set; }
        public string DeliveryUserName { get; set; }
        public int CarId { get; set; } = 0;
        public SelectList Cars { get; set; }
        public string CarNO { get; set; }
        public int? BranchId { get; set; } = 0;

        public int? PrintWholeScrapUserId { get; set; } = 0;
        public bool? PrintWholeScrapStatus { get; set; }

        public DateTime? PrintWholeScrapDate { get; set; }
        public int? PrintWholeScrapNum { get; set; } = 0;
        public int? PrintWholeCarUserId { get; set; } = 0;
        public bool? PrintWholeCarStatus { get; set; }
        public DateTime? PrintWholeCarDate { get; set; }
        public int? PrintWholeCarNum { get; set; } = 0;
        public int? PrintEveryScrapCarUserId { get; set; } = 0;
        public bool? PrintEveryScrapCarStatus { get; set; }
        public DateTime? PrintEveryScrapCarDate { get; set; }
        public int? PrintEveryScrapCarNum { get; set; } = 0;
        public int? PrintOrderUserId { get; set; } = 0;
        public bool? PrintOrderStatus { get; set; }
        public DateTime? PrintOrderDate { get; set; }
        public int? PrintOrderNum { get; set; } = 0;

        public int MakeUserId { get; set; } = 0;
        public string MakeUserName { get; set; }
        public int? AuditedUserId { get; set; } = 0;
        public string AuditedUserName { get; set; }
        public bool AuditedStatus { get; set; }
        public DateTime? AuditedDate { get; set; }
        public int? ReversedUserId { get; set; } = 0;
        public int? ReversedUserName { get; set; }
        public bool ReversedStatus { get; set; }
        public DateTime? ReversedDate { get; set; }
        public int PrintNum { get; set; } = 0;
        public DateTime CreatedOnUtc { get; set; }
        public string SelectDatas { get; set; }
        public int DispatchBillAutoAllocationBill { get; set; } = 0;
        public SelectList DispatchBillAutoAllocationBills { get; set; }
        public int DispatchBillCreatePrint { get; set; } = 0;
        public SelectList DispatchBillCreatePrints { get; set; }
        public int WareHouseId { get; set; } = 0;
        public string WareHouseName { get; set; }
        public SelectList WareHouses { get; set; }
        public bool BillStatus { get; set; }
        public bool ExistSign { get; set; }
        public string OrderQuantityView { get; set; }
        public decimal? OrderAmount { get; set; } = 0;
        public int? OrderQuantitySum { get; set; } = 0;
    }


    /// <summary>
    /// 调拨单据明细
    /// </summary>
    public class DispatchItemModel : EntityBase
    {
        public int? DispatchBillId { get; set; } = 0;
        public int BillId { get; set; } = 0;
        public int? ToBillId { get; set; } = 0;
        public string BillNumber { get; set; }
        public int BillTypeId { get; set; } = 0;
        public string BillTypeName { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? BusinessUserId { get; set; } = 0;
        public string BusinessUserName { get; set; }
        public int? DeliveryUserId { get; set; } = 0;
        public string DeliveryUserName { get; set; }
        public int CarId { get; set; } = 0;
        public string CarNo { get; set; }
        public int? TerminalId { get; set; } = 0;
        public string TerminalName { get; set; }
        public string TerminalPointCode { get; set; }
        public double? Latitude { get; set; } = 0;
        public double? Longitude { get; set; } = 0;
        public string TerminalAddress { get; set; }
        public decimal? OrderAmount { get; set; } = 0;
        public int WareHouseId { get; set; } = 0;
        public string WareHouseName { get; set; }
        public int? OrderQuantitySum { get; set; } = 0;
        public string OrderQuantityView { get; set; }
        public int? SignUserId { get; set; } = 0;
        public int SignStatus { get; set; } = 0;
        public DateTime? SignDate { get; set; }
        public string Remark { get; set; }
        public bool DispatchedStatus { get; set; }


        public ExchangeBillModel ExchangeBillModel { get; set; } = new ExchangeBillModel();
        public SaleReservationBillModel SaleReservationBill { get; set; } = new SaleReservationBillModel();
        public ReturnReservationBillModel ReturnReservationBill { get; set; } = new ReturnReservationBillModel();


        public decimal? SumAmount { get; set; } = 0;
        /// <summary>
        /// 签收验证码
        /// </summary>
        public string VerificationCode { get; set; }

        public bool IsLast { get; set; }


    }
}
