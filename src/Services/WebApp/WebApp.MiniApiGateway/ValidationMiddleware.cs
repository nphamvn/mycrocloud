using Route = WebApp.Domain.Entities.Route;
using WebApp.Domain.Repositories;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace WebApp.MiniApiGateway;

public class ValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IRouteRepository routeRepository)
    {
        var route = (Route)context.Items["_Route"]!;
        if (string.IsNullOrEmpty(route.RequestQuerySchema) 
            && string.IsNullOrEmpty(route.RequestHeaderSchema)
            && string.IsNullOrEmpty(route.RequestBodySchema))
        {
            await next(context);
            return;
        }

        List<ValidationError> errors = [];
        if (!string.IsNullOrEmpty(route.RequestQuerySchema))
        {
            var schema = JSchema.Parse(route.RequestQuerySchema);
            var query = JObject.FromObject(context.Request.Query.ToDictionary());
            query.IsValid(schema, out IList<ValidationError>? validationErrors);
            errors.AddRange(validationErrors);
        }

        if (!string.IsNullOrEmpty(route.RequestHeaderSchema))
        {
            var schema = JSchema.Parse(route.RequestHeaderSchema);
            var query = JObject.FromObject(context.Request.Headers.ToDictionary());
            query.IsValid(schema, out IList<ValidationError>? validationErrors);
            errors.AddRange(validationErrors);
        }

        //TODO: Validate body

        if (errors.Count == 0)
        {
            await next(context);
            return;
        }

        context.Response.StatusCode = 400;
        await context.Response.WriteAsync($"Bad request. {errors.First().Value}");
    }
}

public static class ValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseValidationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ValidationMiddleware>();
    }
}