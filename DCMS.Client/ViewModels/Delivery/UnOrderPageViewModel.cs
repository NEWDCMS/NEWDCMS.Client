using Acr.UserDialogs;
using DCMS.Client.Enums;
using DCMS.Client.Models.Sales;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
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
    /// 订单签收
    /// </summary>
    public class UnOrderPageViewModel : ViewModelBase
    {
        private readonly ISaleBillService _saleBillService;
        [Reactive] public ObservableCollection<DispatchItemModel> Bills { get; set; }
        [Reactive] public decimal? TotalAmount { get; set; }
        public ReactiveCommand<DispatchItemModel, Unit> SelecterCommand { get; set; }

        public UnOrderPageViewModel(INavigationService navigationService,
            ISaleBillService saleBillService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "订单签收(0)";

            _saleBillService = saleBillService;

            this.Bills = new ObservableCollection<DispatchItemModel>();

            //载入未签收单据
            this.Load = DispatchItemsLoader.Load(async () =>
            {
                var pending = new List<DispatchItemModel>();

                try
                {
                    //获取未签收订单单据
                    var result = await _saleBillService.GetUndeliveredSignsAsync(Settings.UserId,
                        Filter.StartTime,
                        Filter.EndTime,
                        Filter.BusinessUserId,
                        Filter.TerminalId,
                        force: this.ForceRefresh,
                        calToken: (cts?.Token) ?? new System.Threading.CancellationToken());

                    if (result != null)
                    {
                        pending = result?.Select(s =>
                        {
                            var sm = s;
                            s.SaleReservationBill.AuditedStatus = true;
                            s.ExchangeBillModel.AuditedStatus = true;
                            s.ReturnReservationBill.AuditedStatus = true;
                            sm.IsLast = !(result.LastOrDefault()?.BillNumber == s.BillNumber);
                            return sm;
                        }).ToList();

                        TotalAmount = pending.Select(b => b.OrderAmount).Sum();
                        Title = $"未签收({pending.Count})";

                        if (pending != null)
                            Bills = new ObservableCollection<DispatchItemModel>(pending);
                        else
                            Bills = new ObservableCollection<DispatchItemModel>();

                    }
                    else
                    {
                        TotalAmount = 0;
                        Title = $"未签收(0)";
                        Bills = new ObservableCollection<DispatchItemModel>();
                    }
                }
                catch (System.Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                return pending;
            });


            //签收
            //注意：这里的换货签收逻辑和销售退货签收逻辑不同
            this.SelecterCommand = ReactiveCommand.CreateFromTask<DispatchItemModel>(async (item) =>
            {
                if (item != null)
                {
                    using (UserDialogs.Instance.Loading("稍等..."))
                    {
                        if (item.BillTypeId == (int)BillTypeEnum.ExchangeBill)
                        {
                            await this.NavigateAsync("ExchangeBillPage", ("Reference", this.PageName),
                                ("DispatchItemModel", item),
                                ("Bill", null));
                        }
                        else if (item.BillTypeId == (int)BillTypeEnum.SaleReservationBill)
                        {
                            await this.NavigateAsync("SaleBillPage", ("Reference", this.PageName),
                                ("DispatchItemModel", item),
                                ("Bill", null));
                        }
                        else if (item.BillTypeId == (int)BillTypeEnum.ReturnReservationBill)
                        {
                            await this.NavigateAsync("ReturnBillPage", ("Reference", this.PageName),
                                ("DispatchItemModel", item),
                                 ("Bill", null));
                        }
                    }

                    /*
                    var result = await CrossDiaglogKit.Current.GetVerificationCodeAsync("签收码", "", Keyboard.Numeric, defaultValue: "", placeHolder: "请输入6位验证码...");
                    if (!string.IsNullOrEmpty(result))
                    {
                        if (result.Equals("123456"))
                        {
                            using (UserDialogs.Instance.Loading("稍等..."))
                            {
                                if (item.BillTypeId == (int)BillTypeEnum.ExchangeBill)
                                {
                                    this.NavigateAsync("ExchangeBillPage", ("Reference", this.PageName),
                                        ("DispatchItemModel", item),
                                        ("Bill", null));
                                }
                                else if (item.BillTypeId == (int)BillTypeEnum.SaleReservationBill)
                                {
                                    this.NavigateAsync("SaleBillPage", ("Reference", this.PageName),
                                        ("DispatchItemModel", item),
                                        ("Bill", null));
                                }
                                else if (item.BillTypeId == (int)BillTypeEnum.ReturnReservationBill)
                                {
                                    this.NavigateAsync("ReturnBillPage", ("Reference", this.PageName),
                                        ("DispatchItemModel", item),
                                         ("Bill", null));
                                }
                            }
                        }
                        else
                        {
                            this.Alert("验证码错误！");
                        }
                    }
                    */
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

            }, string.Format(Constants.MENU_DEV_KEY, 0));



            this.BindBusyCommand(Load);

        }
        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
