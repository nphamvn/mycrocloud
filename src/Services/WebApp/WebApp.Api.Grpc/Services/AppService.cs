using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WebApp.Domain.Repositories;

namespace WebApp.Api.Grpc.Services;

public class AppService(ILogger<AppService> logger
    , IAppRepository webAppRepository) : WebAppGrpcService.WebAppGrpcServiceBase
{
    private readonly ILogger<AppService> _logger = logger;

    public override async Task<CreateAppResponse> CreateApp(CreateAppRequest request, ServerCallContext context)
    {
        var existingApp = await webAppRepository.FindByUserIdAndAppName(request.UserId, request.AppName);
        if (existingApp != null)
        {
            return new();
        }

        await webAppRepository.Add(request.UserId, new()
        {
            Name = request.AppName,
        });
        return new();
    }

    public override async Task<ListAppsByUserIdResponse> ListAppsByUserId(ListAppsByUserIdRequest request,
        ServerCallContext context)
    {
        var apps = await webAppRepository.ListByUserId(request.UserId, null, null);
        var res = new ListAppsByUserIdResponse();
        res.Apps.AddRange(apps.Select(a => new ListAppsByUserIdResponse.Types.App
        {
            AppId = a.AppId,
            AppName = a.Name
        }));
        return res;
    }

    public override async Task<GetAppByUserIdAndAppNameResponse> GetAppByUserIdAndAppName(
        GetAppByUserIdAndAppNameRequest request, ServerCallContext context)
    {
        var app = await webAppRepository.FindByUserIdAndAppName(request.UserId, request.AppName);
        if (app == null)
        {
            return new();
        }

        return new()
        {
            AppId = app.AppId,
            AppName = app.Name,
            Description = app.Description ?? "",
            CreatedTime = DateTime.SpecifyKind(app.CreatedAt, DateTimeKind.Utc).ToTimestamp(),
            UpdatedTime = app.UpdatedAt != null
                ? DateTime.SpecifyKind(app.UpdatedAt.Value, DateTimeKind.Utc).ToTimestamp()
                : null
        };
    }

    public override async Task<GetAppByIdResponse> GetAppById(GetAppByIdRequest request, ServerCallContext context)
    {
        var app = await webAppRepository.GetByAppId(request.AppId);
        return new()
        {
            AppId = app.AppId,
            UserId = app.UserId,
            AppName = app.Name,
            Description = app.Description,
            CreatedTime = DateTime.SpecifyKind(app.CreatedAt, DateTimeKind.Utc).ToTimestamp(),
            UpdatedTime = app.UpdatedAt != null
                ? DateTime.SpecifyKind(app.UpdatedAt.Value, DateTimeKind.Utc).ToTimestamp()
                : null
        };
    }

    public override async Task<DeleteAppResponse> DeleteApp(DeleteAppRequest request, ServerCallContext context)
    {
        await webAppRepository.Delete(request.AppId);
        return new();
    }
}