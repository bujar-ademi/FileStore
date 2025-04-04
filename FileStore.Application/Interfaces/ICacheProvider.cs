using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileStore.Application.Interfaces
{
    public interface ICacheProvider
    {
        bool Store<T>(string key, T data);
        Task<bool> StoreAsync<T>(string key, T data);
        bool Store<T>(string key, T data, DateTimeOffset expiry);
        Task<bool> StoreAsync<T>(string key, T data, DateTimeOffset expiry);
        bool Exists(string key);
        Task<bool> ExistsAsync(string key);
        bool TryRetrieve<T>(string key, out T data);
        T Retrieve<T>(string key);
        Task<T> TryRetrieveAsync<T>(string key);
        void Remove(string key);
        Task RemoveAsync(string key);
        void Remove(IEnumerable<string> keys);
        Task RemoveAsync(IEnumerable<string> keys);
    }
}
