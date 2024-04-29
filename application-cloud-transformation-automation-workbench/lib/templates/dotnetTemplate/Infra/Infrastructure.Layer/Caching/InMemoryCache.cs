using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Collections;
using System.Reflection;

namespace Infrastructure.Common.Caching

{
    public class InMemoryCache : ICache
    {
        #region " Public "

        public InMemoryCache(MyMemoryCache memoryCache)
        {
           
            _cache = memoryCache.Cache;         
            Init();
        }

        public async Task SetStringAsync(string key, string value)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
              .SetSize(1)
              .SetSlidingExpiration(TimeSpan.FromSeconds(3));

            await Task.Run(() => _cache.Set(key, value, cacheEntryOptions));            
        }

        public async Task SetStringAsync(string key, string value, int cacheTime)
        {

            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSize(1)
            .SetSlidingExpiration(TimeSpan.FromSeconds(cacheTime));

            await Task.Run(() => _cache.Set(key, value, cacheEntryOptions));
        }

        public async Task SetObjectAsync(string key, object value)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
          .SetSize(1)
          .SetSlidingExpiration(TimeSpan.FromSeconds(3));

            await Task.Run(() => _cache.Set(key, JsonConvert.SerializeObject(value), cacheEntryOptions));
        }

        public async Task SetObjectAsync(string key, object value, int cacheTime)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
           .SetSize(1)
           .SetSlidingExpiration(TimeSpan.FromSeconds(cacheTime));

            await Task.Run(() => _cache.Set(key, JsonConvert.SerializeObject(value), cacheEntryOptions));
        }

        public async Task<string> GetStringAsync(string key)
        {
            var value = await Task.Run(() => _cache.Get(key));

            return Convert.ToString(value);
        }

        public async Task<T> GetObjectAsync<T>(string key)
        {
            var value = await Task.Run(() => _cache.Get(key));
            return JsonConvert.DeserializeObject<T>(Convert.ToString(value));
        }

        public async Task<bool> ExistAsync(string key)
        {
            bool result = false;

            var value = await Task.Run(() => _cache.Get(key));

            if (value != null) result = true;

            return result;
        }

        public async Task DeleteAsync(string key)
        {

            await Task.Run(() => _cache.Remove(key));
        }

        #endregion

        #region " Private "

   

        private MemoryCache _cache;
        private readonly IDistributedCache _dcache;

        private void Init()
        {
            //connection = ConnectionMultiplexer.Connect(settings.ConnectionString);
            //database = connection.GetDatabase();
            //server = connection.GetServer(connection.GetEndPoints().First());
        }

        public List<string> ListKeys()
        {
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var collection = field.GetValue(_cache) as ICollection;
            var items = new List<string>();
            if (collection != null)
                foreach (var item in collection) {
                    var methodInfo = item.GetType().GetProperty("Key");
                    var val = methodInfo.GetValue(item);
                    items.Add(val.ToString());
                }

            return items;
        }

        public Dictionary<string, string> ListKeyValues()
        {
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var collection = field.GetValue(_cache) as ICollection;
            var items = new Dictionary<string, string>();
            if (collection != null)
                foreach (var item in collection) {
                    var methodInfo = item.GetType().GetProperty("Key");
                    var val = methodInfo.GetValue(item);
                    items.Add(methodInfo.ToString(), val.ToString());
                }

            return items;
        }

        public class MyMemoryCache
        {
            public MemoryCache Cache { get; set; }
            public MyMemoryCache()
            {
                Cache = new MemoryCache(new MemoryCacheOptions {
                    SizeLimit = 2024,
                    
                });
            }
        }

        #endregion
    }
}
