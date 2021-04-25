using Wesley.Client.Models.Users;
using System.Collections.Generic;
namespace Wesley.Client.Models.Products
{
    public partial class GiveQuotaListModel
    {

        public IList<GiveQuotaModel> Items { get; set; } = new List<GiveQuotaModel>();
        public IList<UserModel> Managers { get; set; } = new List<UserModel>();
        public int? UserId { get; set; }
        public int? Year { get; set; }
        public int? GiveQuotaId { get; set; }

    }

    public partial class GiveQuotaModel
    {
        public IList<GiveQuotaOptionModel> Items { get; set; } = new List<GiveQuotaOptionModel>();
        public int? StoreId { get; set; }
        public int? UserId { get; set; }
        public int? Year { get; set; }
        public string Remark { get; set; }

    }


    public partial class GiveQuotaOptionModel : EntityBase
    {
        public int GiveQuotaId { get; set; }
        public string ProductSKU { get; set; }

        //("商品名称", "商品名称
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        //("条形码", "条形码
        public string BarCode { get; set; }

        //("单位换算", "单位换算
        public string UnitConversion { get; set; }

        //("单位", "单位
        public string UnitName { get; set; }
        public int? UnitId { get; set; }
        public Dictionary<string, int> Units { get; set; }

        public decimal? Jan { get; set; }
        public decimal? Feb { get; set; }
        public decimal? Mar { get; set; }
        public decimal? Apr { get; set; }
        public decimal? May { get; set; }
        public decimal? Jun { get; set; }
        public decimal? Jul { get; set; }
        public decimal? Aug { get; set; }
        public decimal? Sep { get; set; }
        public decimal? Oct { get; set; }
        public decimal? Nov { get; set; }
        public decimal? Dec { get; set; }

        //("总计", "总计
        public decimal? Total { get; set; }

        //("备注", "备注
        public string Remark { get; set; }

    }

}