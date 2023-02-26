using MockServer.Api.Interfaces;
using MockServer.Api.Models.Docker;

namespace MockServer.Api.TinyFramework
{
    public class FunctionTriggerIntegrationHandler : RequestHandler
    {
        private readonly IDockerServices _dockerServices;

        public FunctionTriggerIntegrationHandler(IDockerServices dockerServices)
        {
            _dockerServices = dockerServices;
        }

        public override async Task Handle(HttpContext context)
        {
            //1: Prepare source file by replacing user's class file to template source (username, requestId)
            //2: Send message to Go (or Python) GRPC service to build image, start container
            var runContainerResult = await _dockerServices.StartContainer(new RunContainerOptions
            {
            });

            //3: Send request to started container
            using var client = new HttpClient();
            var httpRequest = context.Request;
            var message = new HttpRequestMessage();
            var path = httpRequest.Path.Value.StartsWith("/") ? httpRequest.Path.Value.Remove(0, 1) : httpRequest.Path.Value;
            message.Method = new HttpMethod(httpRequest.Method);
            string host = "ip";
            int port = 1000;
            message.RequestUri = new Uri(string.Format("http://{0}:{1}/{2}", host, port, path));
            var response = await client.SendAsync(message);
        }
    }
}

