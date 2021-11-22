using Wesley.Client.Camera;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;

using System.IO;
using System.Drawing;

using Xamarin.CommunityToolkit.UI.Views;


namespace Wesley.Client.ViewModels
{
    public class CameraPageViewModel : ViewModelBase
    {
        [Reactive] public OverlayShape OverlayShape { get; internal set; } = OverlayShape.Rect;
        [Reactive] public CameraFlashMode CameraFlashMode { get; internal set; }

        [Reactive] public bool IsFlashToggled { get; internal set; }
        [Reactive] public bool IsCameraAvailable { get; internal set; }
        [Reactive] public bool IsCameraBusy { get; internal set; }
        [Reactive] public bool IsSnapButtonEnabled { get; internal set; }
        [Reactive] public string TakeType { get; internal set; }
        [Reactive] public int PhotoRotation { get; internal set; }

        public CameraPageViewModel(INavigationService navigationService, IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "拍照";
            this.IsSnapButtonEnabled = true;
        }

        readonly AsyncLock m_lock = new AsyncLock();
        protected readonly TimeSpan _minTapInterval = new TimeSpan(0, 0, 4);
        protected DateTime _lastTapTimestamp;

        private DelegateCommand<MediaCapturedEventArgs> _mediaCapturedCommand;
        public DelegateCommand<MediaCapturedEventArgs> MediaCapturedCommand
        {
            get
            {
                if (_mediaCapturedCommand == null)
                {
                    _mediaCapturedCommand = new DelegateCommand<MediaCapturedEventArgs>(async (e) =>
                    {
                        try
                        {
                            using (var releaser = await m_lock.LockAsync())
                            {
                                if (e != null && !string.IsNullOrEmpty(this.TakeType))
                                {
                                    await _navigationService.GoBack();

                                    var img = e.ImageData;
                                    var icp = App.Resolve<IImageCompression>();
                                    var newImg = icp?.CompressImage(img, 20);
                                    GC.Collect();
                                    MessageBus
                                    .Current
                                    .SendMessage(newImg, string.Format(Constants.CAMERA_KEY, this.TakeType));
                                }
                            }
                        }
                        catch (System.NullReferenceException ex)
                        {
                            Crashes.TrackError(ex);
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                        }
                    });
                }
                return _mediaCapturedCommand;
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                if (parameters.ContainsKey("TakeType"))
                {
                    parameters.TryGetValue("TakeType", out string taketype);
                    if (!string.IsNullOrEmpty(taketype))
                        this.TakeType = taketype;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

    }
}
