namespace Wesley.FontIcons
{
    public static class ExtensionMethods
    {
        public static string FirstLetterUpperCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return $"{char.ToUpper(value[0])}{value[1..]}";
        }
    }
}
