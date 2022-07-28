using System;
using System.Collections.Generic;

namespace Wesley.Client.Models.Terminals
{

    public partial class ChannelModel : EntityBase
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int? OrderNo { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 预设属性(枚举1 特殊通道 2 餐饮 3 小超 4 大超 5 其他)
        /// </summary>
        public byte Attribute { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 预设属性名称
        /// </summary>
        public string AttributeName { get; set; }
        /// <summary>
        /// 预设属性集合
        /// </summary>
        public IEnumerable<SelectListItem> Attributes { get; set; }
    }
}