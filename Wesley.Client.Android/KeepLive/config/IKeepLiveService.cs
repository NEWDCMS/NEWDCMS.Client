namespace DCMS.Client.Droid.KeepLive.config
{
    /// <summary>
    /// 需要保活的服务
    /// </summary>
    public interface IKeepLiveService
    {
        /**
         * 运行中
         * 由于服务可能会多次自动启动，该方法可能重复调用
         */
        void onWorking();

        /**
         * 服务终止
         * 由于服务可能会被多次终止，该方法可能重复调用，需同onWorking配套使用，如注册和注销
         */
        void onStop();
    }

    /// <summary>
    /// 前后台切换回调
    /// </summary>
    public interface ICactusBackgroundCallback
    {
        void onBackground(bool background);
    }
}