using Android.Content;
using Wesley.Client.Droid.Renderers;
//using Android.Support.Design.Widget;
using Wesley.Client.Pages.Market;
using Google.Android.Material.Tabs;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;


[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    public class CustomTabbedPageRenderer : TabbedPageRenderer
    {
        public CustomTabbedPageRenderer(Context context) : base(context)
        {
        }
        public override void OnViewAdded(Android.Views.View child)
        {
            try
            {
                base.OnViewAdded(child);

                if (child is TabLayout tabLayout)
                {
                    var element = (TabbedPage)Element;
                    if (!(element is DeliveryReceiptPage))
                    {
                        tabLayout.TabMode = TabLayout.ModeScrollable;
                    }
                }
            }
            catch(Java.Lang.Exception ex)
            { }
        }
    }
}