using DCMS.Client.Services;
using ReactiveUI;
using Shiny;
using DCMS.Logger;
using System;


namespace DCMS.Client
{
    //public class GlobalExceptionHandler : IObserver<Exception>, IShinyStartupTask
    //{
    //    readonly IDialogService dialogs;
    //    public GlobalExceptionHandler(IDialogService dialogs) => this.dialogs = dialogs;


    //    public void Start() => RxApp.DefaultExceptionHandler = this;
    //    public void OnCompleted() { }
    //    public void OnError(Exception error) { }


    //    public async void OnNext(Exception value)
    //    {
    //        CrossLogger.Instance.Error("Exception", value);
    //        await this.dialogs.DisplayAlertAsync("ERROR", value.ToString(), "取消");
    //    }
    //}
}
