using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace DCMS.Client.Services
{

    /// <summary>
    /// 用于控制全局导航服务扩展方法
    /// </summary>
    public static class NavigationServiceEx
    {

        public static Task RemovePageAsync(this INavigationService nav, string name)
        {
            var mainPage = Application.Current.MainPage;
            if (mainPage != null)
            {
                try
                {
                    foreach (var page in mainPage.Navigation.NavigationStack)
                    {
                        if (page.GetType().ToString().IndexOf(name) >= 0)
                        {
                            mainPage.Navigation.RemovePage(page);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }

            return Task.FromResult(true);
        }

        public static bool IsCurrentPage(this INavigationService nav, string name)
        {
            bool isExist = false;
            var mainPage = Application.Current.MainPage;
            if (mainPage != null)
            {
                try
                {
                    foreach (var page in mainPage.Navigation.NavigationStack)
                    {
                        if (page.GetType().ToString().IndexOf(name) >= 0)
                        {
                            isExist = true;
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    isExist = false;
                }
            }
            return isExist;
        }

    }
}