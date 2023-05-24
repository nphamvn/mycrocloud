using System.Diagnostics;
using System.Net;
using MockServer.Api.Interfaces;
using MockServer.Api.Models.Docker;
using MockServer.Core.Repositories;
using Yarp.ReverseProxy.Forwarder;
using CoreRoute = MockServer.Core.WebApplications.Route;
namespace MockServer.Api.TinyFramework
{
    public class FunctionTriggerIntegrationHandler : RequestHandler
    {
        private readonly IDockerServices _dockerServices;
        private readonly IWebApplicationRouteRepository _webApplicationRouteRepository;
        private readonly IHttpForwarder _forwarder;
        private readonly CoreRoute _route;

        public FunctionTriggerIntegrationHandler(IDockerServices dockerServices,
            IWebApplicationRouteRepository webApplicationRouteRepository,
            IHttpForwarder forwarder,
            CoreRoute route)
        {
            _dockerServices = dockerServices;
            _webApplicationRouteRepository = webApplicationRouteRepository;
            _forwarder = forwarder;
            _route = route;
        }

        public override async Task Handle(HttpContext context)
        {
            var integration = await _webApplicationRouteRepository.GetFunctionTriggerIntegration(_route.Id);
            
            //1: Prepare source file by replacing user's class file to template source (username, requestId)
            //2: Send message to Go (or Python) GRPC service to build image, start container
            var runContainerResult = await _dockerServices.StartContainer(new RunContainerOptions
            {
            });

            //3: Send request to started container

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
            var destinationPrefix = "container ip";
            var error = await _forwarder.SendAsync(context, destinationPrefix, httpClient, requestConfig, transformer);
            // Check if the operation was successful
            if (error != ForwarderError.None)
            {
                var errorFeature = context.GetForwarderErrorFeature();
                var exception = errorFeature.Exception;
            }
        }
    }
}

