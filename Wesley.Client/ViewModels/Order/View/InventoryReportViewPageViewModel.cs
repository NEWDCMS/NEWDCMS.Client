using Wesley.Client.Models.WareHouses;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
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

            ISaleBillService saleBillService) : base(navigationService, globalService, allocationService, advanceReceiptService, receiptCashService, costContractService, costExpenditureService, inventoryService, purchaseBillService, returnReservationBillService, returnBillService, saleReservationBillService, saleBillService, dialogService)
        {

            Title = "库存上报";

            _wareHousesService = wareHousesService;

            this.Load = BillsLoader.Load(async () =>
            {
                var results = await Sync.Run(() =>
                {
                    var pending = new List<InventoryReportSummaryModel>();

                    int? makeuserId = Settings.UserId;
                    int? terminalId = null;
                    int? businessUserId = null;
                    int? productId = null;
                    int? channelId = null;
                    int? rankId = null;
                    int? districtId = null;

                    DateTime? startTime = DateTime.Now.AddMonths(-1);
                    DateTime? endTime = DateTime.Now;
                    int pageIndex = 0;
                    int pageSize = 20;


                    var result = _wareHousesService.GetInventoryReportAsync(makeuserId, businessUserId, terminalId, channelId, rankId, districtId, productId, startTime, endTime, pageIndex, pageSize, this.ForceRefresh, calToken: cts.Token).Result;


                    if (result != null)
                    {
                        pending = result?.Select(s =>
                        {
                            var sm = s;
                            sm.IsLast = !(result?.LastOrDefault()?.BillNumber == s.BillNumber);
                            return sm;
                        }).ToList();
                    }

                    Title = $"库存上报({pending.Count})";
                    return pending;
                }, (ex) => { Crashes.TrackError(ex); });

                Bills = results;
                return results;
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
            ((ICommand)Load)?.Execute(null);
        }


    }
}
