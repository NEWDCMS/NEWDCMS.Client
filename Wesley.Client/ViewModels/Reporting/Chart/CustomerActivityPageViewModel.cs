using Wesley.ChartJS.Models;
using Wesley.Client.Models.Report;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{
    public class CustomerActivityPageViewModel : ViewModelBaseChart<CustomerActivityRanking>
    {

        private readonly ITerminalService _terminalService;
        public CustomerActivityPageViewModel(INavigationService navigationService,
           IProductService productService,
           ITerminalService terminalService,
           IReportingService reportingService,
           IDialogService dialogService
            ) : base(navigationService,
               productService,
               reportingService,
               dialogService)
        {

            Title = "客户活跃度";
            this.PageType = Enums.ChartPageEnum.CustomerActivity_Template;

            _terminalService = terminalService;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DeactivateWith);
            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    var result = await _terminalService.GetCustomerActivityRankingAsync(Settings.UserId, Filter.TerminalId, this.ForceRefresh, new System.Threading.CancellationToken());
                    if (result != null)
                    {
                        Refresh(result.ToList());
                    }
                }
                catch (Exception ex)
                {

                    Crashes.TrackError(ex);
                }
            });

            this.BindBusyCommand(Load);
        }

        public void Refresh(List<CustomerActivityRanking> analysis)
        {

            RankSeries = new ObservableCollection<CustomerActivityRanking>(analysis);

            var ranks = analysis.ToList();
            if (ranks.Count > 10)
            {
                ranks = ranks.Take(10).ToList();
            }
            var data = new ChartViewConfig()
            {
                BackgroundColor = Color.White,
                ChartConfig = new ChartConfig
                {
                    type = Wesley.ChartJS.ChartTypes.Line,
                    data = ChartDataProvider.GetCustomerActivity(ranks),
                }
            };
            ChartConfig = data;
        }



        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
