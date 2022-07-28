using System;
using UIKit;
using DCMS.Client.Effects.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("DCMS.Client.Effects")]
[assembly: ExportEffect(typeof(KeyboardEnableEffect), nameof(KeyboardEnableEffect))]
namespace DCMS.Client.Effects.iOS
{
    [Foundation.Preserve(AllMembers = true)]
    public class KeyboardEnableEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                if (!(Control is UITextField nativeTextField) || KeyboardEffect.GetEnableKeyboard(Element))
                {
                    return;
                }

                nativeTextField.InputView = new UIView();

                nativeTextField.InputAssistantItem.LeadingBarButtonGroups = null;
                nativeTextField.InputAssistantItem.TrailingBarButtonGroups = null;

                var requestFocus = KeyboardEffect.GetRequestFocus(Element);
                if (requestFocus)
                {
                    nativeTextField.BecomeFirstResponder();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        protected override void OnDetached()
        {
            try
            {
                if (!(Control is UITextField nativeTextField))
                {
                    return;
                }

                nativeTextField.InputView = null;
                var requestFocus = KeyboardEffect.GetRequestFocus(Element);
                if (requestFocus)
                {
                    nativeTextField.BecomeFirstResponder();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}