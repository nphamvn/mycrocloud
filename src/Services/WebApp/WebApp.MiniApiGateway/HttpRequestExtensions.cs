using Microsoft.AspNetCore.Cors.Infrastructure;

namespace WebApp.MiniApiGateway;

public static class HttpRequestExtensions
{
    public static bool IsPreflightRequest(this HttpRequest request)
    {
        return HttpMethods.IsOptions(request.Method)
                    && request.Headers.ContainsKey(CorsConstants.AccessControlRequestMethod);
    }
}
