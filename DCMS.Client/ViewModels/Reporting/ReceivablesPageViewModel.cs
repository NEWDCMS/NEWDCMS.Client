using Acr.UserDialogs;
using DCMS.Client.Models.Finances;
using DCMS.Client.Models.Terminals;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;


namespace DCMS.Client.ViewModels
{
    public class ReceivablesPageViewModel : ViewModelBaseCutom
    {
        private readonly IReceiptCashService _receiptCashService;
        public new int PageSize { get; } = 50;
        [Reactive] public AmountReceivableGroupModel Selecter { get; set; }
        [Reactive] public decimal TotalAmount { get; set; }
        [Reactive] public ObservableCollection<AmountReceivableGroupModel> BillItems { get; set; } = new ObservableCollection<AmountReceivableGroupModel>();


        public ReceivablesPageViewModel(INavigationService navigationService,
            IReceiptCashService receiptCashService,
            IDialogService dialogService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService

            ) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "应收款";

            _receiptCashService = receiptCashService;

            //绑定数据
            this.Load = AmountReceivableGroupsLoader.Load(async () =>
            {
                var pending = new List<AmountReceivableGroupModel>();

                try
                {
                    //这里只取选择客户
                    int? terminalId = Filter.TerminalId;
                    int businessUserId = Filter.BusinessUserId;
                    if (businessUserId == 0)
                    {
                        businessUserId = Settings.UserId;
                    }
                    DateTime? startTime = Filter.StartTime ?? null;
                    DateTime? endTime = Filter.EndTime ?? null;

                    string billNumber = "";
                    string remark = "";

                    var result = await _receiptCashService.GetOwecashBillsAsync(
                       businessUserId,
                        terminalId,
                        null,
                        billNumber,
                        remark,
                        startTime,
                        endTime,
                        0,
                        PageSize,
                        this.ForceRefresh,
                        new System.Threading.CancellationToken());

                    if (result != null)
                    {
                        foreach (IGrouping<int, BillSummaryModel> group in result.GroupBy(c => c.CustomerId))
                        {
                            var code = result.FirstOrDefault(c => c.CustomerId == group.Key)?.CustomerPointCode;
                            var name = result.FirstOrDefault(c => c.CustomerId == group.Key)?.CustomerName;
                            if (!string.IsNullOrEmpty(name))
                            {
                                pending.Add(new AmountReceivableGroupModel()
                                {
                                    CustomerId = group.Key,
                                    CustomerName = result.FirstOrDefault(c => c.CustomerId == group.Key)?.CustomerName,
                                    CustomerPointCode = code,
                                    Amount = result.Where(c => c.CustomerId == group.Key)?.Sum(s => s.ArrearsAmount) ?? 0
                                });
                            }
                        }
                    }

                    if (pending.Any())
                    {
                        TotalAmount = pending.Sum(p => p.Amount);
                        BillItems = new ObservableRangeCollection<AmountReceivableGroupModel>(pending);
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                return pending;
            });


            //以增量方式加载数据
            this.ItemTresholdReachedCommand = ReactiveCommand.Create(async () =>
            {
                int pageIdex = 0;
                if (BillItems.Count != 0)
                    pageIdex = BillItems.Count / (PageSize == 0 ? 1 : PageSize);

                if (PageCounter < pageIdex)
                {
                    PageCounter = pageIdex;
                    using (var dig = UserDialogs.Instance.Loading("加载中..."))
                    {
                        try
                        {
                            int? terminalId = Filter.TerminalId;
                            int businessUserId = Filter.BusinessUserId;
                            if (businessUserId == 0)
                            {
                                businessUserId = Settings.UserId;
                            }
                            DateTime? startTime = Filter.StartTime ?? null;
                            DateTime? endTime = Filter.EndTime ?? null;

                            string billNumber = "";
                            string remark = "";

                            var items = await _receiptCashService.GetOwecashBillsAsync(
                            businessUserId,
                            terminalId,
                            null,
                            billNumber,
                            remark,
                            startTime,
                            endTime,
                            pageIdex,
                            PageSize,
                            this.ForceRefresh,
                            new System.Threading.CancellationToken());

                            if (items != null)
                            {
                                var pending = new List<AmountReceivableGroupModel>();
                                foreach (IGrouping<int, BillSummaryModel> group in items.GroupBy(c => c.CustomerId))
                                {
                                    var code = items.FirstOrDefault(c => c.CustomerId == group.Key)?.CustomerPointCode;
                                    var name = items.FirstOrDefault(c => c.CustomerId == group.Key)?.CustomerName;
                                    if (!string.IsNullOrEmpty(name))
                                    {
                                        pending.Add(new AmountReceivableGroupModel()
                                        {
                                            CustomerId = group.Key,
                                            CustomerName = items.FirstOrDefault(c => c.CustomerId == group.Key)?.CustomerName,
                                            CustomerPointCode = code,
                                            Amount = items.Where(c => c.CustomerId == group.Key)?.Sum(s => s.ArrearsAmount) ?? 0
                                        });
                                    }
                                }

                                foreach (var p in pending)
                                {
                                    if (BillItems.Where(s => s.CustomerId == p.CustomerId).Count() == 0)
                                    {
                                        BillItems.Add(p);
                                    }
                                }

                                if (pending.Count() == 0 || pending.Count() == BillItems.Count)
                                {
                                    ItemTreshold = -1;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                            ItemTreshold = -1;
                        }
                    }
                }
            }, this.WhenAny(x => x.BillItems, x => x.GetValue().Count > 0));


            //选择转向收款单
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
             .Skip(1)
             .Where(x => x != null)
             .SubOnMainThread(async x =>
            {
                await this.NavigateAsync("ReceiptBillPage", ("Terminaler",
                    new TerminalModel()
                    {
                        Id = Selecter.CustomerId,
                        Name = Selecter.CustomerName
                    }));
                Selecter = null;
            }).DisposeWith(DeactivateWith);

            this.BindBusyCommand(Load);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            //选择客户
            if (parameters.ContainsKey("Terminaler"))
            {
                parameters.TryGetValue("Terminaler", out TerminalModel terminaler);
                Filter.TerminalId = terminaler != null ? terminaler.Id : 0;
                Filter.TerminalName = terminaler != null ? terminaler.Name : "";
            }

           ((ICommand)Load)?.Execute(null);
        }


        public override void OnAppearing()
        {
            base.OnAppearing();

            if (BillItems?.Count == 0)
                ((ICommand)Load)?.Execute(null);
        }
    }
}
