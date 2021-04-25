using Wesley.Client.Services;
using System;
using Xamarin.Forms;

namespace Wesley.Client.CustomViews
{
    /// <summary>
    /// 自定义输入框
    /// </summary>
    public class CustomEntry : Entry
    {
        public static readonly BindableProperty PaddingProperty = BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(RoundedButton), new Thickness(0, 0, 0, 0));
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly BindableProperty FontProperty =
            BindableProperty.Create("Font", typeof(Font), typeof(CustomEntry), new Font());

        public static readonly BindableProperty XAlignProperty =
            BindableProperty.Create("XAlign", typeof(TextAlignment), typeof(CustomEntry), TextAlignment.Start);

        public static readonly BindableProperty HasBorderProperty =
            BindableProperty.Create("HasBorder", typeof(bool), typeof(CustomEntry), true);

        public static readonly BindableProperty PlaceholderTextColorProperty =
            BindableProperty.Create("PlaceholderTextColor", typeof(Color), typeof(CustomEntry), Color.Default);


        public static readonly BindableProperty ShowVirtualKeyboardOnFocusProperty =
            BindableProperty.Create("ShowVirtualKeyboardOnFocus", typeof(bool), typeof(CustomEntry), true);

        public IVirtualKeyboard VirtualKeyboardHandler { get; set; }

        public CustomEntry()
        {
            this.Focused += OnFocused;
        }

        public new bool Focus()
        {
            if (ShowVirtualKeyboardOnFocus)
                ShowKeyboard();
            else
                HideKeyboard();

            return true;
        }

        private void OnFocused(object sender, FocusEventArgs e)
        {
            if (e.IsFocused)
            {
                if (ShowVirtualKeyboardOnFocus)
                    ShowKeyboard();
                else
                    HideKeyboard();
            }
        }

        public void ShowKeyboard()
        {
            if (VirtualKeyboardHandler != null)
                VirtualKeyboardHandler.ShowKeyboard();
        }

        public void HideKeyboard()
        {
            if (VirtualKeyboardHandler != null)
                VirtualKeyboardHandler.HideKeyboard();
        }

        public bool ShowVirtualKeyboardOnFocus
        {
            get { return (bool)this.GetValue(ShowVirtualKeyboardOnFocusProperty); }
            set { this.SetValue(ShowVirtualKeyboardOnFocusProperty, value); }
        }


        public Font Font
        {
            get { return (Font)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }


        public TextAlignment XAlign
        {
            get { return (TextAlignment)GetValue(XAlignProperty); }
            set { SetValue(XAlignProperty, value); }
        }


        public bool HasBorder
        {
            get { return (bool)GetValue(HasBorderProperty); }
            set { SetValue(HasBorderProperty, value); }
        }


        public Color PlaceholderTextColor
        {
            get { return (Color)GetValue(PlaceholderTextColorProperty); }
            set { SetValue(PlaceholderTextColorProperty, value); }
        }

        public event EventHandler<EventArgs> LeftSwipe;
        public event EventHandler<EventArgs> RightSwipe;

        public void OnLeftSwipe(object sender, EventArgs e)
        {
            LeftSwipe?.Invoke(this, e);
        }

        public void OnRightSwipe(object sender, EventArgs e)
        {
            RightSwipe?.Invoke(this, e);
        }
    }
}
