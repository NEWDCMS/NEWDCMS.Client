using Wesley.Client.Enums;
using Wesley.Client.Models.Sales;
using Wesley.Client.Pages.Bills;
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

    public class ReturnOrderPageViewModel : ViewModelBaseOrder<ReturnReservationBillModel>
    {

        public ReturnOrderPageViewModel(INavigationService navigationService,
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
            Title = "退货订单";


            this.BillType = BillTypeEnum.ReturnReservationBill;

            this.Load = BillsLoader.Load(async () =>
            {
                var results = await Sync.Run(() =>
                {
                    int? terminalId = 0;
                    int? businessUserId = 0;
                    DateTime? startTime = DateTime.Now.AddMonths(-1);
                    DateTime? endTime = DateTime.Now;

                    int? makeuserId = Settings.UserId;
                    int? wareHouseId = 0;
                    int? deliveryUserId = 0;
                    string terminalName = "";
                    string billNumber = "";
                    string remark = "";
                    int? districtId = 0;
                    //获取未审核
                    bool? auditedStatus = false;
                    bool? sortByAuditedTime = true;
                    bool? showReverse = null;
                    bool? showReturn = null;
                    bool? alreadyChange = null;
                    int pageIndex = 0;
                    int pageSize = 20;


                    var pending = new List<ReturnReservationBillModel>();


                    var result = _returnReservationBillService.GetReturnReservationBillsAsync(makeuserId, terminalId, terminalName, businessUserId, deliveryUserId, wareHouseId, districtId, billNumber, remark, startTime, endTime, auditedStatus, sortByAuditedTime, showReverse, showReturn, alreadyChange, pageIndex, pageSize, this.ForceRefresh, calToken: cts.Token).Result;

                    if (result != null)
                    {
                        pending = result?.Select(s =>
                        {
                            var sm = s;
                            sm.IsLast = !(result.LastOrDefault()?.BillNumber == s.BillNumber);
                            return sm;
                        }).ToList();

                    }

                    Title = $"退货订单({pending.Count})";
                    return pending;
                }, (ex) => { Crashes.TrackError(ex); });

                Bills = results;
                return results;
            });


            //选择单据
            this.SelectedCommand = ReactiveCommand.Create<ReturnReservationBillModel>(async x =>
            {
                if (x != null)
                    await NavigateAsync(nameof(ReturnOrderBillPage), ("Bill", x));
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
