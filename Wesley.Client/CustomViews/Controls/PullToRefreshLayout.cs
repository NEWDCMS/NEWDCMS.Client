using System;
using System.Windows.Input;
using Xamarin.Forms;


namespace DCMS.Client.CustomViews
{
    /// <summary>
    /// 用于自定义拉取刷新布局
    /// </summary>
    public class PullToRefreshLayout : ContentView
    {

        /*
        OnGestureListener有下面的几个动作：
        按下（onDown）： 刚刚手指接触到触摸屏的那一刹那，就是触的那一下。
        抛掷（onFling）： 手指在触摸屏上迅速移动，并松开的动作。
        长按（onLongPress）： 手指按在持续一段时间，并且没有松开。
        滚动（onScroll）： 手指在触摸屏上滑动。
        按住（onShowPress）： 手指按在触摸屏上，它的时间范围在按下起效，在长按之前。
        抬起（onSingleTapUp）：手指离开触摸屏的那一刹那。
        使用OnGestureListener接口，这样需要重载OnGestureListener接口所有的方法，适合监听所有的手势，
        正如官方文档提到的“Detecing All Supported Gestures
        */

        ///// <summary>
        ///// MotionEvent 支持
        ///// </summary>
        //public event EventHandler OnDown;
        //public event EventHandler OnMove;
        //public event EventHandler OnUp;
        //public event EventHandler OnOutSide;

#pragma warning disable IDE0060 
        public void HandOnDown(object sender, bool show)
#pragma warning restore IDE0060 
        {
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    ShowWaitCommandCanExecuteChanged(show);
            //});
        }

#pragma warning disable IDE0060 
        public void HandleOnMove(object sender, bool show)
#pragma warning restore IDE0060 
        {
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    ShowWaitCommandCanExecuteChanged(show);
            //});
        }

#pragma warning disable IDE0060 
        public void HandleOnUp(object sender, bool show)
#pragma warning restore IDE0060 
        {
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    ShowWaitCommandCanExecuteChanged(show);
            //});
        }

#pragma warning disable IDE0060 
        public void HandleOutSide(object sender, bool show)
#pragma warning restore IDE0060 
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ShowWaitCommandCanExecuteChanged(show);
            });
        }

        public PullToRefreshLayout()
        {
            IsClippedToBounds = true;
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;
        }


        public static readonly BindableProperty IsRefreshingProperty =
            BindableProperty.Create(nameof(IsRefreshing), typeof(bool), typeof(PullToRefreshLayout), false);

        public bool IsRefreshing
        {
            get { return (bool)GetValue(IsRefreshingProperty); }
            set
            {
                if ((bool)GetValue(IsRefreshingProperty) == value)
                    OnPropertyChanged(nameof(IsRefreshing));

                SetValue(IsRefreshingProperty, value);
            }
        }


        public static readonly BindableProperty IsPullToRefreshEnabledProperty =
            BindableProperty.Create(nameof(IsPullToRefreshEnabled), typeof(bool), typeof(PullToRefreshLayout), true);


        public bool IsPullToRefreshEnabled
        {
            get { return (bool)GetValue(IsPullToRefreshEnabledProperty); }
            set { SetValue(IsPullToRefreshEnabledProperty, value); }
        }


        #region RefreshCommand

        public static readonly BindableProperty RefreshCommandProperty =
            BindableProperty.Create(nameof(RefreshCommand), typeof(ICommand), typeof(PullToRefreshLayout));
        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }
        public static readonly BindableProperty RefreshCommandParameterProperty =
            BindableProperty.Create(nameof(RefreshCommandParameter),
                typeof(object),
                typeof(PullToRefreshLayout),
                null,
                propertyChanged: (bindable, oldvalue, newvalue) => ((PullToRefreshLayout)bindable).RefreshCommandCanExecuteChanged(bindable, EventArgs.Empty));
        public object RefreshCommandParameter
        {
            get { return GetValue(RefreshCommandParameterProperty); }
            set { SetValue(RefreshCommandParameterProperty, value); }
        }
        void RefreshCommandCanExecuteChanged(object sender, EventArgs eventArgs)
        {
            ICommand cmd = RefreshCommand;
            if (cmd != null)
                IsEnabled = cmd.CanExecute(RefreshCommandParameter);
        }

        #endregion



        #region ShowWaitCommand

        public static readonly BindableProperty ShowWaitCommandProperty =
            BindableProperty.Create(nameof(ShowWaitCommand), typeof(ICommand), typeof(PullToRefreshLayout));
        public ICommand ShowWaitCommand
        {
            get { return (ICommand)GetValue(ShowWaitCommandProperty); }
            set { SetValue(ShowWaitCommandProperty, value); }
        }
        void ShowWaitCommandCanExecuteChanged(bool show)
        {
            ICommand cmd = ShowWaitCommand;
            if (cmd != null)
                cmd.Execute(show);
        }

        #endregion


        public static readonly BindableProperty RefreshColorProperty =
            BindableProperty.Create(nameof(RefreshColor), typeof(Color), typeof(PullToRefreshLayout), Color.Default);

        public Color RefreshColor
        {
            get { return (Color)GetValue(RefreshColorProperty); }
            set { SetValue(RefreshColorProperty, value); }
        }




        public static readonly BindableProperty RefreshBackgroundColorProperty =
            BindableProperty.Create(nameof(RefreshBackgroundColor), typeof(Color), typeof(PullToRefreshLayout), Color.Default);


        public Color RefreshBackgroundColor
        {
            get { return (Color)GetValue(RefreshBackgroundColorProperty); }
            set { SetValue(RefreshBackgroundColorProperty, value); }
        }



        public static readonly BindableProperty IsInterceptHorizontalScrollProperty =
            BindableProperty.Create(nameof(IsInterceptHorizontalScroll), typeof(bool), typeof(PullToRefreshLayout), true);


        public bool IsInterceptHorizontalScroll
        {
            get { return (bool)GetValue(IsInterceptHorizontalScrollProperty); }
            set { SetValue(IsInterceptHorizontalScrollProperty, value); }
        }


        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (Content == null)
                return new SizeRequest(new Size(100, 100));

            return base.OnMeasure(widthConstraint, heightConstraint);
        }
    }
}
