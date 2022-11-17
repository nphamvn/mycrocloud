using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Services;

public class RequestHandler
{
    public async Task Handle(HttpContext context)
    {
        var request = (RequestModel)context.Items[nameof(RequestModel)];

        AppRequest appRequest = Map(request, context);
        IRequestHandler requestHandler = context.RequestServices
                                            .GetRequiredService<IRequestHandlerFactory>()
                                            .GetInstance(appRequest);

        var response = await requestHandler.Handle(appRequest);
        await response.WriteResponse();
    }

    private AppRequest Map(RequestModel request, HttpContext context)
    {
        var appRequest = new AppRequest();
        appRequest.HttpContext = context;

        return appRequest;
    }
}