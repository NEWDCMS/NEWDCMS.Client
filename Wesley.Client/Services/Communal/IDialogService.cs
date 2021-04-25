using Prism.Services;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string title, string buttonLabel);
        void LongAlert(string message);
        void ShortAlert(string message);
        Task<bool> ShowConfirmAsync(string message, string title = null, string okText = null, string cancelText = null);
        Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton);
        Task DisplayAlertAsync(string title, string message, string cancelButton);
        Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons);
        Task DisplayActionSheetAsync(string title, params IActionSheetButton[] buttons);


    }
}
