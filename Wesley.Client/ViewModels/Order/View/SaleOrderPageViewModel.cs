using Acr.UserDialogs;
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
            ISaleBillService saleBillService
            ) : base(navigationService, globalService, allocationService, advanceReceiptService, receiptCashService, costContractService, costExpenditureService, inventoryService, purchaseBillService, returnReservationBillService, returnBillService, saleReservationBillService, saleBillService, dialogService)
        {
            Title = "销售订单";

            this.BillType = BillTypeEnum.SaleReservationBill;

            this.Load = ReactiveCommand.Create(async () =>
            {
                ItemTreshold = 1;
                PageCounter = 0;

                try
                {
                    Bills?.Clear();

                    int? terminalId = Filter.TerminalId;
                    int? businessUserId = Filter.BusinessUserId;
                    string billNumber = Filter.SerchKey;
                    DateTime? startTime = Filter.StartTime ?? DateTime.Now;
                    DateTime? endTime = Filter.EndTime ?? DateTime.Now;

                    int? makeuserId = Settings.UserId;
                    if (Filter.BusinessUserId == Settings.UserId)
                    {
                        businessUserId = 0;
                    }

                    string terminalName = "";
                    int? deliveryUserId = 0;
                    int? wareHouseId = 0;
                    string remark = "";
                    int? districtId = 0;
                    //获取未审核
                    bool? auditedStatus = false;
                    bool? sortByAuditedTime = null;
                    bool? showReverse = null;
                    bool? showReturn = null;
                    bool? alreadyChange = null;


                    var pending = new List<SaleReservationBillModel>();


                    var result = await _saleReservationBillService.GetSaleReservationBillsAsync(makeuserId, terminalId, terminalName, businessUserId, deliveryUserId, billNumber, wareHouseId, remark, districtId, startTime, endTime, auditedStatus, sortByAuditedTime, showReverse, showReturn, alreadyChange, 0, PageSize, this.ForceRefresh, new System.Threading.CancellationToken());

                    if (result != null)
                    {
                        pending = result?.Select(s =>
                        {
                            var sm = s;
                            sm.IsLast = !(result.LastOrDefault()?.BillNumber == s.BillNumber);
                            return sm;
                        }).Where(s => s.MakeUserId == Settings.UserId || s.BusinessUserId == Settings.UserId).ToList();

                    }

                    if (pending.Any())
                        Bills = new System.Collections.ObjectModel.ObservableCollection<SaleReservationBillModel>(pending);
                    UpdateTitle();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

            });

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
                            string billNumber = Filter.SerchKey;
                            DateTime? startTime = Filter.StartTime ?? DateTime.Now;
                            DateTime? endTime = Filter.EndTime ?? DateTime.Now;

                            int? makeuserId = Settings.UserId;
                            if (Filter.BusinessUserId == Settings.UserId)
                            {
                                businessUserId = 0;
                            }

                            string terminalName = "";
                            int? deliveryUserId = 0;
                            int? wareHouseId = 0;
                            string remark = "";
                            int? districtId = 0;
                            //获取未审核
                            bool? auditedStatus = false;
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

                                foreach (var s in Bills)
                                {
                                    s.IsLast = !(Bills.LastOrDefault()?.BillNumber == s.BillNumber);
                                }
                            }
                            UpdateTitle();
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                        }
                    }
                }
            }, this.WhenAny(x => x.Bills, x => x.GetValue().Count > 0));

            //选择单据
            this.SelectedCommand = ReactiveCommand.Create<SaleReservationBillModel>(async x =>
            {
                if (x != null)
                    await NavigateAsync(nameof(SaleOrderBillPage), ("Bill", x), ("IsSubmitBill", true));
            });

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

            }, string.Format(Constants.MENU_VIEW_KEY, 0));


            this.BindBusyCommand(Load);

        }

        private void UpdateTitle()
        {
            Title = $"销售订单({Bills?.Count ?? 0})";
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Filter"))
            {
                var businessUserId = Filter.BusinessUserId;
                var terminalId = Filter.TerminalId;
                if (businessUserId > 0 || terminalId > 0)
                {
                    ((ICommand)Load)?.Execute(null);
                }
            }
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
