namespace Wesley.BitImageEditor.IOS
{
    /// <summary>
    /// Необходим для исспользования <see cref="Wesley.BitImageEditor"/> на IOS
    /// </summary>
    public static class Platform
    {
        internal static bool IsInitialized { get; set; }

        /// <summary>
        /// Инициализирует <see cref="Wesley.BitImageEditor"/>
        /// </summary>
        public static void Init()
        {
            IsInitialized = true;
            LinkAssemblies();
        }

        private static void LinkAssemblies()
        {

        }
    }
}
