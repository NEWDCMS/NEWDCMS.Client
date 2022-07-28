using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
namespace Wesley.Client.Models.Terminals
{

    /// <summary>
    /// 片区
    /// </summary>
    public partial class DistrictModel : EntityBase
    {

        //private bool _selected;
        [Reactive] public bool Selected { get; set; }

        /// <summary>
        /// 片区名称
        /// </summary>
        [Reactive]
        public string Name { get; set; }


        /// <summary>
        /// 父Id
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int? OrderNo { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Deleted { get; set; }

        public IEnumerable<SelectListItem> DistrictList { get; set; }
    }

}