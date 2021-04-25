using System;
using ReactiveUI;
using Shiny;
using Wesley.Client.Services;

namespace Wesley.Client
{
    public static class ApplicationExceptions
    {
        public static string ToString(Exception exception)
        {
            return exception switch
            {
                ServerException _ => "抱歉，服务器错误",
                NetworkException _ => "抱歉，无法连接到Internet",
                _ => "抱歉，出错啦！"
            };
        }
    }
    public class ServerException : Exception{}
    public class NetworkException : Exception{}

    /// <summary>
    /// 全局异常处理
    /// </summary>
    public class GlobalExceptionHandler : IObserver<Exception>, IShinyStartupTask
    {
        readonly IDialogService _dialogs;
        public GlobalExceptionHandler(IDialogService dialogs) => this._dialogs = dialogs;
        public void Start() => RxApp.DefaultExceptionHandler = this;
        public void OnCompleted() { }
        public void OnError(Exception error) { }
        public void OnNext(Exception value)
        {
            this._dialogs.ShortAlert(value.ToString());
        }
    }
}
