using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hazelcast.Core;
using Hazelcast;

namespace Infrastructure.Common.Caching
{
    public class HazelCastCache : IDistributedCache
    {
        private IHazelcastClient _client;
        private HazelcastOptions _options;
        private string _map;
        public HazelCastCache(HazelcastOptions options,
        string map)
        {
            _options = options;
            _map = map;

        }
        public byte[] Get(string key)
        {
            _client =  HazelcastClientFactory.StartNewClientAsync(_options, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
           var map = _client.GetMapAsync<string, byte[]>(_map).Result;
            return map.GetAsync(key).Result;
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
             _client =  HazelcastClientFactory.StartNewClientAsync(_options, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            var map = await _client.GetMapAsync<string, byte[]>(key);
            return  map.GetAsync(key).Result;
        }

        public void Refresh(string key)
        {
             _client =  HazelcastClientFactory.StartNewClientAsync(_options, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
           var map = _client.GetMapAsync<string, byte[]>(_map).Result;
           var value =  map.GetAsync(key).Result;
           map.SetAsync(key,value);
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
             _client =  HazelcastClientFactory.StartNewClientAsync(_options, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            var map = _client.GetMapAsync<string, byte[]>(_map).Result;
           var value =  map.GetAsync(key).Result;
           return map.SetAsync(key,value);
        }

        public void Remove(string key)
        {
             _client =  HazelcastClientFactory.StartNewClientAsync(_options, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            var map = _client.GetMapAsync<string, byte[]>(_map).Result;
            map.DeleteAsync(key);
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
             _client =  HazelcastClientFactory.StartNewClientAsync(_options, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            var map = _client.GetMapAsync<string, byte[]>(_map).Result;
             await map.DeleteAsync(key);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
             _client =  HazelcastClientFactory.StartNewClientAsync(_options, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            var map = _client.GetMapAsync<string, byte[]>(_map).Result;
             map.SetAsync(key,value).GetAwaiter().GetResult();
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
             _client =  HazelcastClientFactory.StartNewClientAsync(_options, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            var map = _client.GetMapAsync<string, byte[]>(_map).Result;
             return map.SetAsync(key,value);
        }
    }
}