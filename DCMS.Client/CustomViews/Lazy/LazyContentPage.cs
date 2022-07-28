using Wesley.Behaviors;
using Prism;
using Prism.AppModel;
using System;
using Xamarin.Forms;

namespace Wesley.Client.CustomViews
{
    /// <summary>
    /// 表示用于延迟加载的内容视图，继承自 ContentPage
    /// </summary>
    /// <typeparam name="TContentView">内容视图</typeparam>
    public class LazyContentPage<TContentView> : ContentPage, IActiveAware where TContentView : View
    {
        public event EventHandler IsActiveChanged;

        public LazyContentPage()
        {
            Behaviors.Add(new LazyLoadContentPageBehavior
            {
                ContentTemplate = new DataTemplate(typeof(TContentView))
            });
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    IsActiveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    /// <summary>
    /// 表示用于延迟加载的内容视图，继承自 ContentPage
    /// </summary>
    /// <typeparam name="TLoadingView">等待进入视图</typeparam>
    /// <typeparam name="TContentView">内容视图</typeparam>
    public class LazyContentPage<TLoadingView, TContentView> : ContentPage, IActiveAware where TLoadingView : View where TContentView : View
    {
        public event EventHandler IsActiveChanged;

        public LazyContentPage()
        {
            Behaviors.Add(new LazyLoadContentPageBehavior
            {
                LoadingTemplate = new DataTemplate(typeof(TLoadingView)),
                ContentTemplate = new DataTemplate(typeof(TContentView))
            });
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    IsActiveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }


    /// <summary>
    /// 表示用于延迟加载的内容视图，继承自 ContentView
    /// </summary>
    /// <typeparam name="TContentView">内容视图</typeparam>
    public class LazyContentView<TContentView> : ContentView, IActiveAware where TContentView : View
    {
        public event EventHandler IsActiveChanged;

        public LazyContentView()
        {
            Behaviors.Add(new LazyLoadContentViewBehavior
            {
                ContentTemplate = new DataTemplate(typeof(TContentView))
            });
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnActivationChanged();
                }
            }
        }

        private void OnActivationChanged()
        {
            var isActiveChanged = IsActiveChanged;
            if (isActiveChanged != null)
                isActiveChanged.Invoke(this, EventArgs.Empty);
            else if (Content is ContentView contentView && contentView.BindingContext is IPageLifecycleAware bindingContext)
                if (_isActive)
                    bindingContext.OnAppearing();
                else
                    bindingContext.OnDisappearing();
        }
    }
    /// <summary>
    /// 表示用于延迟加载的内容视图，继承自 ContentView
    /// </summary>
    /// <typeparam name="TLoadingView">等待进入视图</typeparam>
    /// <typeparam name="TContentView">内容视图</typeparam>
    public class LazyContentView<TLoadingView, TContentView> : ContentView, IActiveAware where TLoadingView : View where TContentView : View
    {
        public event EventHandler IsActiveChanged;

        public LazyContentView()
        {
            Behaviors.Add(new LazyLoadContentViewBehavior
            {
                LoadingTemplate = new DataTemplate(typeof(TLoadingView)),
                ContentTemplate = new DataTemplate(typeof(TContentView))
            });
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnActivationChanged();
                }
            }
        }

        private void OnActivationChanged()
        {
            var isActiveChanged = IsActiveChanged;
            if (isActiveChanged != null)
                isActiveChanged.Invoke(this, EventArgs.Empty);
            else if (Content is ContentView contentView && contentView.BindingContext is IPageLifecycleAware bindingContext)
                if (_isActive)
                    bindingContext.OnAppearing();
                else
                    bindingContext.OnDisappearing();
        }
    }
}

