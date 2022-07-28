using Wesley.Client.Enums;
using LiteDB;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Wesley.Client.Models
{
    /// <summary>
    ///  消息/通知类型枚举
    /// </summary>
    public enum MTypeEnum : int
    {
        [Description("全部")]
        All = 99,

        [Description("审批")]
        Message = 0,

        [Description("收款")]
        Receipt = 1,

        [Description("交账")]
        Hold = 2,

        [Description("推送")]
        Push = 3,

        [Description("审核完成")]
        Audited = 4,

        [Description("调度完成")]
        Scheduled = 5,

        [Description("盘点完成")]
        InventoryCompleted = 6,

        [Description("转单/签收完成")]
        TransferCompleted = 7,

        [Description("库存预警")]
        InventoryWarning = 8,

        [Description("签到异常")]
        CheckException = 9,

        [Description("客户流失预警")]
        LostWarning = 10,

        [Description("开单异常")]
        LedgerWarning = 11,

        [Description("交账完成/撤销")]
        Paymented = 12

    }

    /// <summary>
    /// 消息/通知结构
    /// </summary>
    public class MessageInfo : EntityBase
    {

        [DataMember]
        public int Index { get; set; }

        [DataMember]
        [Reactive] public bool Selected { get; set; }

        /// <summary>
        /// 消息：
        /// 0（审批），1（收款），2（交账），3（推送）
        /// 通知：
        /// 4 (审核完成），5 调度完成，6 盘点完成，7 转单/签收完成，8 库存预警，9 签到异常，10 客户流失预警，11 开单异常，12 交账完成/撤销
        /// </summary>
        [DataMember]
        public int MTypeId { get; set; }

        [BsonIgnore]
        [DataMember]
        public MTypeEnum MType
        {
            get { return (MTypeEnum)MTypeId; }
            set { MTypeId = (int)value; }
        }

        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public string Icon { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Color { get; set; }
        [DataMember]
        public string Navigation { get; set; }
        [DataMember]
        [Reactive] public int Count { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public bool IsRead { get; set; }

        public int BillTypeId { get; set; }
        [BsonIgnore]
        [DataMember]
        public BillTypeEnum BillType
        {
            get { return (BillTypeEnum)BillTypeId; }
            set { BillTypeId = (int)value; }
        }
        [DataMember]
        public string BillNumber { get; set; }
        [DataMember]
        public int BillId { get; set; }

        [DataMember]
        public int MessageId { get; set; }

    }

    /// <summary>
    /// 用于分组
    /// </summary>
    public class MessageItemsGroup : ObservableCollection<MessageInfo>
    {
        public string GroupHeader { get; set; }
        public DateTime DateTime { get; set; }

        public MessageItemsGroup(DateTime dateTime, string groupHeader, IEnumerable<MessageInfo> messages) : base(messages)
        {
            DateTime = dateTime;
            GroupHeader = groupHeader;
        }
    }
}
