using ReactiveUI.Fody.Helpers;
using System;
namespace Wesley.Client.Models.Finances
{
    public class ReceivableModel : EntityBase
    {
        /// <summary>
        /// 终端Id
        /// </summary>
        [Reactive] public int TerminalId { get; set; }
        /// <summary>
        /// 终端名称
        /// </summary>
        public string TerminalName { get; set; }

        /// <summary>
        /// 老板名称
        /// </summary>
        public string BossName { get; set; }

        /// <summary>
        /// 老板电话
        /// </summary>
        public string BossCall { get; set; }

        /// <summary>
        /// 初始欠款
        /// </summary>
        public decimal OweCash { get; set; }

        /// <summary>
        /// 业务员ID
        /// </summary>
        public int OperationUserId { get; set; }
        /// <summary>
        /// 业务员姓名
        /// </summary>
        public string OperationUserName { get; set; }

        /// <summary>
        /// 欠款时间
        /// </summary>
        public DateTime BalanceDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}