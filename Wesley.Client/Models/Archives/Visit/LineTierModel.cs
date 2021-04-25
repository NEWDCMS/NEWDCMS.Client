using Wesley.Client.Models.Terminals;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;

namespace Wesley.Client.Models.Visit
{


    public class LineTierModel : EntityBase
    {

        /// <summary>
        /// 线路类别名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 拜访终端数量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }


        public List<TerminalModel> Terminals { get; set; } = new List<TerminalModel>();
    }

    public class LineTierOptionModel : EntityBase
    {
        /// <summary>
        /// 类别Id
        /// </summary>
        public int LineTierId { get; set; }

        /// <summary>
        /// 拜访顺序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 客户(终端)
        /// </summary>
        [Reactive] public int TerminalId { get; set; }
        public string TerminalName { get; set; }
        public string TerminalAddress { get; set; }
        public string BossName { get; set; }
        public string BossCall { get; set; }

    }

    public class UserLineTierAssignModel : EntityBase
    {
        /// <summary>
        /// 线路顺序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        ///业务员
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 线路
        /// </summary>
        public int LineTierId { get; set; }
        public string LineTierName { get; set; }

        /// <summary>
        /// 客户数量
        /// </summary>
        public int Quantity { get; set; }
    }
}