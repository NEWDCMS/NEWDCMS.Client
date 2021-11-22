using Wesley.Client.Models;
using Wesley.Client.Pages;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace Wesley.Client.CustomViews
{
    public interface IDialogKit
    {
        Task<string> GetVerificationCodeAsync(string title, string message, Keyboard keyboard = null, string defaultValue = "", string placeHolder = "");
        Task<string> GetInputTextAsync(string title, string message, Keyboard keyboard = null, string defaultValue = "", string placeHolder = "");
        Task<Tuple<DateTime, DateTime>> GetDateTimePickerRankAsync(string title, Keyboard keyboard = null);

        Task<PopData> GetRadioButtonResultAsync(string title, string message, Func<Task<List<PopData>>> func);
        void GetMediaResultAsync(string title, string message);
        Task<bool> GetUpgradeResultAsync(string title, string message);
        Task<bool> ShowSuccessAsync(string message, bool success, bool cutdown = false);
        Task<bool> PopViewAsync(string title, string message);
        Task<bool> ShowProgressBarAsync(string message);

        /// <summary>
        /// 手写板
        /// </summary>
        /// <returns></returns>
        Task<string> GetSignaturePadAsync(string title, string message, Keyboard keyboard = null, string defaultValue = "", string placeHolder = "");
    }


    public class DialogKit : IDialogKit
    {
        public async Task<string> GetVerificationCodeAsync(string title, string message, Keyboard keyboard = null, string defaultValue = "", string placeHolder = "")
        {
            if (keyboard == null)
            {
                keyboard = Keyboard.Default;
            }

            var cts = new TaskCompletionSource<string>(TaskCreationOptions.AttachedToParent);
            try
            {
                //文本输入框视图
                var _dialogView = new PopVerificationCodeView(title, message, keyboard, defaultValue, placeHolder);
                _dialogView.FocusEntry();
                _dialogView.Picked += async (s, o) =>
                {
                    if (PopupNavigation.Instance?.PopupStack?.Count > 0)
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                    }
                    cts.TrySetResult(o);
                };

                if (_dialogView != null)
                    await PopupNavigation.Instance.PushAsync(new PopupPage { Content = _dialogView, HasSystemPadding = false });
            }
            catch (InvalidOperationException)
            {
                cts.TrySetResult("");
            }
            catch (Exception)
            {
                cts.TrySetResult("");
            }

            return await cts.Task;
        }
        /// <summary>
        /// 手写板
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSignaturePadAsync(string title, string message, Keyboard keyboard = null, string defaultValue = "", string placeHolder = "")
        {
            if (keyboard == null)
            {
                keyboard = Keyboard.Default;
            }
            var cts = new TaskCompletionSource<string>(TaskCreationOptions.AttachedToParent);
            try
            {
                //手写输入视图
                var _dialogView = new PopSignaturePadView(title, message, keyboard, defaultValue, placeHolder);
                _dialogView.Picked += async (s, o) =>
                {
                    if (PopupNavigation.Instance?.PopupStack?.Count > 0)
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                    }
                    cts.TrySetResult(o);
                };

                if (_dialogView != null)
                    await PopupNavigation.Instance.PushAsync(new PopupPage { Content = _dialogView, HasSystemPadding = false });
            }
            catch (InvalidOperationException)
            {
                cts.TrySetResult("");
            }
            catch (Exception)
            {
                cts.TrySetResult("");
            }

            return await cts.Task;
        }
        public async Task<string> GetInputTextAsync(string title, string message, Keyboard keyboard = null, string defaultValue = "", string placeHolder = "")
        {
            if (keyboard == null)
            {
                keyboard = Keyboard.Default;
            }

            var cts = new TaskCompletionSource<string>(TaskCreationOptions.AttachedToParent);
            try
            {
                //文本输入框视图
                var _dialogView = new PopInputView(title, message, keyboard, defaultValue, placeHolder);
                _dialogView.FocusEntry();
                _dialogView.Picked += async (s, o) =>
                {
                    if (PopupNavigation.Instance?.PopupStack?.Count > 0)
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                    }
                    cts.TrySetResult(o);
                };
                if (_dialogView != null)
                    await PopupNavigation.Instance?.PushAsync(new PopupPage { Content = _dialogView, HasSystemPadding = false });
            }
            catch (InvalidOperationException)
            {
                cts.TrySetResult("");
            }
            catch (Exception)
            {
                cts.TrySetResult("");
            }

            return await cts.Task;
        }
        public async Task<Tuple<DateTime, DateTime>> GetDateTimePickerRankAsync(string title, Keyboard keyboard = null)
        {
            if (keyboard == null)
            {
                keyboard = Keyboard.Default;
            }

            var cts = new TaskCompletionSource<Tuple<DateTime, DateTime>>(TaskCreationOptions.AttachedToParent);
            try
            {
                var _dialogView = new PopTimePickerView(title, keyboard);
                _dialogView.Picked += async (s, o) =>
                {
                    if (PopupNavigation.Instance?.PopupStack?.Count > 0)
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                    }
                    cts.TrySetResult(o);
                };

                if (_dialogView != null)
                    await PopupNavigation.Instance?.PushAsync(new PopupPage { Content = _dialogView, HasSystemPadding = false });
            }
            catch (InvalidOperationException)
            {
                cts.TrySetResult(null);
            }
            catch (Exception)
            {
                cts.TrySetResult(null);
            }

            return await cts.Task;
        }


        public async Task<PopData> GetRadioButtonResultAsync(string title, string message, Func<Task<List<PopData>>> func)
        {
            var tcs = new TaskCompletionSource<PopData>(TaskCreationOptions.AttachedToParent);
            try
            {
                //单选框视图 IPopupNavigation
                var _dialogView = new PopRadioButtonPage(title, message, func);
                _dialogView.Completed += async (sender, e) =>
                {
                    try
                    {
                        if (PopupNavigation.Instance?.PopupStack?.Count > 0)
                        {
                            await PopupNavigation.Instance.PopAllAsync();
                        }
                        tcs.TrySetResult(e);
                    }
                    catch (Exception ex)
                    {

                    }
                };

                if (_dialogView != null)
                    await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (InvalidOperationException)
            {
                tcs.TrySetResult(new PopData());
            }
            catch (Exception)
            {
                tcs.TrySetResult(new PopData());
            }
            return await tcs.Task;
        }


        public async void GetMediaResultAsync(string title, string message)
        {
            var _dialogView = new PopMediaSelectView(title, "");
            var page = new PopupPage { Content = _dialogView, HasSystemPadding = false };
            await PopupNavigation.Instance.PushAsync(page);
            //_dialogView.Completed += (sender, result) =>
            //{
            //    if (result != null)
            //    {
            //        //call?.Invoke(result);
            //    }
            //};
        }


        public async Task<bool> GetUpgradeResultAsync(string title, string message)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.AttachedToParent);
            try
            {
                var _dialogView = new PopUpgradeView(title, message);
                _dialogView.Completed += async (s, e) =>
                {
                    if (PopupNavigation.Instance?.PopupStack?.Count > 0)
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                    }
                    tcs.TrySetResult(e);
                };

                if (_dialogView != null)
                    await PopupNavigation.Instance?.PushAsync(new PopupPage { Content = _dialogView, HasSystemPadding = false });
            }
            catch (InvalidOperationException)
            {
                tcs.TrySetResult(false);
            }
            catch (Exception)
            {
                tcs.TrySetResult(false);
            }

            return await tcs.Task;
        }
        public async Task<bool> ShowSuccessAsync(string message, bool success, bool cutdown = false)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.AttachedToParent);
            try
            {
                var _dialogView = new PopConfirmView(message, success);
                _dialogView.Picked += async (s, e) =>
                {
                    if (PopupNavigation.Instance?.PopupStack?.Count > 0)
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                    }
                    if (!tcs.Task.IsCanceled && !tcs.Task.IsCompleted)
                    {
                        tcs.TrySetResult(e);
                    }
                };

                if (_dialogView != null)
                    await PopupNavigation.Instance?.PushAsync(new PopupPage
                    {
                        Content = _dialogView,
                        HasSystemPadding = false
                    });

                if (cutdown)
                {
                    Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(x =>
                    {
                        if (!tcs.Task.IsCanceled && !tcs.Task.IsCompleted)
                        {
                            _dialogView.Invoke();
                        }
                    });
                }
            }
            catch (InvalidOperationException)
            {
                tcs.TrySetResult(false);
            }
            catch (Exception)
            {
                tcs.TrySetResult(false);
            }

            return await tcs.Task;
        }
        public async Task<bool> PopViewAsync(string title, string message)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.AttachedToParent);
            try
            {
                var _dialogView = new PopView(title, message);
                _dialogView.Completed += async (s, e) =>
                {
                    if (PopupNavigation.Instance?.PopupStack?.Count > 0)
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                    }
                    tcs.TrySetResult(e);
                };

                if (_dialogView != null)
                    await PopupNavigation.Instance?.PushAsync(new PopupPage { Content = _dialogView, HasSystemPadding = false });
            }
            catch (InvalidOperationException)
            {
                tcs.TrySetResult(false);
            }
            catch (Exception)
            {
                tcs.TrySetResult(false);
            }

            return await tcs.Task;
        }
        public async Task<bool> ShowProgressBarAsync(string message)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.AttachedToParent);
            try
            {
                var _dialogView = new PopProgressBarView(message);
                _dialogView.Picked += async (s, e) =>
                {
                    if (PopupNavigation.Instance?.PopupStack?.Count > 0)
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                    }
                    tcs.TrySetResult(e);
                };
                //退出
                if (_dialogView != null)
                    await PopupNavigation.Instance?.PushAsync(new PopupPage { Content = _dialogView, HasSystemPadding = false });
            }
            catch (InvalidOperationException)
            {
                tcs.TrySetResult(false);
            }
            catch (Exception)
            {
                tcs.TrySetResult(false);
            }

            return await tcs.Task;
        }
    }
}
