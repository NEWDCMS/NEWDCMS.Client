using Android.Views.InputMethods;
using Android.Widget;
using DCMS.Client.Droid.Effects;
using DCMS.Client.Effects;
using Google.Android.Material.TextField;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Context = Android.Content.Context;
using View = Android.Views.View;


[assembly: ExportEffect(typeof(KeyboardEnableAndroidEffect), nameof(KeyboardEnableEffect))]
namespace DCMS.Client.Droid.Effects
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

                var visibilityEffect = Element.Effects.OfType<DCMS.Client.Effects.KeyboardEnableEffect>().FirstOrDefault();

                if (visibilityEffect != null)
                {
                    return;
                }

                editText.ShowSoftInputOnFocus = KeyboardEffect.GetEnableKeyboard(Element);
                editText.FocusChange -= HideMethod;

                var currentActivity = MainActivity.Instance;
                var imm = (InputMethodManager)currentActivity.GetSystemService(Context.InputMethodService);
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
                var currentActivity = MainActivity.Instance;
                //hide keyboard for current focused control.
                var imm = (InputMethodManager)currentActivity.GetSystemService(Context.InputMethodService);
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




