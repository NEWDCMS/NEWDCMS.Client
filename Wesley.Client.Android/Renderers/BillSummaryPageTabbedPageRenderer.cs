using Android.Content;
//using Android.Support.Design.Widget;
using Google.Android.Material.Tabs;
using Wesley.Client.Droid.Renderers;
using Wesley.Client.Pages.Archive;
using Wesley.SlideOverKit.Droid;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(BillSummaryPage), typeof(BillSummaryPageTabbedPageRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    public class BillSummaryPageTabbedPageRenderer : MenuTabbedPageDroidRenderer //TabbedPageRenderer
    {
        public BillSummaryPageTabbedPageRenderer(Context context) : base(context)
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