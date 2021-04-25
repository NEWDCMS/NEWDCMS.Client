using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Wesley.Client.Droid.Renderers
{
    public class StatefulStackLayoutRenderer : VisualElementRenderer<StackLayout>, Android.Views.View.IOnTouchListener
    {
        public StatefulStackLayoutRenderer(Context context) : base(context)
        {

        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            //System.Diagnostics.Debug.WriteLine("[OnTouchEvent] - " + e.Action);
            if (e.Action == MotionEventActions.Down)
            {
                VisualStateManager.GoToState(Element, "Pressed");
            }
            else if (e.Action == MotionEventActions.Up || e.Action == MotionEventActions.Cancel)
            {
                VisualStateManager.GoToState(Element, "Normal");
            }
            return base.OnTouchEvent(e);
        }

        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            return base.OnTouchEvent(e);
        }
    }
}