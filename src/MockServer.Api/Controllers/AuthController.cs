using Microsoft.AspNetCore.Mvc;
using MockServer.Api.Models;

namespace MockServer.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    // POST /auth/id-token --data-raw 'grant_type=client_credentials&client_id='jwt_bearer_scheme_1'&username=username&password=password
    [HttpPost("id-token")]
    public async Task<IActionResult> IdToken(IdTokenRequest request)
    {
        
        if (request.GrantType == GrantType.ClientCredentials)
        {
            string jwtId = request.ClientId;
            string userpoolId = GetConfiguredUserPoolId(jwtId);
            string username = request.Username;
            string password = request.Password;
            bool authenticated = Authenticate(username, password, userpoolId);
            if (authenticated)
            {
                object profile = GetProfile(username, userpoolId);
                string idToken = GenerateToken(profile);
            }
        }
        return Ok();
    }

    private string GenerateToken(object profile)
    {
        throw new NotImplementedException();
    }

    private object GetProfile(string username, string userpoolId)
    {
        throw new NotImplementedException();
    }

    private bool Authenticate(string username, string password, string userpoolId)
    {
        throw new NotImplementedException();
    }

    private string GetConfiguredUserPoolId(string jwtId)
    {
        throw new NotImplementedException();
    }
}