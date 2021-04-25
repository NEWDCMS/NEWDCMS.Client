using Wesley.Client.Services;

using Prism.Navigation;
using ReactiveUI;

using System;
using System.Reactive;

namespace Wesley.Client.ViewModels
{
    public class FilterPageViewModel : ViewModelBaseCutom
    {
        public IReactiveCommand CancelCommand { get; set; }
        public ReactiveCommand<object, Unit> CheckCommand { get; }
        public ReactiveCommand<object, Unit> CustomerSelected { get; }
        public ReactiveCommand<object, Unit> UserSelected { get; }
        public ReactiveCommand<object, Unit> CatagorySelected { get; }
        public ReactiveCommand<object, Unit> ProductSelected { get; }


        public FilterPageViewModel(INavigationService navigationService,
           IProductService productService,
           IUserService userService,
           ITerminalService terminalService,
           IWareHousesService wareHousesService,
           IAccountingService accountingService,
            IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {

            Title = "筛选";
            _navigationService = navigationService;
            _dialogService = dialogService;
            _terminalService = terminalService;
            _productService = productService;
            _userService = userService;
            _wareHousesService = wareHousesService;
            _accountingService = accountingService;

            this.CancelCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await _navigationService.GoBackAsync();
            });

            this.CheckCommand = ReactiveCommand.CreateFromTask<object>(async e =>
            {
                await _navigationService.GoBackAsync(("Filter", Filter));
            });

            this.CustomerSelected = ReactiveCommand.Create<object>(async e =>
          {
              await this.NavigateAsync("SelectCustomerPage", ("Filter", Filter),
                  ("Reference", "FilterPage"));
          });

            //用户选择
            this.UserSelected = ReactiveCommand.Create<object>(async e =>
           {
               await SelectUser((data) =>
                {
                    Filter.BusinessUserId = data.Id;
                    Filter.BusinessUserName = data.Column;
                }, Enums.UserRoleType.Employees, true);
           });
            this.CatagorySelected = ReactiveCommand.Create<object>(async e =>
           {
               await SelectCatagory((data) =>
                {
                    Filter.CatagoryId = data.Id;
                    Filter.CatagoryName = data.Name;
                });
           });
            this.ProductSelected = ReactiveCommand.Create<object>(async e =>
          {
              await this.NavigateAsync("SelectProductPage", ("Filter", Filter),
                  ("Reference", "FilterPage"),
                  ("SerchKey", Filter.SerchKey));
          });

            this.CheckCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.CustomerSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.UserSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.CatagorySelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.ProductSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.CancelCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
        }

        /// <summary>
        /// 完成后接收
        /// </summary>
        /// <param name="parameters"></param>
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
