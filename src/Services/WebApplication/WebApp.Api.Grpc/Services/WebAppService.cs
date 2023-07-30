using Grpc.Core;
using WebApp.Domain.Repositories;

namespace WebApp.Api.Grpc.Services;

public class WebAppService(ILogger<WebAppService> logger
        , IWebAppRepository webAppRepository) : WebApp.WebAppBase
{
    private readonly ILogger<WebAppService> _logger = logger;
    private readonly IWebAppRepository _webAppRepository = webAppRepository;

    public override async Task<CreateWebAppResponse> Create(CreateWebAppRequest request, ServerCallContext context)
    {
        var existingApp = await _webAppRepository.FindByUserId(request.UserId, request.Name);
        if (existingApp != null)
        {
            return new();
        }
        await _webAppRepository.Add(request.UserId, new()
        {
            Name = request.Name,
        });
        return new();
    }
    public override async Task<GetAllWebAppResponse> GetAll(GetAllWebAppRequest request, ServerCallContext context)
    {
        var apps = await _webAppRepository.Search(request.UserId, null, null);
        var res = new GetAllWebAppResponse();
        res.WebApps.AddRange(apps.Select(a => new GetAllWebAppResponse.Types.WebApp
        {
            Id = a.WebAppId,
            Name = a.Name
        }));
        return res;
    }
    public override async Task<GetWebAppResponse> Get(GetWebAppRequest request, ServerCallContext context)
    {
        var app = await _webAppRepository.FindByUserId(request.UserId, request.Name);
        if (app == null)
        {
            return new ();
        }
        return new() {
            Name = app.Name,
            Description = app.Description ?? "",
        };
    }
    public override Task<DeleteWebAppResponse> Delete(DeleteWebAppRequest request, ServerCallContext context)
    {
        return base.Delete(request, context);
    }
    public override Task<RenameWebAppResponse> Rename(RenameWebAppRequest request, ServerCallContext context)
    {
        return base.Rename(request, context);
    }
}