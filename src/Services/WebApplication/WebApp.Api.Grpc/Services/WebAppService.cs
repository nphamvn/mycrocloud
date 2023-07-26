using Grpc.Core;
using WebApp.Domain.Repositories;

namespace WebApp.Api.Grpc.Services;

public class WebAppService : WebApp.WebAppBase
{
    private readonly ILogger<WebAppService> _logger;

    public WebAppService(ILogger<WebAppService> logger
        , IWebAppRepository webAppRepository)
    {
        _logger = logger;
    }

    public override Task<CreateWebAppReply> CreateWebApp(CreateWebAppRequest request, ServerCallContext context)
    {
        return base.CreateWebApp(request, context);
    }
}