using MockServer.ReverseProxyServer.Extentions;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Services;

public class RequestHandler
{
    public async Task Handle(HttpContext context)
    {
        var request = (IncomingRequest)context.Items[nameof(IncomingRequest)];

        AppRequest appRequest = new AppRequest
        {
            Id = request.Id,
            Path = request.Path
        };
        appRequest.HttpContext = context;

        IRequestHandler requestHandler = context.RequestServices
                                            .GetRequiredService<IRequestHandlerFactory>()
                                            .GetInstance(request.RequestType);

        var response = await requestHandler.GetResponseMessage(appRequest);

        await context.WriteResponse(response);
    }

    private AppRequest Map(IncomingRequest request, HttpContext context)
    {
        var appRequest = new AppRequest();
        return appRequest;
    }
}