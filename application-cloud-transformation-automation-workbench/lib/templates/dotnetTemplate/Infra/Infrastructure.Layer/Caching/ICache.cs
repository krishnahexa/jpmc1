namespace Infrastructure.Common.Caching
{
    public interface ICache
    {
        Task SetStringAsync(string key, string value);
        Task SetStringAsync(string key, string value, int cacheTime);
        Task SetObjectAsync(string key, object value);
        Task SetObjectAsync(string key, object value, int cacheTime);
        Task<string> GetStringAsync(string key);
        Task<T> GetObjectAsync<T>(string key);
        Task<bool> ExistAsync(string key);
        Task DeleteAsync(string key);
        List<string> ListKeys();
        Dictionary<string, string> ListKeyValues();
    }
}
