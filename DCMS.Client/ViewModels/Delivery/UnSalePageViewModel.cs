using Acr.UserDialogs;
using DCMS.Client.Enums;
using DCMS.Client.Models.Sales;
using DCMS.Client.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;


namespace DCMS.Client.ViewModels
{
    /// <summary>
    /// 销售签收
    /// </summary>
    public class UnSalePageViewModel : ViewModelBaseOrder<SaleBillModel>
    {

        [Reactive] public decimal? TotalAmount { get; set; }
        public ReactiveCommand<SaleBillModel, Unit> SelecterCommand { get; set; }

        public UnSalePageViewModel(INavigationService navigationService,
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

            Title = "销售签收(0)";

            this.Bills = new ObservableCollection<SaleBillModel>();

            this.Load = ReactiveCommand.Create(async () =>
            {
                var pending = new List<SaleBillModel>();

                try
                {
                    int? terminalId = 0;
                    int? businessUserId = 0;
                    DateTime? startTime = DateTime.Now.AddMonths(-7);
                    DateTime? endTime = DateTime.Now;

                    int? makeuserId = Settings.UserId;
                    int? wareHouseId = 0;
                    string billNumber = "";
                    string terminalName = "";
                    string remark = "";
                    int? districtId = 0;
                    int? deliveryUserId = 0;
                    //获取已经审核，未签收单据
                    bool? auditedStatus = true;
                    bool? sortByAuditedTime = null;
                    bool? showReverse = null;
                    bool? showReturn = null;
                    bool? handleStatus = null;
                    int? paymentMethodType = 0;
                    int? billSourceType = 0;
                    int pageIndex = 0;
                    int pageSize = 20;

                    //获取待签收
                    var result = await _saleBillService.GetSalebillsAsync(makeuserId,
                        terminalId,
                        terminalName,
                        businessUserId,
                        districtId,
                        deliveryUserId,
                        wareHouseId,
                        billNumber,
                        remark,
                        startTime,
                        endTime,
                        auditedStatus,
                        sortByAuditedTime,
                        showReverse,
                        showReturn,
                        paymentMethodType,
                        billSourceType,
                        handleStatus,
                        0,
                        pageIndex,
                        pageSize,
                        this.ForceRefresh,
                        new System.Threading.CancellationToken());

                    if (result != null)
                    {
                        pending = result?.Select(s =>
                        {
                            var sm = s;
                            sm.IsLast = !(result.LastOrDefault()?.BillNumber == s.BillNumber);
                            return sm;
                        }).ToList();

                        TotalAmount = pending.Select(b => b.SumAmount).Sum();
                        Title = $"销售单({pending.Count})";

                        if (pending != null && pending.Any())
                            Bills = new ObservableCollection<SaleBillModel>(pending);
                    }
                }
                catch (System.Exception) { }
            });


            //签收 
            this.SelecterCommand = ReactiveCommand.CreateFromTask<SaleBillModel>(async (item) =>
            {
                if (item != null)
                {
                    using (UserDialogs.Instance.Loading("稍等..."))
                    {
                        await this.NavigateAsync("SaleBillPage",
                            ("Reference", this.PageName),
                            ("DispatchItemModel", null),
                            ("Bill", item));
                    }
                }
                item = null;
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
                }

            }, string.Format(Constants.MENU_DEV_KEY, 2));

            this.BindBusyCommand(Load);

        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);

        }

    }
}
