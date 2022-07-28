using Xamarin.Forms;

namespace Wesley.Client.Effects
{
    /// <summary>
    /// Enable Keyboard Effect
    /// </summary>
    public class KeyboardEnableEffect : RoutingEffect
    {
        /// <inheritdoc />
        public KeyboardEnableEffect() : base($"Wesley.Client.{nameof(KeyboardEnableEffect)}")
        {
        }
    }
}