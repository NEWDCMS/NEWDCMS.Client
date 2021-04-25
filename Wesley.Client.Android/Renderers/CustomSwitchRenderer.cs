using Android.Widget;
using Wesley.Client.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(Xamarin.Forms.Switch), typeof(CustomSwitchRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    /// <summary>
    /// 自定义Switch渲染
    /// </summary>
    public class CustomSwitchRenderer : SwitchRenderer
    {
#pragma warning disable IDE0052 // 删除未读的私有成员
        private Xamarin.Forms.Switch view;
#pragma warning restore IDE0052 // 删除未读的私有成员

        public CustomSwitchRenderer(Android.Content.Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Switch> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || e.NewElement == null)
                return;

            view = (Xamarin.Forms.Switch)Element;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean)
            {
                if (Control != null)
                {
                    this.Control.CheckedChange += this.OnCheckedChange;
                    Control.SetTrackResource(Resource.Drawable.track);
                }
            }
        }

        private void OnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Element.IsToggled = Control.Checked;
        }




        // * IOS
        //protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
        //{
        //    base.OnElementChanged(e);
        //    if (e.OldElement != null || e.NewElement == null) return;
        //    CustomSwitch s = Element as CustomSwitch;
        //    //this.Control.ThumbTintColor = s.SwitchThumbColor.ToUIColor();
        //    this.Control.OnTintColor = s.SwitchOnColor.ToUIColor();
        //}


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Control.CheckedChange -= this.OnCheckedChange;
            }
            base.Dispose(disposing);
        }
    }
}
