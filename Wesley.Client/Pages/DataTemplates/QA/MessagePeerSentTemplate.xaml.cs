
using Xamarin.Forms;
namespace Wesley.Client.Pages.DataTemplates
{

    public partial class MessagePeerSentTemplate : BaseTemplate
    {
        public MessagePeerSentTemplate()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            this.ScaleTo(1, 300);
            base.OnBindingContextChanged();
        }
    }
}