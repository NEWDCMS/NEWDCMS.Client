using Android.Content;
using Android.OS;
using Android.Net;
using Java.Util;
using System;
using static Android.Content.PM.PackageManager;

namespace DCMS.Client.Droid
{
    public class KeepAppLifeManager
    {

        /// <summary>
        /// 创建时初始连接
        /// </summary>
        public static readonly string ACTION_CREATE_CIM_CONNECTION = "ACTION_CREATE_CIM_CONNECTION";

        /// <summary>
        /// 退出应用时销毁服务
        /// </summary>
        public static readonly string ACTION_DESTROY_CIM_SERVICE = "ACTION_DESTROY_CIM_SERVICE";

        /// <summary>
        /// 推送服务
        /// </summary>
        public static readonly string ACTION_ACTIVATE_PUSH_SERVICE = "ACTION_ACTIVATE_PUSH_SERVICE";

        /// <summary>
        /// 发送请求体
        /// </summary>
        public static readonly string ACTION_SEND_REQUEST_BODY = "ACTION_SEND_REQUEST_BODY";

        /// <summary>
        /// 关闭连接
        /// </summary>
        public static readonly string ACTION_CLOSE_CIM_CONNECTION = "ACTION_CLOSE_CIM_CONNECTION";

        /// <summary>
        /// 调试时设置日志
        /// </summary>
        public static readonly string ACTION_SET_LOGGER_EATABLE = "ACTION_SET_LOGGER_EATABLE";

        /// <summary>
        /// 显示持久通知
        /// </summary>
        public static readonly string ACTION_SHOW_PERSIST_NOTIFICATION = "ACTION_SHOW_PERSIST_NOTIFICATION";

        /// <summary>
        /// 隐藏持久通知
        /// </summary>
        public static readonly string ACTION_HIDE_PERSIST_NOTIFICATION = "ACTION_HIDE_PERSIST_NOTIFICATION";

        /// <summary>
        /// PONG操作
        /// </summary>
        public static readonly string ACTION_CIM_CONNECTION_PONG = "ACTION_CIM_CONNECTION_PONG";


        public static void KeepApp(Context context)
        {
            //建立一个类型为KeepAppLifeService的Intent
            Intent serviceIntent = new Intent(context, typeof(KeepAppLifeService));
            //serviceIntent.SetAction(ACTION_SHOW_PERSIST_NOTIFICATION);
            //启动KeepAppLifeService 服务
            StartService(context, serviceIntent);
        }

        /// <summary>
        /// 开始服务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        public static void StartService(Context context, Intent intent)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                context.StartForegroundService(intent);
            }
            else
            {
                context.StartService(intent);
            }
        }


        /// <summary>
        /// 获取设备ID
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string GetDeviceId(Context context)
        {
            string deviceId = UUID.RandomUUID().ToString().Replace("-", "").ToLower();
            return deviceId;
        }


        /// <summary>
        /// 开始前台服务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="icon"></param>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        public static void StartForeground(Context context, int icon, string channel, string message)
        {
            //建立一个类型为KeepAppLifeService的Intent
            Intent serviceIntent = new Intent(context, typeof(KeepAppLifeService));
            //serviceIntent.PutExtra(KeepAppLifeService.KEY_NOTIFICATION_MESSAGE, message);
            //serviceIntent.PutExtra(KeepAppLifeService.KEY_NOTIFICATION_CHANNEL, channel);
            //serviceIntent.PutExtra(KeepAppLifeService.KEY_NOTIFICATION_ICON, icon);
            //启动持久通知
            serviceIntent.SetAction(ACTION_SHOW_PERSIST_NOTIFICATION);
            //启动KeepAppLifeService 服务
            StartService(context, serviceIntent);
        }

        /// <summary>
        /// 取消前台服务
        /// </summary>
        /// <param name="context"></param>
        public static void CancelForeground(Context context)
        { 
            //建立一个类型为KeepAppLifeService的Intent
            Intent serviceIntent = new Intent(context, typeof(KeepAppLifeService));
            //取消持久通知
            serviceIntent.SetAction(ACTION_HIDE_PERSIST_NOTIFICATION);
            //启动KeepAppLifeService 服务
            StartService(context, serviceIntent);
        }


        /// <summary>
        /// 停止接受推送，将会退出当前账号登录，端口与服务端的连接
        /// </summary>
        /// <param name="context"></param>
        public static void Stop(Context context)
        {
            //建立一个类型为KeepAppLifeService的Intent
            Intent serviceIntent = new Intent(context, typeof(KeepAppLifeService));
            //关闭连接
            serviceIntent.SetAction(ACTION_CLOSE_CIM_CONNECTION);
            //启动KeepAppLifeService 服务
            StartService(context, serviceIntent);
        }

        /// <summary>
        /// 完全销毁，一般用于完全退出程序，调用resume将不能恢复
        /// </summary>
        /// <param name="context"></param>
        public static void Destroy(Context context)
        {
            //建立一个类型为KeepAppLifeService的Intent
            Intent serviceIntent = new Intent(context, typeof(KeepAppLifeService));
            //销毁服务
            serviceIntent.SetAction(ACTION_DESTROY_CIM_SERVICE);
            //启动KeepAppLifeService 服务
            StartService(context, serviceIntent);
        }
    }
}