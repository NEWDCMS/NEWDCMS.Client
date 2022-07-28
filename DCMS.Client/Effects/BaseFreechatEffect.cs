using Xamarin.Forms;

namespace Wesley.Client.Effects
{
    public class BaseFreechatEffect : RoutingEffect
    {
        public BaseFreechatEffect(string name) : base($"Wesley.Client.{nameof(BaseFreechatEffect)}")
        {
        }
    }
}
