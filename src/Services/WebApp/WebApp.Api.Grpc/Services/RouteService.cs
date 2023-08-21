using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;

namespace WebApp.Api.Grpc.Services
{
    public class RouteService(ILogger<RouteService> logger
        , IRouteRepository routeRepository) : WebAppRouteGrpcService.WebAppRouteGrpcServiceBase
    {
        public override async Task<ListRoutesResponse> ListRoutes(ListRoutesRequest request, ServerCallContext context)
        {
            var routes = await routeRepository.List(request.WebAppId, null, null);
            var res = new ListRoutesResponse();
            res.Routes.AddRange(routes.Select(r =>
            {
                var route = new ListRoutesResponse.Types.Route()
                {
                    RouteId = r.RouteId,
                    Name = r.Name,
                    Description = r.Description ?? "",
                    MatchPath = r.MatchPath,
                    MatchOrder = r.MatchOrder,
                    AuthorizationType = (int)r.AuthorizationType,
                    ResponseProvider = (int)r.ResponseProvider,
                    CreatedDate = DateTime.SpecifyKind(r.CreatedAt, DateTimeKind.Utc).ToTimestamp(),
                    UpdatedDate = r.UpdatedAt != null ? DateTime.SpecifyKind(r.UpdatedAt.Value, DateTimeKind.Utc).ToTimestamp() : default,
                };
                route.MatchMethods.AddRange(r.MatchMethods);
                return route;
            }));
            return res;
        }
        public override async Task<GetRouteByIdResponse> GetRouteById(GetRouteByIdRequest request, ServerCallContext context)
        {
            var route = await routeRepository.GetById(request.Id);
            var res = new GetRouteByIdResponse()
            {
                Id = route.RouteId,
                Name = route.Name,
                Description = route.Description ?? "",
                MatchPath = route.MatchPath,
                MatchOrder = route.MatchOrder,
                AuthorizationType = (int)route.AuthorizationType,
                AuthorizationJson = route.Authorization != null ? JsonSerializer.Serialize(route.Authorization) : "",
                ValidationJson = route.Validation != null ? JsonSerializer.Serialize(route.Validation) : "",
                ResponseProvider = (int)route.ResponseProvider,
                ResponseJson = route.Response != null ? JsonSerializer.Serialize(route.Response) : "",
                CreatedDate = DateTime.SpecifyKind(route.CreatedAt, DateTimeKind.Utc).ToTimestamp(),
                UpdatedDate = route.UpdatedAt != null ? DateTime.SpecifyKind(route.UpdatedAt.Value, DateTimeKind.Utc).ToTimestamp() : default,
            };
            res.MatchMethods.AddRange(route.MatchMethods);
            return res;
        }
        public override async Task<CreateRouteResponse> CreateRoute(CreateRouteRequest request, ServerCallContext context)
        {
            
            var entity = new RouteEntity
            {
                Name = request.Name,
                Description = request.Description,
                MatchPath = request.MatchPath,
                MatchOrder = request.MatchOrder,
                AuthorizationType = Domain.Shared.RouteAuthorizationType.AllowAnonymous
            };
            switch (request.ResponseCase)
            {
                case CreateRouteRequest.ResponseOneofCase.MockResponse:
                    entity.ResponseProvider = Domain.Shared.RouteResponseProvider.Mock;
                    break;
                default:
                    break;
            }
            var id = await routeRepository.Add(request.AppId, entity);
            await routeRepository.AddMatchMethods(id, request.MatchMethods.ToList());
            return new CreateRouteResponse { RouteId = id };
        }
        public override async Task<EditRouteResponse> EdiRoute(EditRouteRequest request, ServerCallContext context)
        {
            return await base.EdiRoute(request, context);
        }
    }
}
