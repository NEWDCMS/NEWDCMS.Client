using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Finances;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;

using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Wesley.Client.ViewModels
{
    public class AddCostPageViewModel : ViewModelBaseCutom
    {
        [Reactive] public CostExpenditureItemModel Model { get; set; } = new CostExpenditureItemModel();
        public ReactiveCommand<object, Unit> RemoveCommand { get; }
        public ReactiveCommand<object, Unit> ContractSelected { get; }
        public ReactiveCommand<object, Unit> AccountingSelected { get; }
        [Reactive] public bool IsRemoveing { get; set; } = false;

        public AddCostPageViewModel(INavigationService navigationService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,


            IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "添加费用";

            //验证
            var valid_TerminalId = this.ValidationRule(x => x.Model.CustomerId, _isZero, "客户未指定");
            var valid_AccountingOptionId = this.ValidationRule(x => x.Model.AccountingOptionId, _isZero, "类别未指定");
            var valid_Amount = this.ValidationRule(x => x.Model.Amount, _isDZero, "金额未指定");

            //费用选择
            this.AccountingSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectCostAccounting((data) =>
                 {
                     this.Accounting = data;

                     Model.AccountingOptionId = data.AccountingOptionId;
                     Model.AccountingOptionName = data.Name;

                     Filter.AccountOptionId = data.AccountingOptionId;

                 }, BillTypeEnum.CostContractBill);
            });


            //合同选择
            this.ContractSelected = ReactiveCommand.Create<object>(async e =>
           {
               if (!valid_TerminalId.IsValid) { this.Alert(valid_TerminalId.Message[0]); return; }
               if (!valid_AccountingOptionId.IsValid) { this.Alert(valid_AccountingOptionId.Message[0]); return; }

               await this.NavigateAsync("SelectContractPage",
                   ("Accounting", new AccountingModel()
                   {
                       AccountingOptionId = Model.AccountingOptionId,
                       AccountingOptionName = Model.AccountingOptionName
                   }),
                   ("Terminaler", new TerminalModel()
                   {
                       Id = Model.CustomerId,
                       Name = Model.CustomerName
                   }));
           });

            //合同余额
            this.WhenAnyValue(x => x.Model.Balance).Subscribe(s =>
            {
                this.Model.ShowBalance = s.HasValue && s.Value > 0;

            }).DisposeWith(this.DeactivateWith);


            this.WhenAnyValue(x => x.Model.AccountingOptionId).Subscribe(s => { IsRemoveing = s > 0; }).DisposeWith(this.DeactivateWith);


            //保存
            this.SaveCommand = ReactiveCommand.CreateFromTask<object>(async e =>
            {
                if (!valid_TerminalId.IsValid) { this.Alert(valid_TerminalId.Message[0]); return; }
                if (!valid_AccountingOptionId.IsValid) { this.Alert(valid_AccountingOptionId.Message[0]); return; }
                if (!valid_Amount.IsValid) { this.Alert(valid_Amount.Message[0]); return; }

                if (Model.CostContractId > 0 && Model.Balance == 0) { this.Alert("余额不足!"); return; }

                if (Model.CostContractId > 0)
                    Model.Amount = Model.Balance;

                await _navigationService.GoBackAsync(("CostExpenditure", Model));
            });

            //移除
            this.RemoveCommand = ReactiveCommand.CreateFromTask<object>(async e =>
            {
                var selecter = e as AddCostPageViewModel;
                await _navigationService.GoBackAsync(("RemoveCostExpenditure", selecter.Model));
            });

            this.ContractSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.AccountingSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.ExceptionsSubscribe();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                //编辑回传
                if (parameters.ContainsKey("Selecter"))
                {
                    parameters.TryGetValue("Selecter", out CostExpenditureItemModel costContract);
                    if (costContract != null)
                    {
                        IsRemoveing = true;
                        Model.CostContractId = costContract.CostContractId;
                        Model.CostContractName = costContract.CostContractName;
                        Model.Amount = costContract.Amount;
                        Model.Balance = costContract.Balance;
                        Model.ShowBalance = costContract.Balance > 0;
                        Model.Remark = costContract.Remark;
                        Model.AccountingOptionId = costContract.AccountingOptionId;
                        Model.AccountingOptionName = costContract.AccountingOptionName;
                    }
                }

                //客户回传
                if (parameters.ContainsKey("Terminaler"))
                {
                    Model.CustomerId = this.Terminal.Id;
                    Model.CustomerName = this.Terminal.Name;
                }

                //合同选择回传
                if (parameters.ContainsKey("CostContract"))
                {
                    parameters.TryGetValue("CostContract", out CostContractBindingModel costContract);
                    if (costContract != null)
                    {
                        Model.CostContractId = costContract.Id;
                        Model.CostContractName = costContract.BillNumber;
                        Model.Amount = costContract.Amount;
                        Model.Balance = costContract.Balance;
                        Model.ShowBalance = costContract.Balance > 0;
                        Model.Month = costContract.Month;
                        Model.Remark = $"{costContract.Year}年{costContract.Month}月合同";
                        Model.AccountingOptionId = costContract.AccountingOptionId;
                        Model.AccountingOptionName = costContract.AccountingOptionName;
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
        }

    }
}
