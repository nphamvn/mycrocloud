using MockServer.Core.WebApplications;

namespace MockServer.Api.TinyFramework;

public interface IRequestHandlerFactory
{
    IRequestHandler GetInstance(RouteIntegrationType type);
}