using DCMS.Client.CustomViews;
using DCMS.Client.Models;
using DCMS.Client.Models.Products;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;


namespace DCMS.Client.ViewModels
{
    public class EditAllocationProductPageViewModel : ViewModelBase
    {
        /// <summary>
        /// 原单据商品数量
        /// </summary>
        public int OldProductQuantity { get; set; }
        public IReactiveCommand DeleteCommand { get; set; }
        public ReactiveCommand<int?, Unit> UnitSelected { get; set; }


        public EditAllocationProductPageViewModel(INavigationService navigationService, IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "修改商品信息";
            _navigationService = navigationService;
            _dialogService = dialogService;

            //Cannot convert null to 'int' because it is a non-nullable value type
            //保存
            this.SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await _navigationService.GoBackAsync(("UpdateProduct", Product));
            });

            //删除商品
            this.DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await _navigationService.GoBackAsync(("DelProduct", Product));
            });

            //单位选择
            this.UnitSelected = ReactiveCommand.CreateFromTask<int?>(async (r) =>
            {
                try
                {
                    int porductId = r ?? 0;
                    var product = this.Product;
                    var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("单位选择", "", (() =>
                    {
                        var popDatas = new List<PopData>();
                        if (product.Units != null && product.Units.Any())
                        {
                            var unt = product.Units.ToList();

                            if (unt.Count >= 1)
                            {
                                if (unt[0].Key != "SMALL")
                                {
                                    popDatas.Add(new PopData()
                                    {
                                        Id = unt[0].Value,
                                        Column = unt[0].Key,
                                        Column1 = "SMALL",
                                        Column1Enable = false,
                                        Data = unt[0].Value,
                                        Selected = this.Product.UnitAlias == "SMALL"
                                    });
                                }
                            }

                            if (unt.Count >= 2)
                            {
                                if (unt[1].Key != "STROK")
                                {
                                    popDatas.Add(new PopData()
                                    {
                                        Id = unt[1].Value,
                                        Column = unt[1].Key,
                                        Column1 = "STROK",
                                        Column1Enable = false,
                                        Data = unt[1].Value,
                                        Selected = this.Product.UnitAlias == "STROK"
                                    });
                                }
                            }

                            if (unt.Count >= 3)
                            {
                                if (unt[2].Key != "BIG")
                                {
                                    popDatas.Add(new PopData()
                                    {
                                        Id = unt[2].Value,
                                        Column = unt[2].Key,
                                        Column1 = "BIG",
                                        Column1Enable = false,
                                        Data = unt[2].Value,
                                        Selected = this.Product.UnitAlias == "BIG"
                                    });
                                }
                            }
                        }
                        return Task.FromResult(popDatas);
                    }));
                    if (result != null)
                    {
                        Product.UnitId = (int)result.Data;
                        Product.UnitName = result.Column;
                        product.UnitAlias = result.Column1;
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                //编辑商品回传
                if (parameters.ContainsKey("Product"))
                {
                    parameters.TryGetValue("Product", out ProductModel product);
                    if (product != null)
                    {
                        Product = product;
                        this.OldProductQuantity = product.Quantity;
                    }
                }
            }
            catch (Exception ex)
            {

                Crashes.TrackError(ex);
            }
        }


    }
}
