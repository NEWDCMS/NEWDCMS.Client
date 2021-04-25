using Wesley.Client.Services;
using System;
using Android.App;
using Wesley.Client.Droid.Utils;


using Wesley.Client.Droid.Services;
[assembly: Xamarin.Forms.Dependency(typeof(SoftwareKeyboardService))]
namespace Wesley.Client.Droid.Services
{

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