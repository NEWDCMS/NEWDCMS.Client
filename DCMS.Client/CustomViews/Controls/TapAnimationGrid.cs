using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.Client.CustomViews
{
    /// <summary>
    ///  自定义栅格控件的攻丝动画效果
    ///  <custom:TapAnimationGrid BackgroundColor="{DynamicResource #ffffff}"
    ///  
    ///  Command="{Binding ItemSelectedCommand}"
    ///  CommandParameter="ConversationsPage"
    ///  
    ///  Tapped="True">
    /// </summary>
    [Preserve(AllMembers = true)]
    public class TapAnimationGrid : Grid
    {
        #region Fields

        /// <summary>
        /// Gets or sets the CommandProperty, and it is a bindable property.
        /// </summary>
        public static readonly BindableProperty CommandProperty =
           BindableProperty.Create("Command", typeof(ICommand), typeof(TapAnimationGrid), null);

        /// <summary>
        /// Gets or sets the CommandParameterProperty, and it is a bindable property.
        /// </summary>
        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create("CommandParameter", typeof(object), typeof(TapAnimationGrid), null);

        /// <summary>
        /// Gets or sets the TappedProperty, and it is a bindable property.
        /// </summary>
        public static readonly BindableProperty TappedProperty =
            BindableProperty.Create("Tapped", typeof(bool), typeof(TapAnimationGrid), false, BindingMode.TwoWay,
                null, propertyChanged: OnTapped);
        private ICommand tappedCommand;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TapAnimationGrid" /> class.
        /// </summary>
        public TapAnimationGrid() => Initialize();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the command value.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the command parameter value.
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Gets or sets the tapped value.
        /// </summary>
        public bool Tapped
        {
            get => (bool)GetValue(TappedProperty);
            set => SetValue(TappedProperty, value);
        }

        /// <summary>
        /// Gets or sets the tapped command.
        /// </summary>
        public ICommand TappedCommand => tappedCommand
                ?? (tappedCommand = new Command(() =>
                {
                    if (Tapped)
                    {
                        Tapped = false;
                    }
                    else
                    {
                        Tapped = true;
                    }
                    if (Command != null)
                    {
                        Command.Execute(CommandParameter);
                    }
                }));

        #endregion

        #region Methods

        /// <summary>
        /// Invoked when control is initialized.
        /// </summary>
        private void Initialize()
        {
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = TappedCommand
            });
        }

        /// <summary>
        /// Invoked when tap on the item.
        /// </summary>
        private static async void OnTapped(BindableObject bindable, object oldValue, object newValue)
        {
            var grid = (TapAnimationGrid)bindable;
            Application.Current.Resources.TryGetValue("#f6f7f8", out var retVal);
            grid.BackgroundColor = (Color)retVal;

            // To make the selected item color changes for 100 milliseconds.
            await Task.Delay(100);
            Application.Current.Resources.TryGetValue("#ffffff", out var retValue);
            grid.BackgroundColor = (Color)retValue;
        }
        #endregion
    }
}
