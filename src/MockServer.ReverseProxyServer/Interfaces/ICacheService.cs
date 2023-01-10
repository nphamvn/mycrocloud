namespace MockServer.ReverseProxyServer.Interfaces;

public interface ICacheService
{
    Task Set<T>(string key, T obj);
    Task<bool> Exists(string key);
    Task<T> Get<T>(string key);
}
