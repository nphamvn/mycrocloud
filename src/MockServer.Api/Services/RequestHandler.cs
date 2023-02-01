using MockServer.Api.Extentions;
using MockServer.Api.Interfaces;
using MockServer.Api.Models;

namespace MockServer.Api.Services;

public class RequestHandler
{
    public async Task Handle(HttpContext context)
    {
        Request req = (Request)context.Items[nameof(Request)];
        req.HttpContext = context;
        IRequestHandler requestHandler = context.RequestServices
                                            .GetRequiredService<IRequestHandlerFactory>()
                                            .GetInstance(req.Type);

        var response = await requestHandler.GetResponseMessage(req);

        await context.WriteResponse(response);
    }
}