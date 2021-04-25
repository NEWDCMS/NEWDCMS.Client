using Wesley.Client.Enums;
using Wesley.Client.Models.Finances;
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
    public class CostExpenditurePageViewModel : ViewModelBaseOrder<CostExpenditureBillModel>
    {
        public CostExpenditurePageViewModel(INavigationService navigationService,
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
            Title = "费用支出";


            this.BillType = BillTypeEnum.CostExpenditureBill;

            this.Load = BillsLoader.Load(async () =>
            {
                var results = await Sync.Run(() =>
                {
                    DateTime? startTime = DateTime.Now.AddMonths(-1);
                    DateTime? endTime = DateTime.Now;

                    int? makeuserId = Settings.UserId;
                    int? customerId = 0;
                    int? employeeId = 0;
                    string billNumber = "";
                    //获取未审核
                    bool? auditedStatus = false;
                    bool? showReverse = null;
                    bool sortByAuditedTime = false;
                    int pagenumber = 0;
                    int pageSize = 20;

                    var pending = new List<CostExpenditureBillModel>();


                    var result = _costExpenditureService.GetCostExpendituresAsync(makeuserId, customerId, "", employeeId, billNumber, auditedStatus, startTime, endTime,
                        showReverse,
                        sortByAuditedTime,
                        null,
                        pagenumber,
                        pageSize, this.ForceRefresh, calToken: cts.Token).Result;

                    if (result != null)
                    {
                        pending = result?.Select(s =>
                        {
                            var sm = s;
                            sm.IsLast = !(result.LastOrDefault()?.BillNumber == s.BillNumber);
                            sm.TotalAmount = s.Items?.Sum(i => i.Amount) ?? 0;

                            return sm;
                        }).ToList();

                    }

                    Title = $"费用支出({pending.Count})";
                    return pending;
                }, (ex) => { Crashes.TrackError(ex); });

                Bills = results;
                return results;
            });

            //选择单据
            this.SelectedCommand = ReactiveCommand.Create<CostExpenditureBillModel>(async x =>
            {
                if (x != null)
                    await NavigateAsync(nameof(CostExpenditureBillPage), ("Bill", x));
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
