using Wesley.Client.Models.Sales;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace Wesley.Client.ViewModels
{
    public class ReconciliationProductsPageViewModel : ViewModelBase
    {
        [Reactive] public IList<AccountProductGroup> Products { get; private set; } = new ObservableCollection<AccountProductGroup>();
        [Reactive] public int TotalCount { get; set; }
        [Reactive] public decimal TotalAmount { get; set; }

        public ReconciliationProductsPageViewModel(INavigationService navigationService, IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "销售商品";
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            try
            {
                if (parameters.ContainsKey("ReconciliationProducts"))
                {
                    parameters.TryGetValue("ReconciliationProducts", out List<AccountProductModel> rconciliationProducts);
                    if (rconciliationProducts != null)
                    {
                        this.Products.Clear();

                        //按类别分组
                        var gProducts = rconciliationProducts.GroupBy(s => s.CategoryName).ToList();
                        foreach (var group in gProducts)
                        {
                            var comps = new List<AccountProductModel>();
                            //按产品分组
                            var gps = group.GroupBy(s => s.ProductId).ToList();
                            foreach (var bPs in gps)
                            {
                                var p = bPs.First();
                                p.QuantityFormat = StockQuantityFormat(bPs.Sum(s => s.Quantity), p.StrokeQuantity, p.BigQuantity, "小", "中", "大");
                                p.GAmount = bPs.Sum(s => s.Amount);
                                comps.Add(p);
                            }
                            Products.Add(new AccountProductGroup(group.Key, comps));
                        }

                        this.TotalCount = rconciliationProducts.Sum(s => s.Quantity);
                        this.TotalAmount = rconciliationProducts.Sum(s => s.Amount);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }



        /// <summary>
        /// 格式化转化量
        /// </summary>
        /// <param name="totalQuantity">总小单位量</param>
        /// <param name="mCQuantity">中单位转化量</param>
        /// <param name="bCQuantity">大单位转化量</param>
        /// <param name="sName">小单位</param>
        /// <param name="mName">中单位</param>
        /// <param name="bName">大单位</param>
        /// <returns></returns>
        public string StockQuantityFormat(int totalQuantity, int mCQuantity, int bCQuantity, string sName, string mName, string bName)
        {
            try
            {
                int thisQuantity = totalQuantity;
                string result = string.Empty;

                var bigUnitName = bName;
                var bigQuantity = bCQuantity;

                var strokeUnitName = mName;
                var strokeQuantity = mCQuantity;

                var smallUnitName = sName;


                int big = 0;
                int stroke = 0;
                int small = 0;

                //大
                if (bigQuantity > 0)
                {
                    big = thisQuantity / bigQuantity;
                    thisQuantity = thisQuantity - big * bigQuantity;
                }
                //中
                if (strokeQuantity > 0)
                {
                    stroke = thisQuantity / bigQuantity;
                    thisQuantity = thisQuantity - stroke * strokeQuantity;
                }

                //小
                small = thisQuantity;

                if (big > 0)
                {
                    result += big.ToString() + bigUnitName;
                }

                if (stroke > 0)
                {
                    result += stroke.ToString() + strokeUnitName;
                }

                if (small > 0)
                {
                    result += small.ToString() + smallUnitName;
                }

                return result;
            }
            catch
            {
                return "";
            }
        }
    }
}
