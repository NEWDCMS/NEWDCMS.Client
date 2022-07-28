using Wesley.BitImageEditor.Resources;
using Wesley.Client.Effects;
using SkiaSharp;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Wesley.BitImageEditor.EditorPage
{
    /// <summary>
    /// 图片编辑视图页
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public sealed partial class ImageEditorPage : ContentPage
    {
        private readonly ImageEditorViewModel viewModel;
        internal ImageEditorPage(SKBitmap bitmap, ImageEditorConfig config)
        {
            try
            {
                InitializeComponent();

                viewModel = new ImageEditorViewModel(bitmap, config);
                this.BindingContext = viewModel;

                //裁切画布
                canvasCropViewHost.Children.Add(viewModel.cropperCanvas, 0, 0);

                //主图预览画布
                canvasMainViewHost.Children.Add(viewModel.mainCanvas, 0, 0);

                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImageEditorViewModel.TextVisible))
            {
                if (viewModel?.TextVisible ?? false)
                    textEditor.Focus();
            }
            if (e.PropertyName == nameof(ImageEditorViewModel.CurrentTextIsFill))
            {
                var imageName = (viewModel?.CurrentTextIsFill ?? false) ? "text_fill" : "text_not_fill";
                typeTextButton.Source = ImageSource.FromResource($"{ImageResourceExtension.resource}{imageName}.png");
            }
        }

        protected override void OnDisappearing()
        {
            ImageEditor.Instance.SetImage();
            base.OnDisappearing();
        }

        private void TouchEffect_TouchAction(object sender, TouchActionEventArgs args) => viewModel.OnTouchEffectTouchAction(sender, args);

        internal void Dispose()
        {
            viewModel.Dispose();
        }

    }
}