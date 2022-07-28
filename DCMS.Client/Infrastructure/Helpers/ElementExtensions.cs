using System;
using Xamarin.Forms;

namespace Wesley.Infrastructure.Helpers
{
    public static class ElementExtensions
    {
        private const int IndentCount = 4;

        private const string StringFormat = "转储元素层次结构:{0}{1}";

        public static string DumpHierarchy(this Element element)
        {
            var hierarchyStringBuilder = new ElementHierarchyStringBuilder(IndentCount);
            ElementVisitor.Visit(element, hierarchyStringBuilder.Add);
            return string.Format(StringFormat, Environment.NewLine, hierarchyStringBuilder);
        }
    }
}
