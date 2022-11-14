using DockerServiceClient;
using Grpc.Net.Client;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models.Docker;

namespace MockServer.ReverseProxyServer.Services
{
    public class DockerServices : IDockerServices
    {
        public async Task<BuildImageResult> BuildImage(BuildImageOptions request)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:7042");
            var client = new DockerService.DockerServiceClient(channel);
            var reply = await client.BuildImageAsync(new BuildImageRequest
            {
                ImageName = "username-requestId"
            });

            return new BuildImageResult
            {
                Result = reply.Result
            };
        }

        public async Task<RunContainerResult> StartContainer(RunContainerOptions request)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:7042");
            var client = new DockerService.DockerServiceClient(channel);
            var reply = await client.RunContainerAsync(new RunContainerRequest
            {
                ImageName = "username-requestId"
            });

            return new RunContainerResult
            {
                Result = reply.Result
            };
        }
    }
}
