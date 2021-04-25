using System;
using System.Collections.Generic;


namespace Wesley.Client.Models.Users
{

    /// <summary>
    /// 用户组
    /// </summary>
    public class UserGroupModel : EntityBase
    {

        private IList<UserRoleModel> _userRoles;
        private IList<UserModel> _users;


        public UserGroupModel()
        {
            UserRoles = new List<UserRoleModel>();
            Users = new List<UserModel>();
        }

        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderSort { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        #region 

        /// <summary>
        /// 角色集合
        /// </summary>
        public IList<UserRoleModel> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new List<UserRoleModel>()); }
            set { _userRoles = value; }
        }

        /// <summary>
        /// 用户集合
        /// </summary>
        public IList<UserModel> Users
        {
            get { return _users ?? (_users = new List<UserModel>()); }
            set { _users = value; }
        }

        #endregion 

    }
}
