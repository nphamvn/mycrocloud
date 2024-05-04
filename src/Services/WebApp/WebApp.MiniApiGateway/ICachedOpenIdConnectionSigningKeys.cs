using System.Collections.Concurrent;
using System.Collections.ObjectModel;
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
    private readonly ConcurrentDictionary<string, JsonWebKeySet> _jsonWebKeySet = new();
    
    public async Task<ICollection<SecurityKey>> Get(string issuer)
    {
        if (_jsonWebKeySet.TryGetValue(issuer, out var jsonWebKeySet))
        {
            ICollection<SecurityKey> keys = new Collection<SecurityKey>();
            foreach (var signingKey in jsonWebKeySet.GetSigningKeys())
                keys.Add(signingKey);
            return keys;
        }
        
        var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"{issuer.TrimEnd('/')}/.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever());
        var openIdConnectConfiguration = await configurationManager.GetConfigurationAsync();
        _jsonWebKeySet.TryAdd(issuer, openIdConnectConfiguration.JsonWebKeySet);
        return openIdConnectConfiguration.SigningKeys;
    }
}