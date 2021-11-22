using Wesley.BitImageEditor.EditorPage;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;

namespace Wesley.BitImageEditor
{

    public enum BackgroundType
    {
        Transparent,
        Color,
        StretchedImage
    }

    public enum BBAspect
    {
        AspectFit = 0,
        AspectFill = 1,
        Fill = 2,
        Auto = 3
    }

    public sealed class ImageEditorConfig : BaseNotifier
    {
        private float? сropAspectRatio;
        public const int maxPixels = 3000;

        public const string _loadingText = "Wait";
        public const string _successSaveText = "Success";
        public const string _errorSaveText = "Error";


        public ImageEditorConfig() { }

        /// <summary></summary>
        public ImageEditorConfig(bool canAddText = true, bool canFingerPaint = true, bool canTransformMainBitmap = true, float? cropAspectRatio = null,
                                 List<SKBitmapImageSource> stickers = null, int? outImageHeight = null, int? outImageWidht = null, BBAspect aspect = BBAspect.Auto,
                                 BackgroundType backgroundType = BackgroundType.Transparent, SKColor backgroundColor = default,
                                 bool canSaveImage = true, string loadingText = _loadingText, string successSaveText = _successSaveText, string errorSaveText = _errorSaveText)
        {
            CanAddText = canAddText;
            CanFingerPaint = canFingerPaint;
            CanTransformMainBitmap = canTransformMainBitmap;
            Stickers = stickers;
            CropAspectRatio = cropAspectRatio;
            Aspect = aspect;
            BackgroundType = backgroundType;
            BackgroundColor = backgroundColor;
            CanSaveImage = canSaveImage;
            LoadingText = loadingText;
            SuccessSaveText = successSaveText;
            ErrorSaveText = errorSaveText;
            SetOutImageSize(outImageHeight, outImageWidht);
        }


        public bool CanAddText { get; set; } = true;
        public bool CanFingerPaint { get; set; } = true;
        public bool CanTransformMainBitmap { get; set; } = true;
        public bool CanSaveImage { get; set; } = true;
        public string LoadingText { get; set; } = _loadingText;
        public string SuccessSaveText { get; set; } = _successSaveText;
        public string ErrorSaveText { get; set; } = _errorSaveText;
        public float? CropAspectRatio
        {
            get => сropAspectRatio;
            set => сropAspectRatio = value <= 0 ? null : value;
        }

        /// <summary>
        /// 设置一组贴纸,不要使用大量的Stickers，这将导致大量的内存消耗,当不再需要标签时，请使用<see cref=“ImageEditorConfig.DisposeStickers”/>方法
        /// </summary>
        public List<SKBitmapImageSource> Stickers { get; set; } = null;
        public int? OutImageHeight { get; private set; }
        public int? OutImageWidht { get; private set; }
        public SKColor BackgroundColor { get; set; } = default;
        public BackgroundType BackgroundType { get; set; } = BackgroundType.StretchedImage;
        public BBAspect Aspect { get; set; } = BBAspect.Auto;
        public bool CanChangeCropAspectRatio => CropAspectRatio == null;
        public bool CanAddStickers => Stickers?.Count > 0;
        public bool IsOutImageAutoSize => OutImageHeight == null || OutImageWidht == null;
        public void SetOutImageSize(int? height = null, int? widht = null)
        {
            if (height == null || widht == null || height < 1 || widht < 1)
            {
                OutImageHeight = null;
                OutImageWidht = null;
            }
            else if (height > maxPixels || widht > maxPixels)
            {
                double outAspect = (double)widht / (double)height;
                OutImageHeight = widht > height ? (int)(maxPixels / outAspect) : maxPixels;
                OutImageWidht = widht > height ? maxPixels : (int)(maxPixels * outAspect);
            }
            else
            {
                OutImageHeight = height;
                OutImageWidht = widht;
            }
        }

        public ImageEditorConfig Clone()
        {
            return (ImageEditorConfig)this.MemberwiseClone();
        }

        public void DisposeStickers()
        {
            if (Stickers?.Count > 0)
            {
                foreach (var a in Stickers)
                {
                    a.Bitmap?.Dispose();
                    a.Bitmap = null;
                }
                Stickers.Clear();
                Stickers = null;
            }
        }

    }
}
