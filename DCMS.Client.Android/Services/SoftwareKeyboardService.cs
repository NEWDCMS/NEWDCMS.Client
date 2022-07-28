using DCMS.Client.Services;
using System;

namespace DCMS.Client.Droid.Services
{

    //public static class Keyboards
    //{
    //    internal static Activity Activity;
    //    public static void Init(Activity activity)
    //    {
    //        //Activity = activity;
    //        //Activity.Window.DecorView.ViewTreeObserver.AddOnGlobalLayoutListener(new SoftKeyboardService());
    //        //SoftwareKeyboardService softwarekeyboardservice = new SoftwareKeyboardService(this);
    //    }
    //}

    public class SoftwareKeyboardService : ISoftwareKeyboardService
    {
        public virtual event EventHandler<SoftwareKeyboardEventArgs> KeyboardHeightChanged;

        private readonly Android.App.Activity _activity;
        private readonly GlobalLayoutListener _globalLayoutListener;

        public bool IsKeyboardVisible { get { return _globalLayoutListener.IsKeyboardVisible; } }

        public SoftwareKeyboardService()
        {
            _activity = MainActivity.Instance;
            _globalLayoutListener = new GlobalLayoutListener(_activity, this);
            _activity.Window.DecorView.ViewTreeObserver.AddOnGlobalLayoutListener(_globalLayoutListener);
        }

        internal void InvokeKeyboardHeightChanged(SoftwareKeyboardEventArgs args)
        {
            var handler = KeyboardHeightChanged;
            handler?.Invoke(this, args);
        }
    }
}