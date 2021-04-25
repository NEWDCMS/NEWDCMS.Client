using Android.Content;
//using Android.Support.Design.Widget;
//using AndroidX.AppCompat.Widget;
using Google.Android.Material.Tabs;
using Wesley.Client.Droid.Renderers;
using Wesley.Client.Pages.Archive;
using Wesley.SlideOverKit.Droid;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(ViewBillPage), typeof(ViewBillPageTabbedPageRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    public class ViewBillPageTabbedPageRenderer : MenuTabbedPageDroidRenderer //TabbedPageRenderer
    {
        public ViewBillPageTabbedPageRenderer(Context context) : base(context)
        {
            // new TabSlideOverKitDroidHandler().Init(this, context);
        }
        public override void OnViewAdded(Android.Views.View child)
        {
            base.OnViewAdded(child);
            if (child is TabLayout tabLayout)
            {
                tabLayout.TabMode = TabLayout.ModeScrollable;
            }
        }

    }
}