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

    public class PurchaseSummeryPageViewModel : ViewModelBaseOrder<PurchaseBillModel>
    {

        public PurchaseSummeryPageViewModel(INavigationService navigationService,
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
                    int? manufacturerId = Filter.ManufacturerId;
                    int? businessUserId = Filter.BusinessUserId;
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;

                    int? makeuserId = Settings.UserId;
                    int? wareHouseId = 0;
                    string billNumber = "";
                    string remark = "";
                    //获取已审核
                    bool? auditedStatus = true;
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


            //菜单选择
            string key = string.Format("Wesley.CLIENT.PAGES.ORDER.{0}_SELECTEDTAB_{1}", this.PageViewName, 9);
            this.SubscribeMenus((x) =>
            {
                this.HitFilterDate(x, () => { ((ICommand)Load)?.Execute(null); });
                //获取当前UTC时间
                DateTime dtime = DateTime.Now;
                switch (x)
                {
                    case MenuEnum.TODAY:
                        {
                            Filter.StartTime = DateTime.Parse(dtime.ToString("yyyy-MM-dd 00:00:00"));
                            Filter.EndTime = dtime;
                            ((ICommand)Load)?.Execute(null);
                        }
                        break;
                    case MenuEnum.YESTDAY:
                        {
                            Filter.StartTime = dtime.AddDays(-1);
                            Filter.EndTime = dtime;
                            ((ICommand)Load)?.Execute(null);
                        }
                        break;
                    case MenuEnum.OTHER:
                        {
                            SelectDateRang();
                            ((ICommand)Load)?.Execute(null);
                        }
                        break;
                    case MenuEnum.SUBMIT30:
                        {
                            Filter.StartTime = dtime.AddMonths(-1);
                            Filter.EndTime = dtime;
                            ((ICommand)Load)?.Execute(null);
                        }
                        break;
                    case Enums.MenuEnum.CLEARHISTORY://清空一个月历史单据
                        {
                            ClearHistory(() => _globalService.UpdateHistoryBillStatusAsync((int)this.BillType));
                            ((ICommand)Load)?.Execute(null);
                        }
                        break;
                }

            }, string.Format(Constants.MENU_KEY, key));

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }

    }

}
