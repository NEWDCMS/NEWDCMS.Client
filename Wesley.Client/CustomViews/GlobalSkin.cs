using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
namespace Wesley.Client.CustomViews
{

    /// <summary>
    /// Enum of Annotations. Detail will be added later.
    /// </summary>
    public enum AnnotationType
    {
        /// <summary>
        /// None of check. Allows all inputs and returns IsValidated as true.
        /// </summary>
        None,
        /// <summary>
        /// Letter chars only
        /// </summary>
        LettersOnly,
        /// <summary>
        /// Digits characters only. Also that means 'Integer' numbers.
        /// </summary>
        DigitsOnly,
        /// <summary>
        /// NonDigits characters only.
        /// </summary>
        NonDigitsOnly,
        /// <summary>
        /// Standard decimal check with coma or dot (depends on culture).
        /// </summary>
        Decimal,
        /// <summary>
        /// Standard email check.
        /// </summary>
        Email,
        /// <summary>
        /// Standard password check (at least a number and at least a char). MinLength should be set seperately.
        /// </summary>
        Password,
        /// <summary>
        /// Standard phone number check. Also you should use MinLength property with this type.
        /// </summary>
        Phone,
        /// <summary>
        /// You need to set RegexPattern property as your regex query to use this.
        /// </summary>
        RegexPattern
    }

    public enum LabelPosition
    {
        After,
        Before
    }

    public class GlobalSkin : INotifyPropertyChanged// ReactiveObject
    {
        ///------------------------------------------------------------------
        /// <summary>
        /// Main color of control
        /// </summary>
        public Color Color { get; set; }
        ///------------------------------------------------------------------
        /// <summary>
        /// Background color of control
        /// </summary>
        public Color BackgroundColor { get; set; }
        ///------------------------------------------------------------------
        /// <summary>
        /// Border color of control
        /// </summary>
        public Color BorderColor { get; set; }
        ///------------------------------------------------------------------
        /// <summary>
        /// If control has a corner radius, this is it.
        /// </summary>
        public float CornerRadius { get; set; }
        ///------------------------------------------------------------------
        /// <summary>
        /// If control has fontsize, this is it.
        /// </summary>
        public double FontSize { get; set; }
        ///------------------------------------------------------------------
        /// <summary>
        /// Size of control. ( Like HeightRequest and WidthRequest )
        /// </summary>
        public double Size { get; set; }
        ///------------------------------------------------------------------
        /// <summary>
        /// Text Color of control.
        /// </summary>
        public Color TextColor { get; set; }
        ///------------------------------------------------------------------
        /// <summary>
        /// Font family of control.
        /// </summary>
        public string FontFamily { get; set; }
        /// <summary>
        /// Label position of control.
        /// </summary>
        public LabelPosition LabelPosition { get; set; }

        ///------------------------------------------------------------------
        /// <summary>
        /// INotifyPropertyChanged Implementation
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
