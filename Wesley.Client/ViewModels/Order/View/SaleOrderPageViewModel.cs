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
    public class SaleOrderPageViewModel : ViewModelBaseOrder<SaleReservationBillModel>
    {
        public SaleOrderPageViewModel(INavigationService navigationService,
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
            Title = "销售订单";

            this.BillType = BillTypeEnum.SaleReservationBill;

            this.Load = BillsLoader.Load(async () =>
            {
                var results = await Sync.Run(() =>
                {
                    int? terminalId = 0;
                    int? businessUserId = 0;
                    DateTime? startTime = DateTime.Now.AddMonths(-1);
                    DateTime? endTime = DateTime.Now;

                    int? makeuserId = Settings.UserId;
                    string terminalName = "";
                    int? deliveryUserId = 0;
                    string billNumber = "";
                    int? wareHouseId = 0;
                    string remark = "";
                    int? districtId = 0;
                    //获取未审核
                    bool? auditedStatus = false;
                    bool? sortByAuditedTime = null;
                    bool? showReverse = null;
                    bool? showReturn = null;
                    bool? alreadyChange = null;
                    int pagenumber = 0;
                    int pageSize = 20;


                    var pending = new List<SaleReservationBillModel>();


                    var result = _saleReservationBillService.GetSaleReservationBillsAsync(makeuserId, terminalId, terminalName, businessUserId, deliveryUserId, billNumber, wareHouseId, remark, districtId, startTime, endTime, auditedStatus, sortByAuditedTime, showReverse, showReturn, alreadyChange, pagenumber, pageSize, this.ForceRefresh, calToken: cts.Token).Result;

                    if (result != null)
                    {
                        pending = result?.Select(s =>
                        {
                            var sm = s;
                            sm.IsLast = !(result.LastOrDefault()?.BillNumber == s.BillNumber);
                            return sm;
                        }).ToList();

                    }

                    Title = $"销售订单({pending.Count})";
                    return pending;
                }, (ex) => { Crashes.TrackError(ex); });

                Bills = results;
                return results;
            });


            //选择单据
            this.SelectedCommand = ReactiveCommand.Create<SaleReservationBillModel>(async x =>
            {
                if (x != null)
                    await NavigateAsync(nameof(SaleOrderBillPage), ("Bill", x));
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
