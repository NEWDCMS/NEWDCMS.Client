using Wesley.Client.Models.Products;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;

namespace Wesley.Client.ViewModels
{
    public class AddCostContractProductPageViewModel : ViewModelBase
    {
        public AddCostContractProductPageViewModel(INavigationService navigationService,


            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "添加费用合同商品";
            _navigationService = navigationService;
            _dialogService = dialogService;
        }

        /// <summary>
        /// 保存
        /// </summary>
        private DelegateCommand<object> _saveCommand;
        public new DelegateCommand<object> SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new DelegateCommand<object>(async (e) =>
                    {
                        try
                        {
                            bool vaild = false;

                            vaild = (Product.BigPriceUnit.Quantity == 0 && Product.SmallPriceUnit.Quantity == 0);
                            if (vaild)
                            {
                                this.Alert("单位数量不能为空");
                                return;
                            }

                            if (!string.IsNullOrEmpty(ReferencePage))
                            {
                                //转向引用页
                                var redirectPage = $"../../";
                                await this.NavigateAsync(redirectPage, ("ProductSeries", new List<ProductModel> { Product }));
                            }
                            else
                            {
                                await _navigationService.GoBackAsync(("ProductSeries", new List<ProductModel> { Product }));
                            }
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                        }
                    });
                }
                return _saveCommand;
            }
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

    }
}
