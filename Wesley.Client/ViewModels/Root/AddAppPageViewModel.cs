using Wesley.Client.Models;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using System.Reactive.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class AddAppPageViewModel : ViewModelBase
    {
        [Reactive] public ObservableCollection<Module> AppList { get; set; } = new ObservableCollection<Module>();
        [Reactive] public bool SelectAll { get; set; }

        public AddAppPageViewModel(INavigationService navigationService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "添加桌面快捷方式";

            this.Load = ReactiveCommand.Create(() =>
            {
                try
                {
                    var allApps = GlobalSettings.AppDatas;
                    if (allApps != null)
                        this.AppList = new ObservableCollection<Module>(allApps);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });

            //全部
            this.WhenAnyValue(x => x.SelectAll)
                .Skip(1)
                .Subscribe(x =>
                {
                    foreach (var app in this.AppList)
                    {
                        app.Selected = x;
                    }
                    var apps = this.AppList.ToList();
                    if (apps != null && apps.Any())
                        Settings.AppDatas = JsonConvert.SerializeObject(apps);
                })
                .DisposeWith(this.DeactivateWith);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            try
            {
                var apps = this.AppList?.Select(a => a).ToList();
                if (apps != null)
                    Settings.AppDatas = JsonConvert.SerializeObject(apps ?? new List<Module>());
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
