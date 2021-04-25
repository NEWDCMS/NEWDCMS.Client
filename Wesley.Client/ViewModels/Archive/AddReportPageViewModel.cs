using Wesley.Client.Models;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class AddReportPageViewModel : ViewModelBase
    {
        [Reactive] public IList<Module> AppList { get; set; } = new ObservableCollection<Module>();

        public AddReportPageViewModel(INavigationService navigationService,

            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "添加报表快捷方式";

            this.Load = ReactiveCommand.Create(() =>
            {
                var seris = GlobalSettings.ReportsDatas;
                if (seris != null)
                    AppList = new ObservableCollection<Module>(seris);
            });


            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            try
            {
                base.OnNavigatedFrom(parameters);
                var apps = this.AppList?.Select(a => a).ToList();
                if (apps != null)
                    Settings.ReportsDatas = JsonConvert.SerializeObject(apps ?? new List<Module>());
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
