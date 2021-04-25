using System;
using System.Collections.Generic;


namespace Wesley.Client.Models.Products
{

    public partial class RecentPriceListModel
    {
        public RecentPriceListModel()
        {

        }


        //("客户名称", "客户名称
        public int? TerminalId { get; set; }
        public string TerminalName { get; set; }


        //("商品名称", "商品名称
        public int? ProductId { get; set; }
        public string ProductName { get; set; }



        public IList<RecentPriceModel> Items { get; set; }
    }


    public partial class RecentPriceModel : EntityBase
    {
        public string StoreName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }


        //("商品名称", "商品名称
        public int ProductId { get; set; }
        public string ProductName { get; set; }


        //("小单位价格", "小单位价格
        public decimal? SmallUnitPrice { get; set; }


        //("中单位价格", "中单位价格
        public decimal? StrokeUnitPrice { get; set; }


        //("大单位价格", "大单位价格
        public decimal? BigUnitPrice { get; set; }


        //("修改时间", "修改时间
        public DateTime UpdateTime { get; set; }

    }

}