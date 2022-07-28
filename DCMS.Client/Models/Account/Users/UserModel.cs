using System;
using System.Collections.Generic;


namespace Wesley.Client.Models.Users
{
    /// <summary>
    /// 业务员数据模型
    /// </summary>
    public partial class BusinessUserModel : EntityBase
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string UserRealName { get; set; }
        public string MobileNumber { get; set; }
        public int? SalesmanExtractPlanId { get; set; }
        public int? DeliverExtractPlanId { get; set; }
        public decimal? MaxAmountOfArrears { get; set; }
        public int[] SelectedUserDistricts { get; set; }
        public List<UserDistrictsModel> AvailableUserDistricts { get; set; }
        public string FaceImage { get; set; }
        public List<PermissionRecordModel> AvailablePermissionRecords { get; set; }
    }

    /// <summary>
    /// 用户实体
    /// </summary>
    public partial class UserModel : EntityBase
    {
        /// <summary>
        /// 经销商
        /// </summary>
        public int? StoreID { get; set; }
        public string StoreName { get; set; }

        /// <summary>
        /// 用户标识（GUID）
        /// </summary>
        public Guid UserGuid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string UserRealName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }


        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 密码格式类型
        /// </summary>
        public int PasswordFormatId { get; set; }

        /// <summary>
        /// 密码秘钥
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        ///管理员评论
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// 用户是否免税
        /// </summary>
        public bool IsTaxExempt { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///是否删除
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// 是否系统账户
        /// </summary>
        public bool IsSystemAccount { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// 最后访问IP
        /// </summary>
        public string LastIpAddress { get; set; }

        public string LastVisitedPage { get; set; }

        /// <summary>
        /// 账户创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime LastActivityDate { get; set; }

        /// <summary>
        /// 是否邮件安全验证
        /// </summary>
        public bool EmailValidation { get; set; }


        /// <summary>
        /// 是否手机安全验证
        /// </summary>
        public bool MobileValidation { get; set; }


        /// <summary>
        /// 账户类型(0 区域公司,1 经销商)
        /// </summary>
        public int AccountType { get; set; }
        public SelectList AccountTypes { get; set; }

        /// <summary>
        /// 组织机构代码(账户类型需为(0 区域公司))
        /// </summary>
        public int BranchCode { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
        public SelectList ParentList { get; set; }

        /// <summary>
        /// 业务员提成方案
        /// </summary>
        public int? SalesmanExtractPlanId { get; set; }
        public SelectList SalesmanExtractPlans { get; set; }

        /// <summary>
        /// 送货员提成方案
        /// </summary>
        public int? DeliverExtractPlanId { get; set; }
        public SelectList DeliverExtractPlans { get; set; }

        /// <summary>
        /// 最大欠款额度
        /// </summary>
        public decimal? MaxAmountOfArrears { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string FaceImage { get; set; }

        public string AppId { get; set; }


        public bool AllowUsersToChangeUsernames { get; set; }
        public bool UsernamesEnabled { get; set; }

        public bool GenderEnabled { get; set; }

        public string Gender { get; set; }
        public SelectList Genders { get; set; }

        public bool DateOfBirthEnabled { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public bool StreetAddressEnabled { get; set; }

        public string StreetAddress { get; set; }

        public bool CityEnabled { get; set; }

        public string City { get; set; }

        public bool CountryEnabled { get; set; }
        public int CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool StateProvinceEnabled { get; set; }
        public int StateProvinceId { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public bool PhoneEnabled { get; set; }

        public string UserRoleNames { get; set; }

        public List<UserRoleModel> AvailableUserRoles { get; set; } = new List<UserRoleModel>();

        public int[] SelectedUserRoleIds { get; set; }
        public bool AllowManagingUserRoles { get; set; }

        public int[] SelectedUserDistricts { get; set; }
        public List<UserDistrictsModel> AvailableUserDistricts { get; set; } = new List<UserDistrictsModel>();
        public string UserDistrictsZTree { get; set; }
        public List<PermissionRecordModel> AvailablePermissionRecords { get; set; } = new List<PermissionRecordModel>();

    }
    public class RevokeTokenRequest
    {
        public string Token { get; set; }
    }

    public partial class UserAuthenticationModel : EntityBase
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string UserRealName { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string MobileNumber { get; set; }

        public string StoreName { get; set; }
        public string FaceImage { get; set; }

        public List<UserRoleQuery> Roles { get; set; } = new List<UserRoleQuery>();
        public List<ModuleQuery> Modules { get; set; } = new List<ModuleQuery>();
        public List<PermissionRecordQuery> PermissionRecords { get; set; } = new List<PermissionRecordQuery>();
        public List<UserDistrictsQuery> Districts { get; set; } = new List<UserDistrictsQuery>();
        public string AppId { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string AccessToken { get; set; }


        /// <summary>
        /// ERP经销商编号
        /// </summary>
        public string DealerNumber { get; set; }

        /// <summary>
        /// ERP营销中心
        /// </summary>
        public string MarketingCenter { get; set; }
        public string MarketingCenterCode { get; set; }

        /// <summary>
        /// ERP销售大区
        /// </summary>
        public string SalesArea { get; set; }
        public string SalesAreaCode { get; set; }

        /// <summary>
        /// ERP业务部
        /// </summary>
        public string BusinessDepartment { get; set; }
        public string BusinessDepartmentCode { get; set; }

    }

    public partial class LoginModel
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string CaptchaCode { get; set; }

        public bool DisplayCaptcha { get; set; }

        public bool IsValid { get; set; }

        public string ReturnUrl { get; set; }

        /// <summary>
        /// UUID
        /// </summary>
        public string AppId { get; set; }
    }

    public partial class UserDistrictsModel : EntityBase
    {
        ////("用户", "用户
        public int UserId { get; set; }
        public string UserName { get; set; }

        // //("片区", "片区
        public int DistrictsId { get; set; }
        public string DistrictsName { get; set; }
    }

    public partial class UserDistrictsQuery
    {
        public int Id { get; set; }
        public int DistrictsId { get; set; }
    }
}