using Xamarin.Forms;
namespace Wesley.Client.CustomViews
{
    public static class HtmlConstants
    {
        /// <summary>
        /// {0} - Your html content
        /// </summary>
        public const string DefaultHtml =
         @"<!DOCTYPE html>
			<html lang=""en"">
			<head>
			    <meta charset=""UTF-8"">
				<meta name=""viewport"" content=""width=device-width, initial-scale=1"">
				<style type=""text/css"">
					html {{ -webkit-touch-callout: none; -webkit-user-select: none; -khtml-user-select: none; -moz-user-select: none; -ms-user-select: none; user-select: none;}}
					body {{ margin: 0; padding:10px; font-size: 14px; font-family: Arial, Helvetica, sans-serif;color:#78919f;}}
					img {{ max-width: 100% }}
                    .news-title{{ text-align: center; font-weight: bold;}}
				</style>
			</head>
			<body>
			 <div style=""line-height:22px;color:#78919f;"">{0} </div>
             <div style=""text-align:center;font-size:10px;color:#eeeeee;margin:10,0,10,0""><font style=""vertical-align: inherit;"">©COPYRIGHT 华润雪花晋陕区域公司</font></div></body></html>";

        public const string InvokeSharpCodeActionName = "invokeAction";
        public const string InvokeSharpFunctionPattern = "function invokeAction(data) {{ {0}(data); }}";
    }


    public class BrowserControl : WebView
    {

    }
}
