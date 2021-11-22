using Android.Content;
using Wesley.Client.Droid.Renderers;
using Wesley.Client.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;
using Wesley.Client.CustomViews;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationPageRenderer))]
namespace Wesley.Client.Droid.Renderers
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