using Wesley.Client.CustomViews;
using Wesley.Client.ViewModels;
using ReactiveUI;
using Rg.Plugins.Popup.Pages;
using System.Reactive.Disposables;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Wesley.Client.Pages
{
    public interface IBaseContainerPage
    {
        public void OnSoftBackButtonPressed() { }
    }

    /// <summary>
    /// 自定义ContentPage基类泛型支持
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseContentPage<T> :  ContentPage, IBaseContainerPage where T : ViewModelBase
    {
        public T ViewModel => (T)this.BindingContext;
        public CompositeDisposable? disposer;

        public BaseContentPage()
        {
            Padding = 0;
            BackgroundColor = Color.White;
            DeviceDisplay.KeepScreenOn = true;
            this.disposer ??= new CompositeDisposable();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.disposer == null)
                this.disposer = new CompositeDisposable();
        }
        protected override void OnDisappearing()
        {
            this.disposer?.Dispose();
            this.disposer = null;

            base.OnDisappearing();
        }

        public virtual void OnSoftBackButtonPressed()
        {
            var bindingContext = BindingContext as ViewModelBase;
            bindingContext?.OnSoftBackButtonPressed(NeedOverrideSoftBackButton);
        }
        public virtual bool NeedOverrideSoftBackButton { get; set; } = false;

    }

    /// <summary>
    /// 自定义TabbedPage基类
    /// </summary>
    public abstract class BaseTabbedPage : TopTabbedPage, IBaseContainerPage
    {
        private CompositeDisposable? disposer;
        public BaseTabbedPage()
        {
            Padding = 0;
            BackgroundColor = Color.White;
            this.disposer ??= new CompositeDisposable();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (this.disposer == null)
                this.disposer = new CompositeDisposable();
        }

        protected override void OnDisappearing()
        {
            this.disposer?.Dispose();
            this.disposer = null;
            base.OnDisappearing();
        }

        public virtual void OnSoftBackButtonPressed()
        {
            var bindingContext = BindingContext as ViewModelBase;
            bindingContext?.OnSoftBackButtonPressed(NeedOverrideSoftBackButton);
        }
        public virtual bool NeedOverrideSoftBackButton { get; set; } = false;
    }

    /// <summary>
    /// 自定义TabbedPage基类泛型支持
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseTabbedPage<T> : TopTabbedPage, IBaseContainerPage where T : ViewModelBase
    {
        private CompositeDisposable? disposer;
        public T ViewModel => (T)this.BindingContext;
        public BaseTabbedPage()
        {
            Padding = 0;
            BackgroundColor = Color.White;
            this.disposer ??= new CompositeDisposable();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (this.disposer == null)
                this.disposer = new CompositeDisposable();
        }

        protected override void OnDisappearing()
        {
            this.disposer?.Dispose();
            this.disposer = null;
            base.OnDisappearing();

        }

        public virtual void OnSoftBackButtonPressed()
        {
            var bindingContext = BindingContext as ViewModelBase;
            bindingContext?.OnSoftBackButtonPressed(NeedOverrideSoftBackButton);
        }
        public virtual bool NeedOverrideSoftBackButton { get; set; } = false;
    }

    /// <summary>
    /// 自定义PopupPage窗体基类
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public abstract class BasePopupPage<TViewModel> : PopupPage, IViewFor<TViewModel> where TViewModel : class
    {
        protected readonly CompositeDisposable SubscriptionDisposables = new CompositeDisposable();
  
        public static readonly BindableProperty ViewModelProperty =
            BindableProperty.Create(nameof(ViewModel),
                typeof(TViewModel),
                typeof(IViewFor<TViewModel>),
                (object)null,
                BindingMode.OneWay,
                (BindableProperty.ValidateValueDelegate)null,
                new BindableProperty.BindingPropertyChangedDelegate(OnViewModelChanged),
                (BindableProperty.BindingPropertyChangingDelegate)null,
                (BindableProperty.CoerceValueDelegate)null,
                (BindableProperty.CreateDefaultValueDelegate)null);

        /// <summary>
        /// 要显示的ViewModel
        /// </summary>
        public TViewModel ViewModel
        {
            get => (TViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, (object)value);
        }

        /// <summary>
        /// 绑定上下文已更改时
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            ViewModel = BindingContext as TViewModel;
        }

        /// <summary>
        /// ViewModel更改时
        /// </summary>
        /// <param name="bindableObject"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        private static void OnViewModelChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (newValue != null)
                bindableObject.BindingContext = newValue;
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }
    }
}
