using System.Diagnostics;
using System.Net;
using MockServer.Core.Repositories;
using Yarp.ReverseProxy.Forwarder;
using CoreRoute = MockServer.Core.WebApplications.Route;

namespace MockServer.Api.TinyFramework;

public class DirectForwardingIntegrationHandler : RequestHandler
{
    private readonly IWebApplicationRouteRepository _requestRepository;
    private readonly IHttpForwarder _forwarder;

    public DirectForwardingIntegrationHandler(IWebApplicationRouteRepository requestRepository,
        IHttpForwarder forwarder)
    {
        _requestRepository = requestRepository;
        _forwarder = forwarder;
    }

    public override async Task Handle(HttpContext context)
    {
        var route = context.Items[nameof(CoreRoute)] as CoreRoute;
        var integration = await _requestRepository.GetDirectForwardingIntegration(route.Id);
        var message = new HttpRequestMessage();
        message.Method = new HttpMethod(context.Request.Method);
        message.RequestUri = new Uri(string.Format("{1}/{2}", integration.ExternalServerHost, context.Request.Path.Value));

        using var client = new HttpClient();
        var response = await client.SendAsync(message);

        var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false,
            ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
            ConnectTimeout = TimeSpan.FromSeconds(15),
        });

        var requestConfig = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) };
        var transformer = new DirectForwardingHttpTransformer();

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