using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WebApp.Domain.Repositories;

namespace WebApp.Api.Grpc.Services;

public class WebAppService(ILogger<WebAppService> logger
    , IWebAppRepository webAppRepository) : WebAppGrpcService.WebAppGrpcServiceBase
{
    private readonly ILogger<WebAppService> _logger = logger;

    public override async Task<CreateAppResponse> CreateApp(CreateAppRequest request, ServerCallContext context)
    {
        var existingApp = await webAppRepository.FindByUserId(request.UserId, request.AppName);
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
        var apps = await webAppRepository.Search(request.UserId, null, null);
        var res = new ListAppsByUserIdResponse();
        res.Apps.AddRange(apps.Select(a => new ListAppsByUserIdResponse.Types.App
        {
            AppId = a.WebAppId,
            AppName = a.Name
        }));
        return res;
    }

    public override async Task<GetAppByUserIdAndAppNameResponse> GetAppByUserIdAndAppName(
        GetAppByUserIdAndAppNameRequest request, ServerCallContext context)
    {
        var app = await webAppRepository.FindByUserId(request.UserId, request.AppName);
        if (app == null)
        {
            return new();
        }

        return new()
        {
            AppId = app.WebAppId,
            AppName = app.Name,
            Description = app.Description ?? "",
            CreatedTime = DateTime.SpecifyKind(app.CreatedDate, DateTimeKind.Utc).ToTimestamp(),
            UpdatedTime = app.UpdatedDate != null
                ? DateTime.SpecifyKind(app.UpdatedDate.Value, DateTimeKind.Utc).ToTimestamp()
                : null
        };
    }

    public override async Task<GetAppByIdResponse> GetAppById(GetAppByIdRequest request, ServerCallContext context)
    {
        var app = await webAppRepository.Get(request.AppId);
        return new()
        {
            AppId = app.WebAppId,
            UserId = app.UserId,
            AppName = app.Name,
            Description = app.Description,
            CreatedTime = DateTime.SpecifyKind(app.CreatedDate, DateTimeKind.Utc).ToTimestamp(),
            UpdatedTime = app.UpdatedDate != null
                ? DateTime.SpecifyKind(app.UpdatedDate.Value, DateTimeKind.Utc).ToTimestamp()
                : null
        };
    }

    public override async Task<DeleteAppResponse> DeleteApp(DeleteAppRequest request, ServerCallContext context)
    {
        await webAppRepository.Delete(request.AppId);
        return new();
    }
}