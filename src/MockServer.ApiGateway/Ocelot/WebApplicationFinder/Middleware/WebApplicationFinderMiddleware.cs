using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MockServer.Core.Repositories;
using Ocelot.Logging;
using Ocelot.Middleware;
using System.Net;
using System.Threading.Tasks;

namespace Ocelot.WebApplicationFinder.Middleware
{
    public class WebApplicationFinderMiddleware : OcelotMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebApplicationRepository _webApplicationRepository;
        private readonly IConfiguration _configuration;

        public WebApplicationFinderMiddleware(
            RequestDelegate next
            ,IWebApplicationRepository webApplicationRepository
            , IConfiguration configuration
            , IOcelotLoggerFactory loggerFactory) : base(loggerFactory.CreateLogger<WebApplicationFinderMiddleware>())
        {
            _next = next;
            _webApplicationRepository = webApplicationRepository;
            _configuration = configuration;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.TryGetValue(_configuration["WebApplicationIdHeader"], out var appId))
            {
                return;
            }
            if (!int.TryParse(appId.ToString().TrimStart('0'), out var id))
            {
                return;
            }
            var app = await _webApplicationRepository.Get(id);
            if (app == null)
            {
                return;
            }
            if (app.Blocked)
            {
                return;
            }
            if (!app.Enabled)
            {
                return;
            }

            httpContext.Items.SetWebApplication(app);

            await _next.Invoke(httpContext);
        }
    }
}
