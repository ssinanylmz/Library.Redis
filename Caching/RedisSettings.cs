namespace Library.Redis.Caching
{
    public class RedisSettings
    {
        public int CacheType { get; set; } = 1; // 1:Redis  0:InMemory  -1:No Cache
        public string ConnectionString { get; set; } = "";
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 6379;
        public string Password { get; set; }
        public int ConnectTimeout { get; set; } = 5000;
        public int ConnectRetry { get; set; } = 3;
        public bool AllowAdmin { get; set; } = true;
        public bool IsSsl { get; set; } = false;
        public bool AbortConnect { get; set; } = false;
        public string SslHost { get; set; }
        public int Database { get; set; } = 0;
    }
}
