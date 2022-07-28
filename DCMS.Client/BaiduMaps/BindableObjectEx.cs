using System.Reflection;
using Xamarin.Forms;
namespace Wesley.Client.BaiduMaps
{
    public static class BindableObjectEx
    {
        public static void SetValueSilent(this BindableObject obj, BindableProperty property, object val)
        {
            //var setValueCore = typeof(BindableObject).GetRuntimeMethods()
            //.Where(m => m.Name.Equals("SetValueCore"))
            //.Where(m => m.ToString().Contains("SetValuePrivateFlags")).First();
            foreach (MethodInfo method in typeof(BindableObject).GetRuntimeMethods())
            {
                if ("SetValueCore" == method.Name && method.ToString().Contains("SetValuePrivateFlags"))
                {
                    method.Invoke(obj, new object[] { property, val, 0, 2 }); // 2 = SetValuePrivateFlags.Silent
                    return;
                }
            }
        }
    }
}

