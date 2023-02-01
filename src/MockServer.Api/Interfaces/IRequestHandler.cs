using MockServer.Api.Models;

namespace MockServer.Api.Interfaces;

public interface IRequestHandler
{
    Task<ResponseMessage> GetResponseMessage(Request request);
}

public interface IRequestHandler<T> : IRequestHandler where T : Request
{

}