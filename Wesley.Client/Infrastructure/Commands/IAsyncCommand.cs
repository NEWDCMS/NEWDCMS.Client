using Wesley.Infrastructure.Tasks;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wesley.Client.Commands
{

    public interface IAsyncCommand : ICommand
    {
        INotifyTask Execution { get; }

        Task ExecuteAsync(object parameter);

        void RaiseCanExecuteChanged();
    }

    public interface IAsyncCommand<TResult> : ICommand
    {
        NotifyTask<TResult> Execution { get; }
    }
}