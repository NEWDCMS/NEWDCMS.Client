using Microsoft.Extensions.Logging;
using ReactiveUI;
//using Shiny;
using System;

namespace DCMS.Client
{
    public class GlobalExceptionHandler : IObserver<Exception>, IShinyStartupTask
    {
        private readonly ILogger logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            this.logger = logger;
        }

        public void Start() => RxApp.DefaultExceptionHandler = this;
        public void OnCompleted() { }
        public void OnError(Exception error) { }

        public void OnNext(Exception value)
        {
            this.logger.LogError(value, "Error in view caught");
        }
    }
}