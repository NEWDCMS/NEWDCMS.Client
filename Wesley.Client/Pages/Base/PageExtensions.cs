using Wesley.Client.CustomViews;
using Wesley.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
namespace Wesley.Client.Pages
{
    /// <summary>
    /// 自定义工具栏菜单扩展
    /// </summary>
    public static class PageExtensions
    {
        private static readonly string Family = "FAS";

        /// <summary>
        /// 构成按钮项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Icon"></param>
        /// <param name="call"></param>
        /// <returns></returns>
        public static BindableToolbarItem BulidButton(string Icon, Action call)
        {
            var item = new BindableToolbarItem
            {
                Command = new Command(() =>
                {
                    call?.Invoke();
                }),
                Priority = 0,
                IconImageSource = new FontImageSource
                {
                    Glyph = Icon,
                    FontFamily = Family,
                    Size = 18
                }
            };
            return item;
        }


        /// <summary>
        /// 提交按钮
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static BindableToolbarItem GetSubmitItem<T>(T viewModel, string icon = "\uf0c7") where T : ViewModelBase
        {
            var item = BulidButton(icon, () =>
            {
                if (viewModel != null)
                {
                    var command = (ICommand)viewModel.SubmitDataCommand;
                    if (command.CanExecute(null))
                        command.Execute(null);
                    else
                    {
                        var msgs = viewModel.ValidationContext?.Text?.GetEnumerator();
                        while (msgs.MoveNext())
                        {
                            viewModel.Alert(msgs.Current);
                            return;
                        }
                    }
                }
            });

            item.SetBinding(BindableToolbarItem.IsVisibleProperty, new Binding("ShowSubmitBtn", BindingMode.TwoWay));
            item.SetBinding(BindableToolbarItem.IsEnabledProperty, new Binding("EnabledSubmitBtn", BindingMode.TwoWay));
            item.SetBinding(BindableToolbarItem.TextProperty, new Binding("SubmitText", BindingMode.TwoWay));
            return item;
        }

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static BindableToolbarItem GetAddItem<T>(T viewModel, string icon = "\xf055") where T : ViewModelBase
        {
            var item = BulidButton(icon, () =>
             {
                 if (viewModel != null)
                 {
                     var command = (ICommand)viewModel.AddCommand;
                     if (command.CanExecute(null))
                         command.Execute(null);
                     else
                         viewModel.Alert("操作包含未指定信息，请完善!");
                 }
             });
            return item;
        }

        /// <summary>
        /// 刷新按钮
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static BindableToolbarItem GetRefreshItem<T>(T viewModel) where T : ViewModelBase
        {
            return BulidButton("\uf021", () =>
            {
                if (viewModel != null)
                {
                    ((ICommand)viewModel.RefreshCommand)?.Execute(null);
                }
            });
        }


        /// <summary>
        /// 溢出按钮
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static BindableToolbarItem GetEllipsisItem(Action call)
        {
            return BulidButton("\uf142", call);
        }


        /// <summary>
        /// 溢出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="page"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static BindableToolbarItem GetMenusItem<T>(T viewModel, BaseContentPage<T> page) where T : ViewModelBase
        {
            return GetEllipsisItem(() =>
           {
               ((RightSideMasterPage)page.SlideMenu).SetBindMenus(viewModel.BindMenus, viewModel.GetType().FullName);
               page.ShowMenu();
           });
        }

        /// <summary>
        /// 溢出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static BindableToolbarItem GetMenusItem<T>(T viewModel, BaseTabbedPage<T> page) where T : ViewModelBase
        {
            return GetEllipsisItem(() =>
           {
               ((RightSideMasterPage)page.SlideMenu).SetBindMenus(viewModel.BindMenus, viewModel.GetType().FullName);
               page.ShowMenu();
           });
        }

        /// <summary>
        /// 提交单据+溢出菜单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <param name="showSubMit"></param>
        /// <returns></returns>
        public static IList<BindableToolbarItem> GetPrintToolBarItems<T>(this BaseContentPage<T> page, T viewModel, bool showSubMit = true) where T : ViewModelBase
        {
            var list = new List<BindableToolbarItem>
            {
                GetSubmitItem(viewModel, "交账"),
                GetPrintItem(viewModel)
            };
            return list;
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static BindableToolbarItem GetPrintItem<T>(T viewModel) where T : ViewModelBase
        {
            return BulidButton("\uf02f", () =>
            {
                if (viewModel != null)
                {
                    ((ICommand)viewModel.PrintCommand)?.Execute(null);
                }
            });
        }


        /// <summary>
        /// 历史
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static BindableToolbarItem GetHistoryItem<T>(T viewModel) where T : ViewModelBase
        {
            return BulidButton("\uf017", () =>
            {
                if (viewModel != null)
                {
                    ((ICommand)viewModel.HistoryCommand)?.Execute(null);
                }
            });
        }
        /// <summary>
        /// 筛选
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="page"></param>
        /// <param name="filterParameters"></param>
        /// <returns></returns>
        public static BindableToolbarItem GetFilterItem<T>(T viewModel, params (string, object)[] parameters) where T : ViewModelBase
        {
            return BulidButton("\uf0b0", async () =>
            {
                if (viewModel != null)
                {
                    await viewModel.NavigateAsync("FilterPage", parameters);
                }
            });
        }
        /// <summary>
        /// 扫描
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static BindableToolbarItem GetScanItem<T>(T viewModel) where T : ViewModelBase
        {
            return BulidButton("\uf029", () =>
            {
                if (viewModel != null) { ((ICommand)viewModel.ScanBarcodeCommand)?.Execute(null); }
            });
        }


        /// <summary>
        /// 提交单据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static IList<BindableToolbarItem> GetToolBarItems<T>(this BaseContentPage<T> page, T viewModel) where T : ViewModelBase
        {
            var list = new List<BindableToolbarItem>
            {
                GetSubmitItem(viewModel)
            };
            return list;
        }

        public static void SetToolBarItems<T>(this BaseContentPage<T> page, T viewModel) where T : ViewModelBase
        {
            page.ToolbarItems.Clear();

            var list = new List<BindableToolbarItem>
            {
                GetSubmitItem(viewModel)
            };
            foreach (var BindableToolbarItem in list)
            {
                page.ToolbarItems.Add(BindableToolbarItem);
            }
        }

        public static void SetToolBarItems<T>(this BaseContentPage<T> page, T viewModel, params (string, object)[] parameters) where T : ViewModelBase
        {
            page.ToolbarItems.Clear();

            var list = new List<BindableToolbarItem>
            {
                GetFilterItem(viewModel,parameters),
                GetMenusItem(viewModel,page)
            };

            foreach (var BindableToolbarItem in list)
            {
                page.ToolbarItems.Add(BindableToolbarItem);
            }
        }


        public static void SetTabsToolBarItems<T>(this BaseTabbedPage<T> page, T viewModel, string constraint = "", bool IsFilter = true, params (string, object)[] parameters) where T : ViewModelBase
        {
            page.ToolbarItems.Clear();

            var list = new List<BindableToolbarItem>();

            if (IsFilter)
            {
                list.Add(BulidButton("\uf0b0", async () =>
               {
                   if (viewModel != null)
                   {
                       await viewModel.NavigateAsync("FilterPage", parameters);
                   }
               }));
            };

            list.Add(BulidButton("\uf142", () =>
            {
                if (viewModel != null)
                {
                    string key = string.Format("{0}_SELECTEDTAB_{1}", constraint.ToUpper(), 0);
                    ((RightSideMasterPage)page.SlideMenu).SetBindMenus(viewModel.BindMenus, key);
                    page.ShowMenu();
                }
            }));

            foreach (var BindableToolbarItem in list)
            {
                page.ToolbarItems.Add(BindableToolbarItem);
            }
        }


        /// <summary>
        /// 提交单据+溢出菜单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <param name="showSubMit"></param>
        /// <returns></returns>
        public static IList<BindableToolbarItem> GetToolBarItems<T>(this BaseContentPage<T> page, T viewModel, bool showSubMit = true) where T : ViewModelBase
        {
            return GetToolBarItems(page, viewModel, showSubMit, null);
        }
        public static IList<BindableToolbarItem> GetToolBarItems<T>(this BaseContentPage<T> page, T viewModel, bool showSubMit = true, string btnText = "\uf0c7") where T : ViewModelBase
        {
            if (string.IsNullOrEmpty(btnText)) btnText = "\uf0c7";

            var list = new List<BindableToolbarItem>();
            if (showSubMit)
            {
                list.Add(GetSubmitItem(viewModel, btnText));
            }
            list.Add(GetMenusItem(viewModel, page));
            return list;
        }


        public static IList<BindableToolbarItem> GetProductToolBarItems<T>(this BaseContentPage<T> page, T viewModel, bool showSubMit = true) where T : ViewModelBase
        {
            var list = new List<BindableToolbarItem>();
            if (showSubMit)
            {
                list.Add(GetSubmitItem(viewModel));
            }

            list.Add(BulidButton("\uf0b0", () =>
            {
                ((RightProductCategoryMasterPage)page.SlideMenu).SetBindMenus(viewModel.BindCategories, viewModel.GetType().FullName);
                page.ShowMenu();
            }));

            return list;
        }

        /// <summary>
        /// 历史+溢出菜单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <param name="showSubMit"></param>
        /// <returns></returns>
        public static IList<BindableToolbarItem> GetToolBarItems8<T>(this BaseContentPage<T> page, T viewModel) where T : ViewModelBase
        {
            var list = new List<BindableToolbarItem>
            {
                GetHistoryItem(viewModel),
                GetMenusItem(viewModel, page)
            };
            return list;
        }

        /// <summary>
        /// 筛选+溢出菜单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <param name="filterParameters"></param>
        /// <returns></returns>
        public static IList<BindableToolbarItem> GetToolBarItems<T>(this BaseContentPage<T> page, T viewModel, params (string, object)[] parameters) where T : ViewModelBase
        {
            var list = new List<BindableToolbarItem>
            {
                GetFilterItem(viewModel,parameters),
                GetMenusItem(viewModel, page)
            };
            return list;
        }

        /// <summary>
        /// 刷新+添加-扫描
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <param name="filterParameters"></param>
        /// <returns></returns>
        public static IList<BindableToolbarItem> GetToolBarItems3<T>(this BaseContentPage<T> page, T viewModel) where T : ViewModelBase
        {
            var list = new List<BindableToolbarItem>
            {
                GetRefreshItem(viewModel),
                GetScanItem(viewModel),
                GetAddItem(viewModel,"\xf055")
            };
            return list;
        }

        /// <summary>
        ///  刷新+添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static IList<BindableToolbarItem> GetToolBarItems4<T>(this BaseContentPage<T> page, T viewModel) where T : ViewModelBase
        {
            var list = new List<BindableToolbarItem>
            {
               GetRefreshItem(viewModel),
               GetAddItem(viewModel,"\xf055")
            };
            return list;
        }


        /// <summary>
        /// 刷新+溢出菜单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <param name="filterParameters"></param>
        /// <returns></returns>
        public static IList<BindableToolbarItem> GetToolBarItems5<T>(this BaseContentPage<T> page, T viewModel) where T : ViewModelBase
        {
            var list = new List<BindableToolbarItem>
            {
                GetRefreshItem(viewModel),
                GetMenusItem(viewModel,page)
            };
            return list;
        }

        /// <summary>
        /// 筛选+刷新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <param name="filterParameters"></param>
        /// <returns></returns>
        public static IList<BindableToolbarItem> GetToolBarItems6<T>(this BaseContentPage<T> page, T viewModel, params (string, object)[] parameters) where T : ViewModelBase
        {
            var list = new List<BindableToolbarItem>
            {
                GetFilterItem(viewModel,parameters),
                GetRefreshItem(viewModel)
            };
            return list;
        }


        /// <summary>
        /// 刷新+添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static IList<BindableToolbarItem> GetToolBarItems7<T>(this BaseContentPage<T> page, T viewModel) where T : ViewModelBase
        {
            var list = new List<BindableToolbarItem>
            {
                GetRefreshItem(viewModel),
                GetAddItem(viewModel)
            };
            return list;
        }
    }
}
