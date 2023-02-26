using AutoMapper;
using CoreWebApplication = MockServer.Core.WebApplications.WebApplication;
namespace MockServer.Api.TinyFramework;

public class WebApplicationBuilder
{
    private WebApplication _builtApplication;
    private readonly IServiceProvider _provider;
    
    public WebApplicationBuilder(IServiceProvider provider)
    {
        _provider = provider;
    }
    public WebApplication Build(CoreWebApplication app){
        var mapper = _provider.GetRequiredService<IMapper>();
        _builtApplication = mapper.Map<WebApplication>(app);
        _builtApplication.ServiceProvider = _provider;
        return _builtApplication;
    }
}
