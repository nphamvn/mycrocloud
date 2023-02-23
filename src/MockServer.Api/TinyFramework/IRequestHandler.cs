namespace MockServer.Api.TinyFramework;

public interface IRequestHandler
{
    Task<ResponseMessage> GetResponseMessage(Request request);
}

public interface IRequestHandler<T> : IRequestHandler where T : Request
{

}