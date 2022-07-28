using Android.Content;
using DCMS.Client.Droid.Renderers;
using DCMS.Client.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;
using DCMS.Client.CustomViews;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationPageRenderer))]
namespace DCMS.Client.Droid.Renderers
{
    public class CustomNavigationPageRenderer : NavigationPageRenderer, Android.Views.View.IOnClickListener
    {
        public CustomNavigationPageRenderer(Context context) : base(context) { }
        public override void OnViewAdded(Android.Views.View child)
        {
            base.OnViewAdded(child);
        }

        public new void OnClick(Android.Views.View v)
        {
            if (Element.CurrentPage is TopTabbedPage curTabPage)
            {
                Element.PopAsync();
            }
            else if (Element.CurrentPage is IBaseContainerPage curPage)
            {
                curPage.OnSoftBackButtonPressed();
            }
            else 
            {
                Element.PopAsync();
            }
        }
    }
}