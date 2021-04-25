using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using AndroidX.AppCompat.Widget;
using Wesley.Client.Droid.Renderers;
using Wesley.Client.Pages;
using Wesley.Client.ViewModels;
using Wesley.SlideOverKit;
using Java.Lang;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;

//[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationPageRenderer))]
//namespace Wesley.Client.Droid.Renderers
//{

//    public class CustomNavigationPageRenderer : NavigationPageRenderer, Android.Views.View.IOnClickListener
//    {
//        #region nav toolbar

//        private static readonly FieldInfo ToolbarFieldInfo;
//        private bool _disposed;
//        private Toolbar _toolbar;
//        static CustomNavigationPageRenderer()
//        {
//            ToolbarFieldInfo = typeof(NavigationPageRenderer).GetField("_toolbar", BindingFlags.NonPublic | BindingFlags.Instance);
//        }
//        public CustomNavigationPageRenderer(Context context) : base(context) { }

//        public override void OnViewAdded(Android.Views.View child)
//        {
//            base.OnViewAdded(child);

//            if (child.GetType() == typeof(Toolbar))
//            {
//                _toolbar = (Toolbar)child;
//                _toolbar.ChildViewAdded += Toolbar_ChildViewAdded;
//            }
//        }

//        private void Toolbar_ChildViewAdded(object sender, ChildViewAddedEventArgs e)
//        {
//            var view = e.Child.GetType();
//            if (view == typeof(AppCompatTextView))
//            {
//                var textView = (AppCompatTextView)e.Child;
//                textView.TextSize = 14;
//                textView.SetTypeface(null, TypefaceStyle.Bold);
//                textView.RequestLayout();
//                if (_toolbar != null)
//                {
//                    _toolbar.ChildViewAdded -= Toolbar_ChildViewAdded;
//                }
//            }
//        }

//        public new void OnClick(Android.Views.View v)
//        {
//            if (!(Element.CurrentPage is MenuContainerPage curPage))
//            {
//                Element.PopAsync();
//            }
//            else
//            {
//                if (curPage.NeedOverrideSoftBackButton)
//                    curPage.OnSoftBackButtonPressed();
//                else
//                    Element.PopAsync();
//            }
//        }

//        protected override void OnLayout(bool changed, int l, int t, int r, int b)
//        {
//            try
//            {
//                base.OnLayout(changed, l, t, r, b);
//                UpdateToolbarInstance();
//            }
//            catch (Exception)
//            {
//            }
//        }

//        protected override void OnConfigurationChanged(Configuration newConfig)
//        {
//            base.OnConfigurationChanged(newConfig);
//            UpdateToolbarInstance();
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && !_disposed)
//            {
//                _disposed = true;
//                RemoveToolbarInstance();
//            }

//            base.Dispose(disposing);
//        }

//        private void UpdateToolbarInstance()
//        {
//            RemoveToolbarInstance();
//            GetToolbarInstance();
//        }

//        private void GetToolbarInstance()
//        {
//            try
//            {
//                _toolbar = (Toolbar)ToolbarFieldInfo.GetValue(this);
//                _toolbar.SetNavigationOnClickListener(this);
//            }
//            catch (Exception exception)
//            {
//                System.Diagnostics.Debug.WriteLine($"Can't get toolbar with error: {exception.Message}");
//            }
//        }

//        private void RemoveToolbarInstance()
//        {
//            if (_toolbar == null) return;

//            _toolbar.ChildViewAdded -= Toolbar_ChildViewAdded;
//            _toolbar.SetNavigationOnClickListener(null);
//            _toolbar = null;
//        }

//        #endregion
//    }
//}