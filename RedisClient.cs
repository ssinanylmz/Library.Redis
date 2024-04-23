using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Library.Redis.Caching;
using StackExchange.Redis;
using System.Text;

namespace Library.Redis
{
    public class RedisClient : IRedisClient
    {
        private IDatabase _db;
        private StackExchange.Redis.IServer _server;
        private ConnectionMultiplexer _redis;
        private static readonly Object _multiplexerLock = new Object();
        #region ctor

        public RedisClient(RedisClientConfigurations conf)
        {
            lock (_multiplexerLock)
            {
                if (_redis == null || !_redis.IsConnected)
                {
                    ConnectRedis(conf);
                }
            }
        }

        #endregion

        #region Public Methods

        public async Task<bool> RemoveAsync(string key)
        {
            var result = await _db.KeyDeleteAsync(key);
            return result;
        }
        public async Task RemoveByPrefixAsync(string key)
        {
            if (_server == null || !_server.IsConnected)
                _server = GetServer();

            var keys = _server.Keys(_db.Database, key);

            foreach (var item in keys)
                _ = await _db.KeyDeleteAsync(item);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        public async Task<bool> AddAsync<T>(string key, T value, TimeSpan? timespan) where T : class
        {
            var stringContent = SerializeContent(value);
            return await _db.StringSetAsync(key, stringContent, timespan);
        }

        public async Task<bool> AddLargeDataAsync<T>(string key, T value, TimeSpan? timespan, int chunkSize = 4096) where T : class
        {
            var stringContent = SerializeContent(value);

            int numberOfChunks = (stringContent.Length + chunkSize - 1) / chunkSize;

            for (int i = 0; i < numberOfChunks; i++)
            {
                int startIndex = i * chunkSize;
                int length = Math.Min(chunkSize, stringContent.Length - startIndex);
                string chunk = stringContent.Substring(startIndex, length);

                await _db.StringSetAsync($"{key}:chunk:{i}", chunk);
            }

            return await _db.StringSetAsync($"{key}:chunks", numberOfChunks, timespan);

        }

        public async Task<RedisValue> GetLargeDataAsync<T>(string key) where T : class
        {
            int numberOfChunks = (int)await _db.StringGetAsync($"{key}:chunks");
            StringBuilder largeData = new StringBuilder();

            for (int i = 0; i < numberOfChunks; i++)
            {
                string chunk = await _db.StringGetAsync($"{key}:chunk:{i}");
                largeData.Append(chunk);
            }

            return largeData.ToString();
        }
        public async Task<RedisValue> GetAsync<T>(string key) where T : class
        {
            return await _db.StringGetAsync(key);
        }

        public async void AddToListAsync<T>(string key, T objectToCache)
        {
            _ = await _db.ListRightPushAsync(key, JsonConvert.SerializeObject(objectToCache));
        }

        public async Task<long> SortedSetLengthAsync(string key)
        {
            return await _db.SortedSetLengthAsync(key);
        }

        public async void AddToSortedSetAsync<T>(string key, T objectToCache, double score)
        {
            _ = await _db.SortedSetRemoveRangeByScoreAsync(key, score, score);
            _ = await _db.SortedSetAddAsync(key, JsonConvert.SerializeObject(objectToCache), score);
        }

        public async void RemoveSortedSetAsync<T>(string key, T objectToCache)
        {
            _ = await _db.SortedSetRemoveAsync(key, JsonConvert.SerializeObject(objectToCache));
        }

        #endregion

        #region Private Methods

        private void ConnectRedis(RedisClientConfigurations conf)
        {
            try
            {
                const string configuration = "{0},password={1},abortConnect=false,defaultDatabase={2},ssl=false,connectTimeout={3},asyncTimeout={4},syncTimeout={5},allowAdmin=true,connectRetry={6}";

                _redis = ConnectionMultiplexer.ConnectAsync(
                    string.Format(configuration,
                        conf.Url,
                        conf.Password,
                        conf.DefaultDatabase,
                        conf.ConnectTimeout,
                        conf.ConnectTimeout,
                        conf.ConnectTimeout,
                        conf.ConnectRetry)).Result;

                _db = _redis.GetDatabase();
                _server = GetServer();

            }
            catch (Exception ex)
            {
            }
        }

        private IServer GetServer()
        {
            var endpoint = _db.IdentifyEndpoint();
            return _redis.GetServer(endpoint);
        }

        private static string SerializeContent(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        private static T DeserializeContent<T>(RedisValue value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        #endregion
    }
}
