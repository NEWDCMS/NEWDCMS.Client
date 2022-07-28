using Android.Content;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using DCMS.Client.CustomViews;
using DCMS.Client.Droid.Renderers;
using DCMS.Client.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace DCMS.Client.Droid.Renderers
{
    /// <summary>
    /// 自定义EditText
    /// </summary>
    public class CustomEntryRenderer : EntryRenderer, IVirtualKeyboard
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
        }

        private const int MIN_DISTANCE = 10;
        private float _downX, _downY, _upX, _upY;


        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if ((e.OldElement == null) && (Control != null))
            {
                var customEntry = (CustomEntry)e.NewElement;

                EditText edittext = (EditText)Control;

                edittext.Background = null;

                edittext.SetPadding(10, 0, 0, 0);
                edittext.SetPadding((int)customEntry.Padding.Left, (int)customEntry.Padding.Top, (int)customEntry.Padding.Right, (int)customEntry.Padding.Bottom);

                edittext.SetTextIsSelectable(true);
                edittext.SetSelectAllOnFocus(true);
                edittext.ShowSoftInputOnFocus = false;

                var view = (CustomEntry)Element;

                SetFont(view);
                SetTextAlignment(view);

                //SetBorder(view);
                SetPlaceholderTextColor(view);
                SetMaxLength(view);

                view.VirtualKeyboardHandler = this;
            }
        }

        public void ShowKeyboard()
        {
            try
            {
                if (Control != null)
                {
                    Control.RequestFocus();
                    InputMethodManager inputMethodManager = Control.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
                    inputMethodManager.ShowSoftInput(Control, ShowFlags.Forced);
                    inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
                }
            }
            catch (Exception) { }
        }

        public void HideKeyboard()
        {
            try
            {
                if (Control != null)
                {
                    Control.RequestFocus();
                    InputMethodManager inputMethodManager = Control.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
                    inputMethodManager.HideSoftInputFromWindow(this.Control.WindowToken, HideSoftInputFlags.None);
                }
            }
            catch (Exception) { }
        }


        /// <summary>
        /// 处理touch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleTouch(object sender, global::Android.Views.View.TouchEventArgs e)
        {
            try
            {
                if (Control != null)
                {
                    var element = this.Element as CustomEntry;
                    switch (e.Event.Action)
                    {
                        case MotionEventActions.Down:
                            _downX = e.Event.GetX();
                            _downY = e.Event.GetY();
                            return;
                        case MotionEventActions.Up:
                        case MotionEventActions.Cancel:
                        case MotionEventActions.Move:
                            _upX = e.Event.GetX();
                            _upY = e.Event.GetY();

                            float deltaX = _downX - _upX;
                            float deltaY = _downY - _upY;

                            // swipe horizontal?
                            if (Math.Abs(deltaX) > Math.Abs(deltaY))
                            {
                                if (Math.Abs(deltaX) > MIN_DISTANCE)
                                {
                                    // left or right
                                    if (deltaX < 0) { element.OnRightSwipe(this, EventArgs.Empty); return; }
                                    if (deltaX > 0) { element.OnLeftSwipe(this, EventArgs.Empty); return; }
                                }
                                else
                                {
                                    global::Android.Util.Log.Info("ExtendedEntry", "Horizontal Swipe was only " + Math.Abs(deltaX) + " long, need at least " + MIN_DISTANCE);
                                    return;
                                }
                            }
                            //                    else 
                            //                    {
                            //                        if(Math.abs(deltaY) > MIN_DISTANCE){
                            //                            // top or down
                            //                            if(deltaY < 0) { this.onDownSwipe(); return true; }
                            //                            if(deltaY > 0) { this.onUpSwipe(); return true; }
                            //                        }
                            //                        else {
                            //                            Log.i(logTag, "Vertical Swipe was only " + Math.abs(deltaX) + " long, need at least " + MIN_DISTANCE);
                            //                            return false; // We don't consume the event
                            //                        }
                            //                    }

                            return;
                    }
                }
            }
            catch (Exception) { }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var view = (CustomEntry)Element;

            if (e.PropertyName == CustomEntry.FontProperty.PropertyName)
                SetFont(view);
            if (e.PropertyName == CustomEntry.XAlignProperty.PropertyName)
                SetTextAlignment(view);
            //if (e.PropertyName == CustomEntry.HasBorderProperty.PropertyName)
            //    SetBorder(view);
            if (e.PropertyName == CustomEntry.TextColorProperty.PropertyName)
                SetTextColor(view);
            if (e.PropertyName == CustomEntry.PlaceholderTextColorProperty.PropertyName)
                SetPlaceholderTextColor(view);
            if (e.PropertyName == CustomEntry.MaxLengthProperty.PropertyName)
                SetMaxLength(view);
        }

        private void SetTextAlignment(CustomEntry view)
        {
            switch (view.XAlign)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    Control.Gravity = GravityFlags.CenterHorizontal;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    Control.Gravity = GravityFlags.End;
                    break;
                case Xamarin.Forms.TextAlignment.Start:
                    Control.Gravity = GravityFlags.Start;
                    break;
            }
        }

        private void SetFont(CustomEntry view)
        {
            if (view.Font != Font.Default && Control != null)
            {
                Control.TextSize = view.Font.ToScaledPixel();
                Control.Typeface = view.Font.ToTypeface();
            }
        }


        private void SetPlaceholderTextColor(CustomEntry view)
        {
            if (view.PlaceholderTextColor != Xamarin.Forms.Color.Default && Control != null)
                Control.SetHintTextColor(view.PlaceholderTextColor.ToAndroid());
        }


        private void SetTextColor(CustomEntry view)
        {
            if (view.TextColor != Xamarin.Forms.Color.Default && Control != null)
                Control.SetTextColor(view.TextColor.ToAndroid());
        }

        private void SetMaxLength(CustomEntry view)
        {
            if (Control != null)
                Control.SetFilters(new IInputFilter[] { new global::Android.Text.InputFilterLengthFilter(view.MaxLength) });
        }
    }

}