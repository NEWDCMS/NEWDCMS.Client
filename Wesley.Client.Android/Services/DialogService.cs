using Acr.UserDialogs;
using Android.Views;
using Android.Widget;
using Wesley.Client.Services;
using Prism.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using Java.Lang;

namespace Wesley.Client.Droid.Services
{
    /// <summary>
    /// 对话框
    /// </summary>
    public class DialogService : IDialogService
    {
        private readonly IPageDialogService _pageDialogService;

        public DialogService(IPageDialogService pageDialogService)
        {
            _pageDialogService = pageDialogService;
        }

        public async Task ShowAlertAsync(string message, string title, string buttonLabel)
        {
            await UserDialogs.Instance.AlertAsync(message, title, buttonLabel);
        }

        public void LongAlert(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    var toast = Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long);
                    toast.SetGravity(GravityFlags.Center, 0, 0);
                    toast.Show();
                }
                catch (Exception e) { }
            });
        }

        public void ShortAlert(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                { 
                var toast = Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short);
                toast.SetGravity(GravityFlags.Center, 0, 0);
                toast.Show();
                }
                catch (Exception e) { }
            });
        }

        public async Task<bool> ShowConfirmAsync(string message, string title = null, string okText = null, string cancelText = null)
        {
            return await UserDialogs.Instance.ConfirmAsync(message, title, okText, cancelText);
        }

        public async Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton)
        {
            return await _pageDialogService.DisplayAlertAsync(title, message, acceptButton, cancelButton);
        }

        public async Task DisplayAlertAsync(string title, string message, string cancelButton)
        {
            await _pageDialogService.DisplayAlertAsync(title, message, cancelButton);
        }

        public async Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            return await _pageDialogService.DisplayActionSheetAsync(title, cancelButton, destroyButton, otherButtons);
        }

        public async Task DisplayActionSheetAsync(string title, params IActionSheetButton[] buttons)
        {
            await _pageDialogService.DisplayActionSheetAsync(title, buttons);
        }
    }
}