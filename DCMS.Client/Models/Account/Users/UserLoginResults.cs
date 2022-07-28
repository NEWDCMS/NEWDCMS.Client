namespace Wesley.Client.Models.Users
{
    /// <summary>
    /// 用户登录状态枚举
    /// </summary>
    public enum UserLoginResults : int
    {
        /// <summary>
        /// 登录成功
        /// </summary>
        Successful = 1,
        /// <summary>
        /// 账户信息不存在（电子邮箱/用户名/手机号无效）
        /// </summary>
        UserNotExist = 2,
        /// <summary>
        /// 密码错误
        /// </summary>
        WrongPassword = 3,
        /// <summary>
        /// 帐户尚未激活
        /// </summary>
        NotActive = 4,
        /// <summary>
        /// 该用户已被删除
        /// </summary>
        Deleted = 5,
        /// <summary>
        /// 用户该没有注册
        /// </summary>
        NotRegistered = 6,
    }
}
