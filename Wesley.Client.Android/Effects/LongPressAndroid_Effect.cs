using Wesley.Effects.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Wesley.Effects;

[assembly: ExportEffect(typeof(LongPressAndroid_Effect), nameof(LongPressEffect))]
namespace Wesley.Effects.Droid
{
    /// <summary>
    /// 长按事件响应
    /// </summary>
    public class LongPressAndroid_Effect : PlatformEffect
    {
        private bool _attached;
        public static void Initialize() { }
        protected override void OnAttached()
        {
            if (!_attached)
            {
                //var clickPressed = DateTime.Now;
                //if (!MainActivity.setLongClickTime.HasValue || clickPressed.Subtract(MainActivity.setLongClickTime.Value).Seconds > 5)
                //{
                //MainActivity.setLongClickTime = clickPressed;
                if (Control != null)
                {
                    Control.LongClickable = true;
                    Control.LongClick += Control_LongClick;
                    //Control.SetBackgroundColor(Color.FromHex("#81c14b").ToAndroid());
                }
                else
                {
                    Container.LongClickable = true;
                    Container.LongClick += Control_LongClick;
                }
                //}

            }
        }

        private void Control_LongClick(object sender, Android.Views.View.LongClickEventArgs e)
        {
            var command = Wesley.Effects.LongPressEffect.GetCommand(Element);
            command?.Execute(Wesley.Effects.LongPressEffect.GetCommandParameter(Element));
        }



        protected override void OnDetached()
        {
            if (_attached)
            {
                if (Control != null)
                {
                    Control.LongClickable = false;
                    Control.LongClick -= Control_LongClick;
                }
                else
                {
                    Container.LongClickable = false;
                    Container.LongClick -= Control_LongClick;
                }
                _attached = false;
            }
        }
    }
}