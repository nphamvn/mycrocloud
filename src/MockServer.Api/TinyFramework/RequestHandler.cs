using MockServer.Api.Interfaces;

namespace MockServer.Api.TinyFramework;

public class RequestHandler
{
    public async Task Handle(Request request)
    {
        IRequestHandler requestHandler = request.GetRequiredService<IRequestHandlerFactory>()
                                            .GetInstance(request.Type);

        var response = await requestHandler.GetResponseMessage(request);

        await request.HttpContext.WriteResponse(response);
    }
}