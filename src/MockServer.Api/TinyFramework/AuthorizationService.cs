using System.Security.Claims;
using System.Text.Json;
using Jint;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Api.TinyFramework;

public class AuthorizationService : IAuthorizationService
{
    private readonly Engine _engine;
    public AuthorizationService()
    {
        _engine = new Engine();
    }
    public bool CheckRequirement(Policy policy, ClaimsPrincipal user)
    {
        var userDic = new Dictionary<string, object>();
        foreach (var claim in user.Claims)
        {
            userDic[claim.Type] = claim.Value;
        }
        _engine.SetValue("user", userDic);
        _engine.Execute($"let user = {JsonSerializer.Serialize(userDic)};");
        foreach (var condition in policy.ConditionalExpressions)
        {
            var result = _engine.Evaluate(condition).AsBoolean();
            if (result == false)
            {
                return false;
            }
        }
        
        return true;
    }
}
