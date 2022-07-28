
using Wesley.Client.Enums;

namespace Wesley.Client.Models.Users
{

    /// <summary>
    /// 用户系统设置
    /// </summary>

    public class UserSettings
    {
        /// <summary>
        /// 是否启用用户名
        /// </summary>
        public bool UsernamesEnabled { get; set; }

        /// <summary>
        /// 是否检测用户名是否有效
        /// </summary>
        public bool CheckUsernameAvailabilityEnabled { get; set; }

        /// <summary>
        /// 是否允许用户更改用户名
        /// </summary>
        public bool AllowUsersToChangeUsernames { get; set; }

        /// <summary>
        /// 密码格式
        /// </summary>
        public PasswordFormat DefaultPasswordFormat { get; set; }

        /// <summary>
        /// SHA1, MD5 密码格式
        /// </summary>
        public string HashedPasswordFormat { get; set; }

        /// <summary>
        /// 密码最小长度
        /// </summary>
        public int PasswordMinLength { get; set; }


        /// <summary>
        ///  是否用户用户上传头像
        /// </summary>
        public bool AllowUsersToUploadAvatars { get; set; }

        /// <summary>
        /// 用户头像最大字节
        /// </summary>
        public int AvatarMaximumSizeBytes { get; set; }

        /// <summary>
        /// 是否启用默认头像
        /// </summary>
        public bool DefaultAvatarEnabled { get; set; }

        /// <summary>
        /// 是否显示用户默认位置
        /// </summary>
        public bool ShowUsersLocation { get; set; }

        /// <summary>
        /// 是否显示用户加入时间
        /// </summary>
        public bool ShowUsersJoinDate { get; set; }

        /// <summary>
        /// 是否允许显示用户的详细信息
        /// </summary>
        public bool AllowViewingProfiles { get; set; }

        /// <summary>
        /// 新用户注册是否通知
        /// </summary>
        public bool NotifyNewUserRegistration { get; set; }

        /// <summary>
        /// 用户在线时长
        /// </summary>
        public int OnlineUserMinutes { get; set; }

        /// <summary>
        /// 是否存储用户最后访问页面
        /// </summary>
        public bool StoreLastVisitedPage { get; set; }

    }
}
