using Android.Content;
using Android.Widget;
using System;

namespace Wesley.Client.Droid.Utils
{
    /// <summary>
    /// （实验性：待提交到Wesley.logger里程碑)）
    /// </summary>
    public class ToastUtils
    {
        private static Toast mToast;
        private static readonly Context context = AppUtils.GetAppContext();

        #region 非连续弹出的Toast

        public static void ShowSingleToast(int resId)
        {
            GetSingleToast(resId, ToastLength.Short).Show();
        }

        public static void ShowSingleToast(String text)
        {
            GetSingleToast(text, ToastLength.Short).Show();
        }

        public static void ShowSingleLongToast(int resId)
        {
            GetSingleToast(resId, ToastLength.Long).Show();
        }

        public static void ShowSingleLongToast(String text)
        {
            GetSingleToast(text, ToastLength.Long).Show();
        }

        #endregion

        #region  连续弹出的Toast

        public static void ShowToast(int resId)
        {
            GetToast(resId, ToastLength.Short).Show();
        }

        public static void ShowToast(String text)
        {
            GetToast(text, ToastLength.Short).Show();
        }

        public static void ShowLongToast(int resId)
        {
            GetToast(resId, ToastLength.Long).Show();
        }

        public static void ShowLongToast(String text)
        {
            GetToast(text, ToastLength.Long).Show();
        }

        #endregion


        public static Toast GetSingleToast(int resId, ToastLength duration)
        {
            //连续调用不会连续弹出，只是替换文本
            return GetSingleToast(context.Resources.GetText(resId).ToString(), duration);
        }

        public static Toast GetSingleToast(String text, ToastLength duration)
        {
            if (mToast == null)
            {
                mToast = Toast.MakeText(context, text, duration);
            }
            else
            {
                mToast.SetText(text);
            }
            return mToast;
        }

        public static Toast GetToast(int resId, ToastLength duration)
        {
            // 连续调用会连续弹出
            return GetToast(context.Resources.GetText(resId).ToString(), duration);
        }

        public static Toast GetToast(String text, ToastLength duration)
        {
            return Toast.MakeText(context, text, duration);
        }
    }
}