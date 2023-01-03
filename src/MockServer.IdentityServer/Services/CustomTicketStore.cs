using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace MockServer.IdentityServer.Services;

public class DistributedCacheTicketStore : ITicketStore
{
    private readonly IDistributedCache _distributedCache;
    public DistributedCacheTicketStore(IDistributedCache distributedCache)
    {
        _distributedCache = _distributedCache;
    }
    public async Task RemoveAsync(string key)
    {
        var ticketBytes = await _distributedCache.GetStringAsync(key);
        if (ticketBytes != null)
        {
            await _distributedCache.RemoveAsync(key);
        }
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var ticketBytes = TicketSerializer.Default.Serialize(ticket);
        await _distributedCache.SetAsync(
                  key
                , ticketBytes
            );
    }

    public async Task<AuthenticationTicket> RetrieveAsync(string key)
    {
        var ticketBytes = await _distributedCache.GetAsync(key);
        var ticket = ticketBytes != null ? TicketSerializer.Default.Deserialize(ticketBytes) : null;
        return ticket;
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = Guid.NewGuid().ToString();
        var ticketBytes = TicketSerializer.Default.Serialize(ticket);
        await _distributedCache.SetAsync(
                  key
                , ticketBytes
            );
        return key;
    }
}
