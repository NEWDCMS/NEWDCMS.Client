using Wesley.Client.CustomViews;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
namespace Wesley.Client.ViewModels
{

    public class NewsViewerPageViewModel : ViewModelBase
    {
        private readonly INewsService _newsService;
        [Reactive] public string Html { get; set; } = "前不见古人，后不见来者！";


        public NewsViewerPageViewModel(INavigationService navigationService,


            INewsService newsService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _newsService = newsService;

            Title = "资讯预览";
            NewsLoader = new ViewModelLoader<string>(ApplicationExceptions.ToString, "前不见古人，后不见来者！");

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                parameters.TryGetValue("newsId", out int newsId);
                this.Load = NewsLoader.Load(async () =>
                {
                    var news = await _newsService.GetNewsAsync(newsId);
                    if (news != null)
                    {
                        if (!string.IsNullOrEmpty(news.Content))
                            Html = string.Format(HtmlConstants.DefaultHtml, news.Content);

                        Title = news?.Title;
                    }
                    return await Task.FromResult(Html);
                });
                this.BindBusyCommand(Load);
                this.ExceptionsSubscribe();
                ((ICommand)Load)?.Execute(null);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

    }

}
