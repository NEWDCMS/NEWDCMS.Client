using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace Wesley.Client.Models
{

    public class PaymentMethodBaseModel : Base
    {
        [Reactive] public ObservableCollection<AccountingModel> Selectes { get; set; } = new ObservableCollection<AccountingModel>();

        /// <summary>
        /// 合计
        /// </summary>
        [Reactive] public decimal SubAmount { get; set; }

        /// <summary>
        /// 本次收款
        /// </summary>
        [Reactive] public decimal CurrentCollectionAmount { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        [Reactive] public decimal PreferentialAmount { get; set; }
        [Reactive] public bool PreferentialAmountShowFiled { get; set; } = true;

        /// <summary>
        /// 欠款金额
        /// </summary>
        [Reactive] public decimal OweCash { get; set; }
        [Reactive] public bool OweCashShowFiled { get; set; } = true;
        [Reactive] public string OweCashName { get; set; } = "欠款：";

    }

}
