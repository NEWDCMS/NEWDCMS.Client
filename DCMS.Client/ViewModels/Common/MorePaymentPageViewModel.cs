using DCMS.Client.CustomViews;
using DCMS.Client.Enums;
using DCMS.Client.Models;
using DCMS.Client.Models.Settings;
using DCMS.Client.Services;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace DCMS.Client.ViewModels
{
    public class MorePaymentPageViewModel : ViewModelBase
    {

        private readonly IAccountingService _accountingService;
        [Reactive] public AccountingOption AdvancePayment { get; set; } = new AccountingOption();
        [Reactive] public ObservableCollection<TreeViewNode> RootNodes { get; set; } = new ObservableCollection<TreeViewNode>();


        public MorePaymentPageViewModel(INavigationService navigationService,
            IAccountingService accountingService,
              IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "选择支付方式";
            _navigationService = navigationService;
            _dialogService = dialogService;
            _accountingService = accountingService;

            //加载配置
            this.Load = AccountLoader.Load(async () =>
            {
                var results = await Sync.RunResult(async () =>
                {
                    var result = await _accountingService.GetPaymentMethodsAsync((int)BillType, this.ForceRefresh);
                    //绑定Accounts
                    var options = new List<AccountingModel>();
                    switch (BillType)
                    {
                        case BillTypeEnum.SaleBill:
                            {
                                options = result?.Select(a =>
                                {
                                    return new AccountingModel()
                                    {
                                        Default = a.IsDefault,
                                        AccountingOptionId = a.Id,
                                        AccountCodeTypeId = a.AccountCodeTypeId,
                                        Selected = PaymentMethods.Selectes.Where(s => s.AccountCodeTypeId == a.Number).FirstOrDefault()?.Selected ?? false,
                                        Name = a.Name,
                                        CollectionAmount = PaymentMethods.Selectes.Where(s => s.AccountingOptionId == a.Id).Select(s => s.CollectionAmount).FirstOrDefault(),
                                        ParentId = a.ParentId,
                                        Number = a.Number
                                    };
                                }).ToList();

                            }
                            break;
                        case BillTypeEnum.SaleReservationBill:
                            {
                                options = result?.Select(a =>
                                {
                                    return new AccountingModel()
                                    {
                                        Default = a.IsDefault,
                                        AccountingOptionId = a.Id,
                                        AccountCodeTypeId = a.AccountCodeTypeId,
                                        Selected = PaymentMethods.Selectes.Where(s => s.AccountCodeTypeId == a.Number).FirstOrDefault()?.Selected ?? false,
                                        Name = a.Name,
                                        CollectionAmount = PaymentMethods.Selectes.Where(s => s.AccountingOptionId == a.Id).Select(s => s.CollectionAmount).FirstOrDefault(),
                                        ParentId = a.ParentId,
                                        Number = a.Number
                                    };
                                }).ToList();
                            }
                            break;
                        case BillTypeEnum.ReturnBill:
                            {
                                options = result?.Select(a =>
                                {
                                    return new AccountingModel()
                                    {
                                        Default = a.IsDefault,
                                        AccountingOptionId = a.Id,
                                        AccountCodeTypeId = a.AccountCodeTypeId,
                                        Selected = PaymentMethods.Selectes.Where(s => s.AccountCodeTypeId == a.Number).FirstOrDefault()?.Selected ?? false,
                                        Name = a.Name,
                                        CollectionAmount = PaymentMethods.Selectes.Where(s => s.AccountingOptionId == a.Id).Select(s => s.CollectionAmount).FirstOrDefault(),
                                        ParentId = a.ParentId,
                                        Number = a.Number
                                    };
                                }).ToList();
                            }
                            break;
                        case BillTypeEnum.ReturnReservationBill:
                            {
                                options = result?.Select(a =>
                                {
                                    return new AccountingModel()
                                    {
                                        Default = a.IsDefault,
                                        AccountingOptionId = a.Id,
                                        AccountCodeTypeId = a.AccountCodeTypeId,
                                        Selected = PaymentMethods.Selectes.Where(s => s.AccountCodeTypeId == a.Number).FirstOrDefault()?.Selected ?? false,
                                        Name = a.Name,
                                        CollectionAmount = PaymentMethods.Selectes.Where(s => s.AccountingOptionId == a.Id).Select(s => s.CollectionAmount).FirstOrDefault(),
                                        ParentId = a.ParentId,
                                        Number = a.Number
                                    };
                                }).ToList();
                            }
                            break;
                        case BillTypeEnum.CashReceiptBill:
                            {
                                options = result?.Select(a =>
                                {
                                    return new AccountingModel()
                                    {
                                        Default = a.IsDefault,
                                        AccountingOptionId = a.Id,
                                        AccountCodeTypeId = a.AccountCodeTypeId,
                                        Selected = PaymentMethods.Selectes.Where(s => s.AccountCodeTypeId == a.Number).FirstOrDefault()?.Selected ?? false,
                                        Name = a.Name,
                                        CollectionAmount = PaymentMethods.Selectes.Where(s => s.AccountingOptionId == a.Id).Select(s => s.CollectionAmount).FirstOrDefault(),
                                        ParentId = a.ParentId,
                                        Number = a.Number
                                    };
                                }).ToList();
                            }
                            break;
                        case BillTypeEnum.PaymentReceiptBill:
                            {
                                options = result?.Select(a =>
                                {
                                    return new AccountingModel()
                                    {
                                        Default = a.IsDefault,
                                        AccountingOptionId = a.Id,
                                        AccountCodeTypeId = a.AccountCodeTypeId,
                                        Selected = PaymentMethods.Selectes.Where(s => s.AccountCodeTypeId == a.Number).FirstOrDefault()?.Selected ?? false,
                                        Name = a.Name,
                                        CollectionAmount = PaymentMethods.Selectes.Where(s => s.AccountingOptionId == a.Id).Select(s => s.CollectionAmount).FirstOrDefault(),
                                        ParentId = a.ParentId,
                                        Number = a.Number
                                    };
                                }).ToList();
                            }
                            break;
                        case BillTypeEnum.AdvanceReceiptBill:
                            {
                                options = result?.Select(a =>
                                {
                                    return new AccountingModel()
                                    {
                                        Default = a.IsDefault,
                                        AccountingOptionId = a.Id,
                                        AccountCodeTypeId = a.AccountCodeTypeId,
                                        Selected = PaymentMethods.Selectes.Where(s => s.AccountCodeTypeId == a.Number).FirstOrDefault()?.Selected ?? false,
                                        Name = a.Name,
                                        CollectionAmount = PaymentMethods.Selectes.Where(s => s.AccountingOptionId == a.Id).Select(s => s.CollectionAmount).FirstOrDefault(),
                                        ParentId = a.ParentId,
                                        Number = a.Number
                                    };
                                }).ToList();
                            }
                            break;
                        case BillTypeEnum.AdvancePaymentBill:
                            {
                                options = result?.Select(a =>
                                {
                                    return new AccountingModel()
                                    {
                                        Default = a.IsDefault,
                                        AccountingOptionId = a.Id,
                                        AccountCodeTypeId = a.AccountCodeTypeId,
                                        Selected = PaymentMethods.Selectes.Where(s => s.AccountCodeTypeId == a.Number).FirstOrDefault()?.Selected ?? false,
                                        Name = a.Name,
                                        CollectionAmount = PaymentMethods.Selectes.Where(s => s.AccountingOptionId == a.Id).Select(s => s.CollectionAmount).FirstOrDefault(),
                                        ParentId = a.ParentId,
                                        Number = a.Number
                                    };
                                }).ToList();
                            }
                            break;
                        case BillTypeEnum.PurchaseBill:
                            {
                                options = result?.Select(a =>
                                {
                                    var p = PaymentMethods.Selectes.Where(s => s.AccountingOptionId == a.Id).FirstOrDefault();
                                    return new AccountingModel()
                                    {
                                        Default = a.IsDefault,
                                        AccountingOptionId = a.Id,
                                        Selected = PaymentMethods.Selectes.Where(s => s.AccountCodeTypeId == a.Number).FirstOrDefault()?.Selected ?? false,
                                        AccountCodeTypeId = a.AccountCodeTypeId,
                                        Name = a.Name,
                                        CollectionAmount = p != null ? p.CollectionAmount : 0,
                                        Balance = (int)AccountingCodeEnum.AdvancePayment == a.AccountCodeTypeId ? AdvancePayment.Balance : 0,
                                        ParentId = a.ParentId,
                                        Number = a.Number
                                    };
                                }).ToList();
                            }
                            break;
                        case BillTypeEnum.PurchaseReturnBill:
                            {
                                options = result?.Select(a =>
                                {
                                    return new AccountingModel()
                                    {
                                        Default = a.IsDefault,
                                        AccountingOptionId = a.Id,
                                        AccountCodeTypeId = a.AccountCodeTypeId,
                                        Selected = PaymentMethods.Selectes.Where(s => s.AccountingOptionId == a.Id).Any(),
                                        Name = a.Name,
                                        CollectionAmount = PaymentMethods.Selectes.Where(s => s.AccountingOptionId == a.Id).Select(s => s.CollectionAmount).FirstOrDefault(),
                                        ParentId = a.ParentId,
                                        Number = a.Number
                                    };
                                }).ToList();
                            }
                            break;
                        case BillTypeEnum.CostExpenditureBill:
                            {
                                options = result?.Select(a =>
                                {
                                    return new AccountingModel()
                                    {
                                        Default = a.IsDefault,
                                        AccountingOptionId = a.Id,
                                        AccountCodeTypeId = a.AccountCodeTypeId,
                                        Selected = PaymentMethods.Selectes.Where(s => s.AccountCodeTypeId == a.Number).FirstOrDefault()?.Selected ?? false,
                                        Name = a.Name,
                                        CollectionAmount = PaymentMethods.Selectes.Where(s => s.AccountingOptionId == a.Id).Select(s => s.CollectionAmount).FirstOrDefault(),
                                        ParentId = a.ParentId,
                                        Number = a.Number
                                    };
                                }).ToList();
                            }
                            break;
                        case BillTypeEnum.FinancialIncomeBill:
                            {
                                options = result?.Select(a =>
                                {
                                    return new AccountingModel()
                                    {
                                        Default = a.IsDefault,
                                        AccountingOptionId = a.Id,
                                        AccountCodeTypeId = a.AccountCodeTypeId,
                                        Name = a.Name,
                                        Selected = PaymentMethods.Selectes.Where(s => s.AccountCodeTypeId == a.Number).FirstOrDefault()?.Selected ?? false,
                                        CollectionAmount = PaymentMethods.Selectes.Where(s => s.AccountingOptionId == a.Id).Select(s => s.CollectionAmount).FirstOrDefault(),
                                        ParentId = a.ParentId,
                                        Number = a.Number
                                    };
                                }).ToList();
                            }
                            break;
                    }
                    return options;
                });

                //初始
                Accounts = results.Result;

                //RootNodes
                var rootNodes = ProcessXamlItemGroups(GroupData(results.Result));
                //foreach (var node in rootNodes)
                //{
                //    //StackLayout
                //    var xamlItemGroup = (XamlItemGroup)node.BindingContext;
                //}

                this.RootNodes = rootNodes;

                MessagingCenter.Send(rootNodes, Constants.MorePayments);

                return rootNodes.ToList();
            });


            this.BindBusyCommand(Load);

        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }

        /// <summary>
        /// 保存支付
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
                        var checks = new List<XamlItem>();
                        var contents = RootNodes.Select(s => s.Content);
                        foreach (var cc in contents)
                        {
                            if (cc is StackLayout st)
                            {
                                if (st.BindingContext is XamlItemGroup vmg)
                                {
                                    vmg.XamlItems.ForEach(vm =>
                                    {
                                        if (vm.Selected)
                                            checks.Add(vm);
                                    });
                                }

                            }
                        }

                        bool rept = false;
                        var grp = checks.GroupBy(s => s.ParentId);
                        foreach (var g in grp)
                        {
                            if (g.ToList().Count > 1)
                            {
                                rept = true;
                                break;
                            }
                        }
                        if (rept)
                        {
                            this.Alert("同科目子类型只能选择一种.");
                            return;
                        }

                        var accs = Accounts.Where(s => checks.Select(s => s.ItemId).Contains(s.Number)).ToList();
                        PaymentMethods.Selectes = new ObservableCollection<AccountingModel>(accs);
                        if (accs.Count() == 0)
                        {
                            this.Alert("请选择支付方式.");
                            return;
                        }

                        if (accs.Count() > 2)
                        {
                            this.Alert("最多支持2种支付方式.");
                            return;
                        }

                        await _navigationService.GoBackAsync(("Selectes", accs));
                    });
                }
                return _saveCommand;
            }
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("BillType"))
            {
                parameters.TryGetValue<BillTypeEnum>("BillType", out BillTypeEnum billType);
                BillType = billType;
                ((ICommand)Load)?.Execute(null);
            }

            // PaymentMethodPage 回传
            if (parameters.ContainsKey("Selectes"))
            {
                parameters.TryGetValue<List<AccountingModel>>("Selectes", out List<AccountingModel> paymentmethods);
                if (paymentmethods != null)
                {
                    this.PaymentMethods.Selectes = new ObservableCollection<AccountingModel>(paymentmethods);
                }
            }

        }



        #region TreeView


        public XamlItemGroup GroupData(IList<AccountingModel> accountingOptions)
        {
            var accountings = accountingOptions.OrderBy(x => x.ParentId);
            var accountingGroup = new XamlItemGroup();
            foreach (var dept in accountings)
            {
                var itemGroup = new XamlItemGroup
                {
                    Name = dept.Name,
                    GroupId = dept.Number
                };

                if (dept.ParentId <= 0)
                {
                    accountingGroup.Children.Add(itemGroup);
                }
                else
                {
                    XamlItemGroup parentGroup = null;
                    foreach (var group in accountingGroup.Children)
                    {
                        parentGroup = FindParent(group, dept);

                        if (parentGroup != null)
                        {
                            if (parentGroup.Children.Count == 0)
                            {
                                var item = new XamlItem
                                {
                                    ItemId = dept.Number,
                                    ParentId = dept.ParentId,
                                    Key = dept.Name,
                                    SelectedCommand = ReactiveCommand.Create<XamlItem>(r =>
                                    {
                                        if (r == null) return;
                                        //r.Selected = r.Selected;
                                        return;
                                    })
                                };
                                parentGroup.XamlItems.Add(item);
                            }
                            else
                            {
                                parentGroup.Children.Add(itemGroup);
                                break;
                            }
                        }
                    }
                }

            }
            return accountingGroup;
        }
        public XamlItemGroup FindParent(XamlItemGroup group, AccountingModel account)
        {
            if (group.GroupId == account.ParentId)
                return group;

            if (group.Children != null)
            {
                foreach (var currentGroup in group.Children)
                {
                    var search = FindParent(currentGroup, account);

                    if (search != null)
                        return search;
                }
            }
            return null;
        }

        private static TreeViewNode CreateTreeViewNode(object bindingContext, Label label, CheckBox checkBox, bool isItem)
        {
            var node = new TreeViewNode
            {
                BindingContext = bindingContext,
                Content = new StackLayout
                {
                    Children =
                    {
                        new ResourceImage
                        {
                            Resource = isItem? "DCMS.Client.Resources.Item.png" :"DCMS.Client.Resources.FolderOpen.png" ,
                            HeightRequest= 16,
                            WidthRequest = 16
                        },
                        label,
                        checkBox
                    },
                    Orientation = StackOrientation.Horizontal
                }
            };

            //为展开按钮内容设置数据模板
            node.ExpandButtonTemplate = new DataTemplate(() => new ExpandButtonContent { BindingContext = node });

            return node;
        }
        private class ExpandButtonContent : ContentView
        {

            protected override void OnBindingContextChanged()
            {
                base.OnBindingContextChanged();

                var node = (BindingContext as TreeViewNode);
                bool isLeafNode = (node.Children == null || node.Children.Count == 0);


                if ((isLeafNode) && !node.ShowExpandButtonIfEmpty)
                {
                    Content = new ResourceImage
                    {
                        Resource = isLeafNode ? "DCMS.Client.Resources.Blank.png" : "DCMS.Client.Resources.FolderOpen.png",
                        HeightRequest = 16,
                        WidthRequest = 16
                    };
                }
                else
                {
                    Content = new ResourceImage
                    {
                        Resource = node.IsExpanded ? "DCMS.Client.Resources.OpenGlyph.png" : "DCMS.Client.Resources.CollpsedGlyph.png",
                        HeightRequest = 16,
                        WidthRequest = 16
                    };
                }
            }

        }

        /// <summary>
        /// 递归结构
        /// </summary>
        /// <param name="xamlItemGroups"></param>
        /// <returns></returns>
        private static ObservableCollection<TreeViewNode> ProcessXamlItemGroups(XamlItemGroup xamlItemGroups)
        {
            var rootNodes = new ObservableCollection<TreeViewNode>();

            foreach (var xamlItemGroup in xamlItemGroups.Children.OrderBy(xig => xig.Name))
            {

                var label = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Color.Black,
                    Text = xamlItemGroup.Name
                };
                label.SetBinding(Label.TextProperty, "Name");


                label.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() =>
                    {
                        //
                    })
                });

                var checkBox = new CheckBox()
                {
                    Margin = new Thickness(10, 5, 10, 0),
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    IsVisible = false
                };


                checkBox.SetBinding(CheckBox.IsCheckedProperty, "Selected", mode: BindingMode.TwoWay);


                var groupTreeViewNode = CreateTreeViewNode(xamlItemGroup, label, checkBox, false);

                rootNodes.Add(groupTreeViewNode);

                //添加Children
                groupTreeViewNode.Children = ProcessXamlItemGroups(xamlItemGroup);

                foreach (var xamlItem in xamlItemGroup.XamlItems)
                {
                    CreateXamlItem(groupTreeViewNode.Children, xamlItem);
                }

            }

            return rootNodes;
        }
        private static void CreateXamlItem(IList<TreeViewNode> children, XamlItem xamlItem)
        {
            var label = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.Black,
                Text = xamlItem.Key
            };
            label.SetBinding(Label.TextProperty, "Key");

            label.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    xamlItem.Selected = !xamlItem.Selected;
                })
            });

            var checkBox = new CheckBox()
            {
                Margin = new Thickness(10, 5, 10, 0),
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            checkBox.SetBinding(CheckBox.IsCheckedProperty, "Selected", mode: BindingMode.TwoWay);

            var xamlItemTreeViewNode = CreateTreeViewNode(xamlItem, label, checkBox, true);
            children.Add(xamlItemTreeViewNode);
        }

        #endregion
    }
}
