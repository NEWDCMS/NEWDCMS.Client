using Wesley.Client.Models;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Wesley.Client.ViewModels
{
    public class PopCheckBoxPageViewModel : ViewModelBase
    {

        [Reactive] public string Message { get; set; }
        [Reactive] public PopData Selecter { get; set; }
        [Reactive] public ObservableCollection<PopData> Options { get; set; } = new ObservableCollection<PopData>();


        public Func<Task<List<PopData>>> BindDataAction { get; set; }

        public PopCheckBoxPageViewModel(INavigationService navigationService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {

            this.WhenAnyValue(x => x.Options)
                .Subscribe(x => { this.IsNull = x.Count == 0; })
                .DisposeWith(DeactivateWith);

            this.Load = ReactiveCommand.CreateFromTask<Func<Task<List<PopData>>>>(async (process) =>
            {
                try
                {
                    var result = await process?.Invoke();
                    if (result != null && result.Count > 0)
                    {
                        if (result.Where(s => s.Selected == true).Count() == 0)
                            result.First().Selected = true;
                    }
                    this.Options = new ObservableCollection<PopData>(result);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });

            //选择
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
            .Skip(1)
            .Where(x => x != null)
            .SubOnMainThread(item =>
            {
                item.Selected = !item.Selected;
            }).DisposeWith(DeactivateWith);

            this.WhenAnyValue(x => x.Options)
                .Subscribe(x => { this.IsNull = x.Count == 0; })
                .DisposeWith(DeactivateWith);

            this.BindBusyCommand(Load);

        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
