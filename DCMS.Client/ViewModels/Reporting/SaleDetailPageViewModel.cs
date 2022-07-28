using Acr.UserDialogs;
using DCMS.Client.Models.Sales;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace DCMS.Client.ViewModels
{
    public class SaleDetailPageViewModel : ViewModelBaseCutom
    {
        [Reactive] public decimal TotalSumReturnAmount { get; set; }
        [Reactive] public decimal TotalSumNetAmount { get; set; }
        [Reactive] public bool DataVewEnable { get; set; } = true;
        [Reactive] public bool NullViewEnable { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }

        [Reactive] public ObservableCollection<SaleReportItem> Bills { get; set; } = new ObservableCollection<SaleReportItem>();

        public SaleDetailPageViewModel(INavigationService navigationService,
           IReceiptCashService receiptCashService,
           IDialogService dialogService,
           IProductService productService,
           IUserService userService,
           ITerminalService terminalService,
           IWareHousesService wareHousesService,
           IAccountingService accountingService,
           IReportingService reportingService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "商品销售明细";


            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!string.IsNullOrEmpty(ProductName))
                {
                    Title = $"{ProductName}-明细";
                }

                //重载时排它
                ItemTreshold = 1;

                try
                {
                    this.Bills?.Clear();
                    PageCounter = 0;

                    var rankings = new List<SaleReportItem>();
                    int? productId = ProductId;
                    int? businessUserId = Filter.BusinessUserId;
                    DateTime? startTime = Filter.StartTime ?? DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01 00:00:00"));
                    DateTime? endTime = Filter.EndTime ?? DateTime.Now;

                    var result = await reportingService.GetHotSaleReportItemAsync(productId,
                        businessUserId,
                        startTime.Value,
                        endTime.Value,
                        this.ForceRefresh,
                        0,
                        new System.Threading.CancellationToken());

                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            this.Bills.Add(item);
                        }

                        if (this.Bills.Any())
                            this.Bills = new ObservableRangeCollection<SaleReportItem>(Bills);
                    }

                    UpdateUI();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });


            this.ItemTresholdReachedCommand = ReactiveCommand.Create(async () =>
            {

                if (ItemTreshold == -1) return;

                int pageIdex = 0;

                var p = Bills.Count;

                if (this.Bills?.Count != 0)
                    pageIdex = Bills.Count / 30;

                if (PageCounter < pageIdex)
                {
                    PageCounter = pageIdex;
                    using (var dig = UserDialogs.Instance.Loading("加载中..."))
                    {
                        try
                        {
                            var rankings = new List<SaleReportItem>();
                            int? productId = ProductId;
                            int? businessUserId = Filter.BusinessUserId;
                            DateTime? startTime = Filter.StartTime ?? DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01 00:00:00"));
                            DateTime? endTime = Filter.EndTime ?? DateTime.Now;

                            var result = await reportingService.GetHotSaleReportItemAsync(productId,
                                businessUserId,
                                startTime.Value,
                                endTime.Value,
                                this.ForceRefresh,
                                pageIdex,
                                new System.Threading.CancellationToken());

                            if (result != null && result.Any())
                            {
                                foreach (var item in result)
                                {
                                    Bills.Add(item);
                                }
                            }
                            else
                            {
                                ItemTreshold = -1;
                            }
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                            ItemTreshold = -1;
                        }
                    }
                }
            }, this.WhenAny(x => x.Bills, x => x.GetValue().Count > 0));


            this.BindBusyCommand(Load);
        }
        public void UpdateUI()
        {
            if (Bills?.Count() > 0)
            {
                NullViewEnable = false;
                DataVewEnable = true;
            }
            else
            {
                NullViewEnable = true;
                DataVewEnable = false;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                string fromPage = "";
                if (parameters.ContainsKey("Reference"))
                {
                    parameters.TryGetValue("Reference", out fromPage);
                }
                int? _productId = 0;
                if (parameters.ContainsKey("ProductId"))
                {
                    parameters.TryGetValue("ProductId", out _productId);
                    ProductId = _productId ?? 0;
                }

                decimal? _totalSumNetAmount = 0;
                if (parameters.ContainsKey("TotalSumNetAmount"))
                {
                    parameters.TryGetValue("TotalSumNetAmount", out _totalSumNetAmount);
                    TotalSumNetAmount = _totalSumNetAmount ?? 0;
                }
                decimal? _totalSumReturnAmount = 0;
                if (parameters.ContainsKey("TotalSumReturnAmount"))
                {
                    parameters.TryGetValue("TotalSumReturnAmount", out _totalSumReturnAmount);
                    TotalSumReturnAmount = _totalSumReturnAmount ?? 0;
                }
                string _productName = "";
                if (parameters.ContainsKey("ProductName"))
                {
                    parameters.TryGetValue("ProductName", out _productName);
                    ProductName = _productName;
                }
                int? BusinessUserId = 0;
                if (parameters.ContainsKey("BusinessUserId"))
                {
                    parameters.TryGetValue("BusinessUserId", out BusinessUserId);
                    Filter.BusinessUserId = BusinessUserId ?? 0;
                }
                DateTime? StartTime = null;
                if (parameters.ContainsKey("StartTime"))
                {
                    parameters.TryGetValue("StartTime", out StartTime);
                    Filter.StartTime = StartTime;
                }
                DateTime? EndTime = null;
                if (parameters.ContainsKey("EndTime"))
                {
                    parameters.TryGetValue("EndTime", out EndTime);
                    Filter.EndTime = EndTime;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
