using System.Collections.Generic;

namespace Wesley.Client.Models.Users
{
    public partial class UserRoleListModel : EntityBase
    {

        // //("角色名", "角色名
        //
        public string Name { get; set; }

        // //("启用", "启用
        public bool Active { get; set; }

        ////("是否系统角色", "是否系统角色
        public bool IsSystemRole { get; set; }

        ////("系统名", "系统名
        public string SystemName { get; set; }

        //
        public IList<UserRoleModel> UserRoleItems { get; set; }

        public SelectList Stores { get; set; }
    }




    /// <summary>
    /// 用户角色
    /// </summary>
    public partial class UserRoleModel : EntityBase
    {
        private IList<PermissionRecordRoles> _permissionRecords;
        //private IList<ModuleModel> _modules;
        private IList<UserGroupModel> _userGroups;

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// 是否系统角色
        /// </summary>
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        #region 

        ///// <summary>
        ///// 获取/设置模块
        ///// </summary>
        //public IList<ModuleModel> Modules
        //{
        //    get { return _modules ?? (_modules = new List<ModuleModel>()); }
        //    set { _modules = value; }
        //}

        /// <summary>
        /// 获取/设置权限
        /// </summary>
        public IList<PermissionRecordRoles> PermissionRecords
        {
            get { return _permissionRecords ?? (_permissionRecords = new List<PermissionRecordRoles>()); }
            set { _permissionRecords = value; }
        }


        /// <summary>
        /// 用户组
        /// </summary>
        public IList<UserGroupModel> UserGroups
        {
            get { return _userGroups ?? (_userGroups = new List<UserGroupModel>()); }
            set { _userGroups = value; }
        }


        #endregion
    }


    public partial class UserRoleQuery
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}