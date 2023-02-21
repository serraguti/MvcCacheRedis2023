using StackExchange.Redis;

namespace MvcCacheRedis.Helpers
{
    public static class HelperCacheMultiplexer
    {
        private static Lazy<ConnectionMultiplexer> CreateConnection =
            new Lazy<ConnectionMultiplexer>(() =>
            {
                string cnn = "bbddproductoscachepgs.redis.cache.windows.net:6380,password=g6KAL1sldSFEtzhEjNn5dG9Fy68EeDgc7AzCaD4mT3s=,ssl=True,abortConnect=False";
                return ConnectionMultiplexer.Connect(cnn);
            });

        public static ConnectionMultiplexer GetConnection
        {
            get
            {
                return CreateConnection.Value;
            }
        }
    }
}
