using Wesley.Client.Services;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;
using Newtonsoft.Json;

namespace Wesley.Client.ViewModels
{
    public class ScanBarcodePageViewModel : ViewModelBase
    {
        [Reactive] public bool IsAnalyzing { get; set; }
        [Reactive] public bool IsScanning { get; set; }

        public ICommand ScanResultCommand { get; set; }
        public MobileBarcodeScanningOptions Options { get; }

        public ScanBarcodePageViewModel(INavigationService navigationService, IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "扫一扫";
            _navigationService = navigationService;
            _dialogService = dialogService;

            this.ScanResultCommand = new Command<object>((r) => this.OnScanResult(r));
            this.Options = new MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE },
                //CameraResolutionSelector = (r) => this.CameraResolutionSelector(r),
                //DelayBetweenAnalyzingFrames = 150, // Default value: 150
                //DelayBetweenContinuousScans = 1000, // Default value: 1000
            };
        }


        private void OnScanResult(object result)
        {
            if (this.IsScanning && this.IsAnalyzing)
            {
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    if (!string.IsNullOrEmpty(result.ToString()))
                    {
                        var data = JsonConvert.DeserializeObject<ScanData>(result.ToString());
                        if (data.Type == "Login")
                        {
                            await _navigationService.NavigateAsync("ScanLoginPage", ("ScanData", data));
                        }
                        else
                        {
                            _dialogService.LongAlert(result.ToString());
                            await _navigationService.GoBackAsync();
                        }
                    }
                });
            }

            this.IsAnalyzing = false;
            this.IsScanning = false;
        }


        public override void OnAppearing()
        {
            base.OnAppearing();

            this.IsScanning = true;
            this.IsAnalyzing = true;
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            this.IsAnalyzing = false;
            this.IsScanning = false;
        }
    }


    public class ScanData
    {
        public string UUID { get; set; }
        public string Type { get; set; }
    }
}
