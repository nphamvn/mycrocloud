namespace MockServer.Api.TinyFramework;

public interface IWebApplication
{
    Task Handle(Request request);
}
