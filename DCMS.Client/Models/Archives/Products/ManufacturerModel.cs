using Wesley.Client.Models.Users;
using System;
using System.Collections.Generic;

namespace Wesley.Client.Models.Products
{

    public class ManufacturerModel : EntityBase
    {
        public ManufacturerModel()
        {
            Status = true;
        }


        /// <summary>
        /// 提供商名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 助记码
        /// </summary>
        public string MnemonicName { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string ContactPhone { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 价格范围
        /// </summary>
        public string PriceRanges { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }


        /// <summary>
        /// 经销商 欠 供应商 总欠款
        /// </summary>
        public decimal OweCashTotal { get; set; }

        public bool Published { get; set; }


        public bool SubjectToAcl { get; set; }
        public List<UserRoleModel> AvailableUserRoles { get; set; }
        public int[] SelectedCustomerRoleIds { get; set; }

        //库存映射
        public bool LimitedToStores { get; set; }
        //public List<StoreModel> AvailableStores { get; set; }
        public int[] SelectedStoreIds { get; set; }

        #region 嵌套类

        public partial class ManufacturerProductModel : EntityBase
        {
            public int ManufacturerId { get; set; }

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public bool IsFeaturedProduct { get; set; }

            public int DisplayOrder1 { get; set; }
        }

        public partial class AddManufacturerProductModel
        {
            public AddManufacturerProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            public string SearchProductName { get; set; }
            public int SearchCategoryId { get; set; }
            public int SearchManufacturerId { get; set; }
            public int SearchStoreId { get; set; }
            public int SearchVendorId { get; set; }
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }

            public int ManufacturerId { get; set; }

            public int[] SelectedProductIds { get; set; }
        }

        #endregion

    }


    /// <summary>
    /// 供应商账户余额
    /// </summary>
    public class ManufacturerBalance : EntityBase
    {
        /// <summary>
        /// 科目Id
        /// </summary>
        public int AccountingOptionId { get; set; }

        /// <summary>
        /// 科目名称
        /// </summary>
        public string AccountingName { get; set; }

        /// <summary>
        /// 剩余预付款金额
        /// </summary>
        public decimal AdvanceAmountBalance { get; set; } = 0;
        /// <summary>
        /// 总欠款
        /// </summary>
        public decimal TotalOweCash { get; set; } = 0;
        /// <summary>
        /// 剩余欠款额度
        /// </summary>
        public decimal OweCashBalance { get; set; } = 0;

    }
}