namespace MockServer.Api.TinyFramework;

public abstract class RequestHandler
{
    public abstract Task Handle(HttpContext request);
}