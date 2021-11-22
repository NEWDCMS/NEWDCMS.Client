using Android.Content;
using Wesley.Client.CustomViews;
using Wesley.Client.Droid.Renderers;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Android.Webkit.WebView;


[assembly: ExportRenderer(typeof(BrowserControl), typeof(BrowserControlRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    public class BrowserControlRenderer : WebViewRenderer
    {
        private WebView _webView;
        public BrowserControlRenderer(Context context) : base(context)
        {
        }

        private class ExtendedWebViewClient : Android.Webkit.WebViewClient
        {
            private BrowserControl _xwebView = null;
            public ExtendedWebViewClient(BrowserControl xwebView)
            {
                _xwebView = xwebView;
            }

            public override async void OnPageFinished(WebView view, string url)
            {
                try
                {
                    if (_xwebView != null)
                    {
                        int i = 10;
                        if (view != null)
                        {
                            // 在此处等待，直到呈现内容
                            while (view?.ContentHeight < i && i-- > 0)
                                await System.Threading.Tasks.Task.Delay(100);
                        }
                        _xwebView.HeightRequest = view?.ContentHeight ?? 0;

                    }
                }
                catch (Exception) { }
                base.OnPageFinished(view, url);
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);

            try
            {
                _webView = Control;
                if (e.OldElement == null)
                {
                    _webView.SetWebViewClient(new ExtendedWebViewClient(e.NewElement as BrowserControl));
                }
            }
            catch (Exception) { }
        }
    }
}