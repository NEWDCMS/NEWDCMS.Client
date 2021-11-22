using Acr.UserDialogs;
using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Sales;
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
    public class SaleOrderSummeryPageViewModel : ViewModelBaseOrder<SaleReservationBillModel>
    {
        [Reactive] public new SaleReservationBillModel Selecter { get; set; }

        public SaleOrderSummeryPageViewModel(INavigationService navigationService,
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
            Title = "销售订单";

            this.BillType = BillTypeEnum.SaleReservationBill;


            //加载数据
            this.Load = ReactiveCommand.Create(async () =>
            {
                //重载时排它
                ItemTreshold = 1;
                PageCounter = 0;

                try
                {
                    int? terminalId = Filter.TerminalId;
                    int? businessUserId = Filter.BusinessUserId;
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;
                    string billNumber = Filter.SerchKey;
                    int? makeuserId = Settings.UserId;

                    if (businessUserId.HasValue && businessUserId > 0)
                        makeuserId = 0;

                    string terminalName = "";
                    int? deliveryUserId = 0;
                    int? wareHouseId = 0;
                    string remark = "";
                    int? districtId = 0;
                    //获取已审核
                    bool? auditedStatus = true;
                    bool? sortByAuditedTime = null;
                    bool? showReverse = null;
                    bool? showReturn = null;
                    bool? alreadyChange = null;

                    //清除列表
                    Bills?.Clear();

                    var items = await _saleReservationBillService.GetSaleReservationBillsAsync(makeuserId, terminalId, terminalName, businessUserId, deliveryUserId, billNumber, wareHouseId, remark, districtId, startTime, endTime, auditedStatus, sortByAuditedTime, showReverse, showReturn, alreadyChange, 0, PageSize, this.ForceRefresh, new System.Threading.CancellationToken());

                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            if (Bills.Count(s => s.Id == item.Id) == 0)
                            {
                                Bills.Add(item);
                            }
                        }

                        foreach (var s in Bills)
                        {
                            s.IsLast = !(Bills.LastOrDefault()?.BillNumber == s.BillNumber);
                        }

                        if (Bills.Count > 0)
                            this.Bills = new ObservableRangeCollection<SaleReservationBillModel>(Bills);
                        UpdateTitle();
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
                            int? terminalId = Filter.TerminalId;
                            int? businessUserId = Filter.BusinessUserId;
                            DateTime? startTime = Filter.StartTime;
                            DateTime? endTime = Filter.EndTime;
                            string billNumber = Filter.SerchKey;

                            int? makeuserId = Settings.UserId;
                            string terminalName = "";
                            int? deliveryUserId = 0;
                            int? wareHouseId = 0;
                            string remark = "";
                            int? districtId = 0;
                            //获取已审核
                            bool? auditedStatus = true;
                            bool? sortByAuditedTime = null;
                            bool? showReverse = null;
                            bool? showReturn = null;
                            bool? alreadyChange = null;

                            var items = await _saleReservationBillService.GetSaleReservationBillsAsync(makeuserId, terminalId, terminalName, businessUserId, deliveryUserId, billNumber, wareHouseId, remark, districtId, startTime, endTime, auditedStatus, sortByAuditedTime, showReverse, showReturn, alreadyChange, pageIdex, PageSize, this.ForceRefresh, new System.Threading.CancellationToken());

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
                                UpdateTitle();
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
                   await NavigateAsync(nameof(SaleOrderBillPage), ("Bill", x));
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
                    case MenuEnum.CLEARHISTORY://清空一个月历史单据
                        {
                            ClearHistory(() => _globalService.UpdateHistoryBillStatusAsync((int)this.BillType));
                            ((ICommand)Load)?.Execute(null);
                        }
                        break;
                }

            }, string.Format(Constants.MENU_KEY, 0));


            this.BindBusyCommand(Load);

        }

        private void UpdateTitle()
        {
            if (ReferencePage == "NewOrderPage")
            {
                Title = $"历史销售订单({Bills?.Count ?? 0})";
            }
            else
            {
                Title = $"销售订单({Bills?.Count ?? 0})";
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("DateType"))
            {
                parameters.TryGetValue<int>("DateType", out int dateType);
                switch (dateType)
                {
                    //今日订单
                    case 1:
                        Filter.StartTime = DateTime.Now;
                        Filter.EndTime = DateTime.Now;
                        break;
                    //昨天订单
                    case 2:
                        Filter.StartTime = DateTime.Now.AddDays(-1);
                        Filter.EndTime = DateTime.Now;
                        break;
                    //前天订单
                    case 3:
                        Filter.StartTime = DateTime.Now.AddDays(-2);
                        Filter.EndTime = DateTime.Now;
                        break;
                    //上周订单
                    case 4:
                        Filter.StartTime = DateTime.Now.AddDays(0 - Convert.ToInt16(DateTime.Now.DayOfWeek) - 6);
                        Filter.EndTime = DateTime.Now.AddDays(6 - Convert.ToInt16(DateTime.Now.DayOfWeek) - 6);
                        break;
                    //本周订单
                    case 5:
                        Filter.StartTime = DateTime.Now.AddDays(0 - Convert.ToInt16(DateTime.Now.DayOfWeek) + 1);
                        Filter.EndTime = DateTime.Now.AddDays(6 - Convert.ToInt16(DateTime.Now.DayOfWeek) + 1);
                        break;
                    //上月订单
                    case 6:
                        Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(-1);
                        Filter.EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1);
                        break;
                    //本月订单
                    case 7:
                        Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01"));
                        Filter.EndTime = DateTime.Now;
                        break;
                    //本年订单
                    case 8:
                        Filter.StartTime = new DateTime(DateTime.Now.Year, 1, 1);
                        Filter.EndTime = DateTime.Now;
                        break;
                }
            }

            //过滤器
            if (parameters.ContainsKey("Filter"))
            {
                parameters.TryGetValue("Filter", out FilterModel filter);
                if (filter != null)
                    this.Filter = filter;
            }

             ((ICommand)Load)?.Execute(null);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            ThrottleLoad(() =>
            {
                ((ICommand)Load)?.Execute(null);
            }, (Bills?.Count == 0), false);
        }
    }
}
