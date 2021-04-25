using Android.Content;
using Wesley.Client.CustomViews;
using Wesley.Client.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Android.Webkit.WebView;


[assembly: ExportRenderer(typeof(BrowserControl), typeof(BrowserControlRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    public class BrowserControlRenderer : WebViewRenderer
    {
        WebView _webView;
        public BrowserControlRenderer(Context context) : base(context)
        {
        }

        class ExtendedWebViewClient : Android.Webkit.WebViewClient
        {
            BrowserControl _xwebView = null;
            public ExtendedWebViewClient(BrowserControl xwebView)
            {
                _xwebView = xwebView;
            }

            public override async void OnPageFinished(WebView view, string url)
            {
                if (_xwebView != null)
                {
                    int i = 10;
                    // wait here till content is rendered
                    while (view.ContentHeight < i && i-- > 0)
                        await System.Threading.Tasks.Task.Delay(100);
                    _xwebView.HeightRequest = view.ContentHeight;
                }
                base.OnPageFinished(view, url);
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);
            _webView = Control;

            if (e.OldElement == null)
            {
                _webView.SetWebViewClient(new ExtendedWebViewClient(e.NewElement as BrowserControl));
            }
        }
    }
}