using System;
using System.Collections.Generic;


namespace Wesley.Client.Models.Users
{
    public partial class PermissionModel : EntityBase
    {

        public string Name { get; set; }

        public IList<UserRoleModel> UserRoleItems { get; set; }
        public SelectList Stores { get; set; }

        //public List<ModuleTree> MenuTrees { get; set; }

        public DataChannelPermissionModel DataChannelPermission { get; set; } = new DataChannelPermissionModel();

    }

    /// <summary>
    /// 权限记录
    /// </summary>
    public partial class PermissionRecordModel : EntityBase
    {

        public string StoreName { get; set; }
        public SelectList Stores { get; set; }


        ////("权限名", "权限名
        public string Name { get; set; }

        // //("系统名称", "系统名称
        public string SystemName { get; set; }


        ////("权限编码", "权限编码
        public string Code { get; set; }


        ////("描述", "描述
        public string Description { get; set; }

        // //("所属模块", "所属模块Id
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public SelectList ParentList { get; set; }


        // //("是否启用", "是否启用
        public bool Enabled { get; set; }


        ////("创建时间", "创建时间
        public DateTime? CreatedOn { get; set; }

        public bool Selected { get; set; }
    }

    public partial class PermissionRecordRoles : EntityBase
    {
        public int PermissionRecord_Id { get; set; }
        public int UserRole_Id { get; set; }
        /// <summary>
        /// 平台（0 表示PC 端，1：表示APP）
        /// </summary>
        public int Platform { get; set; }

        public virtual UserRoleModel UserRole { get; set; }
        public virtual PermissionRecordModel PermissionRecord { get; set; }
    }

    public class DataChannelPermissionModel : EntityBase
    {
        public int UserRoleId { get; set; }


        #region  价格

        /// <summary>
        /// 是否允许查看进价
        /// </summary>
        public bool ViewPurchasePrice { get; set; }

        /// <summary>
        /// WEB端开单价格允许低于最低售价
        /// </summary>
        public bool PlaceOrderPricePermitsLowerThanMinPriceOnWeb { get; set; }

        /// <summary>
        /// APP销售类单据允许优惠
        /// </summary>
        public bool APPSaleBillsAllowPreferences { get; set; }

        /// <summary>
        /// APP预收款单允许优惠
        /// </summary>
        public bool APPAdvanceReceiptFormAllowsPreference { get; set; }

        /// <summary>
        /// 最大优惠额度
        /// </summary>
        public decimal? MaximumDiscountAmount { get; set; }

        /// <summary>
        /// APP销售类单据允许欠款
        /// </summary>
        public bool APPSaleBillsAllowArrears { get; set; }

        /// <summary>
        /// App开单选赠品权限
        /// </summary>
        public bool AppOpenChoiceGift { get; set; }
        public IEnumerable<SelectListItem> AppOpenChoiceGifts { get; set; }

        #endregion

        #region  打印

        /// <summary>
        /// 销售单/销售订单/退货单/退货订单不审核也可以打印 
        /// </summary>
        public bool PrintingIsNotAudited { get; set; }


        #endregion

        #region  单据

        /// <summary>
        /// 允许查看自己单据报表
        /// </summary>
        public int AllowViewReportId { get; set; }
        public IEnumerable<SelectListItem> AllowViewReports { get; set; }


        #endregion

        #region  客户档案

        /// <summary>
        /// APP允许修改客户档案
        /// </summary>
        public bool APPAllowModificationCustomerInfo { get; set; }

        #endregion

        #region  通知

        /// <summary>
        /// 审核完成通知
        /// </summary>
        public bool Auditcompleted { get; set; }
        /// <summary>
        /// 调度完成通知
        /// </summary>
        public bool EnableSchedulingCompleted { get; set; }
        /// <summary>
        /// 盘点完成通知
        /// </summary>
        public bool EnableInventoryCompleted { get; set; }
        /// <summary>
        /// 转单/签收完成通知
        /// </summary>
        public bool EnableTransfeCompleted { get; set; }
        /// <summary>
        /// 库存预警通知
        /// </summary>
        public bool EnableStockEarlyWarning { get; set; }
        /// <summary>
        /// 客户流失预警通知
        /// </summary>
        public bool EnableCustomerLossWarning { get; set; }
        /// <summary>
        /// 开单异常通知
        /// </summary>
        public bool EnableBillingException { get; set; }
        /// <summary>
        /// 交账完成/撤销通知
        /// </summary>
        public bool EnableCompletionOrCancellationOfAccounts { get; set; }


        #endregion

        #region  待办

        /// <summary>
        /// 审批访问控制
        /// </summary>
        public bool EnableApprovalAcl { get; set; }

        /// <summary>
        /// 收款访问控制
        /// </summary>
        public bool EnableReceivablesAcl { get; set; }

        /// <summary>
        /// 交账访问控制
        /// </summary>
        public bool EnableAccountAcl { get; set; }

        /// <summary>
        /// 资料审核访问控制
        /// </summary>
        public bool EnableDataAuditAcl { get; set; }


        #endregion
    }

    public partial class PermissionRecordQuery : EntityBase
    {
        public string Name { get; set; }
        public int Code { get; set; }

    }

}