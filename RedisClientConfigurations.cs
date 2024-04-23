namespace Library.Redis
{
    public class RedisClientConfigurations
    {
        public string Url { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 6379;
        public string Password { get; set; }
        public int ConnectTimeout { get; set; } = 10000;
        public int ConnectRetry { get; set; } = 3;
        public int DefaultDatabase { get; set; } = 0;
    }
}
