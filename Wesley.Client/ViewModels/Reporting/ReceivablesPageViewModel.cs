using Wesley.Client.Models.Finances;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
namespace Wesley.Client.ViewModels
{
    public class ReceivablesPageViewModel : ViewModelBaseCutom
    {

        private readonly IReceiptCashService _receiptCashService;


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

                    string billNumber = "";
                    string remark = "";
                    DateTime? startTime = null;
                    DateTime? endTime = null;
                    int pageIndex = 0;
                    int pageSize = 50;

                    var result = await _receiptCashService.GetOwecashBillsAsync(
                        Settings.UserId,
                        terminalId, null,
                        billNumber, remark,
                        startTime,
                        endTime,
                        pageIndex,
                        pageSize, this.ForceRefresh, calToken: cts.Token);

                    if (result != null)
                    {
                        foreach (IGrouping<int, BillSummaryModel> group in result.GroupBy(c => c.CustomerId))
                        {
                            var code = result.FirstOrDefault(c => c.CustomerId == group.Key)?.CustomerPointCode;
                            var name = result.FirstOrDefault(c => c.CustomerId == group.Key)?.CustomerName;

                            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(name))
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
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                TotalAmount = pending.Sum(p => p.Amount);
                BillItems = new ObservableRangeCollection<AmountReceivableGroupModel>(pending);

                return pending;
            });


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
            });

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
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
                ((ICommand)Load)?.Execute(null);
            }
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
