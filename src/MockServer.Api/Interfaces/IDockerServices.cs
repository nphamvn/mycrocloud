using MockServer.Api.Models.Docker;

namespace MockServer.Api.Interfaces
{
    public interface IDockerServices
    {
        Task<BuildImageResult> BuildImage(BuildImageOptions request);
        Task<RunContainerResult> StartContainer(RunContainerOptions request);
    }
}
