using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Wesley.Effects;
using Wesley.Effects.Droid;
using Google.Android.Material.TextField;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Context = Android.Content.Context;
using View = Android.Views.View;


[assembly: ExportEffect(typeof(KeyboardEnableAndroidEffect), nameof(KeyboardEnableEffect))]
namespace Wesley.Effects.Droid
{
    public class KeyboardEnableAndroidEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                EditText editText;
                # region Material Design
                if (Control is TextInputLayout inputLayout)
                {
                    editText = inputLayout.EditText;
                }
                #endregion
                else if (Control is EditText text)
                {
                    editText = text;
                }
                else
                {
                    return;
                }

                editText.ShowSoftInputOnFocus = KeyboardEffect.GetEnableKeyboard(Element);

                if (!editText.ShowSoftInputOnFocus)
                {
                    editText.FocusChange += HideMethod;
                }

                var requestFocus = KeyboardEffect.GetRequestFocus(Element);
                if (requestFocus)
                {
                    editText.RequestFocus();
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
                if (Element == null || Control == null)
                {
                    return;
                }

                EditText editText;

                #region Material Design
                if (Control is TextInputLayout inputLayout)
                {
                    editText = inputLayout.EditText;
                }
                #endregion
                else if (Control is EditText text)
                {
                    editText = text;
                }
                else
                {
                    return;
                }

                var visibilityEffect = Element.Effects.OfType<Wesley.Effects.KeyboardEnableEffect>().FirstOrDefault();

                if (visibilityEffect != null)
                {
                    return;
                }

                editText.ShowSoftInputOnFocus = KeyboardEffect.GetEnableKeyboard(Element);
                editText.FocusChange -= HideMethod;

                var imm = (InputMethodManager)Effects.Activity?.GetSystemService(Context.InputMethodService);
                imm?.ShowSoftInput(Control, ShowFlags.Implicit);
                var requestFocus = KeyboardEffect.GetRequestFocus(Element);
                if (requestFocus)
                {
                    editText.RequestFocus();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void HideMethod(object sender, View.FocusChangeEventArgs e)
        {
            try
            {
                //hide keyboard for current focused control.
                var imm = (InputMethodManager)Effects.Activity?.GetSystemService(Context.InputMethodService);
                imm?.HideSoftInputFromWindow(Control.WindowToken, HideSoftInputFlags.None);
                SoftKeyboard.Current.InvokeVisibilityChanged(!e.HasFocus);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }

}




