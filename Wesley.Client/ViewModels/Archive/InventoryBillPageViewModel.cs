using Acr.UserDialogs;
using Wesley.Client.Enums;
using Wesley.Client.Models.WareHouses;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
namespace Wesley.Client.ViewModels
{
    public class InventoryBillPageViewModel : ViewModelBaseCutom
    {
        private readonly IInventoryService _inventoryService;
        [Reactive] public WareHouseModel Selecter { get; set; }



        public InventoryBillPageViewModel(INavigationService navigationService,
           IProductService productService,
           ITerminalService terminalService,
           IUserService userService,
           IWareHousesService wareHousesService,
           IAccountingService accountingService,
           IInventoryService inventoryService,
           IDialogService dialogService) : base(navigationService, productService,
               terminalService,
               userService,
               wareHousesService,
               accountingService,
               dialogService)
        {

            Title = "选择盘点库存";

            _inventoryService = inventoryService;


            //载入仓库
            this.Load = WareHousesLoader.Load(async () =>
            {
                try
                {
                    var result = await _wareHousesService.GetWareHousesAsync(BillTypeEnum.InventoryAllTaskBill, force: this.ForceRefresh, calToken: new System.Threading.CancellationToken());
                    this.WareHouses = new ObservableCollection<WareHouseModel>(result?.ToList());
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                return WareHouses.ToList();
            });

            //开始盘点
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
             .Skip(1)
             .Where(x => x != null)
             .SubOnMainThread(async wareHouse =>
             {
                 if (wareHouse != null)
                 {
                     var pendings = await _inventoryService.CheckInventoryAsync(wareHouse.Id, new System.Threading.CancellationToken());
                     if (pendings != null && pendings.Count > 0)
                     {
                         await UserDialogs.Instance.AlertAsync("库存正在盘点中，不能在生成盘点单.", okText: "确定");
                         return;
                     }
                     //转向盘点
                     await this.NavigateAsync("SelectProductPage", ("Reference", this.PageName), ("WareHouse", wareHouse), ("SerchKey", ""));
                 }
             })
             .DisposeWith(DeactivateWith);

            this.BindBusyCommand(Load);

        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
