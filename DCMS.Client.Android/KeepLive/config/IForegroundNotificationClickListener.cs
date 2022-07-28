using Android.Content;

namespace DCMS.Client.Droid.KeepLive.config
{
    /// <summary>
    /// 前台服务通知点击事件
    /// </summary>
    public interface IForegroundNotificationClickListener
    {
        void foregroundNotificationClick(Context context, Intent intent);
    }
}