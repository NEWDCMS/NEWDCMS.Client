using Android.App;
using Android.Runtime;
using Wesley.Client.Droid.Utils;
using Microsoft.AppCenter.Crashes;
using Shiny;
using System;
using System.Threading.Tasks;


namespace Wesley.Client.Droid
{

    [Application(LargeHeap = true)]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            base.OnCreate();

            //百度地图初始
            FormsBaiduMaps.Init(this);

            //AppUtils
            AppUtils.Init(this);

            //Crash
            //CrashHandler.GetInstance().Init();

            this.ShinyOnCreate(new Startup());

        }


        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            var newExc = new System.Exception("TaskSchedulerOnUnobservedTaskException", e.Exception);
            Crashes.TrackError(newExc);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var newExc = new System.Exception("CurrentDomainOnUnhandledException", e.ExceptionObject as System.Exception);
            Crashes.TrackError(newExc);
        }

        private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            var newExc = new System.Exception("UnhandledExceptionRaiser", e.Exception);
            Crashes.TrackError(newExc);
        }

        protected override void Dispose(bool disposing)
        {
            //注销异常处理
            AndroidEnvironment.UnhandledExceptionRaiser -= AndroidEnvironment_UnhandledExceptionRaiser;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
            //释放MQ
            //RabbitMQManage.DisposeBus();
            base.Dispose(disposing);
        }

    }
}