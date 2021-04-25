using ReactiveUI.Fody.Helpers;
namespace Wesley.Client.Models.Users
{
    /// <summary>
    /// 用于表示用于片区映射
    /// </summary>
    public partial class UserDistrictModel : EntityBase
    {
        public int UserId { get; set; }
        [Reactive] public int DistrictId { get; set; }
    }
}
