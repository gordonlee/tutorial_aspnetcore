using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;

namespace RedisConsole
{
	public class RedisConnector
	{
		// private readonly IDistributedCache cache;

		public RedisConnector()
		{
			RedisCache cache = new RedisCache(new RedisCacheOptions()
			{
				Configuration = "localhost",
				InstanceName = "master:"
			});
			var a = new RedisCacheOptions();

			// cache.Set("a", new byte[] { 4, 2, 31, 2, 4, 12, 3, 1, 2, 3, 14, 1, 2 });

			var b = cache.Get("a");
			// var b = cache.Get("a");
			Console.WriteLine(b);

			cache.Refresh("a");
			
		}
	}



    class Program
    {
        static void Main(string[] args)
        {
			var redis = new RedisConnector();



            Console.WriteLine("Hello World!");
        }
    }
}
