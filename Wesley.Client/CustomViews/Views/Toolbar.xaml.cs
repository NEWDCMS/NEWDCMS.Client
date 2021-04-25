
using Wesley.Effects;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.Client.CustomViews.Views
{

    public partial class Toolbar : ContentView
    {
        public static readonly BindableProperty ShowBackButtonProperty = BindableProperty.Create(
            nameof(ShowBackButton),
            typeof(bool),
            typeof(Toolbar),
            defaultValue: false);

        public static readonly BindableProperty HasShadowProperty = BindableProperty.Create(
            nameof(HasShadow),
            typeof(bool),
            typeof(Toolbar),
            defaultValue: false,
            propertyChanged: HasShadowPropertyChanged);

        public static readonly BindableProperty ForegroundColorProperty = BindableProperty.Create(
            nameof(ForegroundColor),
            typeof(Color),
            typeof(Toolbar));

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(Toolbar),
            string.Empty);

        public static readonly BindableProperty CommandProperty =
    BindableProperty.Create(nameof(Command), typeof(Command), typeof(FloatingActionButtonView), null, BindingMode.OneWay);


        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private const int ShadowHeight = 6;


        public Toolbar()
        {
            InitializeComponent();
            TapCommandEffect.SetTap(BackButton, Command);
            UpdateShadow();
        }

        public bool HasShadow
        {
            get => (bool)GetValue(HasShadowProperty);
            set => SetValue(HasShadowProperty, value);
        }

        public bool ShowBackButton
        {
            get => (bool)GetValue(ShowBackButtonProperty);
            set => SetValue(ShowBackButtonProperty, value);
        }

        public Color ForegroundColor
        {
            get => (Color)GetValue(ForegroundColorProperty);
            set => SetValue(ForegroundColorProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        private static void HasShadowPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var toolbar = (Toolbar)bindable;
            toolbar.UpdateShadow();
        }

        private void UpdateShadow()
        {
            if (HasShadow)
            {
                ShadowRowDefinition.Height = new GridLength(ShadowHeight);
                ShadowBoxView.IsVisible = true;
                Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom - ShadowHeight);

                var boxView1 = new BoxView { BackgroundColor = BackgroundColor };
                var boxView2 = new BoxView { BackgroundColor = BackgroundColor };
                BackgroundColor = Color.Transparent;
                Grid.Children.Insert(0, boxView1);
                Grid.Children.Insert(1, boxView2);
                Grid.SetRow(boxView1, 0);
                Grid.SetColumnSpan(boxView1, 3);
                Grid.SetRow(boxView2, 1);
                Grid.SetColumnSpan(boxView2, 3);
            }
            else
            {
                ShadowRowDefinition.Height = new GridLength(0);
                ShadowBoxView.IsVisible = false;
            }
        }
    }
}