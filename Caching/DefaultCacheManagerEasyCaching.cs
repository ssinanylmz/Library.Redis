using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;

namespace Library.Redis.Caching
{
    public class DefaultCacheManagerEasyCaching 
    {
        private readonly IEasyCachingProvider _provider;
        private const int defaultCacheMinutes = 360;
        private readonly ILogger<DefaultCacheManager> _logger;
        public DefaultCacheManagerEasyCaching(IEasyCachingProvider provider, ILogger<DefaultCacheManager> logger)
        {
            _logger = logger;
            _provider = provider;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var serializedObject = await _provider.GetAsync<string>(key);
                if (!serializedObject.HasValue)
                    return default;

                return JsonConvert.DeserializeObject<T>(serializedObject.Value);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis Error Request: {LogType} - GetAsync() - Key: {Key} - ErrorMessage: {@ContextMessage}", "Redis", key, ex.Message);
                return default;
            }

        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> acquire)
        {
            try
            {
                return GetAsync(key, defaultCacheMinutes, acquire);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis Error Request: {LogType} - GetAsync() - Key: {Key} - ErrorMessage: {@ContextMessage}", "Redis", key, ex.Message);
                return default;
            }

        }

        public async Task<T> GetAsync<T>(string key, int cacheMinutes, Func<Task<T>> acquire)
        {
            try
            {
                var serializedObject = await _provider.GetAsync<string>(key);
                if (serializedObject.HasValue)
                    return JsonConvert.DeserializeObject<T>(serializedObject.Value);

                var result = await acquire();
                if (result != null && cacheMinutes > 0)
                    await SetAsync(key, result, cacheMinutes);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis Error Request: {LogType} - GetAsync() - Key: {Key} - ErrorMessage: {@ContextMessage}", "Redis", key, ex.Message);
                return default;
            }

        }

        public async Task SetAsync(string key, object data, int cacheTime)
        {
            try
            {
                if (cacheTime <= 0)
                    _ = Task.CompletedTask;
                else
                {
                    var serializedObject = JsonConvert.SerializeObject(data);
                    //key = key.ToLowerInvariant();
                    _ = _provider.SetAsync(key, serializedObject, TimeSpan.FromMinutes(cacheTime));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis Error Request: {LogType} - SetAsync() - Key: {Key} - ErrorMessage: {@ContextMessage}", "Redis", key, ex.Message);
            }

        }

        public Task<bool> IsSetAsync(string key)
        {
            try
            {
                return _provider.ExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis Error Request: {LogType} - IsSetAsync() - Key: {Key} - ErrorMessage: {@ContextMessage}", "Redis", key, ex.Message);
                return default;
            }

        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                _ = _provider.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis Error Request: {LogType} - RemoveAsync() - Key: {Key} - ErrorMessage: {@ContextMessage}", "Redis", key, ex.Message);
            }

        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            try
            {
                _ = _provider.RemoveByPrefixAsync(prefix);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis Error Request: {LogType} - RemoveByPrefixAsync() - Prefix : {Prefix} - {@ContextMessage}", "Redis", prefix, ex.Message);
            }
        }

        public async Task ClearAsync()
        {
            try
            {
                _ = _provider.FlushAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis Error Request: {LogType} - ClearAsync() - {@ContextMessage}", "Redis", ex.Message);
            }
        }
    }
}
