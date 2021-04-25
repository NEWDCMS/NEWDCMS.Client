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
    public class PopRadioButtonViewModel : ViewModelBase
    {
        [Reactive] public string Message { get; set; }
        [Reactive] public PopData Selecter { get; set; }
        [Reactive] public ObservableCollection<PopData> Options { get; set; } = new ObservableCollection<PopData>();

        public PopRadioButtonViewModel(INavigationService navigationService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {

            this.WhenAnyValue(x => x.Options).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DestroyWith);

            this.Load = ReactiveCommand.CreateFromTask<Func<Task<List<PopData>>>>(async (process) =>
            {
                try
                {
                    if (process != null)
                    {
                        var result = await process?.Invoke();
                        if (result != null && result.Any())
                        {
                            //if (result.Where(s => s.Selected == true).Count() == 0)
                            //    result.First().Selected = true;
                            this.Options = new ObservableCollection<PopData>(result);
                        }
                        else
                        {
                            this.IsNull = false;
                        }
                    }
                }
                catch (Exception ex)
                {

                    Crashes.TrackError(ex);
                }
            });

            //选择
            this.WhenAnyValue(x => x.Selecter)
            .Skip(1)
            .Where(x => x != null)
            .SubOnMainThread(item =>
            {
                item.Selected = !item.Selected;
            });

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
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
