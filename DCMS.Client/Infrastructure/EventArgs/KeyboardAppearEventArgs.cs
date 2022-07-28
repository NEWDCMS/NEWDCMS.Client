using System;
namespace Wesley.Client
{
    public class KeyboardAppearEventArgs : EventArgs
    {
        public float KeyboardSize { get; set; }

        public KeyboardAppearEventArgs()
        {
        }
    }
}
