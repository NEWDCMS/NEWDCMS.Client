using ReactiveUI.Fody.Helpers;
using System;
namespace Wesley.Client.Models.Terminals
{
    //[CrudEntity(GlobalSettings.BaseEndpoint + "/api/v3/dcms/getTerminals", typeof(int), typeof(PagedResult<>))]
    public partial class TerminalModel : EntityBase
    {
        [Reactive] public string Name { get; set; }
        [Reactive] public string MnemonicName { get; set; }
        [Reactive] public string BossName { get; set; }
        [Reactive] public string BossCall { get; set; }
        [Reactive] public bool Status { get; set; } = true;
        [Reactive] public decimal? MaxAmountOwed { get; set; }
        [Reactive] public string Code { get; set; }
        [Reactive] public string Address { get; set; }
        [Reactive] public string Remark { get; set; }
        [Reactive] public int DistrictId { get; set; }
        [Reactive] public int ChannelId { get; set; }
        [Reactive] public int? LineId { get; set; }
        [Reactive] public int RankId { get; set; }
        [Reactive] public int PaymentMethod { get; set; }
        [Reactive] public double? Location_Lng { get; set; }
        [Reactive] public double? Location_Lat { get; set; }
        [Reactive] public string BusinessNo { get; set; }
        [Reactive] public string FoodBusinessLicenseNo { get; set; }
        [Reactive] public string EnterpriseRegNo { get; set; }
        [Reactive] public bool Deleted { get; set; }
        [Reactive] public string DistrictName { get; set; }
        [Reactive] public string ChannelName { get; set; }
        [Reactive] public string LineName { get; set; }
        [Reactive] public string RankName { get; set; }
        [Reactive] public string RankColor { get; set; }
        [Reactive] public bool TodayIsVisit { get; set; }
        [Reactive] public double? Distance { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        [Reactive] public bool HasGives { get; set; }
        [Reactive] public string DoorwayPhoto { get; set; }
        [Reactive] public bool Related { get; set; }

        public SelectList DistrictList { get; set; }
        public SelectList ChannelList { get; set; }
        public SelectList RankList { get; set; }
        public SelectList LineList { get; set; }
        public SelectList PaymentMethodType { get; set; }


        /// <summary>
        /// 是否协议店
        /// </summary>
        [Reactive] public bool IsAgreement { get; set; }
        /// <summary>
        /// 合作意向
        /// </summary>
        [Reactive] public string Cooperation { get; set; }
        /// <summary>
        /// 展示是否陈列
        /// </summary>
        [Reactive] public bool IsDisplay { get; set; }
        /// <summary>
        /// 展示是否生动化
        /// </summary>
        [Reactive] public bool IsVivid { get; set; }
        /// <summary>
        /// 展示是否促销
        /// </summary>
        [Reactive] public bool IsPromotion { get; set; }
        /// <summary>
        /// 展示其它
        /// </summary>
        [Reactive] public string OtherRamark { get; set; }

        /// <summary>
        /// 是否新增
        /// </summary>
        [Reactive] public bool IsNewAdd { get; set; }

    }


    /// <summary>
    /// 用于表示经销商账户余额
    /// </summary>
    public class TerminalBalance : EntityBase
    {
        public int AccountingOptionId { get; set; }
        public string AccountingName { get; set; }
        public decimal MaxOweCashBalance { get; set; }
        public decimal AdvanceAmountBalance { get; set; }
        public decimal TotalOweCash { get; set; }
        public decimal OweCashBalance { get; set; }
    }

}