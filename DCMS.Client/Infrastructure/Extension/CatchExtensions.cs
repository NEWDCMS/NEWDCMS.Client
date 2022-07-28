using Acr.UserDialogs;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace Wesley.Client
{
    public static class CatchExtensions
    {
        public class TryUnit<T> where T : class
        {
            public TryUnit(T obj, Action<T> action)
            {
                this.Obj = obj;
                this.Action = action;
            }
            public T Obj { get; private set; }
            public Action<T> Action { get; private set; }
        }

        public static TryUnit<T> Try<T>(this T obj, Action<T> action) where T : class
        {
            return new TryUnit<T>(obj, action);
        }

        public class CatchUnit<T> where T : class
        {
            public CatchUnit(TryUnit<T> tryUnit, Action<Exception> exAction)
            {
                this.Obj = tryUnit.Obj;
                this.Action = tryUnit.Action;
                this.ExAction = exAction;
            }
            public T Obj { get; private set; }
            public Action<T> Action { get; private set; }
            public Action<Exception> ExAction { get; private set; }
        }

        public static CatchUnit<T> Catch<T>(this TryUnit<T> tryUnit, Action<Exception> exAction) where T : class
        {
            return new CatchUnit<T>(tryUnit, exAction);
        }

        public static void Finally<T>(this TryUnit<T> tryUnit) where T : class
        {
            try
            {
                tryUnit.Action(tryUnit.Obj);
            }
            finally
            {

            }
        }

        public static void Finally<T>(this CatchUnit<T> catchUnit) where T : class
        {
            try
            {
                catchUnit.Action(catchUnit.Obj);
            }
            catch (Exception e)
            {
                catchUnit.ExAction(e);
            }
            finally
            {

            }
        }

        public static void Finally<T>(this TryUnit<T> tryUnit, Action<T> action) where T : class
        {
            try
            {
                tryUnit.Action(tryUnit.Obj);
            }
            finally
            {
                action(tryUnit.Obj);
            }
        }

        public static void Finally<T>(this CatchUnit<T> catchUnit, Action<T> action) where T : class
        {
            try
            {
                catchUnit.Action(catchUnit.Obj);
            }
            catch (Exception e)
            {
                catchUnit.ExAction(e);
            }
            finally
            {
                action(catchUnit.Obj);
            }
        }
    }

    public static class AsyncErrorHandler
    {
        private static bool IsAuthenticated = true;

        public static void HandleException(this Exception ex)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (ex is OperationCanceledException)
                {
                    //var message = "操作已取消";
                    //using (UserDialogs.Instance.Alert(message)) { }
                }
                else if (ex is TaskCanceledException)
                {
                    //var message = "操作已取消";
                    //using (UserDialogs.Instance.Alert(message)) { }
                }
                else if (ex is Refit.ApiException s)
                {
                    if (IsAuthenticated && s.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        IsAuthenticated = false;
                        var ok = await UserDialogs.Instance.ConfirmAsync("账户失效", "账户会话已经过期，请重新登录！", okText: "确定", cancelText: "");
                        if (ok)
                        {
                            var nav = App.Resolve<INavigationService>();
                            await nav?.NavigateAsync("LoginPage");
                        }
                    }
                }
                else
                {
                    Crashes.TrackError(ex);
                }
            });
        }
    }
}
