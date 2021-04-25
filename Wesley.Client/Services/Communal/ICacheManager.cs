namespace Wesley.Client.Services
{
    public interface ICacheManager
    {
        void ClearCache(bool clearReadPos, bool clearCollect);
        string GetCacheSize();
    }
}