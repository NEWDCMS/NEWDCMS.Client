namespace Wesley.Client.Models.WareHouses
{
    /// <summary>
    /// 仓库表
    /// </summary>
    public class WareHouseModel : EntityBase
    {
        public WareHouseModel() { Status = true; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 仓库类型（枚举 1 仓库 2 车辆）
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 是否允许库存(销售订单)
        /// </summary>
        public bool IsXSDD { get; set; }
        /// <summary>
        /// 是否允许库存(销售单)
        /// </summary>
        public bool IsXSD { get; set; }
        /// <summary>
        /// 是否允许库存(调拨单)
        /// </summary>
        public bool IsDBD { get; set; }
        /// <summary>
        /// 是否允许库存(采购退货单)
        /// </summary>
        public bool IsCGTHD { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

    }
}
