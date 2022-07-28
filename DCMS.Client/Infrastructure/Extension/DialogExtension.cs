using DCMS.Client.Services;
//using Shiny;
using System;
using System.Threading.Tasks;

namespace DCMS.Client
{
    public static class DialogExtension
    {
        /// <summary>
        /// 访问请求
        /// </summary>
        /// <param name="dialogs"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<bool> RequestAccess(this IDialogService dialogs, Func<Task<AccessState>> request)
        {
            var access = await request();
            return dialogs.AlertAccess(access);
        }

        public static bool AlertAccess(this IDialogService dialogs, AccessState access)
        {
            switch (access)
            {
                case AccessState.Available:
                    return true;
                case AccessState.Restricted:
                    dialogs.LongAlert("警告：访问受限");
                    return true;
                default:
                    dialogs.LongAlert("无效的访问状态: " + access);
                    return false;
            }
        }
    }
}
