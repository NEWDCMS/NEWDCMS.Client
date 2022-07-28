using DCMS.Client.Camera;
using DCMS.Client.Services;
using Newtonsoft.Json;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;

namespace DCMS.Client.ViewModels
{
    public class ScanBarcodePageViewModel : ViewModelBase
    {
        [Reactive] public bool IsAnalyzing { get; set; }
        [Reactive] public bool IsScanning { get; set; }
        [Reactive] public OverlayShape OverlayShape { get; internal set; } = OverlayShape.Rect;


        public ICommand ScanResultCommand { get; set; }
        public MobileBarcodeScanningOptions Options { get; }

        public ScanBarcodePageViewModel(INavigationService navigationService, IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "扫一扫";
            _navigationService = navigationService;
            _dialogService = dialogService;

            this.ScanResultCommand = new Command<object>((r) => this.OnScanResult(r));
            this.Options = new MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<BarcodeFormat>
                {
                    BarcodeFormat.CODE_39 ,
                    BarcodeFormat.CODE_93,
                    BarcodeFormat.CODE_128,
                    BarcodeFormat.EAN_8,
                    BarcodeFormat.EAN_13,
                    BarcodeFormat.QR_CODE,
                    BarcodeFormat.IMB
                },
                //CameraResolutionSelector = (r) => this.CameraResolutionSelector(r),
                //DelayBetweenAnalyzingFrames = 150,
                //DelayBetweenContinuousScans = 1000,
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
                        try
                        {
                            var data = JsonConvert.DeserializeObject<ScanData>(result.ToString());
                            if (data.Type == "Login")
                            {
                                await _navigationService.NavigateAsync("ScanLoginPage", ("ScanData", data));
                            }
                            else if (data.Type == "Redeem")
                            {
                                await _navigationService.GoBackAsync(("Data", result.ToString()));
                            }
                            else
                            {
                                _dialogService.LongAlert(result.ToString());
                                await _navigationService.GoBackAsync();
                            }
                        }
                        catch (Exception)
                        {
                            var ok = await _dialogService.ShowConfirmAsync("无法识别扫描信息！", okText: "", cancelText: "确定");
                            if (!ok)
                            {
                                await _navigationService.GoBackAsync();
                            }
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
