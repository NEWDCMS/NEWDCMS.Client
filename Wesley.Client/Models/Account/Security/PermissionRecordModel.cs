using Wesley.Client.Enums;
using Wesley.Client.Models.Users;
using System;
using System.Collections.Generic;

namespace Wesley.Client.Models.Security
{

    /// <summary>
    /// 表示权限
    /// </summary>
    public class PermissionRecordModel : EntityBase
    {
        private IList<UserRoleModel> _userRoles;
        /// <summary>
        /// 权限名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// 所属模块Id
        /// </summary>
        public int ModuleId { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        #region  

        /// <summary>
        /// 模块
        /// </summary>
        //public virtual ModuleModel Module { get; set; }


        /// <summary>
        /// 角色集合
        /// </summary>
        public IList<UserRoleModel> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new List<UserRoleModel>()); }
            protected set { _userRoles = value; }
        }

        #endregion

    }

    public partial class PermissionRecordQuery
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }

    }


    /// <summary>
    /// 表示数据和频道权限
    /// </summary>
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
        public AllowViewReport AllowViewReport
        {
            get { return (AllowViewReport)AllowViewReportId; }
            set { AllowViewReportId = (int)value; }
        }

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


}
