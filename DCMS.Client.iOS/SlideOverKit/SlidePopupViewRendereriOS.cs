using System;
using Xamarin.Forms;
using DCMS.SlideOverKit;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly: ExportRenderer (typeof(SlidePopupView), typeof(DCMS.SlideOverKit.iOS.SlidePopupViewRendereriOS))]
namespace DCMS.SlideOverKit.iOS
{
    public class SlidePopupViewRendereriOS : VisualElementRenderer<SlidePopupView>
    {
      
    }
}

