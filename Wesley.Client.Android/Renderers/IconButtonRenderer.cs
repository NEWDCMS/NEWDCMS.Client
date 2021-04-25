using Wesley.Client.CustomViews;
using Wesley.Client.Droid.Renderers;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(IconButton), typeof(IconButtonRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    /// <summary>
    /// 按钮ICON
    /// </summary>
    public class IconButtonRenderer : VisualElementRenderer<ContentView>
    {
        public IconButtonRenderer(Android.Content.Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<ContentView> e)
        {
            base.OnElementChanged(e);


            if (e.OldElement != null)
                return;

            var element = (IconButton)Element;

            //element.CornerRadius = 0;
            //if (element.IsEnabled)
            //    element.BackgroundColor = Xamarin.Forms.Color.FromHex("#eeeeee");

            element.Pressed += (object sender, EventArgs args) =>
            {
                element.OnPressed();
            };

            element.Released += (object sender, EventArgs args) =>
            {
                element.OnReleased();
            };

            element.Clicked += (object sender, EventArgs args) =>
            {
                element.OnClicked();
            };
        }

    }


}
