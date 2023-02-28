using System.Net;
using System.Text;
using MockServer.Core.Interfaces;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using CoreRoute = MockServer.Core.WebApplications.Route;
using CoreWebApplication = MockServer.Core.WebApplications.WebApplication;

namespace MockServer.Api.TinyFramework;

public class MockIntegrationHandler : RequestHandler
{
    private readonly IWebApplicationRouteRepository _requestRepository;
    private readonly IWebApplicationRepository _projectRepository;
    private readonly IFactoryService _factoryService;

    public MockIntegrationHandler(IWebApplicationRouteRepository requestRepository,
        IWebApplicationRepository projectRepository,
        IFactoryService factoryService)
    {
        _requestRepository = requestRepository;
        _projectRepository = projectRepository;
        _factoryService = factoryService;
    }
    
    public override async Task Handle(HttpContext context)
    {
        var route = context.Items[typeof(CoreRoute).Name] as CoreRoute;
        var app = context.Items[typeof(CoreWebApplication).Name] as CoreWebApplication;
        var integration = await _requestRepository.GetMockIntegration(route.Id);
        var handlerContext = _factoryService.Create<JintHandlerContext>(context) ;
        handlerContext.WebApplication = app;
        handlerContext.Setup();

        if (!string.IsNullOrEmpty(integration.Code))
        {
            handlerContext.JintEngine.Execute(integration.Code);
        }
        var headers = integration.ResponseHeaders;
        foreach (var header in headers)
        {
            context.Response.Headers.Add(header.Name, header.Value);
        }
        string body = "";
        if (integration.ResponseBodyTextRenderEngine == 1)
        {
            body = integration.ResponseBodyText;
        }
        else if (integration.ResponseBodyTextRenderEngine == 2)
        {
            //Handlebars
            var renderService = _factoryService.Create<HandlebarsTemplateRenderer>(handlerContext.JintEngine) ;
            body = renderService.Render(integration.ResponseBodyText);
        }
        else if (integration.ResponseBodyTextRenderEngine == 3)
        {
            var renderService = _factoryService.Create<ExpressionTemplateWithScriptRenderer>(handlerContext.JintEngine) ;
            body = renderService.Render(integration.ResponseBodyText);
        }

        await context.WriteResponse(new ResponseMessage
        {
            StatusCode = (HttpStatusCode)integration.ResponseStatusCode,
            Content = new StringContent(body, Encoding.UTF8)
        });
    }
}