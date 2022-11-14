using MockServer.ReverseProxyServer.Models.Docker;

namespace MockServer.ReverseProxyServer.Interfaces
{
    public interface IDockerServices
    {
        Task<BuildImageResult> BuildImage(BuildImageOptions request);
        Task<RunContainerResult> StartContainer(RunContainerOptions request);

    }
}
