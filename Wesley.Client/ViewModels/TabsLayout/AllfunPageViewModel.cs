using Acr.UserDialogs;
using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Pages.Market;
using Wesley.Client.Services;

using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class AllfunPageViewModel : ViewModelBaseCutom
    {
        public ReactiveCommand<Module, Unit> InvokeAppCommand { get; }

        [Reactive] public IList<ModuleGroup> Modules { get; private set; } = new ObservableCollection<ModuleGroup>();

        public AllfunPageViewModel(INavigationService navigationService,
                  IProductService productService,
                  IUserService userService,
                  ITerminalService terminalService,
                  IWareHousesService wareHousesService,
                  IAccountingService accountingService,


                  IDialogService dialogService) : base(navigationService,
                      productService,
                      terminalService,
                      userService,
                      wareHousesService,
                      accountingService,
                      dialogService)
        {
            Title = "全部功能";


            this.Load = ModulesLoader.Load(async () =>
           {
               if (this.Modules.Any())
                   this.Modules.Clear();

               var gModules = GlobalSettings.AppDatas?.GroupBy(s => s.AType)?.ToList();
               if (gModules != null && gModules.Any())
               {
                   foreach (var group in gModules)
                   {
                       var first = group.FirstOrDefault();

                       if (first.AType == 0)
                           first.ATypeName = "单据类";
                       else if (first.AType == 1)
                           first.ATypeName = "报表类";
                       else if (first.AType == 2)
                           first.ATypeName = "市场类";
                       else if (first.AType == 3)
                           first.ATypeName = "档案类";

                       if (first != null)
                       {
                           if (group != null && group.Any())
                               Modules.Add(new ModuleGroup(first.ATypeName, group.ToList()));
                       }
                   }
               }

               if (this.Modules.Any())
                   return await Task.FromResult(this.Modules?.ToList());
               else
                   return await Task.FromResult(new ObservableCollection<ModuleGroup>());
           });

            this.InvokeAppCommand = ReactiveCommand.Create<Module>(async r =>
            {
                if (!IsFastClick())
                    return;

                await this.Access(r, AccessStateEnum.View, async () =>
                {
                    using (UserDialogs.Instance.Loading("加载中..."))
                    {
                        if (r.Navigation == "VisitStorePage")
                        {
                            var csg = await this.CheckSignIn();
                            if (!csg)
                            {
                                await this.NavigateAsync($"{nameof(CurrentCustomerPage)}");
                                return;
                            }
                        }
                        await this.NavigateAsync(r.Navigation);
                    }
                });
            });


            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
