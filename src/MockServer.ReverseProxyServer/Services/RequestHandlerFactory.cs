using MockServer.Core.Enums;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Services;

public class RequestHandlerFactory : IRequestHandlerFactory
{
    private readonly IEnumerable<IRequestHandler> _requestHandlers;
    public RequestHandlerFactory(IEnumerable<IRequestHandler> requestHandlers)
    {
        _requestHandlers = requestHandlers;
    }
    public IRequestHandler GetInstance(AppRequest request)
    {
        switch (request)
        {
            case FixedRequest _:
                return this.GetService(typeof(FixedRequestHandler));
            case ForwardingRequest _:
                return this.GetService(typeof(ForwardingRequestHandler));
            case CallbackRequest _:
                return this.GetService(typeof(ForwardingRequestHandler));
            default:
                throw new InvalidOperationException();
        }
    }

    public IRequestHandler GetInstance(RequestType type)
    {
        switch (type)
        {
            case RequestType.Fixed:
                return this.GetService(typeof(FixedRequestHandler));
            case RequestType.Forwarding:
                return this.GetService(typeof(ForwardingRequestHandler));
            case RequestType.Callback:
                return this.GetService(typeof(ExpectionCallbackExecutor));
            default:
                throw new InvalidOperationException();
        }
    }

    private IRequestHandler GetService(Type type)
    {
        return this._requestHandlers
                .FirstOrDefault(x => x.GetType() == type);
    }
}