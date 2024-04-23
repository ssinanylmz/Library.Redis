using EasyCaching.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Configuration;

namespace Library.Redis.Caching
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryCaching(this IServiceCollection services)
        {
            services.AddEasyCaching(options =>
            {
                // with a default name [json]
                options.WithJson();

                options.UseInMemory(config =>
                {
                    config.EnableLogging = true;
                    config.SerializerName = "json";
                });
            });

            //add cache manager
            services.AddSingleton<ICacheManager, DefaultCacheManager>();

            return services;
        }

        public static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
        {
            var redisSettings = configuration.GetSection("RedisSettings").Get<RedisSettings>();

            services.AddEasyCaching(options =>
            {
                // with a default name [json]
                options.WithJson();

                options.UseRedis(config =>
                {
                    config.DBConfig.Endpoints.Add(new ServerEndPoint(redisSettings.Host, redisSettings.Port));

                    //for example connection string: localhost:6379;PASSWORD;allowAdmin=true
                    var configurationOptions = ConfigurationOptions.Parse(redisSettings.ConnectionString);

                    config.DBConfig.Configuration = configurationOptions.ToString();
                    //config.DBConfig.ConnectionTimeout = redisSettings.ConnectTimeout;
                    //config.DBConfig.AllowAdmin = redisSettings.AllowAdmin;
                    //config.DBConfig.IsSsl = redisSettings.IsSsl;
                    //config.DBConfig.Database = redisSettings.Database;
                    //config.DBConfig.AbortOnConnectFail = redisSettings.AbortConnect;
                    config.EnableLogging = true;
                    config.SerializerName = "json";

                    //set ssl host
                    //if (redisSettings.IsSsl)
                    //    config.DBConfig.SslHost = redisSettings.SslHost;

                    //////set password
                    //if (!string.IsNullOrEmpty(redisSettings.Password))
                    //    config.DBConfig.Password = redisSettings.Password;
                });
            });

            //add cache manager
            services.AddSingleton<ICacheManager, DefaultCacheManager>();

            return services;
        }
    }
}
