using Acr.UserDialogs;
using Wesley.Client.Models.Products;
using Wesley.Client.Services;
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
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class SelectManufacturerPageViewModel : ViewModelBase
    {
        private readonly IManufacturerService _manufacturerService;

        [Reactive] public ManufacturerModel Selecter { get; set; }

        public SelectManufacturerPageViewModel(INavigationService navigationService,
            IManufacturerService manufacturerService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "选择供应商";

            _navigationService = navigationService;
            _dialogService = dialogService;
            _manufacturerService = manufacturerService;

            //Load
            this.Load = ManufacturersLoader.Load(async () =>
            {
                //重载时排它
                ItemTreshold = 1;
                try
                {
                    //清除列表
                    Manufacturers.Clear();

                    var items = await GetManufacturersPage(0, PageSize);
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            if (Manufacturers.Count(s => s.Id == item.Id) == 0)
                            {
                                Manufacturers.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                this.Manufacturers = new ObservableRangeCollection<ManufacturerModel>(Manufacturers);


                return Manufacturers.ToList();
            });

            //以增量方式加载数据
            this.ItemTresholdReachedCommand = ReactiveCommand.Create(async () =>
            {

                using (var dig = UserDialogs.Instance.Loading("加载中..."))
                {
                    try
                    {
                        int pageIdex = Manufacturers.Count / (PageSize == 0 ? 1 : PageSize);
                        var items = await GetManufacturersPage(pageIdex, PageSize);
                        var previousLastItem = Terminals.Last();
                        if (items != null)
                        {
                            foreach (var item in items)
                            {
                                if (Manufacturers.Count(s => s.Id == item.Id) == 0)
                                {
                                    Manufacturers.Add(item);
                                }
                            }

                            if (items.Count() == 0 || items.Count() == Terminals.Count)
                            {
                                ItemTreshold = -1;
                                return Manufacturers.ToList();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                        ItemTreshold = -1;
                    }


                    this.Manufacturers = new ObservableRangeCollection<ManufacturerModel>(Manufacturers);
                    return Manufacturers.ToList();
                }

            }, this.WhenAny(x => x.Terminals, x => x.GetValue().Count > 0));


            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
            .Skip(1)
            .Where(x => x != null)
            .SubOnMainThread(async item =>
            {
                await _navigationService.GoBackAsync(("Manufacturer", item));
                this.Selecter = null;
            }).DisposeWith(DeactivateWith);

            this.BindBusyCommand(Load);

        }



        public async Task<IList<ManufacturerModel>> GetManufacturersPage(int pageNumber, int pageSize)
        {

            var manufacturers = new List<ManufacturerModel>();

            var result = await _manufacturerService.GetManufacturersAsync(this.ForceRefresh, new System.Threading.CancellationToken());

            if (result != null)
            {
                manufacturers = result.ToList();
                if (!string.IsNullOrEmpty(this.Filter.SerchKey))
                {
                    manufacturers = manufacturers.Where(c => c.Name.Contains(this.Filter.SerchKey) || c.ContactPhone.Contains(this.Filter.SerchKey)).ToList();
                }

            }
            return manufacturers;
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            if (this.Manufacturers.Count == 0)
                ((ICommand)Load)?.Execute(null);
        }
    }
}
