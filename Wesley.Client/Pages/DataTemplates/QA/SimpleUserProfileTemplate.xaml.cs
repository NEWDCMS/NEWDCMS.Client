using Xamarin.Forms;
namespace Wesley.Client.Pages.DataTemplates
{

    public partial class SimpleUserProfileTemplate : ContentView
    {
        #region IsInTitle
        public static readonly BindableProperty IsInTitleProperty = BindableProperty.Create(nameof(IsInTitle), typeof(bool), typeof(SimpleUserProfileTemplate), propertyChanged: (obj, old, newV) =>
        {
            var me = obj as SimpleUserProfileTemplate;
            if (newV != null && !(newV is bool)) return;
            var oldIsInTitle = (bool)old;
            var newIsInTitle = (bool)newV;
            me?.IsInTitleChanged(oldIsInTitle, newIsInTitle);
        });

        private void IsInTitleChanged(bool oldIsInTitle, bool newIsInTitle)
        {

        }

        public bool IsInTitle
        {
            get => (bool)GetValue(IsInTitleProperty);
            set => SetValue(IsInTitleProperty, value);
        }
        #endregion

        #region IsTyping
        public static readonly BindableProperty IsUserTypingProperty = BindableProperty.Create(nameof(IsUserTyping), typeof(bool), typeof(SimpleUserProfileTemplate), propertyChanged: (obj, old, newV) =>
        {
            var me = obj as SimpleUserProfileTemplate;
            if (newV != null && !(newV is bool)) return;
            var oldIsTyping = (bool)old;
            var newIsTyping = (bool)newV;

            me?.IsTypingChanged(oldIsTyping, newIsTyping);
        });

        private void IsTypingChanged(bool oldIsTyping, bool newIsTyping)
        {
        }

        public bool IsUserTyping
        {
            get => (bool)GetValue(IsUserTypingProperty);
            set => SetValue(IsUserTypingProperty, value);
        }
        #endregion


        public SimpleUserProfileTemplate()
        {
            InitializeComponent();
        }
    }
}