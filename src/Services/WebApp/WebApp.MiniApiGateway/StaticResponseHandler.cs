using Route = WebApp.Domain.Entities.Route;

namespace WebApp.MiniApiGateway;

public static class StaticResponseHandler
{
    public static Task Handle(HttpContext context)
    {
        var route = (Route)context.Items["_Route"]!;
        context.Response.StatusCode = route.ResponseStatusCode ??
                                      throw new InvalidOperationException("ResponseStatusCode is null");
        if (route.ResponseHeaders is not null)
        {
            foreach (var header in route.ResponseHeaders)
            {
                context.Response.Headers.Append(header.Name, header.Value);
            }
        }

        return context.Response.WriteAsync(route.ResponseBody ??
                                           throw new InvalidOperationException("ResponseBody is null"));
    }
}