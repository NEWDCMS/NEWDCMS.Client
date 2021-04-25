using Wesley.Client.Enums;
using Wesley.Client.Models.WareHouses;
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
    public class InventoryPageViewModel : ViewModelBaseOrder<InventoryPartTaskBillModel>
    {
        public InventoryPageViewModel(INavigationService navigationService,
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
            Title = "盘点单据";


            this.BillType = BillTypeEnum.InventoryPartTaskBill;

            this.Load = BillsLoader.Load(async () =>
            {
                var results = await Sync.Run(() =>
                {
                    DateTime? startTime = DateTime.Now.AddMonths(-1);
                    DateTime? endTime = DateTime.Now;

                    int? makeuserId = Settings.UserId;
                    int? inventoryPerson = 0;
                    int? wareHouseId = 0;
                    string billNumber = "";
                    //获取未审核
                    bool? auditedStatus = false;
                    int? inventoryStatus = null;
                    bool? showReverse = null;
                    bool? sortByCompletedTime = null;
                    string remark = "";
                    int pagenumber = 0;
                    int pageSize = 20;


                    var pending = new List<InventoryPartTaskBillModel>();


                    var result = _inventoryService.GetInventoryAllsAsync(makeuserId, inventoryPerson, wareHouseId, billNumber, remark, auditedStatus, startTime, endTime, inventoryStatus, showReverse, sortByCompletedTime, pagenumber, pageSize, this.ForceRefresh, calToken: cts.Token).Result;

                    if (result != null)
                    {
                        pending = result?.Select(s =>
                        {
                            var sm = s;
                            sm.IsLast = !(result.LastOrDefault()?.BillNumber == s.BillNumber);
                            return sm;
                        }).ToList();
                    }

                    Title = $"盘点单据({pending.Count})";

                    return pending;

                }, (ex) => { Crashes.TrackError(ex); });

                Bills = results;
                return results;
            });


            //选择单据
            this.SelectedCommand = ReactiveCommand.Create<InventoryPartTaskBillModel>(async x =>
            {
                if (x != null)
                    await this.NavigateAsync($"{nameof(InventoryOPBillPage)}", ("Reference", "InventoryOPBillPage"),
                              ("Bill", x),
                               ("WareHouse", new WareHouseModel() { Id = x.WareHouseId, Name = x.WareHouseName }));
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
