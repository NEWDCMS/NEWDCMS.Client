using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Wesley.Client.Droid.Utils
{

    public class Contants
    {
        //public static final boolean DEBUG = true;
        // public static final String PACKAGE_NAME = "com.jiangjiesheng.keepappalive";//需要传进来
        public static string KEY_RESTART_ACTIVITY_LIST = "key_restart_activity_list";
        public static string KEY_PACKAGE_NAME = "key_package_name";//包名保存一下
        public static string KEY_IS_DEBUG = "key_is_debug";//包名保存一下
        public static string KEY_IS_RUNNING_KEEP_ALIVE = "key_is_runing_keep_alive";//标记当前是不是保活状态
        public static readonly string CONNECT_TO_WIFI = "WIFI";
        public static readonly string CONNECT_TO_MOBILE = "MOBILE";
        public static readonly string NOT_CONNECT = "NOT_CONNECT";
        public static readonly string CONNECTIVITY_ACTION = "android.net.conn.CONNECTIVITY_CHANGE";
    }

}