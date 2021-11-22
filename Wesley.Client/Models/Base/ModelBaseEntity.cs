using Wesley.ChartJS.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace Wesley.Client.Models
{
    [Serializable]
    public class SelectList : List<SelectListItem>
    {
    }

    [Serializable]
    public class SelectListItem
    {
        /// <summary>
        /// 获取或设置一个值，该值指示是否禁用此 System.Web.Mvc.SelectListItem。
        /// </summary>
        public bool Disabled { get; set; }
        /// <summary>
        /// 表示此项已包装到的选项组 HTML 元素。在选择列表中，支持多个同名组。它们与引用相等性进行比较。
        /// </summary>
        public SelectListGroup Group { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示是否选择此 System.Web.Mvc.SelectListItem。  如果选定此项，则为 true；否则为 false
        /// </summary>
        public bool Selected { get; set; }
        /// <summary>
        /// 获取或设置选定项的文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 获取或设置选定项的值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 自定义字段
        /// </summary>
        public string CustomField { get; set; }
    }

    [Serializable]
    public class SelectListGroup
    {
        /// <summary>
        /// 获取或设置一个值，该值指示是否禁用此 System.Web.Mvc.SelectListGroup。
        /// </summary>
        public bool Disabled { get; set; }
        /// <summary>
        /// 表示选项组的标签的值
        /// </summary>
        public string Name { get; set; }
    }



    /// <summary>
    ///  支付方式
    /// </summary>
    [Serializable]
    public class AccountingModel : AccountMaping
    {
        [Reactive] public bool Default { get; set; }
        [Reactive] public bool Selected { get; set; }
        [Reactive] public decimal Balance { get; set; }
        [Reactive] public string BalanceName { get; set; }
        public int? ParentId { get; set; } = 0;
        public ReactiveCommand<int, Unit> SelectedCommand { get; set; }
    }


    /// <summary>
    /// 条件筛选
    /// </summary>
    public class FilterModel : Base
    {
        [Reactive] public int SelectedTab { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        [Reactive] public string SerchKey { get; set; }
        [Reactive] public bool SerchKeyEnable { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        [Reactive] public int ManufacturerId { get; set; }

        [Reactive] public string ManufacturerName { get; set; }

        [Reactive] public bool ManufacturerEnable { get; set; }



        /// <summary>
        /// 客户
        /// </summary>
        [Reactive] public int TerminalId { get; set; }

        [Reactive] public string TerminalName { get; set; }

        [Reactive] public bool TerminalEnable { get; set; }


        /// <summary>
        /// 片区
        /// </summary>
        [Reactive] public int DistrictId { get; set; }

        [Reactive] public string DistrictName { get; set; }

        [Reactive] public bool DistrictEnable { get; set; }




        /// <summary>
        /// 业务员
        /// </summary>
        [Reactive] public int BusinessUserId { get; set; }

        [Reactive] public string BusinessUserName { get; set; }

        [Reactive] public bool BusinessUserEnable { get; set; }




        /// <summary>
        /// 品牌
        /// </summary>
        public int BrandId { get; set; }
        [Reactive] public string BrandName { get; set; }
        [Reactive] public bool BrandEnable { get; set; }



        /// <summary>
        /// 商品
        /// </summary>
        public int ProductId { get; set; }
        [Reactive] public string ProductName { get; set; }
        [Reactive] public bool ProductEnable { get; set; }



        /// <summary>
        /// 仓库
        /// </summary>
        [Reactive] public int WareHouseId { get; set; }
        [Reactive] public string WareHouseName { get; set; }
        [Reactive] public bool WareHouseEnable { get; set; }



        /// <summary>
        /// 商品类别
        /// </summary>
        public int CatagoryId { get; set; }
        public int[] CatagoryIds { get; set; }
        [Reactive] public string CatagoryName { get; set; }
        [Reactive] public bool CatagoryEnable { get; set; }



        /// <summary>
        /// 创建时间
        /// </summary>
        [Reactive] public DateTime? CreateTime { get; set; }
        [Reactive] public bool CreateTimeEnable { get; set; }



        /// <summary>
        /// 开始时间
        /// </summary>
        [Reactive] public DateTime? StartTime { get; set; }
        [Reactive] public bool StartTimeEnable { get; set; }


        /// <summary>
        /// 结束时间
        /// </summary>
        [Reactive] public DateTime? EndTime { get; set; }
        [Reactive] public bool EndTimeEnable { get; set; }



        /// <summary>
        /// 线路
        /// </summary>
        public int LineId { get; set; }

        [Reactive] public string LineName { get; set; }



        [Reactive] public int ChannelId { get; set; }
        [Reactive] public string ChannelName { get; set; }



        [Reactive] public int RankId { get; set; }
        [Reactive] public string RankName { get; set; }



        [Reactive] public bool Status { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Reactive] public int AccountOptionId { get; set; }
        [Reactive] public string AccountOptionName { get; set; }

        [Reactive] public int DistanceOrderBy { get; set; } = 1;

    }

    public class MyReportingModel
    {
        public string Title { get; set; }
        public ChartViewConfig ChartConfig { get; set; }
    }
}
