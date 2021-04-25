using Wesley.Client.CustomViews;
using Wesley.Client.ViewModels;
using Wesley.SlideOverKit;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Wesley.Client.Pages
{

    public abstract class BaseContentPage<T> : MenuContainerPage where T : ViewModelBase
    {
        public T ViewModel => (T)this.BindingContext;
        public BaseContentPage()
        {
            Padding = 0;
            this.SlideMenu = new RightSideMasterPage();
            BackgroundColor = Color.White;
            DeviceDisplay.KeepScreenOn = true;
        }
        protected override void OnAppearing()
        {
            //await ViewModel.Initialize();
            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            //await ViewModel.Stop();
            base.OnDisappearing();
        }


        public override void OnSoftBackButtonPressed()
        {
            var bindingContext = BindingContext as ViewModelBaseCutom;
            bindingContext?.OnSoftBackButtonPressed();
        }
        public override bool NeedOverrideSoftBackButton { get; set; } = false;

    }
    public abstract class BaseTabbedPage<T> : TopTabbedPage where T : ViewModelBase
    {
        public T ViewModel => (T)this.BindingContext;
        public BaseTabbedPage()
        {
            Padding = 0;
            this.SlideMenu = new RightSideMasterPage();
            BackgroundColor = Color.White;
        }

        protected override void OnAppearing()
        {
            //await ViewModel.Initialize();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            //await ViewModel.Stop();
            base.OnDisappearing();
        }
    }
}
