using DCMS.Client.CustomViews;
using DCMS.Client.Droid.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]

namespace DCMS.Client.Droid.Renderers
{
    /// <summary>
    /// 自定义编辑器
    /// </summary>
    public class CustomEditorRenderer : EditorRenderer
    {
        public CustomEditorRenderer(Android.Content.Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
                return;

            var nativeEditText = (global::Android.Widget.EditText)Control;
            nativeEditText.Background = null;

            if (e.NewElement is CustomEditor element)
                this.Control.Hint = element.Placeholder;

            // For Custom Fonts
            if (!(e.NewElement is CustomEditor editor) || string.IsNullOrEmpty(editor.FontFamily))
                return;

            //Typeface typeface = Typeface.CreateFromAsset(this.Context.Assets, editor.FontFamily);
            //Control.SetTypeface(typeface, TypefaceStyle.Normal);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == CustomEditor.PlaceholderProperty.PropertyName)
            {
                var element = this.Element as CustomEditor;
                this.Control.Hint = element?.Placeholder;
            }
        }
    }
}