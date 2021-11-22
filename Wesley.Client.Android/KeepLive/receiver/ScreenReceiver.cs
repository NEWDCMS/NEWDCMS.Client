using Android.App;
using Android.Content;
using Android.OS;
using Java.Lang;

namespace DCMS.Client.Droid.KeepLive.receiver
{
    //public class ScreenReceiver : BroadcastReceiver
    //{
    //    readonly Handler mHander;
    //    public ScreenReceiver()
    //    {
    //        mHander = new Handler(Looper.MainLooper);
    //    }

    //    public override void OnReceive(Context context, Intent intent)
    //    {
    //        if (intent.Action.Equals(Intent.ActionScreenOff))
    //        {
    //            //通知屏幕已关闭，开始播放无声音乐
    //            context.SendBroadcast(new Intent("_ACTION_SCREEN_OFF"));
    //        }
    //        else if (intent.Action.Equals(Intent.ActionScreenOn))
    //        {   //屏幕打开的时候发送广播  结束一像素
    //            //screenOn = true;
    //            //通知屏幕已点亮，停止播放无声音乐
    //            context.SendBroadcast(new Intent("_ACTION_SCREEN_ON"));
    //        }
    //    }
    //}


}
