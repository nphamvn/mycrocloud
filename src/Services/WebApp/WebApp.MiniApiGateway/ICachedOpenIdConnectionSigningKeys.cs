using System.Collections.Concurrent;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.MiniApiGateway;

public interface ICachedOpenIdConnectionSigningKeys
{
    Task<ICollection<SecurityKey>> Get(string issuer);
}

public class MemoryCachedOpenIdConnectionSigningKeys : ICachedOpenIdConnectionSigningKeys
{
    private readonly ConcurrentDictionary<string, ICollection<SecurityKey>> _cache = new();
    public async Task<ICollection<SecurityKey>> Get(string issuer)
    {
        if (_cache.TryGetValue(issuer, out var keys))
        {
            return keys;
        }
        var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"{issuer.TrimEnd('/')}/.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever());
        var discoveryDocument = await configurationManager.GetConfigurationAsync();
        keys = discoveryDocument.SigningKeys;
        _cache.TryAdd(issuer, keys);
        return keys;
    }
}