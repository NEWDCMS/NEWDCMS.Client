using Android.Content;
using System;

namespace DCMS.Client.Droid.KeepLive.receiver
{
    public class StopReceiver : BroadcastReceiver
    {
        /// <summary>
        /// 待操作事件
        /// </summary>
        private Action _mBlock;

        private string mActionStop = "_PACKAGE.FLAG.STOP";
        StopReceiver() { }

        public  StopReceiver(Context context) : this()
        {
            context.RegisterReceiver(this, new IntentFilter(mActionStop));
        }

        private static readonly StopReceiver _stopReceiver = new StopReceiver();
        public static StopReceiver Instance
        {
            get
            {
                return _stopReceiver;
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="block"></param>
        public void Register(Action block)
        {
            _mBlock = block;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals(mActionStop))
            {
                context.UnregisterReceiver(this);
                _mBlock?.Invoke();
            }
        }
    }
}