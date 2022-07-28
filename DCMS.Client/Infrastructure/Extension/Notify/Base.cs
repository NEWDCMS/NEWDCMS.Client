using Newtonsoft.Json;
using ReactiveUI;

namespace Wesley.Client
{

    //public class Base : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    [NotifyPropertyChangedInvocator]
    //    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}


    [JsonObject(MemberSerialization.OptOut)]
    public class Base : ReactiveObject
    {

    }

    /// <summary>
    /// 表示余额（经销商、客户，业务员）
    /// </summary>
    public partial class BaseBalance : Base
    {
        /// <summary>
        /// 当前交易预付/预收（金额）
        /// </summary>
        public decimal AdvanceAmount { get; set; }
        /// <summary>
        /// 余额（预付/预收）
        /// </summary>
        public decimal AdvanceAmountBalance { get; set; }
    }


}
