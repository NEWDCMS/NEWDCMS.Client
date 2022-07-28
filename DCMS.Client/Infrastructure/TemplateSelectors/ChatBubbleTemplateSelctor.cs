using Wesley.Client.Models.QA;
using Wesley.Client.Pages.DataTemplates;
using Xamarin.Forms;
namespace Wesley.Client.Selector
{
    public class ChatBubbleTemplateSelctor : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var message = item as Message;

            if (message.ISent)
            {
                var messageTemplate = new MesageISentTemplate
                {
                    ParentContext = container.BindingContext
                };
                return new DataTemplate(() => messageTemplate);
            }
            else
            {
                var messageTemplate = new MessagePeerSentTemplate
                {
                    ParentContext = container.BindingContext
                };
                return new DataTemplate(() => messageTemplate);
            }
        }
    }
}
