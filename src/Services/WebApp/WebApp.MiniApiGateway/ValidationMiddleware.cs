using Jint;
using Route = WebApp.Domain.Entities.Route;
using WebApp.Domain.Repositories;

namespace WebApp.MiniApiGateway;

public class ValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IRouteRepository routeRepository)
    {
        var route = (Route)context.Items["_Route"]!;
            var validations = await routeRepository.GetValidations(route.Id);
            if (validations.Count == 0)
            {
                await next(context);
                return;
            }
            var request = context.Items["_Request"]!;
            var engine = new Engine()
                .SetValue("request", request);
            var errors = new Dictionary<string,string>();
            foreach (var validation in validations)
            {
                var errorKey = $"{validation.Source.ToLower()}:{validation.Name}";
                switch (validation.Source.ToLower())
                {
                    case "header":
                    {
                        foreach (var rule in validation.Rules)
                        {
                            switch (rule.Key.ToLower())
                            {
                                case "required":
                                    if (!context.Request.Headers.TryGetValue(validation.Name, out var value) || string.IsNullOrEmpty(value))
                                    {
                                        var property = rule.Value.GetType().GetProperty("message");
                                        var message = property != null ? property.GetValue(rule.Value)?.ToString() ?? "" : $"header {validation.Name} is required";
                                        errors.Add(errorKey, message);
                                    }
                                    break;
                            }
                        }
        
                        foreach (var expression in validation.Expressions ?? [])
                        {
                            engine.Evaluate(expression);
                        }
                        break;
                    }
                }
            }
        
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