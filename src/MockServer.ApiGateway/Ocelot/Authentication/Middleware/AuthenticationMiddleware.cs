namespace Ocelot.Authentication.Middleware
{
    using Microsoft.AspNetCore.Http;
    using Ocelot.Configuration;
    using Ocelot.Logging;
    using Ocelot.Middleware;
    using System.Threading.Tasks;
    using System.Text.Encodings.Web;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Logging;
    using Ocelot.Authentication.JwtBearer;

    public class AuthenticationMiddleware : OcelotMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _loggerFactory;

        public AuthenticationMiddleware(RequestDelegate next,
            ILoggerFactory loggerFactory,
            IOcelotLoggerFactory ocelotLoggerFactory)
            : base(ocelotLoggerFactory.CreateLogger<AuthenticationMiddleware>())
        {
            _next = next;
            _loggerFactory = loggerFactory;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            //var downstreamRoute = httpContext.Items.DownstreamRoute();
            // if (httpContext.Request.Method.ToUpper() != "OPTIONS" && IsAuthenticatedRoute(downstreamRoute))
            // {
            //     Logger.LogInformation($"{httpContext.Request.Path} is an authenticated route. {MiddlewareName} checking if client is authenticated");

            //     var result = await httpContext.AuthenticateAsync(downstreamRoute.AuthenticationOptions.AuthenticationProviderKey);

            //     httpContext.User = result.Principal;

            //     if (httpContext.User.Identity.IsAuthenticated)
            //     {
            //         Logger.LogInformation($"Client has been authenticated for {httpContext.Request.Path}");
            //         await _next.Invoke(httpContext);
            //     }
            //     else
            //     {
            //         var error = new UnauthenticatedError(
            //             $"Request for authenticated route {httpContext.Request.Path} by {httpContext.User.Identity.Name} was unauthenticated");

            //         Logger.LogWarning($"Client has NOT been authenticated for {httpContext.Request.Path} and pipeline error set. {error}");

            //         httpContext.Items.SetError(error);
            //     }
            // }
            // else
            // {
            //     Logger.LogInformation($"No authentication needed for {httpContext.Request.Path}");

            //     await _next.Invoke(httpContext);
            // }
            var app = httpContext.Items.WebApplication();

            var options = new JwtBearerOptions {
                Configuration = new() {
                    
                }
            };
            var loggerFactory = _loggerFactory;
            var encoder = UrlEncoder.Default;
            var systemClock = new SystemClock();
            var jwtBearerHandler = new JwtBearerHandler(
                Options.Create(options),
                loggerFactory,
                encoder,
                systemClock
            );
            await jwtBearerHandler.InitializeAsync(new AuthenticationScheme("Bearer", "Bearer", typeof(JwtBearerHandler)), httpContext);
            var result = jwtBearerHandler.AuthenticateAsync();
        }

        private static bool IsAuthenticatedRoute(DownstreamRoute route)
        {
            return route.IsAuthenticated;
        }
    }
}
