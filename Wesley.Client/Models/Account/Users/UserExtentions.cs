using System;
using System.Linq;

namespace Wesley.Client.Models.Users
{
    public static class UserExtentions
    {
        #region User role

        /// <summary>
        /// 是否具有有效的用户角色
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="userRoleSystemName"></param>
        /// <param name="onlyActiveUserRoles"></param>
        /// <returns>Result</returns>
        public static bool IsInUserRole(this UserModel user,
            string userRoleSystemName, bool onlyActiveUserRoles = true)
        {
            if (user == null)
            {
                return false; //throw new ArgumentNullException("user");
            }

            if (String.IsNullOrEmpty(userRoleSystemName))
            {
                return false; //throw new ArgumentNullException("userRoleSystemName");
            }

            var result = user.AvailableUserRoles
                .FirstOrDefault(cr => (!onlyActiveUserRoles || cr.Active) && (cr.SystemName == userRoleSystemName)) != null;

            return result;
        }

        /// <summary>
        /// 是否管理员
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles"</param>
        /// <returns>Result</returns>
        public static bool IsAdmin(this UserModel user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.Administrators, onlyActiveUserRoles);
        }


        /// <summary>
        /// 是否营销总经理
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles"></param>
        /// <returns>Result</returns>
        public static bool IsMarketManager(this UserModel user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.MarketManagers, onlyActiveUserRoles);
        }


        /// <summary>
        /// 是否大区总经理
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles"></param>
        /// <returns>Result</returns>
        public static bool IsRegionManager(this UserModel user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.RegionManagers, onlyActiveUserRoles);
        }



        /// <summary>
        /// 是否财务经理
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles"></param>
        /// <returns>Result</returns>
        public static bool IsFinancialManager(this UserModel user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.FinancialManagers, onlyActiveUserRoles);
        }



        /// <summary>
        /// 是否业务部经理
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles"></param>
        /// <returns>Result</returns>
        public static bool IsGuest(this UserModel user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.BusinessManagers, onlyActiveUserRoles);
        }



        /// <summary>
        /// 是否业务员
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles"></param>
        /// <returns>Result</returns>
        public static bool IsSalesman(this UserModel user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.Salesmans, onlyActiveUserRoles);
        }


        /// <summary>
        /// 是否员工
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles"></param>
        /// <returns>Result</returns>
        public static bool IsEmployee(this UserModel user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.Employees, onlyActiveUserRoles);
        }


        /// <summary>
        /// 是否经销商
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles"></param>
        /// <returns>Result</returns>
        public static bool IsDistributor(this UserModel user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.Distributors, onlyActiveUserRoles);
        }


        #endregion

    }
}
