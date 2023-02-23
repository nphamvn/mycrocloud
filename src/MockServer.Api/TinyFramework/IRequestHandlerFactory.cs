using MockServer.Core.Enums;

namespace MockServer.Api.TinyFramework;

public interface IRequestHandlerFactory
{
    IRequestHandler GetInstance(RequestType type);
}