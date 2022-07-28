using System;
using DCMS.BitImageEditor.Helper;
using DCMS.BitImageEditor.IOS;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(IOSHapticFeedback))]
namespace DCMS.BitImageEditor.IOS
{
    class IOSHapticFeedback : IHapticFeedback
    {
        public void Excute()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                var impact = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Light);
                impact.Prepare();
                impact.ImpactOccurred();
                impact.Dispose();
            }
            else
                Vibration.Vibrate();
        }
    }
}