using Wesley.SlideOverKit;
using Xamarin.Forms;
namespace Wesley.Client.CustomViews
{
    public class TopTabbedPage : MenuTabbedPage
    {
        private bool _isTabPageVisible;

        public static readonly BindableProperty BarIndicatorColorProperty = BindableProperty.Create(
            nameof(BarIndicatorColor),
            typeof(Color),
            typeof(TopTabbedPage),
            Color.White,
            BindingMode.OneWay);
        public Color BarIndicatorColor
        {
            get { return (Color)GetValue(BarIndicatorColorProperty); }
            set { SetValue(BarIndicatorColorProperty, value); }
        }


        public static readonly BindableProperty SwipeEnabledColorProperty = BindableProperty.Create(
            nameof(SwipeEnabled),
            typeof(bool),
            typeof(TopTabbedPage),
            true,
            BindingMode.OneWay);
        public bool SwipeEnabled
        {
            get { return (bool)GetValue(SwipeEnabledColorProperty); }
            set { SetValue(SwipeEnabledColorProperty, value); }
        }


        public static readonly BindableProperty SelectedTabIndexProperty =
           BindableProperty.Create(
               nameof(SelectedTabIndex),
               typeof(int),
               typeof(TopTabbedPage), 0,
               BindingMode.TwoWay, null,
               propertyChanged: OnSelectedTabIndexChanged);
        public int SelectedTabIndex
        {
            get { return (int)GetValue(SelectedTabIndexProperty); }
            set { SetValue(SelectedTabIndexProperty, value); }
        }

        private static void OnSelectedTabIndexChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (((TopTabbedPage)bindable)._isTabPageVisible)
            {
                // update the Selected Child-Tab page only if Tabbed Page is visible..
                ((TopTabbedPage)bindable).CurrentPage = ((TopTabbedPage)bindable).Children[(int)newValue];
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // the tabbed page is now visible...
            _isTabPageVisible = true;

            // go ahead and update the Selected Child-Tab page..
            this.CurrentPage = this.Children[SelectedTabIndex];
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // the Tabbed Page is not visible anymore...
            _isTabPageVisible = false;
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();

            // when the user manually changes the Tab by themselves
            // we need to update it back to the ViewModel...
            SelectedTabIndex = this.Children.IndexOf(this.CurrentPage);
        }
    }
}
