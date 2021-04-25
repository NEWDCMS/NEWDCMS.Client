using Newtonsoft.Json;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;
namespace Wesley.Client.Models.Census
{
    /// <summary>
    /// 门头照片
    /// </summary>
    public class DoorheadPhoto : EntityBase
    {
        [Reactive] public string StoragePath { get; set; }

        [JsonIgnore]
        [Reactive] public ImageSource ThumbnailPhoto { get; set; }

    }

}
