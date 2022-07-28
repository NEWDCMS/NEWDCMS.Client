using Wesley.BitImageEditor.Croping;
using Wesley.BitImageEditor.Helper;
using Wesley.BitImageEditor.ManipulationBitmap;
using Wesley.Client.Effects;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.BitImageEditor.EditorPage
{
    internal class ImageEditorViewModel : BaseNotifier, IDisposable
    {
        private TouchManipulationBitmap currentTextBitmap = null;

        //裁切画布
        internal ImageCropperCanvasView cropperCanvas;
        //主图预览画布
        internal TouchManipulationCanvasView mainCanvas;

        internal ImageEditorViewModel(SKBitmap bitmap, ImageEditorConfig config)
        {
            Config = config;

            cropperCanvas = new ImageCropperCanvasView(bitmap, config.CropAspectRatio);
            cropperCanvas.SetAspectRatio(new CropItem("crop_square", CropRotateType.CropSquare));

            mainCanvas = new TouchManipulationCanvasView(config);
            mainCanvas.AddBitmapToCanvas(bitmap.Copy(), BitmapType.Main);
            mainCanvas.TextBitmapClicked += MainCanvas_TextBitmapClicked;
            mainCanvas.TrashEnabled += MainCanvas_TrashVisebled;
            ColorCollect = SkiaHelper.GetColors();
            CropCollect = CropItem.GetCropItems(config.CanChangeCropAspectRatio);
            Message = config?.LoadingText;
            MessagingCenter.Subscribe<Application>(this, "BBDroidBackButton", (sender) => OnBackPressed());
        }


        public bool CropVisible => CurrentEditType == ImageEditType.CropRotate;
        public bool MainVisible => !CropVisible;
        public bool TextVisible => CurrentEditType == ImageEditType.Text;
        public bool StickersVisible => CurrentEditType == ImageEditType.Stickers;
        public bool PaintVisible => CurrentEditType == ImageEditType.Paint && !IsMoved;
        public bool InfoVisible => CurrentEditType == ImageEditType.Info;
        public bool ButtonsVisible => CurrentEditType == ImageEditType.SelectType && !IsMoved;
        public bool TrashVisible { get; private set; }
        public bool TrashBigVisible { get; private set; }
        public bool IsMoved { get; private set; }


        public ImageEditorConfig Config { get; private set; }
        public ImageEditType CurrentEditType { private set; get; } = ImageEditType.SelectType;
        public Color CurrentColor { get; set; } = Color.White;
        public string CurrentText { set; get; } = "";
        public bool CurrentTextIsFill { set; get; } = false;
        public string Message { private set; get; } = "";
        public ObservableCollection<Color> ColorCollect { get; private set; }
        public ObservableCollection<CropItem> CropCollect { get; private set; }

        /// <summary>
        /// 应用更改
        /// </summary>
        public ICommand ApplyChangesCommand => new Command<string>((value) =>
        {
            if (cropperCanvas != null && !string.IsNullOrWhiteSpace(value) && value.ToLower() == "apply")
            {
                switch (CurrentEditType)
                {
                    //添加文字
                    case ImageEditType.Text:
                        {
                            if (currentTextBitmap == null)
                                mainCanvas.AddBitmapToCanvas(CurrentText, CurrentColor.ToSKColor(), CurrentTextIsFill);
                            else
                            {
                                currentTextBitmap.Bitmap?.Dispose();
                                currentTextBitmap.Bitmap = null;
                                currentTextBitmap.Bitmap = SKBitmapBuilder.FromText(CurrentText, CurrentColor.ToSKColor(), CurrentTextIsFill);
                                currentTextBitmap.Text = CurrentText;
                                currentTextBitmap.IsHide = false;
                                currentTextBitmap.Color = CurrentColor.ToSKColor();
                                mainCanvas?.InvalidateSurface();
                            }

                            currentTextBitmap = null;
                            CurrentText = "";
                        }
                        break;
                    //裁切
                    case ImageEditType.CropRotate:
                        mainCanvas.AddBitmapToCanvas(cropperCanvas.CroppedBitmap, BitmapType.Main);
                        break;
                }
            }
            CurrentEditType = ImageEditType.SelectType;
        });

        public ICommand CancelCommand => new Command(() =>
        {
            if (CurrentEditType == ImageEditType.Paint)
                mainCanvas.DeleteEndPath();
        });

        /// <summary>
        /// 选择缩放比例
        /// </summary>
        public ICommand SelectItemCommand => new Command<object>((valueObj) =>
        {
            if (cropperCanvas != null)
            {
                switch (valueObj)
                {
                    case ImageEditType value:
                        CurrentEditType = value;
                        break;
                    case Color value:
                        CurrentColor = value;
                        break;
                    case CropItem value:
                        {
                            cropperCanvas.SetAspectRatio(value);
                        }
                        break;
                    case SKBitmapImageSource value:
                        mainCanvas.AddBitmapToCanvas(value, BitmapType.Stickers);
                        CurrentEditType = ImageEditType.SelectType;
                        break;
                    default:
                        //ImageEditType.CropRotate
                        CurrentEditType = ImageEditType.SelectType;
                        break;
                }
            }
        });

        private readonly bool lockFinish = false;

        /// <summary>
        /// 编辑完成
        /// </summary>
        public ICommand EditFinishCommand => new Command<string>((value) =>
        {
            if (!lockFinish)
            {
                EditFinish(!string.IsNullOrWhiteSpace(value) && value.ToLower() == "save");
            }
        });

        /// <summary>
        /// 更改文本
        /// </summary>
        public ICommand ChangeTextTypeCommand => new Command<string>((value) =>
        {
            CurrentTextIsFill = !CurrentTextIsFill;
        });

        /// <summary>
        /// 保存
        /// </summary>
        public ICommand SaveCommand => new Command<string>(async (value) =>
        {
            CurrentEditType = ImageEditType.Info;

            var bitmap = mainCanvas.EditedBitmap;

            if (await ImageEditor.Instance.SaveImage(SkiaHelper.SKBitmapToBytes(bitmap), $"img{DateTime.Now:dd.MM.yyyy HH-mm-ss}.png"))
                Message = Config?.SuccessSaveText;
            else
                Message = Config?.ErrorSaveText;
            bitmap.Dispose();
            bitmap = null;
            GC.Collect();

            int time = (int)(Message?.Length * 75);
            await Task.Delay(time >= 1500 ? time : 1500);
            Message = Config?.LoadingText;
            CurrentEditType = ImageEditType.SelectType;
        });

        /// <summary>
        /// 编辑完成时是否保存
        /// </summary>
        /// <param name="isSave"></param>
        private void EditFinish(bool isSave)
        {
            if (!lockFinish)
            {
                if (mainCanvas != null)
                {
                    //System.NullReferenceException: Object reference not set to an instance of an objec
                    if (mainCanvas.EditedBitmap != null)
                        ImageEditor.Instance?.SetImage(isSave ? mainCanvas.EditedBitmap : null);
                }
            }
        }

        private void OnBackPressed()
        {
            if (CurrentEditType != ImageEditType.SelectType)
                CurrentEditType = ImageEditType.SelectType;
            else
                EditFinish(false);
        }

        internal void OnTouchEffectTouchAction(object sender, TouchActionEventArgs args)
        {
            if (cropperCanvas != null)
            {
                IsMoved = Device.RuntimePlatform != Device.UWP && (args.Type == TouchActionType.Moved);
                if (CurrentEditType != ImageEditType.CropRotate)
                    mainCanvas?.OnTouchEffectTouchAction(args, CurrentEditType, CurrentColor.ToSKColor());
                else
                    cropperCanvas?.OnTouchEffectTouchAction(args);
            }
        }

        private void MainCanvas_TextBitmapClicked(TouchManipulationBitmap value)
        {
            CurrentColor = value?.Color.ToFormsColor() ?? Color.Black;
            CurrentText = value?.Text ?? "";
            CurrentEditType = ImageEditType.Text;
            currentTextBitmap = value;
        }

        private void MainCanvas_TrashVisebled(bool arg1, bool arg2, bool arg3)
        {
            if (CurrentEditType == ImageEditType.SelectType)
            {
                TrashVisible = arg1;
                TrashBigVisible = arg2;
                if (arg3)
                    HapticFeedback.Excute();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    MessagingCenter.Unsubscribe<Application>(this, "BBDroidBackButton");
                    Config = null;
                    ColorCollect?.Clear();
                    ColorCollect = null;
                    CurrentText = null;
                }

                if (CropCollect?.Count > 0)
                    for (int i = 0; i < CropCollect.Count; i++)
                        CropCollect[i] = null;

                CropCollect?.Clear();
                CropCollect = null;

                cropperCanvas?.Dispose();
                mainCanvas?.Dispose();
                currentTextBitmap?.Bitmap?.Dispose();
                if (currentTextBitmap != null)
                    currentTextBitmap.Bitmap = null;
                mainCanvas = null;
                cropperCanvas = null;
                currentTextBitmap = null;
                disposedValue = true;
            }
        }

        ~ImageEditorViewModel()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
