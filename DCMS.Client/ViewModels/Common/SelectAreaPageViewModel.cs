using DCMS.Client.Models.Terminals;
using DCMS.Client.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace DCMS.Client.ViewModels
{
    public class SelectAreaPageViewModel : ViewModelBase
    {
        private readonly ITerminalService _terminalService;
        [Reactive] public DistrictModel Selecter { get; set; }

        public IReactiveCommand DeleteCommand { get; }
        public IReactiveCommand CancelCommand { get; }
        public IReactiveCommand CheckCommand { get; }


        public SelectAreaPageViewModel(INavigationService navigationService,
            ITerminalService terminalService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "选择片区";

            _terminalService = terminalService;

            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await _terminalService.GetDistrictsAsync(true, new System.Threading.CancellationToken());
                if (result != null)
                {
                    var series = result;
                    Districts = new ObservableCollection<DistrictModel>(series);
                }
            });

            this.CancelCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await _navigationService.GoBackAsync();
            });

            this.CheckCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var item = Districts.Where(s => s.Selected).ToList().FirstOrDefault();
                if (item == null)
                {
                    this.Alert("请选择项目！");
                    return;
                }

                await _navigationService.GoBackAsync(("District", item));
            });

            this.DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var item = Districts.Where(s => s.Selected).ToList().FirstOrDefault();
                await _navigationService.GoBackAsync(("ClearDistrict", item));
            });


            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
            .Skip(1)
            .Where(x => x != null)
            .SubOnMainThread(item =>
           {
               item.Selected = !item.Selected;
               this.Selecter = null;
           }).DisposeWith(DeactivateWith);

            this.BindBusyCommand(Load);

        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
