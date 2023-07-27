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
        //var userId = request.UserId;
        var entity = new WebAppEntity() {
            //request.
        };
        return await base.CreateWebApp(request, context);
    }
    public override async Task<RenameWebAppResponse> RenameWebApp(RenameWebAppRequest request, ServerCallContext context)
    {
        return await base.RenameWebApp(request, context);
    }
    private WebAppEntity CreateWebAppRequestToWebAppEntity(CreateWebAppRequest request)
    {
        return new()
        {

        };
    }
    private WebAppEntity RenameWebAppRequestToWebAppEntity(RenameWebAppRequest request)
    {
        return new()
        {

        };
    }
}