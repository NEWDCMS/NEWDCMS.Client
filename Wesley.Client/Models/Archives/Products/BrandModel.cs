using System;
namespace Wesley.Client.Models.Products
{

    public partial class BrandModel : EntityBase
    {


        /// <summary>
        /// 品牌名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}