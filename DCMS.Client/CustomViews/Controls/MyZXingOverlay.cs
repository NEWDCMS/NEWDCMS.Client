using System;
using System.Windows.Input;
using Xamarin.Forms;
namespace DCMS.Client.CustomViews
{
    public class MyZXingOverlay : Grid
    {
        private Label topText;
        private Label botText;
        private Button flash;

        //初始位置
        private int Ypoi = -120;

        public delegate void FlashButtonClickedDelegate(Button sender, EventArgs e);
        public event FlashButtonClickedDelegate FlashButtonClicked;

        public MyZXingOverlay()
        {
            BindingContext = this;

            RowSpacing = 0;

            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;

            RowDefinitions.Add(new RowDefinition { Height = new GridLength(50, GridUnitType.Absolute) });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });


            //ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            //ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            //ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            //ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            //var boxview = new BoxView
            //{
            //    VerticalOptions = LayoutOptions.Fill,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    BackgroundColor = Color.Black,
            //    Opacity = 0.7
            //};


            //var boxview2 = new BoxView
            //{
            //    VerticalOptions = LayoutOptions.Fill,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    BackgroundColor = Color.Black,
            //    Opacity = 0.7
            //};

            //Children.Add(boxview, 0, 0);
            //Children.Add(boxview2, 0, 2);

            //SetColumnSpan(boxview, 5);
            //SetColumnSpan(boxview2, 5);


            //Children.Add(new BoxView
            //{
            //    VerticalOptions = LayoutOptions.Fill,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    BackgroundColor = Color.Black,
            //    Opacity = 0.7,
            //}, 0, 1);

            //Children.Add(new BoxView
            //{
            //    VerticalOptions = LayoutOptions.Fill,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    BackgroundColor = Color.Black,
            //    Opacity = 0.7,
            //}, 4, 1);

            //头部提示
            topText = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                AutomationId = "zxingDefaultOverlay_TopTextLabel",
            };
            topText.SetBinding(Label.TextProperty, new Binding(nameof(TopText)));
            Children.Add(topText, 0, 0);


            //扫描布局定位
            var absoluteLayouts = new AbsoluteLayout();
            var redline = new Image { Source = "saomiao.png" };
            AbsoluteLayout.SetLayoutBounds(redline, new Rectangle(1, Ypoi, 1, 1));
            AbsoluteLayout.SetLayoutFlags(redline, AbsoluteLayoutFlags.SizeProportional);
            absoluteLayouts.Children.Add(redline);

            Children.Add(absoluteLayouts, 1, 0);

            //SetColumnSpan(absoluteLayouts, 3);
            //SetColumnSpan(topText, 5);
            //botText = new Label
            //{
            //    VerticalOptions = LayoutOptions.Center,
            //    HorizontalOptions = LayoutOptions.Center,
            //    TextColor = Color.White,
            //    AutomationId = "zxingDefaultOverlay_BottomTextLabel",
            //};

            //botText.SetBinding(Label.TextProperty, new Binding(nameof(BottomText)));
            //var myStackLayout = new StackLayout
            //{
            //    VerticalOptions = LayoutOptions.Center,
            //    HorizontalOptions = LayoutOptions.Center
            //};
            //flash = new Button
            //{
            //    VerticalOptions = LayoutOptions.Center,
            //    HorizontalOptions = LayoutOptions.Center,
            //    Text = "按钮",
            //    FontSize = 14,
            //    CornerRadius = 20,
            //    TextColor = Color.White,
            //    BackgroundColor = Color.FromHex("7fadf7"),
            //    Opacity = 0.7,
            //    AutomationId = "zxingDefaultOverlay_FlashButton",
            //};
            //flash.SetBinding(Button.IsVisibleProperty, new Binding(nameof(ShowFlashButton)));
            //flash.SetBinding(Button.TextProperty, new Binding(nameof(ButtonText)));
            //flash.Clicked += (sender, e) =>
            //{
            //    FlashButtonClicked?.Invoke(flash, e);
            //};
            //myStackLayout.Children.Add(botText);
            //myStackLayout.Children.Add(flash);
            //Children.Add(myStackLayout, 0, 2);
            //SetColumnSpan(myStackLayout, 5);


            Device.StartTimer(TimeSpan.FromMilliseconds(10), () =>
            {
                Device.BeginInvokeOnMainThread(() => 
                {
                    Ypoi += 2;
                    AbsoluteLayout.SetLayoutBounds(redline, new Rectangle(1, Ypoi, 1, 1));
                    if (Ypoi > 120)
                    {
                        Ypoi = -110;
                    }
                });
                return true;
            });
        }

        public static readonly BindableProperty TopTextProperty =
            BindableProperty.Create(nameof(TopText), typeof(string), typeof(MyZXingOverlay), string.Empty);
        public string TopText
        {
            get { return (string)GetValue(TopTextProperty); }
            set { SetValue(TopTextProperty, value); }
        }

        public static readonly BindableProperty BottomTextProperty =
            BindableProperty.Create(nameof(BottomText), typeof(string), typeof(MyZXingOverlay), string.Empty);
        public string BottomText
        {
            get { return (string)GetValue(BottomTextProperty); }
            set { SetValue(BottomTextProperty, value); }
        }

        public static readonly BindableProperty ButtonTextProperty =
        BindableProperty.Create(nameof(ButtonText), typeof(string), typeof(MyZXingOverlay), string.Empty);
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }
        public static readonly BindableProperty ShowFlashButtonProperty =
            BindableProperty.Create(nameof(ShowFlashButton), typeof(bool), typeof(MyZXingOverlay), false);
        public bool ShowFlashButton
        {
            get { return (bool)GetValue(ShowFlashButtonProperty); }
            set { SetValue(ShowFlashButtonProperty, value); }
        }

        public static BindableProperty FlashCommandProperty =
            BindableProperty.Create(nameof(FlashCommand), typeof(ICommand), typeof(MyZXingOverlay),
                defaultValue: default(ICommand),
                propertyChanged: OnFlashCommandChanged);

        public ICommand FlashCommand
        {
            get { return (ICommand)GetValue(FlashCommandProperty); }
            set { SetValue(FlashCommandProperty, value); }
        }

        private static void OnFlashCommandChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            var overlay = bindable as MyZXingOverlay;
            if (overlay?.flash == null)
            {
                return;
            }
            overlay.flash.Command = newValue as Command;
        }
    }


}
