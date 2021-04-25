using System;
namespace Wesley.Client.CustomViews
{
    public static class CrossDiaglogKit
    {
        private static Lazy<IDialogKit> implementation = new Lazy<IDialogKit>(() => CreateDiaglogKit(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// 获取当前平台上是否支持该插件
        /// </summary>
        public static bool IsSupported => implementation.Value != null;

        private static IDialogKit _current;

        /// <summary>
        /// 要使用的当前插件实现
        /// </summary>
        public static IDialogKit Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new DialogKit();
                }

                return _current;
            }
        }

        private static IDialogKit CreateDiaglogKit()
        {
#if NETSTANDARD1_0 || NETSTANDARD2_0|| NETSTANDARD2_1
            return null;
#else
#pragma warning disable IDE0022 // Use expression body for methods
            return new DiaglogKitImplementation();
#pragma warning restore IDE0022 // Use expression body for methods
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException("此程序集的可移植版本中未实现此功能。为了引用特定于平台的实现，您应该引用主应用程序项目中的nuget包.");

    }
}
