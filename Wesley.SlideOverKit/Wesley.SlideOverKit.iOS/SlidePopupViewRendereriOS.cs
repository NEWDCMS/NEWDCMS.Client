using System;
using Xamarin.Forms;
using Wesley.SlideOverKit;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly: ExportRenderer (typeof(SlidePopupView), typeof(Wesley.SlideOverKit.iOS.SlidePopupViewRendereriOS))]
namespace Wesley.SlideOverKit.iOS
{
    public class SlidePopupViewRendereriOS : VisualElementRenderer<SlidePopupView>
    {
      
    }
}

