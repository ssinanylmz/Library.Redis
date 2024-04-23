namespace Library.Redis.Caching
{
    public interface ICacheManager
    {
        Task<T> GetAsync<T>(string key);
        Task<T> GetLargeDataAsync<T>(string key);
        Task SetAsync(string key, object data, int cacheTime);
        Task SetLargeDataAsync(string key, object data, int cacheTime);
        Task RemoveByPrefixAsync(string pattern);
    }
}
