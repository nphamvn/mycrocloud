using AutoMapper;
using MockServer.Core.Repositories;
using MockServer.Core.WebApplications;
using MockServer.Web.Models.WebApplications;
using CoreWebApplication = MockServer.Core.WebApplications.WebApplication;
using WebApplication = MockServer.Web.Models.WebApplications.WebApplication;
namespace MockServer.Web.Services;

public class WebApplicationWebService : BaseWebService, IWebApplicationWebService
{
    private readonly IWebApplicationRepository _webApplicationRepository;
    private readonly IMapper _mapper;
    private readonly IWebApplicationRouteRepository _requestRepository;

    public WebApplicationWebService(IHttpContextAccessor contextAccessor,
        IWebApplicationRepository projectRepository,
        IWebApplicationRouteRepository requestRepository,
        IMapper mapper) : base(contextAccessor)
    {
        _requestRepository = requestRepository;
        _mapper = mapper;
        _webApplicationRepository = projectRepository;
    }

    public async Task Create(WebApplicationCreateModel app)
    {
        var existing = await _webApplicationRepository.Find(AuthUser.Id, app.Name);
        if (existing != null)
        {
            return;
        }
        var mapped = _mapper.Map<CoreWebApplication>(app);
        await _webApplicationRepository.Add(AuthUser.Id, mapped);
    }

    public async Task Delete(int appId)
    {
        var app = await _webApplicationRepository.Get(appId);
        if (app != null)
        {
            await _webApplicationRepository.Delete(app.Id);
        }
    }

    public async Task<WebApplication> Get(int appId)
    {
        var app = await _webApplicationRepository.Get(appId);
        return _mapper.Map<WebApplication>(app);
    }

    public async Task<WebApplicationIndexViewModel> GetIndexViewModel(WebApplicationSearchModel searchModel)
    {
        var apps = await _webApplicationRepository.Search(AuthUser.Id, searchModel.Query, searchModel.Sort);
        var vm = new WebApplicationIndexViewModel
        {
            Applications = _mapper.Map<IEnumerable<WebApplicationIndexItem>>(apps)
        };
        return vm;
    }

    public async Task Rename(int appId, string name)
    {
        var app = await _webApplicationRepository.Get(appId);
        if (app != null)
        {
            app.Name = name;
            await _webApplicationRepository.Update(app);
        }
    }

    public async Task SetAccessibility(int appId, WebApplicationAccessibility accessibility)
    {
        var app = await _webApplicationRepository.Get(appId);
        if (app != null)
        {
            app.Accessibility = accessibility;
            await _webApplicationRepository.Update(app);
        }
    }
}