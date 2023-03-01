using System.Diagnostics;
using System.Net;
using MockServer.Core.Repositories;
using Yarp.ReverseProxy.Forwarder;
using CoreRoute = MockServer.Core.WebApplications.Route;

namespace MockServer.Api.TinyFramework;

public class DirectForwardingIntegrationHandler : RequestHandler
{
    private readonly IWebApplicationRouteRepository _webApplicationRouteRepository;
    private readonly IHttpForwarder _forwarder;
    private readonly CoreRoute _route;

    public DirectForwardingIntegrationHandler(
        IWebApplicationRouteRepository webApplicationRouteRepository,
        IHttpForwarder forwarder,
        CoreRoute route)
    {
        _webApplicationRouteRepository = webApplicationRouteRepository;
        _forwarder = forwarder;
        _route = route;
    }

    public override async Task Handle(HttpContext context)
    {
        var integration = await _webApplicationRouteRepository.GetDirectForwardingIntegration(_route.Id);

        var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false,
            ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
            ConnectTimeout = TimeSpan.FromSeconds(15),
        });
        var transformer = new DirectForwardingIntegrationHttpTransformer(); // or HttpTransformer.Default;
        var requestConfig = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) };

        var error = await _forwarder.SendAsync(context, integration.ExternalServerHost,
                        httpClient, requestConfig, transformer);
        // Check if the operation was successful
        if (error != ForwarderError.None)
        {
            var errorFeature = context.GetForwarderErrorFeature();
            var exception = errorFeature.Exception;
        }
    }
}