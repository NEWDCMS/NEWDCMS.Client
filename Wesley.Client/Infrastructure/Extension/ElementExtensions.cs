using Xamarin.Forms;
namespace Wesley.Client.Extension
{
    public static class ElementExtensions
    {
        public static Page GetCurrentPage(this Element element)
        {
            while (true)
            {
                if (element == null)
                {
                    return null;
                }

                if (element is Page page)
                {
                    return page;
                }
                else
                {
                    element = element.Parent;
                }
            }
        }


        public static Page GetParentPage(this VisualElement element)
        {
            if (element != null)
            {
                var parent = element.Parent;
                while (parent != null)
                {
                    if (parent is Page page)
                    {
                        return page;
                    }
                    parent = parent.Parent;
                }
            }
            return null;
        }
    }
}
