using Acr.UserDialogs;
using Microsoft.AppCenter.Crashes;
using Prism.Ioc;
using Prism.Navigation;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Wesley.Client.Enums;


namespace Wesley.Client
{
    /// <summary>
    /// 导航扩展方法
    /// </summary>
    public static class NavigationExtension
    {


        /// <summary>
        /// 尝试导航并捕获异常
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="uri"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task TryNavigateAsync(this INavigationService navigationService, Uri uri, INavigationParameters parameters = null)
        {
            var result = await navigationService.NavigateAsync(uri, parameters, false, true);
            HandleNavigationResult(result);
        }


        /// <summary>
        /// 尝试导航到Modally并捕获异常
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="uri"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task TryNavigateModallyAsync(this INavigationService navigationService, Uri uri, INavigationParameters parameters = null, bool animated = true)
        {
            var result = await navigationService.NavigateAsync(uri, parameters, animated, true);
            HandleNavigationResult(result);
        }

        /// <summary>
        /// 尝试导航并捕获异常
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="path"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task TryNavigateAsync(this INavigationService navigationService, string path, INavigationParameters parameters = null)
        {
            var result = await navigationService.NavigateAsync(path, parameters, false, true);
            HandleNavigationResult(result);
        }

        /// <summary>
        /// 尝试导航并捕获异常
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="path"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task TryNavigateModallyAsync(this INavigationService navigationService, string path, INavigationParameters parameters = null)
        {

            //NavigateAsync(Uri uri, INavigationParameters parameters, bool? useModalNavigation, bool animated);
            var result = await navigationService.NavigateAsync(path, parameters, false, true);
            HandleNavigationResult(result);
        }

        /// <summary>
        /// 尝试导航并捕获异常
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task TryNavigateBackAsync(this INavigationService navigationService, INavigationParameters parameters = null)
        {
            var result = await navigationService.GoBackAsync(parameters);
            HandleNavigationResult(result);
        }


        /// <summary>
        /// 捕获异常
        /// </summary>
        /// <param name="navigationResult"></param>
        private static void HandleNavigationResult(INavigationResult result)
        {
            if (!result.Success)
            {
                //Exception ex = new InvalidNavigationException();
                //if (navigationResult.Exception != null)
                //    ex = navigationResult.Exception;

                switch (result.Exception)
                {
                    case NavigationException ne:
                        HandleNavigationException(ne);
                        break;
                    default:
                        break;
                }
            }
        }




        public static void HandleNavigationException(Exception ex)
        {
            Crashes.TrackError(ex, new System.Collections.Generic.Dictionary<string, string>() { { "Navigation", "SetMainPageFromException" } });

            //var layout = new StackLayout
            //{
            //    Padding = new Thickness(40)
            //};
            //layout.Children.Add(new Label
            //{
            //    Text = ex?.GetType()?.Name ?? "遇到未知错误",
            //    FontAttributes = FontAttributes.Bold,
            //    HorizontalOptions = LayoutOptions.Center
            //});

            //layout.Children.Add(new ScrollView
            //{
            //    Content = new Label
            //    {
            //        Text = $"{ex}",
            //        LineBreakMode = LineBreakMode.WordWrap
            //    }
            //});

            //PrismApplicationBase.Current.MainPage = new ContentPage
            //{
            //    Content = layout
            //};
        }


        public static NavigationParameters ToNavParams(this (string Key, object Value)[] parameters)
        {

            var navParams = new NavigationParameters();
            if (parameters != null && parameters.Any())
                navParams.AddRange(parameters);

            return navParams;
        }

        public static INavigationParameters AddRange(this INavigationParameters parameters, params (string Key, object Value)[] args)
        {
            foreach (var (Key, Value) in args)
                parameters.Add(Key, Value);

            return parameters;
        }


        public static Task Navigate(this INavigationService navigation, string uri, params (string, object)[] parameters) => navigation.Navigate(uri, parameters.ToNavParams());


        public static async Task Navigate(this INavigationService navigation, string uri, INavigationParameters parameters)
        {
            var result = await navigation.NavigateAsync(uri, parameters);
            if (!result.Success)
                Console.WriteLine("[NAV FAIL] " + result.Exception);
        }
        //===============



        public static async Task<bool> RequestAccess(this IUserDialogs dialogs, Func<Task<AccessState>> request)
        {
            var access = await request();
            return await dialogs.AlertAccess(access);
        }


        public static async Task<bool> AlertAccess(this IUserDialogs dialogs, AccessState access)
        {
            switch (access)
            {
                case AccessState.Available:
                    return true;
                case AccessState.Restricted:
                    await dialogs.AlertAsync("警告：访问受限");
                    return false;
                default:
                    await dialogs.AlertAsync("无效的访问状态: " + access);
                    return false;
            }
        }


        public static IDisposable SubOnMainThread<T>(this IObservable<T> obs, Action<T> onNext)
            => obs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext);


        public static IDisposable SubOnMainThread<T>(this IObservable<T> obs, Action<T> onNext, Action<Exception> onError)
            => obs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext, onError);


        public static IDisposable SubOnMainThread<T>(this IObservable<T> obs, Action<T> onNext, Action<Exception> onError, Action onComplete)
            => obs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext, onError, onComplete);


        public static async Task Navigate(this INavigationService nav, string uri, INavigationParameters parms, bool useModal = false)
        {
            var result = await nav.NavigateAsync(uri, parms, useModal, true);
            if (!result.Success)
            {
               
            }
        }


        public static async Task Navigate(this INavigationService nav, string uri, bool useModal = false, params (string, object)[] args)
        {
            var result = await nav.NavigateAsync(uri, ToParameters(args), useModal, true);
            if (!result.Success)
            {
               
            }
        }


        public static ICommand NavigateCommand(this INavigationService nav, string uri, Action<INavigationParameters> getArgs = null, bool useModal = false)
            => ReactiveCommand.CreateFromTask(async _ =>
            {
                var parms = new NavigationParameters();
                getArgs?.Invoke(parms);
                await nav.Navigate(uri, parms, useModal);
            });


        public static ICommand NavigateCommand<T>(this INavigationService nav, string uri, Action<T, INavigationParameters> getArgs = null, bool useModal = false)
            => ReactiveCommand.CreateFromTask<T>(async arg =>
            {

                var parms = new NavigationParameters();
                getArgs?.Invoke(arg, parms);
                await nav.Navigate(uri, parms, useModal);
            });


        public static ICommand GoBackCommand(this INavigationService nav, Action<INavigationParameters> getArgs = null)
            => ReactiveCommand.CreateFromTask(async _ =>
            {
                var parms = new NavigationParameters();
                getArgs?.Invoke(parms);
                await nav.GoBack(false, parms);
            });


        public static Task GoBack(this INavigationService nav, bool toRoot = false, params (string, object)[] args)
        {
            var parms = ToParameters(args);
            return nav.GoBack(toRoot, parms);
        }


        public static async Task GoBack(this INavigationService nav, bool toRoot = false, INavigationParameters parms = null)
        {
            if (toRoot)
            {
                await nav.GoBackToRootAsync(parms);
            }
            else
            {
                await nav.GoBackAsync(parms);
            }
        }


        public static INavigationParameters Set(this INavigationParameters parms, string key, object value)
        {
            parms.Add(key, value);
            return parms;
        }


        public static INavigationParameters ToParameters(params (string, object)[] args)
        {
            var parms = new NavigationParameters();
            if (args != null)
            {
                foreach (var arg in args)
                {
                    parms.Add(arg.Item1, arg.Item2);
                }
            }

            return parms;
        }



    }
}
