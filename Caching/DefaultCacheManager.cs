using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Library.Redis.Caching
{
    public class DefaultCacheManager : ICacheManager
    {
        private readonly IRedisClient _provider;
        private const int defaultCacheMinutes = 360;
        private readonly ILogger<DefaultCacheManager> _logger;
        //private readonly TelemetryClient _telemetryClient;
        public DefaultCacheManager(IRedisClient provider, ILogger<DefaultCacheManager> logger, TelemetryClient telemetryClient)
        {
            _logger = logger;
            _provider = provider;
            //_telemetryClient = telemetryClient;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            //var operation = _telemetryClient.StartOperation<DependencyTelemetry>("Redis");
            //operation.Telemetry.Type = "Redis";
            //operation.Telemetry.Properties.Add("Redis Key", key);
            try
            {
                var serializedObject = await _provider.GetAsync<string>(key);
                if (!serializedObject.HasValue)
                    return default;
                var result = JsonConvert.DeserializeObject<T>(serializedObject);

                return result;
            }
            catch (Exception ex)
            {
                //_telemetryClient.TrackException(ex);
                _logger.LogWarning(ex, "Redis Error Request: {LogType} - GetAsync() - Key: {Key} - ErrorMessage: {@ContextMessage}", "Redis", key, ex.Message);
                return default;
            }
            finally
            {
                 //_telemetryClient.StopOperation(operation);
            }

        }

        public async Task<T> GetLargeDataAsync<T>(string key)
        {
            //var operation = _telemetryClient.StartOperation<DependencyTelemetry>("Redis");
            //operation.Telemetry.Type = "Redis";
            //operation.Telemetry.Properties.Add("Redis Key", key);
            try
            {
                var serializedObject = await _provider.GetLargeDataAsync<string>(key);
                if (!serializedObject.HasValue)
                    return default;
                var result = JsonConvert.DeserializeObject<T>(serializedObject);

                return result;
            }
            catch (Exception ex)
            {
                //_telemetryClient.TrackException(ex);
                _logger.LogWarning(ex, "Redis Error Request: {LogType} - GetAsync() - Key: {Key} - ErrorMessage: {@ContextMessage}", "Redis", key, ex.Message);
                return default;
            }
            finally
            {
                //_telemetryClient.StopOperation(operation);
            }

        }

        public async Task SetLargeDataAsync(string key, object data, int cacheTime)
        {
            //var operation = _telemetryClient.StartOperation<DependencyTelemetry>("Redis");
            //operation.Telemetry.Type = "Redis";
            //operation.Telemetry.Properties.Add("Redis Key", key);
            try
            {
                if (cacheTime <= 0)
                    _ = Task.CompletedTask;
                else
                {
                    //key = key.ToLowerInvariant();
                    await _provider.AddLargeDataAsync(key, data, TimeSpan.FromMinutes(cacheTime));
                }
            }
            catch (Exception ex)
            {
                //_telemetryClient.TrackException(ex);
                _logger.LogError(ex, "Redis Error Request: {LogType} - SetAsync() - Key: {Key} - ErrorMessage: {@ContextMessage}", "Redis", key, ex.Message);
            }
            finally
            {
                //_telemetryClient.StopOperation(operation);
            }
        }

        public async Task SetAsync(string key, object data, int cacheTime)
        {
            //var operation = _telemetryClient.StartOperation<DependencyTelemetry>("Redis");
            //operation.Telemetry.Type = "Redis";
            //operation.Telemetry.Properties.Add("Redis Key", key);
            try
            {
                if (cacheTime <= 0)
                    _ = Task.CompletedTask;
                else
                {
                    //key = key.ToLowerInvariant();
                    await _provider.AddAsync(key, data, TimeSpan.FromMinutes(cacheTime));
                }
            }
            catch (Exception ex)
            {
                //_telemetryClient.TrackException(ex);
                _logger.LogError(ex, "Redis Error Request: {LogType} - SetAsync() - Key: {Key} - ErrorMessage: {@ContextMessage}", "Redis", key, ex.Message);
            }
            finally
            {
                //_telemetryClient.StopOperation(operation);
            }
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            //var operation = _telemetryClient.StartOperation<DependencyTelemetry>("Redis");
            //operation.Telemetry.Type = "Redis";
            //operation.Telemetry.Properties.Add("Redis Key", prefix);
            try
            {
                await _provider.RemoveByPrefixAsync(prefix);
            }
            catch (Exception ex)
            {
                //_telemetryClient.TrackException(ex);
                _logger.LogError(ex, "Redis Error Request: {LogType} - RemoveByPrefixAsync() - Prefix : {Prefix} - {@ContextMessage}", "Redis", prefix, ex.Message);
            }
            finally
            {
                //_telemetryClient.StopOperation(operation);
            }
            
        }
    }
}
