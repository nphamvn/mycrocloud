using MockServer.ReverseProxyServer.Interfaces;

namespace MockServer.ReverseProxyServer.Services;

public class LiteDBCacheService : ICacheService
{
    public Task Set<T>(string key, T obj)
    {
        throw new NotImplementedException();
    }

    public Task<T> Get<T>(string Key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Exists(string key)
    {
        throw new NotImplementedException();
    }
}
