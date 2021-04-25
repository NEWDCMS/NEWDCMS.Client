using Wesley.Client.Models;
using Wesley.Client.Pages;
using Wesley.Client.Services;

using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class NewsPageViewModel : ViewModelBase
    {
        private readonly INewsService _newsService;

        [Reactive] public ObservableCollection<NewsInfoModel> NewsSeries { get; set; } = new ObservableCollection<NewsInfoModel>();
        [Reactive] public NewsInfoModel Selecter { get; set; }
        public NewsPageViewModel(INavigationService navigationService,
            INewsService newsService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _newsService = newsService;


            Title = "促销活动";

            this.Load = NewsInfosLoader.Load(async () =>
            {
                var pending = new List<NewsInfoModel>();
                var result = await _newsService.GetNewsAsync(this.ForceRefresh, cts.Token);
                if (result != null)
                {
                    var ram = new Random();
                    pending = result.ToList();
                    pending.ForEach(r =>
                    {
                        r.ViewCount = ram.Next(100, 500);
                        r.HeartCount = ram.Next(50, 300);
                    });
                    NewsSeries = new ObservableCollection<NewsInfoModel>(pending);
                }
                return pending;
            });

            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
           .Skip(1)
           .Where(x => x != null)
           .SubOnMainThread(async item =>
          {
              if (item != null)
              {
                  //注意参数：useModalNavigation
                  await this.NavigateAsync($"{nameof(NewsViewerPage)}", ("newsId", item.Id));
              }
              this.Selecter = null;
          });

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
