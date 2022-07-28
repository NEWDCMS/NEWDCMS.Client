using System;

namespace Wesley.Client.Effects
{
    public delegate void SoftKeyboardEventHandler(SoftKeyboardEventArgs e);

    public class SoftKeyboardEventArgs : EventArgs
    {
        public SoftKeyboardEventArgs(bool isVisible)
        {
            IsVisible = isVisible;
        }

        public bool IsVisible { get; private set; }
    }
}