using Wesley.Client.CustomViews.Views;
using Wesley.Client.Models;
using Wesley.Client.Pages;
using Microsoft.AppCenter.Crashes;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
        Task<List<PopData>> GetCheckboxResultAsync(string title, string message, Func<Task<List<PopData>>> func);
        Task<PopData> GetRadioButtonResultAsync(string title, string message, Func<Task<List<PopData>>> func);
        Task<object> GetMediaResultAsync(string title, string message);
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
        public Task<string> GetVerificationCodeAsync(string title, string message, Keyboard keyboard = null, string defaultValue = "", string placeHolder = "")
        {
            if (keyboard == null)
            {
                keyboard = Keyboard.Default;
            }

            var cts = new TaskCompletionSource<string>();
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

                PopupNavigation.Instance.PushAsync(new PopupPage { Content = _dialogView });
            }
            catch (InvalidOperationException ex)
            {
                Crashes.TrackError(ex);
                cts.TrySetResult("");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                cts.TrySetResult("");
            }

            return cts.Task;
        }
        /// <summary>
        /// 手写板
        /// </summary>
        /// <returns></returns>
        public Task<string> GetSignaturePadAsync(string title, string message, Keyboard keyboard = null, string defaultValue = "", string placeHolder = "")
        {
            if (keyboard == null)
            {
                keyboard = Keyboard.Default;
            }
            var cts = new TaskCompletionSource<string>();
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
                PopupNavigation.Instance.PushAsync(new PopupPage { Content = _dialogView });
            }
            catch (InvalidOperationException ex)
            {
                Crashes.TrackError(ex);
                cts.TrySetResult("");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                cts.TrySetResult("");
            }

            return cts.Task;
        }

        public Task<string> GetInputTextAsync(string title, string message, Keyboard keyboard = null, string defaultValue = "", string placeHolder = "")
        {
            if (keyboard == null)
            {
                keyboard = Keyboard.Default;
            }

            var cts = new TaskCompletionSource<string>();
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

                PopupNavigation.Instance?.PushAsync(new PopupPage { Content = _dialogView });
            }
            catch (InvalidOperationException ex)
            {
                Crashes.TrackError(ex);
                cts.TrySetResult("");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                cts.TrySetResult("");
            }

            return cts.Task;
        }


        public Task<Tuple<DateTime, DateTime>> GetDateTimePickerRankAsync(string title, Keyboard keyboard = null)
        {
            if (keyboard == null)
            {
                keyboard = Keyboard.Default;
            }

            var cts = new TaskCompletionSource<Tuple<DateTime, DateTime>>();
            try
            {
                var _dialogView = new PopTimePickerView(title, keyboard);
                //_dialogView.FocusEntry();
                _dialogView.Picked += async (s, o) =>
                {
                    if (PopupNavigation.Instance?.PopupStack?.Count > 0)
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                    }
                    cts.TrySetResult(o);
                };
                PopupNavigation.Instance?.PushAsync(new PopupPage { Content = _dialogView });
            }
            catch (InvalidOperationException ex)
            {
                Crashes.TrackError(ex);
                cts.TrySetResult(null);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                cts.TrySetResult(null);
            }

            return cts.Task;
        }


        public Task<List<PopData>> GetCheckboxResultAsync(string title, string message, Func<Task<List<PopData>>> func)
        {
            var tcs = new TaskCompletionSource<List<PopData>>();
            try
            {
                //复选框视图
                var _dialogView = new PopCheckBoxView(title, message, func);
                _dialogView.Completed += async (s, e) =>
                {
                    if (PopupNavigation.Instance?.PopupStack?.Count > 0)
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                    }
                    tcs.TrySetResult(e?.ToList());
                };
                PopupNavigation.Instance.PushAsync(_dialogView, true);
            }
            catch (InvalidOperationException ex)
            {
                Crashes.TrackError(ex);
                tcs.TrySetResult(null);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                tcs.TrySetResult(null);
            }

            return tcs.Task;
        }


        public Task<PopData> GetRadioButtonResultAsync(string title, string message, Func<Task<List<PopData>>> func)
        {
            var tcs = new TaskCompletionSource<PopData>();
            try
            {
                //单选框视图
                var _dialogView = new PopRadioButtonView(title, message, func);
                _dialogView.Completed += async (sender, e) =>
                {
                    if (PopupNavigation.Instance?.PopupStack?.Count > 0)
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                    }
                    tcs.TrySetResult(e);
                };
                PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (InvalidOperationException ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string> { { "Exception1:", "InvalidOperationException" } });
                tcs.TrySetResult(null);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string> { { "Exception2:", "InvalidOperationException" } });
                tcs.TrySetResult(null);
            }

            return tcs.Task;
        }

        public Task<object> GetMediaResultAsync(string title, string message)
        {
            var tcs = new TaskCompletionSource<object>();
            try
            {
                //单选框视图
                var _dialogView = new PopMediaSelectView(title, message);
                _dialogView.Completed += async (s, e) =>
                {
                    if (PopupNavigation.Instance?.PopupStack?.Count > 0)
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                    }
                    tcs.TrySetResult(_dialogView.SelectecItem);
                };
                PopupNavigation.Instance?.PushAsync(new PopupPage { Content = _dialogView });
            }
            catch (InvalidOperationException ex)
            {
                Crashes.TrackError(ex);
                tcs.TrySetResult(null);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                tcs.TrySetResult(null);
            }
            return tcs.Task;
        }


        public Task<bool> GetUpgradeResultAsync(string title, string message)
        {
            var tcs = new TaskCompletionSource<bool>();
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

                PopupNavigation.Instance?.PushAsync(new PopupPage { Content = _dialogView });
            }
            catch (InvalidOperationException ex)
            {
                Crashes.TrackError(ex);
                tcs.TrySetResult(false);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                tcs.TrySetResult(false);
            }

            return tcs.Task;
        }
        public Task<bool> ShowSuccessAsync(string message, bool success, bool cutdown = false)
        {
            var tcs = new TaskCompletionSource<bool>();
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
                PopupNavigation.Instance?.PushAsync(new PopupPage { Content = _dialogView });
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
            catch (InvalidOperationException ex)
            {
                Crashes.TrackError(ex);
                tcs.TrySetResult(false);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                tcs.TrySetResult(false);
            }

            return tcs.Task;
        }
        public Task<bool> PopViewAsync(string title, string message)
        {
            var tcs = new TaskCompletionSource<bool>();
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
                PopupNavigation.Instance?.PushAsync(new PopupPage { Content = _dialogView });
            }
            catch (InvalidOperationException ex)
            {
                Crashes.TrackError(ex);
                tcs.TrySetResult(false);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                tcs.TrySetResult(false);
            }

            return tcs.Task;
        }
        public Task<bool> ShowProgressBarAsync(string message)
        {
            var tcs = new TaskCompletionSource<bool>();
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
                PopupNavigation.Instance?.PushAsync(new PopupPage { Content = _dialogView });
            }
            catch (InvalidOperationException ex)
            {
                Crashes.TrackError(ex);
                tcs.TrySetResult(false);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                tcs.TrySetResult(false);
            }

            return tcs.Task;
        }
    }
}
