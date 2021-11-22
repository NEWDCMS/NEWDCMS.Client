
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Wesley.Client.Enums;
using Wesley.Infrastructure.Helpers;
using Newtonsoft.Json;

namespace Wesley.Client.Models.QA
{
    /// <summary>
    /// 表示 TSS 服务支持（在线帮助）
    /// </summary>
    public class MessagesGroup : ObservableCollection<Message>
    {
        public string GroupHeader { get; set; }
        public DateTime DateTime { get; set; }

        public MessagesGroup(DateTime dateTime, string groupHeader, IEnumerable<Message> messages) : base(messages)
        {
            DateTime = dateTime;
            GroupHeader = groupHeader;
        }
    }

    /// <summary>
    /// 表示 TSS 服务支持（在线帮助）
    /// </summary>
    public class Message : Base
    {
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
        [Reactive] public string Content { get; set; }
        [Reactive] public string ConversationId { get; set; }
        [Reactive] public bool ISent { get; set; }

        [Reactive] public bool ISentPreviousMessage { get; set; }

        [Reactive] public Message ReplyTo { get; set; }
        [Reactive] public string SenderId { get; set; }
        [Reactive] public User Sender { get; set; }
    }

    public class QueuedMessage : Base
    {
        public int Id { get; set; }
        public int StoreId { get; set; } = 0;
        public bool IsRead { get; set; }
        public int Priority { get; set; } 

        [JsonIgnore]
        public MTypeEnum MType
        {
            get { return (MTypeEnum)MTypeId; }
            set { MTypeId = (int)value; }
        }
        [JsonIgnore]
        public BillTypeEnum BillType
        {
            get { return (BillTypeEnum)BillTypeId; }
            set { BillTypeId = (int)value; }
        }

        public int MTypeId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Icon { get; set; } 
        public DateTime Date { get; set; } 
        public int? BillTypeId { get; set; }
        public string BillNumber { get; set; } 
        public int? BillId { get; set; } = 0;
        public DateTime CreatedOnUtc { get; set; } 
        public DateTime? SentOnUtc { get; set; }
        public int SentTries { get; set; } = 0;
        /// <summary>
        /// 手机号（标识）
        /// </summary>
        public string ToUser { get; set; }
        public string TerminalNames { get; set; }
        public string ProductNames { get; set; } 
        public string BillNumbers { get; set; }
        public string BusinessUser { get; set; } 
        public string TerminalName { get; set; } 
        public double? Distance { get; set; } 
        public int? Month { get; set; } = 0;
        public decimal? Amount { get; set; } 
    }


    [Serializable]
    public class MessageStructure : QueuedMessage
    {
        /// <summary>
        /// 客户序列“|”分割
        /// </summary>
        public List<string> Terminals { get; set; } = new List<string>();
        /// <summary>
        /// 商品序列“|”分割
        /// </summary>
        public List<string> Products { get; set; } = new List<string>();
        /// <summary>
        /// 单据序列“|”分割
        /// </summary>
        public List<string> Bills { get; set; } = new List<string>();

        public QueuedMessage ToEntity()
        {
            TerminalNames = string.Join("|", Terminals);
            ProductNames = string.Join("|", Products);
            BillNumbers = string.Join("|", Bills);
            return this;
        }

    }

    public static class MessagExt
    {
        public static MessageStructure ToStructure(this QueuedMessage queuedMessage)
        {
            var terminalNames = new List<string>();
            var productNames = new List<string>();
            var billNumbers = new List<string>();

            if (!string.IsNullOrEmpty(queuedMessage.TerminalNames))
            {
                terminalNames = queuedMessage.TerminalNames.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            if (!string.IsNullOrEmpty(queuedMessage.ProductNames))
            {
                productNames = queuedMessage.ProductNames.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            if (!string.IsNullOrEmpty(queuedMessage.BillNumbers))
            {
                billNumbers = queuedMessage.BillNumbers.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            var ms =  new MessageStructure()
            {
                Id = queuedMessage.Id,
                StoreId = queuedMessage.StoreId,
                IsRead = queuedMessage.IsRead,
                Priority = queuedMessage.Priority,
                MType = (MTypeEnum)queuedMessage.MTypeId,
                BillType = (BillTypeEnum)queuedMessage.BillTypeId,
                MTypeId = queuedMessage.MTypeId,
                Title = queuedMessage.Title,
                Content = queuedMessage.Content,
                Icon = queuedMessage.Icon,
                Date = queuedMessage.Date,
                BillTypeId = queuedMessage.BillTypeId ?? 0,
                BillNumber = queuedMessage.BillNumber,
                BillId = queuedMessage.BillId,
                CreatedOnUtc = queuedMessage.CreatedOnUtc,
                SentOnUtc = queuedMessage.SentOnUtc,
                SentTries = queuedMessage.SentTries,
                ToUser = queuedMessage.ToUser,
                TerminalNames = queuedMessage.TerminalNames,
                ProductNames = queuedMessage.ProductNames,
                BillNumbers = queuedMessage.BillNumbers,
                BusinessUser = queuedMessage.BusinessUser,
                TerminalName = queuedMessage.TerminalName,
                Distance = queuedMessage.Distance ?? 0,
                Month = queuedMessage.Month ?? 0,
                Amount = queuedMessage.Amount ?? 0,
                Terminals = terminalNames,
                Products = productNames,
                Bills = billNumbers
            };

            return ms.SendMessageOrNotity();
        }


        public static MessageStructure SendMessageOrNotity(this MessageStructure ms)
        {
            try
            {
                string title = CommonHelper.GetEnumDescription<MTypeEnum>(ms.MType);
                string billName = CommonHelper.GetEnumDescription<BillTypeEnum>(ms.BillType);
                switch (ms.MType)
                {
                    case MTypeEnum.Message://审批
                        ms.Content = $"{billName} ({ms.BillNumber}) 需要审批，请尽快处理。";
                        break;
                    case MTypeEnum.Receipt://收款
                        ms.Content = $"{string.Join("，", ms.Terminals)} 等 {ms.Terminals.Count} 家门店欠款近 {ms.Month} 月，累计 {ms.Amount.Value:#.00} 元，请尽快收取。";
                        break;
                    case MTypeEnum.Hold://交账
                        ms.Content = $"你有{ms.Amount.Value:#.00} 元需要交账，请尽快处理。";
                        break;
                    case MTypeEnum.Audited://审核完成
                        ms.Content = $"你保存的 {billName} ({ms.BillNumber}) 已经审核通过，请留意。";
                        break;
                    case MTypeEnum.Scheduled://调度完成
                        ms.Content = $"你保存的 {billName} ({ms.BillNumber}) 已经调度完成，请留意。";
                        break;
                    case MTypeEnum.InventoryCompleted://盘点完成
                        ms.Content = $"{billName} ({ms.BillNumber}) 已经盘点完成，请留意。";
                        break;
                    case MTypeEnum.TransferCompleted://转单/签收完成
                        ms.Content = $"{billName} ({ms.BillNumber}) 转单/签收完成，请留意。";
                        break;
                    case MTypeEnum.InventoryWarning://库存预警
                        ms.Content = $"{string.Join("，", ms.Products)}  等 {ms.Products.Count}件商品缺货，请尽快处理。";
                        break;
                    case MTypeEnum.CheckException://签到异常
                        ms.Content = $"{ms.BusinessUser}在拜访{ms.TerminalName}时，距离{ms.Distance}米签到，请留意。";
                        break;
                    case MTypeEnum.LostWarning://客户流失预警
                        ms.Content = $"你有{ms.Terminals.Count}个客户可能流失，请关注。";
                        break;
                    case MTypeEnum.LedgerWarning://开单异常
                        ms.Content = $"{ms.BusinessUser} 在交账后，开具单据({ms.BillNumber})，请留意。";
                        break;
                    case MTypeEnum.Paymented://交账完成/撤销
                        ms.Content = $"于{ms.Date:yyyy年MM月dd日hh时MM分}，你已上交钱款{ms.Amount.Value:#.00}，包含单据： ({string.Join("，", ms.Bills)})， 请留意。";
                        break;
                    default:
                        break;
                }

                return ms;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
