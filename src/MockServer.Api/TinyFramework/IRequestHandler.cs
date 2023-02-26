namespace MockServer.Api.TinyFramework;

public interface IRequestHandler
{
    Task Handle(Request request);
}

public interface IRequestHandler<T> : IRequestHandler where T : Request
{
    
}