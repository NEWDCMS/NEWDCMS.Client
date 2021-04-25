using System;
namespace Wesley.Client.Services
{
    public class SoftwareKeyboardEventArgs : EventArgs
    {
        public SoftwareKeyboardEventArgs(int keyboardheight)
        {
            KeyboardHeight = keyboardheight;
        }
        public int KeyboardHeight { get; private set; }
    }

    public interface IVirtualKeyboard
    {
        void ShowKeyboard();
        void HideKeyboard();
    }

    public interface ISoftwareKeyboardService
    {
        event EventHandler<SoftwareKeyboardEventArgs> KeyboardHeightChanged;
        bool IsKeyboardVisible { get; }
    }
}
