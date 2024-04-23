using StackExchange.Redis;

namespace Library.Redis
{
    public interface IRedisClient
    {

        Task<bool> RemoveAsync(string key);
        Task RemoveByPrefixAsync(string key);

        Task<bool> ExistsAsync(string key);

        Task<bool> AddAsync<T>(string key, T value, TimeSpan? timeSpan) where T : class;
        Task<bool> AddLargeDataAsync<T>(string key, T value, TimeSpan? timeSpan, int chunkSize = 4096) where T : class;

        Task<RedisValue> GetLargeDataAsync<T>(string key) where T : class;
        Task<RedisValue> GetAsync<T>(string key) where T : class;

        void AddToListAsync<T>(string key, T objectToCache);

        void RemoveSortedSetAsync<T>(string key, T objectToCache);

        void AddToSortedSetAsync<T>(string key, T objectToCache, double score);

        Task<long> SortedSetLengthAsync(string key);
    }
}
