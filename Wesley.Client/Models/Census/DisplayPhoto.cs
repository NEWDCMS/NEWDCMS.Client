using Newtonsoft.Json;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;
namespace Wesley.Client.Models.Census
{
    /// <summary>
    /// 陈列照片
    /// </summary>
    public class DisplayPhoto : EntityBase
    {
        [Reactive] public string DisplayPath { get; set; }

        [JsonIgnore]
        [Reactive] public ImageSource ThumbnailPhoto { get; set; }
    }
}
