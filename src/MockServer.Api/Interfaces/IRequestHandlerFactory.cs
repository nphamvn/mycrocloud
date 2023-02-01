using MockServer.Core.Enums;

namespace MockServer.Api.Interfaces;

public interface IRequestHandlerFactory
{
    IRequestHandler GetInstance(RequestType type);
}