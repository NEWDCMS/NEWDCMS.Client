using Wesley.Effects.iOS.GestureCollectors;
using Wesley.Effects.iOS.Renderers;
using System;
using UIKit;

namespace Wesley.Effects.iOS
{
    public static class Effects
    {
        public static void Init()
        {
            CommandsPlatform.Init();
            TouchEffectPlatform.Init();
            BorderViewRenderer.Link();

            StartNotifying();
        }

        private static void StartNotifying()
        {
            try
            {
                UIKeyboard.Notifications.ObserveDidShow(OnKeyboardDidShow);
                UIKeyboard.Notifications.ObserveDidHide(OnKeyboardDidHide);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private static void OnKeyboardDidHide(object sender, UIKeyboardEventArgs e)
        {
            SoftKeyboard.Current.InvokeVisibilityChanged(false);
        }

        private static void OnKeyboardDidShow(object sender, UIKeyboardEventArgs e)
        {
            SoftKeyboard.Current.InvokeVisibilityChanged(true);
        }
    }
}



