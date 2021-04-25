using Wesley.Client.Services;

using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{

    public class AgreementPageViewModel : ViewModelBase
    {
        [Reactive] public string AgreementText { get; internal set; }
        public AgreementPageViewModel(INavigationService navigationService,

             IDialogService dialogService) : base(navigationService, dialogService)
        {

            Title = "声明";
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(() =>
           {
               AgreementText = GlobalSettings.AgreementText;
           }));
            BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}




