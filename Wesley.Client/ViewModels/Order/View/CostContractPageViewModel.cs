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
    public class CostContractPageViewModel : ViewModelBaseOrder<CostContractBillModel>
    {
        public CostContractPageViewModel(INavigationService navigationService,
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
            Title = "费用合同";

            this.BillType = BillTypeEnum.CostContractBill;
            this.Load = BillsLoader.Load(async () =>
            {
                var results = await Sync.Run(() =>
                {
                    DateTime? startTime = DateTime.Now.AddMonths(-1);
                    DateTime? endTime = DateTime.Now;

                    var pending = new List<CostContractBillModel>();

                    int? makeuserId = Settings.UserId;
                    int? customerId = 0;
                    string customerName = "";
                    //获取未审核
                    bool? auditedStatus = false;
                    bool? showReverse = null;
                    int? employeeId = 0;
                    string billNumber = "";
                    string remark = "";
                    int pagenumber = 0;
                    int pageSize = 20;



                    var result = _costContractService.GetCostContractsAsync(makeuserId, customerId, customerName, employeeId, billNumber, remark, startTime, endTime, auditedStatus, showReverse, pagenumber, pageSize, this.ForceRefresh, calToken: cts.Token).Result;

                    if (result != null)
                    {
                        pending = result?.Select(s =>
                        {
                            var sm = s;
                            sm.IsLast = !(result.LastOrDefault()?.BillNumber == s.BillNumber);
                            return sm;
                        }).ToList();

                    }

                    Title = $"费用合同({pending.Count})";
                    return pending;
                }, (ex) => { Crashes.TrackError(ex); });

                Bills = results;
                return results;
            });


            //选择单据
            this.SelectedCommand = ReactiveCommand.Create<CostContractBillModel>(async x =>
            {
                if (x != null)
                    await NavigateAsync(nameof(CostContractBillPage), ("Bill", x));
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
