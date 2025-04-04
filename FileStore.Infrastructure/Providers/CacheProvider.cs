using FileStore.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileStore.Infrastructure.Providers
{
    public class CacheProvider : ICacheProvider
    {
        private readonly IDistributedCache _distributed;
        private readonly ILogger<CacheProvider> _logger;

        public CacheProvider(
            IDistributedCache distributed,
            ILogger<CacheProvider> logger)
        {
            _distributed = distributed;
            _logger = logger;
        }

        public bool Exists(string key)
        {
            return _distributed.Get(key) != null;
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var item = await _distributed.GetAsync(key);
            return item != null;
        }

        public void Remove(string key)
        {
            _distributed.Remove(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributed.RemoveAsync(key);
        }

        public void Remove(IEnumerable<string> keys)
        {
            foreach (string key in keys)
            {
                _distributed.Remove(key);
            }
        }

        public async Task RemoveAsync(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                await _distributed.RemoveAsync(key);
            }
        }

        public bool Store<T>(string key, T data)
        {
            return Store(key, data, new DateTimeOffset(DateTime.UtcNow.AddDays(1)));
        }

        public async Task<bool> StoreAsync<T>(string key, T data)
        {
            return await StoreAsync(key, data, new DateTimeOffset(DateTime.UtcNow.AddDays(1)));
        }

        public bool Store<T>(string key, T data, DateTimeOffset expiry)
        {
            string str = JsonSerializer.Serialize(data);

            byte[] bytes = Encoding.UTF8.GetBytes(str);

            _distributed.Set(
                key,
                bytes,
                new DistributedCacheEntryOptions { AbsoluteExpiration = expiry }
            );

            return true;
        }

        public async Task<bool> StoreAsync<T>(string key, T data, DateTimeOffset expiry)
        {
            string str = JsonSerializer.Serialize(data);
            byte[] bytes = Encoding.UTF8.GetBytes(str);

            await _distributed.SetAsync(key, bytes, new DistributedCacheEntryOptions { AbsoluteExpiration = expiry });
            return true;
        }

        public T Retrieve<T>(string key)
        {
            TryRetrieve(key, out T data);
            return data;
        }

        public bool TryRetrieve<T>(string key, out T data)
        {
            data = default;

            byte[] bytes = _distributed.Get(key);
            if (bytes != null)
            {
                var str = Encoding.UTF8.GetString(bytes);
                try
                {
                    data = JsonSerializer.Deserialize<T>(str);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public async Task<T> TryRetrieveAsync<T>(string key)
        {
            T data = default;
            byte[] bytes = await _distributed.GetAsync(key);
            if (bytes != null)
            {
                var str = Encoding.UTF8.GetString(bytes);
                try
                {
                    data = JsonSerializer.Deserialize<T>(str);
                    return data;
                }
                catch
                {

                }
            }
            return data;
        }
    }
}
