using Acr.UserDialogs;
using Wesley.Client.Models.WareHouses;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class InventoryReportViewPageViewModel : ViewModelBaseOrder<InventoryReportSummaryModel>
    {
        private readonly IWareHousesService _wareHousesService;
        public InventoryReportViewPageViewModel(INavigationService navigationService,
            IGlobalService globalService,
            IDialogService dialogService,
            IAllocationService allocationService,
            IAdvanceReceiptService advanceReceiptService,
            IReceiptCashService receiptCashService,
            ICostContractService costContractService,
            ICostExpenditureService costExpenditureService,
            IInventoryService inventoryService,
            IPurchaseBillService purchaseBillService,
            IReturnReservationBillService returnReservationBillService,
            IReturnBillService returnBillService,
            ISaleReservationBillService saleReservationBillService,
            IWareHousesService wareHousesService,
            ISaleBillService saleBillService
            ) : base(navigationService, globalService, allocationService, advanceReceiptService, receiptCashService, costContractService, costExpenditureService, inventoryService, purchaseBillService, returnReservationBillService, returnBillService, saleReservationBillService, saleBillService, dialogService)
        {

            Title = "库存上报";

            _wareHousesService = wareHousesService;

            this.Load = ReactiveCommand.Create(async () =>
            {
                ItemTreshold = 1;
                PageCounter = 0;

                try
                {
                    Bills?.Clear();
                    var pending = new List<InventoryReportSummaryModel>();

                    int? makeuserId = Settings.UserId;
                    int? terminalId = null;
                    int? businessUserId = null;
                    int? productId = null;
                    int? channelId = null;
                    int? rankId = null;
                    int? districtId = null;

                    string billNumber = Filter.SerchKey;
                    DateTime? startTime = Filter.StartTime ?? DateTime.Now.AddMonths(-1);
                    DateTime? endTime = Filter.EndTime ?? DateTime.Now;


                    var result = await _wareHousesService.GetInventoryReportAsync(makeuserId, businessUserId, terminalId, channelId, rankId, districtId, productId, startTime, endTime, 0, PageSize, this.ForceRefresh, new System.Threading.CancellationToken());


                    if (result != null)
                    {
                        pending = result?.Select(s =>
                        {
                            var sm = s;
                            sm.IsLast = !(result?.LastOrDefault()?.BillNumber == s.BillNumber);
                            return sm;
                        }).Where(s => s.MakeUserId == Settings.UserId || s.BusinessUserId == Settings.UserId).ToList();
                    }
                    if (pending.Any())
                        Bills = new System.Collections.ObjectModel.ObservableCollection<InventoryReportSummaryModel>(pending);


                    UpdateTitle();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }


            });


            this.ItemTresholdReachedCommand = ReactiveCommand.Create(async () =>
            {
                int pageIdex = 0;
                if (Bills?.Count != 0)
                    pageIdex = Bills.Count / (PageSize == 0 ? 1 : PageSize);

                if (PageCounter < pageIdex)
                {
                    PageCounter = pageIdex;
                    using (var dig = UserDialogs.Instance.Loading("加载中..."))
                    {
                        try
                        {
                            int? makeuserId = Settings.UserId;
                            int? terminalId = null;
                            int? businessUserId = null;
                            int? productId = null;
                            int? channelId = null;
                            int? rankId = null;
                            int? districtId = null;

                            string billNumber = Filter.SerchKey;
                            DateTime? startTime = Filter.StartTime ?? DateTime.Now.AddMonths(-1);
                            DateTime? endTime = Filter.EndTime ?? DateTime.Now;


                            var result = await _wareHousesService.GetInventoryReportAsync(makeuserId, businessUserId, terminalId, channelId, rankId, districtId, productId, startTime, endTime, 0, PageSize, this.ForceRefresh, new System.Threading.CancellationToken());

                            if (result != null)
                            {
                                foreach (var item in result)
                                {
                                    if ((item.MakeUserId == Settings.UserId || item.BusinessUserId == Settings.UserId) && Bills.Count(s => s.Id == item.Id) == 0)
                                    {
                                        Bills.Add(item);
                                    }
                                }

                                foreach (var s in Bills)
                                {
                                    s.IsLast = !(Bills.LastOrDefault()?.BillNumber == s.BillNumber);
                                }
                            }

                            UpdateTitle();
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                        }
                    }
                }
            }, this.WhenAny(x => x.Bills, x => x.GetValue().Count > 0));

            this.BindBusyCommand(Load);

        }

        private void UpdateTitle()
        {
            Title = $"库存上报({Bills?.Count ?? 0})";
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ThrottleLoad(() =>
            {
                ((ICommand)Load)?.Execute(null);
            }, (Bills?.Count == 0));
        }
    }
}
