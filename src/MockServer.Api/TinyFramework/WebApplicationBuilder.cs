using AutoMapper;
using MockServer.Core.Models.Projects;

namespace MockServer.Api.TinyFramework;

public class WebApplicationBuilder
{
    public WebApplicationBuilder(IServiceProvider provider)
    {
        _provider = provider;
    }
    private WebApplication _builtApplication;
    private readonly IServiceProvider _provider;
    public WebApplication Build(Project app){
        var mapper = _provider.GetRequiredService<IMapper>();
        _builtApplication = mapper.Map<WebApplication>(app);
        _builtApplication.ServiceProvider = _provider;
        return _builtApplication;
    }
}
