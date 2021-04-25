using Foundation;
using Prism;
using Prism.Ioc;
using Shiny;
using System;
using UIKit;


namespace Wesley.Client.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            this.ShinyFinishedLaunching(new Startup());

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App(new iOSInitializer()));
            SlideOverKit.iOS.SlideOverKit.Init();
            return base.FinishedLaunching(app, options);
        }

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
         => this.ShinyPerformFetch(completionHandler);

        public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, Action completionHandler)
            => this.ShinyHandleEventsForBackgroundUrl(sessionIdentifier, completionHandler);

        ////iOS: Different than Android. Must be in FinishedLaunching, not in Main.
        //// In AppDelegate
        //public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary options)
        //{
        //    AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        //    TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        //    // Rest of your code...
        //}

        ///// <summary>
        //// If there is an unhandled exception, the exception information is diplayed 
        //// on screen the next time the app is started (only in debug configuration)
        ///// </summary>
        //[Conditional("DEBUG")]
        //private static void DisplayCrashReport()
        //{
        //    const string errorFilename = "Fatal.log";
        //    var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
        //    var errorFilePath = Path.Combine(libraryPath, errorFilename);

        //    if (!File.Exists(errorFilePath))
        //    {
        //        return;
        //    }

        //    var errorText = File.ReadAllText(errorFilePath);
        //    var alertView = new UIAlertView("Crash Report", errorText, null, "Close", "Clear") { UserInteractionEnabled = true };
        //    alertView.Clicked += (sender, args) =>
        //    {
        //        if (args.ButtonIndex != 0)
        //        {
        //            File.Delete(errorFilePath);
        //        }
        //    };
        //    alertView.Show();
        //}
    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //注册任何特定于平台的实现
        }
    }



}
