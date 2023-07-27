using Grpc.Core;
using WebApp.Domain.Repositories;

namespace WebApp.Api.Grpc.Services
{
    public class WebAppRouteService(ILogger<WebAppRouteService> logger
        , IWebAppRouteRepository webAppRouteRepository) : WebAppRoute.WebAppRouteBase
    {
        private readonly ILogger<WebAppRouteService> _logger = logger;
        private readonly IWebAppRouteRepository _webAppRouteRepository = webAppRouteRepository;
        public override async Task<CreateRouteResponse> CreateRoute(CreateRouteRequest request, ServerCallContext context)
        {
            return await base.CreateRoute(request, context);
        }
        public override async Task<EditRouteResponse> EdiRoute(EditRouteRequest request, ServerCallContext context)
        {
            return await base.EdiRoute(request, context);
        }
    }
}
