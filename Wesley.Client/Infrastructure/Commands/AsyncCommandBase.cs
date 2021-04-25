
using System;
using System.Threading.Tasks;

namespace Wesley.Client.Commands
{
    public abstract class AsyncCommandBase
    {
        public abstract bool CanExecute(object parameter = null);

        public abstract Task ExecuteAsync(object parameter);

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}