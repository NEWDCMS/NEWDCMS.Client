namespace Wesley.Client.CustomViews
{
    public static class CrossDiaglogKit
    {
        // private static Lazy<IDialogKit> implementation = new Lazy<IDialogKit>(() => null, System.Threading.LazyThreadSafetyMode.PublicationOnly);

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
                    _current = App.Resolve<IDialogKit>();
                }

                return _current;
            }
        }


    }
}
