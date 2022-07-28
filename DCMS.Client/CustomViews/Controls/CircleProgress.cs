using Xamarin.Forms;
namespace Wesley.Client.CustomViews
{
    /// <summary>
    /// 自定义 CircleProgress
    /// </summary>
    public class CircleProgress : BoxView
    {
        public static readonly BindableProperty BackColorProperty = BindableProperty.Create(nameof(BackColor),
            typeof(Color),
            typeof(CircleProgress),
            Color.Transparent);

        public static readonly BindableProperty ForeColorProperty = BindableProperty.Create(nameof(ForeColor),
            typeof(Color),
            typeof(CircleProgress),
            Color.Transparent);

        public static readonly BindableProperty BarHeightProperty = BindableProperty.Create(nameof(BarHeight),
            typeof(double),
            typeof(CircleProgress),
            default(double));

        public readonly BindableProperty MinimunProperty = BindableProperty.Create(nameof(Minimun),
            typeof(int),
            typeof(CircleProgress),
            default(int));

        public static readonly BindableProperty MaximunProperty = BindableProperty.Create(nameof(Maximun),
            typeof(int),
            typeof(CircleProgress),
            default(int));

        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value),
            typeof(int),
            typeof(CircleProgress),
            default(int));

        public static readonly BindableProperty AnimationDurationProperty = BindableProperty.Create(nameof(AnimationDuration),
            typeof(int),
            typeof(CircleProgress),
            default(int));

        public static readonly BindableProperty TextSizeProperty = BindableProperty.Create(nameof(TextSize),
            typeof(int),
            typeof(CircleProgress),
            default(int));

        public static readonly BindableProperty TextMarginProperty = BindableProperty.Create(nameof(TextMargin),
            typeof(int),
            typeof(CircleProgress),
            default(int));

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text),
            typeof(string),
            typeof(CircleProgress),
            string.Empty);

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor),
            typeof(Color),
            typeof(CircleProgress),
            Color.Black);

        public CircleProgress()
        {
        }

        public Color BackColor
        {
            get { return (Color)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }

        public Color ForeColor
        {
            get { return (Color)GetValue(ForeColorProperty); }
            set { SetValue(ForeColorProperty, value); }
        }

        public double BarHeight
        {
            get { return (double)GetValue(BarHeightProperty); }
            set { SetValue(BarHeightProperty, value); }
        }

        public int Minimun
        {
            get { return (int)GetValue(MinimunProperty); }
            set { SetValue(MinimunProperty, value); }
        }

        public int Maximun
        {
            get { return (int)GetValue(MaximunProperty); }
            set { SetValue(MaximunProperty, value); }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public int AnimationDuration
        {
            get { return (int)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        public int TextSize
        {
            get { return (int)GetValue(TextSizeProperty); }
            set { SetValue(TextSizeProperty, value); }
        }

        public int TextMargin
        {
            get { return (int)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        public string Text
        {
            get { return GetValue(TextProperty).ToString(); }
            set { SetValue(TextProperty, value); }
        }


        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }
    }
}
