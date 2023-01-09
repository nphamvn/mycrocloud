using MockServer.ReverseProxyServer.Extentions;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Services;

public class RequestHandler
{
    public async Task Handle(HttpContext context)
    {
        AppRequest req = (AppRequest)context.Items[nameof(AppRequest)];
        req.HttpContext = context;
        IRequestHandler requestHandler = context.RequestServices
                                            .GetRequiredService<IRequestHandlerFactory>()
                                            .GetInstance(req.Type);

        var response = await requestHandler.GetResponseMessage(req);

        await context.WriteResponse(response);
    }
}