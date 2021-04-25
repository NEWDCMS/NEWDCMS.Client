using Wesley.Client.Enums;
using Wesley.Client.Models.Purchases;
using Wesley.Client.Pages.Archive;
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

    public class PurchasePageViewModel : ViewModelBaseOrder<PurchaseBillModel>
    {
        public PurchasePageViewModel(INavigationService navigationService,
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

            ISaleBillService saleBillService) : base(navigationService, globalService, allocationService, advanceReceiptService, receiptCashService, costContractService, costExpenditureService, inventoryService, purchaseBillService, returnReservationBillService, returnBillService, saleReservationBillService, saleBillService, dialogService)
        {
            Title = "采购单据";

            this.BillType = BillTypeEnum.PurchaseBill;

            this.Load = BillsLoader.Load(async () =>
            {

                var results = await Sync.Run(() =>
                {
                    int? manufacturerId = 0;
                    int? businessUserId = 0;
                    DateTime? startTime = DateTime.Now.AddMonths(-1);
                    DateTime? endTime = DateTime.Now;

                    int? makeuserId = Settings.UserId;
                    int? wareHouseId = 0;
                    string billNumber = "";
                    string remark = "";
                    //获取未审核
                    bool? auditedStatus = false;
                    bool? sortByAuditedTime = null;
                    bool? showReverse = null;
                    int pageIndex = 0;
                    int pageSize = 20;


                    var pending = new List<PurchaseBillModel>();


                    var result = _purchaseBillService.GetPurchaseBillsAsync(makeuserId, businessUserId, manufacturerId, wareHouseId, billNumber, remark, null, startTime, endTime, auditedStatus, sortByAuditedTime, showReverse, pageIndex, pageSize, this.ForceRefresh, calToken: cts.Token).Result;

                    if (result != null)
                    {
                        pending = result?.Select(s =>
                        {
                            var sm = s;
                            sm.IsLast = !(result.LastOrDefault()?.BillNumber == s.BillNumber);
                            return sm;
                        }).ToList();

                    }
                    Title = $"采购单({pending.Count})";

                    return pending;
                }, (ex) => { Crashes.TrackError(ex); });

                Bills = results;
                return results;

            });


            //选择单据
            this.SelectedCommand = ReactiveCommand.Create<PurchaseBillModel>(async x =>
            {
                if (x != null)
                    await NavigateAsync(nameof(PurchaseOrderBillPage), ("Bill", x));
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
