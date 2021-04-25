using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
namespace Wesley.Client.Models.WareHouses
{

    /// <summary>
    /// 用于表示商品报损单列表
    /// </summary>
    public partial class ScrapProductBillListModel : Base
    {

        public IList<ScrapProductBillModel> Items { get; set; }

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
    /// 用于表示商品报损单
    /// </summary>
    public class ScrapProductBillModel : EntityBase
    {

        public ScrapProductBillModel()
        {
            Items = new List<ScrapProductItemModel>();
        }
        public IList<ScrapProductItemModel> Items { get; set; }

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

        //("是否按基本单位报损", "是否按基本单位报损
        public bool ScrapByBaseUnit { get; set; }

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
        //("制单时间显示", "制单时间显示
        public string CreatedOnUtcView { get; set; }

    }


    /// <summary>
    /// 商品报损单项目
    /// </summary>

    public class ScrapProductItemModel : ProductBaseModel
    {

        //("商品报损单", "商品报损单
        public int ScrapProductBillId { get; set; }

        //("数量", "数量
        public int Quantity { get; set; }

        //("成本价", "成本价
        public decimal? CostPrice { get; set; }

        //("成本金额", "成本金额
        public decimal? CostAmount { get; set; }

        //("批发价", "批发价
        public decimal? TradePrice { get; set; }

        //("批发金额", "批发金额
        public decimal? TradeAmount { get; set; }

        //("创建时间", "创建时间
        public DateTime CreatedOnUtc { get; set; }

    }

    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class ScrapProductUpdateModel : Base
    {
        public ScrapProductUpdateModel()
        {
            Items = new List<ScrapProductItemModel>();
        }

        //("经办人", "经办人
        public int ChargePerson { get; set; }

        //("仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }

        //("是否按基本单位报损", "是否按基本单位报损
        public bool ScrapByBaseUnit { get; set; }

        //("备注", "备注
        public string Remark { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        public List<ScrapProductItemModel> Items { get; set; }

    }
}
