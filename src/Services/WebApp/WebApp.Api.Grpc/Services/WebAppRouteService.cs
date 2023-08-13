using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WebApp.Domain.Repositories;

namespace WebApp.Api.Grpc.Services
{
    public class WebAppRouteService(ILogger<WebAppRouteService> logger
        , IWebAppRouteRepository webAppRouteRepository) : WebAppRouteGrpcService.WebAppRouteGrpcServiceBase
    {
        private readonly ILogger<WebAppRouteService> _logger = logger;
        private readonly IWebAppRouteRepository _webAppRouteRepository = webAppRouteRepository;
        public override async Task<ListRoutesResponse> ListRoutes(ListRoutesRequest request, ServerCallContext context)
        {
            var routes = await _webAppRouteRepository.List(request.WebAppId, null, null);
            var res = new ListRoutesResponse();
            res.Routes.AddRange(routes.Select(r =>
            {
                var route = new ListRoutesResponse.Types.Route()
                {
                    RouteId = r.RouteId,
                    Name = r.Name,
                    Description = r.Description,
                    MatchPath = r.MatchPath,
                    MatchOrder = r.MatchOrder,
                    AuthorizationType = (int)r.AuthorizationType,
                    ResponseProvider = (int)r.ResponseProvider,
                    CreatedDate = DateTime.SpecifyKind(r.CreatedDate, DateTimeKind.Utc).ToTimestamp(),
                    UpdatedDate = r.UpdatedDate != null ? DateTime.SpecifyKind(r.UpdatedDate.Value, DateTimeKind.Utc).ToTimestamp() : default,
                };
                route.MatchMethods.AddRange(r.MatchMethods);
                return route;
            }));
            return res;
        }
        public override async Task<GetRouteByIdResponse> GetRouteById(GetRouteByIdRequest request, ServerCallContext context)
        {
            var route = await _webAppRouteRepository.GetById(request.Id);
            var res = new GetRouteByIdResponse()
            {
                Id = route.RouteId,
                Name = route.Name,
                Description = route.Description,
                MatchPath = route.MatchPath,
                MatchOrder = route.MatchOrder,
                AuthorizationType = (int)route.AuthorizationType,
                AuthorizationJson = route.Authorization != null ? JsonSerializer.Serialize(route.Authorization) : "",
                ValidationJson = route.Validation != null ? JsonSerializer.Serialize(route.Validation) : "",
                ResponseProvider = (int)route.ResponseProvider,
                ResponseJson = route.Response != null ? JsonSerializer.Serialize(route.Response) : "",
                CreatedDate = DateTime.SpecifyKind(route.CreatedDate, DateTimeKind.Utc).ToTimestamp(),
                UpdatedDate = route.UpdatedDate != null ? DateTime.SpecifyKind(route.UpdatedDate.Value, DateTimeKind.Utc).ToTimestamp() : default,
            };
            res.MatchMethods.AddRange(route.MatchMethods);
            return res;
        }
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
