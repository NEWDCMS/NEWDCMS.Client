using DCMS.Client.Models;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;


namespace DCMS.Client.ViewModels
{
    public class AddSubscribePageViewModel : ViewModelBase
    {
        [Reactive] public ObservableCollection<MessageInfo> AppList { get; set; } = new ObservableCollection<MessageInfo>();
        [Reactive] public bool SelectedALL { get; set; }

        public AddSubscribePageViewModel(INavigationService navigationService, IDialogService dialogService
            ) :
            base(navigationService, dialogService)
        {
            Title = "订阅";

            this.Load = ReactiveCommand.Create(() =>
            {
                try
                {
                    var seris = GlobalSettings.SubscribeDatas;
                    var filters = seris?.Where(s => !string.IsNullOrEmpty(s.Title));
                    if (filters != null)
                        this.AppList = new ObservableCollection<MessageInfo>(filters);
                }
                catch (Exception) { }
            });

            //全部
            this.WhenAnyValue(x => x.SelectedALL)
                .Skip(1)
                .Subscribe(x =>
                {
                    foreach (var app in this.AppList)
                    {
                        app.Selected = x;
                    }
                    var apps = this.AppList.Where(a => a.Selected).ToList();
                    Settings.SubscribeDatas = JsonConvert.SerializeObject(apps);
                })
                .DisposeWith(this.DeactivateWith);

            this.BindBusyCommand(Load);

        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }

        //OnNavigatedTo -> OnAppearing-> OnDisappearing-> OnNavigatedFrom
        public override void OnDisappearing()
        {
            base.OnDisappearing();
            try
            {
                var apps = this.AppList?.Select(a => a).ToList();
                if (apps != null)
                    Settings.SubscribeDatas = JsonConvert.SerializeObject(apps ?? new List<MessageInfo>());
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
