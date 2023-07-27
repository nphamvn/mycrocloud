using Grpc.Core;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;

namespace WebApp.Api.Grpc.Services;

public class WebAppService(ILogger<WebAppService> logger
        , IWebAppRepository webAppRepository) : WebApp.WebAppBase
{
    private readonly ILogger<WebAppService> _logger = logger;
    private readonly IWebAppRepository _webAppRepository = webAppRepository;

    public override async Task<CreateWebAppResponse> CreateWebApp(CreateWebAppRequest request, ServerCallContext context)
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
    public override async Task<RenameWebAppResponse> RenameWebApp(RenameWebAppRequest request, ServerCallContext context)
    {
        return await base.RenameWebApp(request, context);
    }
}