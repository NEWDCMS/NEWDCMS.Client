using Wesley.BitImageEditor.Controls;
using Wesley.BitImageEditor.IOS.Renders;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRender))]
namespace Wesley.BitImageEditor.IOS.Renders
{
    internal class CustomEditorRender : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.TextAlignment = UITextAlignment.Center;
            }
        }
    }
}