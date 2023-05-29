using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MockServer.Core.Repositories;
using Ocelot.DownstreamWebApplicationFinder.Finder;
using Ocelot.Logging;
using Ocelot.Middleware;
using System.Threading.Tasks;

namespace Ocelot.DownstreamWebApplicationFinder.Middleware
{
    public class DownstreamWebApplicationFinderMiddleware : OcelotMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebApplicationRepository _webApplicationRepository;
        private readonly IConfiguration _configuration;

        public DownstreamWebApplicationFinderMiddleware(
            RequestDelegate next
            , IWebApplicationRepository webApplicationRepository
            , IConfiguration configuration
            , IOcelotLoggerFactory loggerFactory) : base(loggerFactory.CreateLogger<DownstreamWebApplicationFinderMiddleware>())
        {
            _next = next;
            _webApplicationRepository = webApplicationRepository;
            _configuration = configuration;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.TryGetValue(_configuration["WebApplicationIdHeader"], out var appIdString) ||
                !int.TryParse(appIdString.ToString().TrimStart('0'), out var appId))
            {
                httpContext.Items.UpsertErrors(new() { new UnableToFindApplicationError() });
                return;
            }
            var app = await _webApplicationRepository.Get(appId);
            if (app == null)
            {
                httpContext.Items.UpsertErrors(new() { new UnableToFindApplicationError() });
                return;
            }
            if (app.Blocked)
            {
                httpContext.Items.UpsertErrors(new() { new ApplicationIsBlockedError() });
                return;
            }
            if (!app.Enabled)
            {
                httpContext.Items.UpsertErrors(new() { new ApplicationIsNotEnabledError() });
                return;
            }
            httpContext.Request.Headers.Remove(_configuration["WebApplicationIdHeader"]);
            httpContext.Items.SetWebApplication(app);

            await _next.Invoke(httpContext);
        }
    }
}
