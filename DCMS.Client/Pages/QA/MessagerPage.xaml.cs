using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace Wesley.Client.Pages
{

    public partial class MessagerPage : BaseContentPage<MessagerPageViewModel>
    {
        public MessagerPage()
        {
            try
            {
                InitializeComponent();
                NavigationPage.SetHasNavigationBar(this, false);
                TextInput.Focused += TextInput_Focused;
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }


        private void TextInput_Focused(object sender, FocusEventArgs e)
        {
            if (!e.IsFocused)
                VisualStateManager.GoToState(TextInput, "Unfocused");
        }

        private void IsFocusOnKeyBoardChanged(bool newIsFocusOnKeyBoard)
        {
            if (newIsFocusOnKeyBoard)
                TextInput.Focus();
            else
                TextInput.Unfocus();
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<MessagerPageViewModel, MyFocusEventArgs>(this, Constants.ShowKeyboard, (s, args) =>
                IsFocusOnKeyBoardChanged(args.IsFocused));

            MessagingCenter.Subscribe<MessagerPageViewModel, ScrollToItemEventArgs>(this, Constants.ScrollToItem, (s, eargs) =>
            {
                MessagesCollectionView.ScrollTo(eargs.Item);
            });
            MessagingCenter.Subscribe<object, KeyboardAppearEventArgs>(this, Constants.iOSKeyboardAppears, (sender, eargs) =>
            {
                if (MessagesGrid.TranslationY == 0)
                {
                    MessagesGrid.TranslationY -= eargs.KeyboardSize;
                }
            });
            MessagingCenter.Subscribe<object, string>(this, Constants.iOSKeyboardDisappears, (sender, eargs) =>
            {
                MessagesGrid.TranslationY = 0;
            });
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<MessagerPageViewModel, MyFocusEventArgs>(this, Constants.ShowKeyboard);
            MessagingCenter.Unsubscribe<MessagerPageViewModel, ScrollToItemEventArgs>(this, Constants.ScrollToItem);
            MessagingCenter.Unsubscribe<object, KeyboardAppearEventArgs>(this, Constants.iOSKeyboardAppears);
            base.OnDisappearing();
        }
    }
}
