using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace ECommerceProductsAPI.Services.Caching;

public class RedisCacheService(IDistributedCache? cache) : IRedisCacheService
{
    private readonly IDistributedCache? _cache = cache;

    public T? GetData<T>(string key)
    {
        var data = _cache?.GetString(key);

        if (data is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(data);
    }

    public void SetData<T>(string key, T data)
    {
        var options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
        };

        _cache?.SetString(key, JsonSerializer.Serialize(data), options);
    }

    public void RemoveData(string key)
    {
        _cache?.Remove(key);
    }
}