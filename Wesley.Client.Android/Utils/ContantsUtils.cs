namespace DCMS.Client.Droid.Utils
{
    public class ContantsUtils
    {
        public static bool IsDubugMode()
        {
            return SharePrefs.Get().GetBoolean(Contants.KEY_IS_DEBUG);
        }
    }
}