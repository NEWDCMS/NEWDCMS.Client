using Wesley.Client.Models.Report;
using Wesley.Client.Pages.Market;
using Wesley.Client.Services;
using Wesley.Easycharts;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class NewCustomersPageViewModel : ViewModelBaseCutom
    {
        private readonly IReportingService _reportingService;
        [Reactive] public Chart ChartData { get; set; } = null;
        [Reactive] public NewCustomerAnalysis Data { get; set; } = new NewCustomerAnalysis();

        public NewCustomersPageViewModel(INavigationService navigationService,
                   IProductService productService,
                   IUserService userService,
                   ITerminalService terminalService,
                   IWareHousesService wareHousesService,
                   IAccountingService accountingService,
                   IReportingService reportingService,
                   IDialogService dialogService) : base(navigationService,
                       productService,
                       terminalService,
                       userService,
                       wareHousesService,
                       accountingService,
                       dialogService)
        {
            Title = "今日新增客户";
            _reportingService = reportingService;

            this.WhenAnyValue(x => x.Data).Subscribe(x => { this.IsNull = (x == null); }).DisposeWith(DestroyWith);


            this.WhenAnyValue(x => x.Filter.BusinessUserId)
            .Where(x => x > 0)
            .Subscribe(x =>
            {
                ((ICommand)Load)?.Execute(null);
            }).DisposeWith(DestroyWith);

            this.Load = ReactiveCommand.Create(() => Task.Run(async () =>
            {
                var result = await _reportingService.GetNewCustomerAnalysisAsync(Filter.BusinessUserId, this.ForceRefresh, calToken: cts.Token);
                if (result != null && result.ChartDatas != null)
                {
                    Data = result;
                    ChartData = CreateLineChart(result.ChartDatas);
                }
            }));

            //历史记录选择
            this.HistoryCommand = ReactiveCommand.Create<object>(async e => await this.NavigateAsync($"{nameof(CustomerArchivesPage)}", null));

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public Chart CreateLineChart(Dictionary<string, double> datas)
        {
            var entries = new List<ChartEntry>();
            int i = 0;
            foreach (var item in datas)
            {
                entries.Add(new ChartEntry((float)(item.Value))
                {
                    Label = item.Key,
                    ValueLabel = item.Value.ToString(),
                    Color = ChartDataProvider.Colors[i]
                });
                i++;
            }
            return ChartDataProvider.CreateLineChart(entries);
        }



        public override void OnAppearing()
        {
            base.OnAppearing();
            if (ChartData == null)
                ((ICommand)Load)?.Execute(null);
        }
    }
}
