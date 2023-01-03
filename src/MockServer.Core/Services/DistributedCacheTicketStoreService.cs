using MNB.Domain.Services.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace MNB.Web.Services
{
    public class DistributedCacheTicketStoreService : BaseService, ITicketStore
    {
        private IConfiguration Configuration { get; }
        private readonly IDistributedCache _distributedCache;
        private readonly DistributedCacheEntryOptions _distributedCacheEntryOptions;

        public DistributedCacheTicketStoreService(
            IConfiguration configuration,
            IDistributedCache distributedCache
        )
        {
            Configuration = configuration;
            _distributedCache = distributedCache;
            _distributedCacheEntryOptions = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(int.Parse(Configuration["AspNetCoreConfig:DistributedCacheEntryOptions:SlidingExpirationMinutes"])) };

            Logger.Trace($"{GetType().Name}");
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
                    , _distributedCacheEntryOptions
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
                    , _distributedCacheEntryOptions
                );
            return key;
        }
    }
}