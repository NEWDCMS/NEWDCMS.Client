using System;

namespace DCMS.Client.Droid.KeepLive.config
{
    /// <summary>
    /// 默认前台服务样式
    /// </summary>
    [Serializable]
    public class ForegroundNotification //: Java.IO.Serializable
    {
        private String _title;
        private String _description;
        private int? _iconRes;
        private IForegroundNotificationClickListener _foregroundNotificationClickListener;

        private ForegroundNotification() { }

        public ForegroundNotification(String title, String description, int iconRes)
        {
            this._title = title;
            this._description = description;
            this._iconRes = iconRes;
        }

        public ForegroundNotification(String title, String description, int iconRes, IForegroundNotificationClickListener foregroundNotificationClickListener) : this(title, description, iconRes)
        {
            this._foregroundNotificationClickListener = foregroundNotificationClickListener;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static ForegroundNotification ini()
        {
            return new ForegroundNotification();
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public ForegroundNotification title(String? title)
        {
            this._title = title;
            return this;
        }

        /// <summary>
        /// 设置副标题
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public ForegroundNotification description(String? description)
        {
            this._description = description;
            return this;
        }

        /// <summary>
        /// 设置图标
        /// </summary>
        /// <param name="iconRes"></param>
        /// <returns></returns>
        public ForegroundNotification icon(int? iconRes)
        {
            this._iconRes = iconRes;
            return this;
        }

        /// <summary>
        /// 设置前台通知点击事件
        /// </summary>
        /// <param name="foregroundNotificationClickListener"></param>
        /// <returns></returns>
        public ForegroundNotification foregroundNotificationClickListener(IForegroundNotificationClickListener? foregroundNotificationClickListener)
        {
            this._foregroundNotificationClickListener = foregroundNotificationClickListener;
            return this;
        }

        public String getTitle()
        {
            return _title == null ? "" : _title;
        }

        public String getDescription()
        {
            return _description == null ? "" : _description;
        }

        public int getIconRes()
        {
            return _iconRes ?? 0;
        }

        public IForegroundNotificationClickListener getForegroundNotificationClickListener()
        {
            return _foregroundNotificationClickListener;
        }
    }
}