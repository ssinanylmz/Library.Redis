namespace Library.Redis.Caching
{
    public interface ICacheManagerEasyCaching
    {
        Task<T> GetAsync<T>(string key);
        Task<T> GetAsync<T>(string key, Func<Task<T>> acquire);
        Task<T> GetAsync<T>(string key, int cacheMinutes, Func<Task<T>> acquire);
        Task SetAsync(string key, object data, int cacheTime);
        Task<bool> IsSetAsync(string key);
        Task RemoveAsync(string key);
        Task RemoveByPrefixAsync(string pattern);
        Task ClearAsync();
    }
}
