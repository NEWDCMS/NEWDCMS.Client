namespace Wesley.Client.Models.Users
{
    /// <summary>
    /// 系统用户属性名
    /// </summary>
    public static partial class SystemUserAttributeNames
    {
        /// <summary>
        /// 表单字段
        /// </summary>
        public static string UserRealName { get { return "UserRealName"; } }
        public static string PetName { get { return "PetName"; } }
        public static string Gender { get { return "Gender"; } }
        public static string DateOfBirth { get { return "DateOfBirth"; } }
        public static string Company { get { return "Company"; } }
        public static string StreetAddress { get { return "StreetAddress"; } }
        public static string StreetAddress2 { get { return "StreetAddress2"; } }
        public static string ZipPostalCode { get { return "ZipPostalCode"; } }
        public static string City { get { return "City"; } }
        public static string CountryId { get { return "CountryId"; } }
        public static string StateProvinceId { get { return "StateProvinceId"; } }
        public static string Phone { get { return "Phone"; } }
        public static string Mobile { get { return "Mobile"; } }
        public static string Fax { get { return "Fax"; } }
        public static string VatNumber { get { return "VatNumber"; } }
        public static string VatNumberStatusId { get { return "VatNumberStatusId"; } }
        public static string TimeZoneId { get { return "TimeZoneId"; } }


        /// 头像图片标识
        /// </summary>
        public static string AvatarPictureId { get { return "AvatarPictureId"; } }
        /// <summary>
        /// 签名
        /// </summary>
        public static string Signature { get { return "Signature"; } }
        /// <summary>
        /// 密码找回令牌
        /// </summary>
        public static string PasswordRecoveryToken { get { return "PasswordRecoveryToken"; } }
        /// <summary>
        /// 帐户激活令牌
        /// </summary>
        public static string AccountActivationToken { get { return "AccountActivationToken"; } }
        /// <summary>
        /// 最后访问页面
        /// </summary>
        public static string LastVisitedPage { get { return "LastVisitedPage"; } }
        /// <summary>
        /// 模拟用户标识
        /// </summary>
        public static string ImpersonatedCustomerId { get { return "ImpersonatedCustomerId"; } }
        /// <summary>
        /// 店铺管理区域配置
        /// </summary>
        public static string AdminAreaStoreScopeConfiguration { get { return "AdminAreaStoreScopeConfiguration"; } }

        /// <summary>
        /// 关于新的个人消息
        /// </summary>
        public static string NotifiedAboutNewPrivateMessages { get { return "NotifiedAboutNewPrivateMessages"; } }

        public static string WorkingDesktopThemeName { get { return "WorkingDesktopThemeName"; } }
        public static string DontUseMobileVersion { get { return "DontUseMobileVersion"; } }
        public static string TaxDisplayTypeId { get { return "TaxDisplayTypeId"; } }
        public static string UseRewardPointsDuringCheckout { get { return "UseRewardPointsDuringCheckout"; } }


        //用户中心常规菜单项
        public static string UseDefinedMenuCodes { get { return "UseDefinedMenuCodes"; } }
        //默认常规菜单项是否收起
        public static string UseDefinedMenuRetractFlag { get { return "UseDefinedMenuRetractFlag"; } }
    }

}
