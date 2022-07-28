using System;
using Xamarin.Forms;
namespace Wesley.Client.Commands
{
    public class TapCommand : Command
    {
        private DateTime _lastExecution = DateTime.MinValue;

        public TapCommand(Action<object> executeMethod)
            : base(executeMethod)
        {
        }

        public TapCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
            : base(executeMethod, canExecuteMethod)
        {
        }

        protected new void Execute(object parameter)
        {
            // 防止多次触击（点击提交）
            if (DateTime.Now - _lastExecution < TimeSpan.FromMilliseconds(1000))
            {
                return;
            }

            _lastExecution = DateTime.Now;
            base.Execute(parameter);
        }
    }
}