using Acr.UserDialogs;
using Wesley.Client.Enums;
using Wesley.Client.Models.Finances;
using Wesley.Client.Pages.Bills;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class CostExpenditureSummeryPageViewModel : ViewModelBaseOrder<CostExpenditureBillModel>
    {
        [Reactive] public new CostExpenditureBillModel Selecter { get; set; }
        public CostExpenditureSummeryPageViewModel(INavigationService navigationService,
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

            ISaleBillService saleBillService
            ) : base(navigationService, globalService, allocationService, advanceReceiptService, receiptCashService, costContractService, costExpenditureService, inventoryService, purchaseBillService, returnReservationBillService, returnBillService, saleReservationBillService, saleBillService, dialogService)
        {
            Title = "费用支出";


            this.BillType = BillTypeEnum.CostExpenditureBill;

            this.Load = ReactiveCommand.Create(async () =>
            {
                //重载时排它
                ItemTreshold = 1;
                PageCounter = 0;
                try
                {
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;
                    int? customerId = Filter.TerminalId;
                    int? employeeId = Filter.BusinessUserId;
                    string billNumber = Filter.SerchKey;

                    int? makeuserId = Settings.UserId;
                    if (employeeId.HasValue && employeeId > 0)
                        makeuserId = 0;


                    //获取已审核
                    bool? auditedStatus = true;
                    bool? showReverse = null;
                    bool sortByAuditedTime = false;

                    //清除列表
                    Bills?.Clear();

                    var items = await _costExpenditureService.GetCostExpendituresAsync(makeuserId,
                        customerId,
                        "",
                        employeeId,
                        billNumber,
                        auditedStatus,
                        startTime,
                        endTime,
                        showReverse,
                        sortByAuditedTime,
                        null,
                        0,
                        PageSize, this.ForceRefresh, new System.Threading.CancellationToken());

                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            if (Bills.Count(s => s.Id == item.Id) == 0)
                            {
                                Bills.Add(item);
                            }
                        }

                        if (items.Count() == 0)
                        {
                            ItemTreshold = -1;
                        }

                        foreach (var s in Bills)
                        {
                            s.IsLast = !(Bills.LastOrDefault()?.BillNumber == s.BillNumber);
                        }

                        Title = $"费用支出({Bills.Count})";

                        if (Bills.Count > 0)
                            this.Bills = new ObservableRangeCollection<CostExpenditureBillModel>(Bills);
                    }

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });
            //以增量方式加载数据
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
                            DateTime? startTime = Filter.StartTime;
                            DateTime? endTime = Filter.EndTime;
                            int? customerId = Filter.TerminalId;
                            int? employeeId = Filter.BusinessUserId;
                            string billNumber = Filter.SerchKey;

                            int? makeuserId = Settings.UserId;
                            if (employeeId.HasValue && employeeId > 0)
                                makeuserId = 0;


                            //获取已审核
                            bool? auditedStatus = true;
                            bool? showReverse = null;
                            bool sortByAuditedTime = false;


                            var items = await _costExpenditureService.GetCostExpendituresAsync(makeuserId,
                                customerId,
                                "",
                                employeeId,
                                billNumber,
                                auditedStatus,
                                startTime,
                                endTime,
                                showReverse,
                                sortByAuditedTime,
                                null,
                                pageIdex, PageSize, this.ForceRefresh, new System.Threading.CancellationToken());
                            if (items != null)
                            {
                                foreach (var item in items)
                                {
                                    if (Bills.Count(s => s.Id == item.Id) == 0)
                                    {
                                        Bills.Add(item);
                                    }
                                }

                                if (items.Count() == 0)
                                {
                                    ItemTreshold = -1;
                                }

                                foreach (var s in Bills)
                                {
                                    s.IsLast = !(Bills.LastOrDefault()?.BillNumber == s.BillNumber);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                        }
                    }
                }
            }, this.WhenAny(x => x.Bills, x => x.GetValue().Count > 0));

            //选择单据
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
          .Skip(1)
          .Where(x => x != null)
          .SubOnMainThread(async x =>
          {
              if (x != null)
                  await NavigateAsync(nameof(CostExpenditureBillPage), ("Bill", x));
              this.Selecter = null;
          }).DisposeWith(DeactivateWith);

            //菜单选择
            this.SubscribeMenus((x) =>
            {
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

            }, string.Format(Constants.MENU_KEY, 6));

            this.BindBusyCommand(Load);

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
