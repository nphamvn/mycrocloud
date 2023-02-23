using System.Security.Claims;
using System.Text.Json;
using Jint;
using MockServer.Core.Models.Auth;

namespace MockServer.Api.TinyFramework;

public class AuthorizationService : IAuthorizationService
{
    private readonly Engine _engine;
    public AuthorizationService()
    {
        _engine = new Engine();
    }
    public bool CheckRequirement(Policy requirement, ClaimsPrincipal user)
    {
        var userDic = new Dictionary<string, object>();
        foreach (var claim in user.Claims)
        {
            userDic[claim.Type] = claim.Value;
        }
        _engine.SetValue("user", userDic);
        _engine.Execute($"let user = {JsonSerializer.Serialize(userDic)};");
        var result = _engine.Evaluate(requirement.ConditionalExpression).AsBoolean();
        return result;
    }
}
