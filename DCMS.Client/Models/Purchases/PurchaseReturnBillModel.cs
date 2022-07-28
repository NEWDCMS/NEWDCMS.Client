using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wesley.Client.Models.Purchases
{

    public partial class PurchaseReturnBillListModel : Base
    {

        public PurchaseReturnBillListModel()
        {

            Lists = new List<PurchaseReturnBillModel>();
            DynamicColumns = new List<string>();
        }


        public IList<PurchaseReturnBillModel> Lists { get; set; }
        public List<string> DynamicColumns { get; set; }


        //("员工", "员工
        [Reactive] public int BusinessUserId { get; set; }
        public SelectList BusinessUsers { get; set; }

        //("供应商", "供应商
        public int ManufacturerId { get; set; }
        public SelectList Manufacturers { get; set; }


        //("仓库", "仓库
        [Reactive] public int WareHouseId { get; set; }
        public SelectList WareHouses { get; set; }

        //("单据编号", "单据编号
        public string BillNumber { get; set; }

        //("状态(打印)", "状态(打印)
        public bool PrintStatus { get; set; }

        //开始时间

        public DateTime StartTime { get; set; }

        //结束时间

        public DateTime EndTime { get; set; }

        //("状态(审核)", "状态(审核)
        public bool AuditedStatus { get; set; }

        //("备注", "备注
        public string Remark { get; set; }

        //("按审核时间", " 按审核时间
        public bool SortByAuditedTime { get; set; }

        //("显示红冲的数据", " 显示红冲的数据
        public bool ShowReverse { get; set; }

    }


    /// <summary>
    /// 采购退货单
    /// </summary>
    public class PurchaseReturnBillModel : AbstractBill, IBCollection<PurchaseReturnItemModel>
    {
        public string BillBarCode { get; set; }
        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public SelectList Manufacturers { get; set; }
        public SelectList BusinessUsers { get; set; }
        public SelectList WareHouses { get; set; }
        public DateTime? TransactionDate { get; set; }
        public bool IsMinUnitPurchase { get; set; }
        public decimal ReceivableAmount { get; set; }
        public decimal PreferentialEndAmount { get; set; }
        public int CollectionAccount { get; set; }
        public decimal CollectionAmount { get; set; }
        public string AuditedUserName { get; set; }
        public bool Paymented { get; set; }
        public ObservableCollection<PurchaseReturnItemModel> Items { get; set; } = new ObservableCollection<PurchaseReturnItemModel>();
        public ObservableCollection<AccountMaping> PurchaseReturnBillAccountings { get; set; } = new ObservableCollection<AccountMaping>();

    }


    /// <summary>
    /// 采购退货单明细
    /// </summary>
    public class PurchaseReturnItemModel : ProductBaseModel
    {

        /// <summary>
        /// 采购退货单Id
        /// </summary>
        public int PurchaseReturnBillId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        //("数量", "数量
        public int Quantity { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        //("价格", "价格
        public decimal Price { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        //("金额", "金额
        public decimal Amount { get; set; }


        /// <summary>
        /// 剩余还款数量
        /// </summary>
        public int RemainderQty { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        #region 商品信息
        //("小单位", "规格属性小单位
        public int SmallUnitId { get; set; }

        //("中单位", "规格属性中单位
        public int? StrokeUnitId { get; set; }

        //("大单位", "规格属性大单位
        public int? BigUnitId { get; set; }
        #endregion

        /// <summary>
        /// 税率%
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// 税额
        /// </summary>
        public decimal TaxPrice { get; set; }

        /// <summary>
        /// 含税价格
        /// </summary>
        public decimal ContainTaxPrice { get; set; }

        /// <summary>
        /// 税价总计
        /// </summary>
        public decimal TaxPriceAmount { get; set; }

    }


    /// <summary>
    /// 项目提交或者编辑
    /// </summary>
    public class PurchaseReturnItemUpdateModel : Base
    {
        public string BillNumber { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public int ManufacturerId { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        [Reactive] public int BusinessUserId { get; set; }

        /// <summary>
        /// 仓库
        /// </summary>
        [Reactive] public int WareHouseId { get; set; }

        /// <summary>
        /// 交易日期
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// 按最小单位采购
        /// </summary>
        public bool IsMinUnitPurchase { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal PreferentialAmount { get; set; }
        /// <summary>
        /// 优惠后金额
        /// </summary>
        public decimal PreferentialEndAmount { get; set; }

        /// <summary>
        /// 欠款金额
        /// </summary>
        public decimal OweCash { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        [Reactive] public IList<PurchaseReturnItemModel> Items { get; set; } = new ObservableCollection<PurchaseReturnItemModel>();

        /// <summary>
        /// 收款账户
        /// </summary>
        [Reactive] public IList<AccountMaping> Accounting { get; set; } = new ObservableCollection<AccountMaping>();

    }


}
