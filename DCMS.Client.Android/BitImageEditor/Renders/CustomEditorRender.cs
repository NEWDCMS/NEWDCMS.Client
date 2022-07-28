using Android.Content;
using DCMS.BitImageEditor.Controls;
using DCMS.BitImageEditor.Droid.Renders;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRender))]

namespace DCMS.BitImageEditor.Droid.Renders
{
    internal class CustomEditorRender : EditorRenderer
    {
        public CustomEditorRender(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.TextAlignment = Android.Views.TextAlignment.Center;
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
                Control.Gravity = Android.Views.GravityFlags.Center;
            }
        }
    }
}