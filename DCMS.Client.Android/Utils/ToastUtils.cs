using Android.Views;
using Android.Widget;
using Java.Lang;

namespace DCMS.Client.Droid.Utils
{
    /// <summary>
    /// （实验性：待提交到DCMS.logger里程碑)）
    /// </summary>
    public class ToastUtils
    {
        public static void ShowSingleToast(string text)
        {
            try
            {
                var toast = Toast.MakeText(Android.App.Application.Context, text, ToastLength.Short);
                toast.SetGravity(GravityFlags.Center, 0, 0);
                toast.Show();
            }
            catch (Exception e) { }
        }
    }
}