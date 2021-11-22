using Acr.UserDialogs;
using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Pages.Market;
using Wesley.Client.Services;
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

namespace Wesley.Client.ViewModels
{
    public class AllfunPageViewModel : ViewModelBaseCutom
    {
        public ReactiveCommand<Module, Unit> InvokeAppCommand { get; }
        [Reactive] public ObservableCollection<ModuleGroup> Modules { get; private set; } = new ObservableCollection<ModuleGroup>();
        [Reactive] public bool ShowBack { get; set; } = false;
        [Reactive] public Xamarin.Forms.Thickness NavPadding { get; set; } = new Xamarin.Forms.Thickness(0, 20, 0, 0);
        [Reactive] public Xamarin.Forms.Rectangle NavRectangle { get; set; } = new Xamarin.Forms.Rectangle(0, 0, 1, 65);

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
            Title = "全部";

            this.Load = ReactiveCommand.Create(() =>
            {
                try
                {
                    var pending = new List<ModuleGroup>();
                    var gModules = GlobalSettings.AppDatas?.GroupBy(s => s.AType)?.ToList();
                    if (gModules != null && gModules.Any())
                    {
                        foreach (var group in gModules)
                        {
                            var first = group.FirstOrDefault();
                            if (first != null)
                            {
                                if (first.AType == 0)
                                    first.ATypeName = "单据类";
                                else if (first.AType == 1)
                                    first.ATypeName = "报表类";
                                else if (first.AType == 2)
                                    first.ATypeName = "市场类";
                                else if (first.AType == 3)
                                    first.ATypeName = "档案类";

                                if (group != null && group.Any())
                                    pending.Add(new ModuleGroup(first.ATypeName, group.ToList()));
                            }
                        }
                    }

                    if (pending.Any())
                        this.Modules = new ObservableCollection<ModuleGroup>(pending);
                }
                catch (Exception)
                {

                }
            });

            this.InvokeAppCommand = ReactiveCommand.CreateFromTask<Module>(async r =>
            {
                if (!IsFastClick())
                    return;

                await this.Access(r, AccessStateEnum.View, async () =>
                {
                    using (UserDialogs.Instance.Loading("加载中..."))
                    {
                        if (r.Navigation == "VisitStorePage")
                        {
                            var check = await CheckSignIn();
                            if (!check)
                            {
                                await this.NavigateAsync($"{nameof(CurrentCustomerPage)}");
                                return;
                            }
                        }
                        await this.NavigateAsync(r.Navigation, ("Reference", this.PageName));
                    }
                });
            });

            this.BindBusyCommand(Load);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (ReferencePage.Equals("HomePage"))
            {
                ShowBack = true;
                NavPadding = new Xamarin.Forms.Thickness(0, 0, 0, 0);
                NavRectangle = new Xamarin.Forms.Rectangle(0, 0, 0, 0);
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ThrottleLoad(() =>
            {
                ((ICommand)Load)?.Execute(null);
            }, (this.Modules?.Count ?? 0) == 0);
        }
    }
}
