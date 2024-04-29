using Newtonsoft.Json;
using MongoDB.Driver;
using System.Threading;
using Microsoft.Extensions.Caching.Distributed; 

namespace Infrastructure.Common.Caching
{
    public class MongoDbCache : ICache
    {
        #region " Private "
        
       

        #endregion
        
        #region " Public "

        public MonogoDbCache()
        {
           
        }

        public async Task SetStringAsync(string key, string value)
        {

            await database.StringSetAsync(key, value).ConfigureAwait(false);
        }

        public async Task SetStringAsync(string key, string value, int cacheTime)
        {
            DateTime expireDate;
            if (cacheTime == 99)
                expireDate = DateTime.Now + TimeSpan.FromSeconds(30);
            else
                expireDate = DateTime.Now + TimeSpan.FromMinutes(cacheTime);

            await database.StringSetAsync(key, value, new TimeSpan(expireDate.Ticks)).ConfigureAwait(false);
        }

        public async Task SetObjectAsync(string key, object value)
        {
            await database.StringSetAsync(key, JsonConvert.SerializeObject(value)).ConfigureAwait(false);
        }

        public async Task SetObjectAsync(string key, object value, int cacheTime)
        {
            DateTime expireDate;
            if (cacheTime == 99)
                expireDate = DateTime.Now + TimeSpan.FromSeconds(30);
            else
                expireDate = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            await database.StringSetAsync(key, JsonConvert.SerializeObject(value), new TimeSpan(expireDate.Ticks)).ConfigureAwait(false);
        }

        public async Task<string> GetStringAsync(string key)
        {
            var value = await database.StringGetAsync(key).ConfigureAwait(false);
            return value.IsNullOrEmpty ? "" : value.ToString();
        }

        public async Task<T> GetObjectAsync<T>(string key)
        {
            var value = await database.StringGetAsync(key).ConfigureAwait(false);
            return value.IsNullOrEmpty ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public async Task<bool> ExistAsync(string key)
        {
            return await database.KeyExistsAsync(key).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string key)
        {
            await database.KeyDeleteAsync(key).ConfigureAwait(false);
        }

        public List<string> ListKeys()
        {
            var list = new List<string>();
            foreach (var item in server.Keys()) {
                list.Add(item.ToString());
            }
            return list;
        }

        public Dictionary<string, string> ListKeyValues()
        {
            var list = new Dictionary<string, string>();
            var keys = ListKeys();
            foreach (var item in keys) {
                if (database.KeyType(item) == RedisType.String)
                    list.Add(item.ToString(), database.StringGet(item));
            }
            return list;
        }

        #endregion

        
    }
}
