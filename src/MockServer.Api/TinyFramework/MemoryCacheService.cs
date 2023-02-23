using Microsoft.Extensions.Caching.Memory;
using MockServer.Api.Interfaces;

namespace MockServer.Api.Services;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }
    public Task Set<T>(string key, T obj)
    {
        return Task.FromResult(_cache.Set<T>(key, obj));
    }

    public Task<T> Get<T>(string key)
    {
        return Task.FromResult<T>(_cache.Get<T>(key));
    }

    public Task<bool> Exists(string key)
    {
        return Task.FromResult(_cache.TryGetValue(key, out _));
    }
}
