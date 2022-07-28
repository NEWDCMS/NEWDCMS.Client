using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Wesley.Client.CustomViews
{

    public partial class PopProgressBarView : ContentView
    {
        public PopProgressBarView(string message)
        {
            try
            {
                InitializeComponent();

                BindingContext = new
                {
                    Message = message,
                    Icon = "check-circle"
                };

                Task.Run(async () =>
                {
                    try
                    {
                        var _globalService = App.Resolve<IGlobalService>();
                        _globalService?.GetAPPFeatures();
                        for (int i = 1; i <= 50; i++)
                        {
                            await Task.Delay(10);
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                var value = Convert.ToDouble(i / 10);
                                defaultProgressBar.Progress = value;
                            });
                        }
                        Picked?.Invoke(this, true);
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                });

                defaultProgressBar.Progress = 0;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public event EventHandler<bool> Picked;
    }
}