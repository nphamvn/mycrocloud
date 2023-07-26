using AutoMapper;
using MicroCloud.Web.Models.WebApplications;
using MicroCloud.Web.Repositories;
using WebApplication.Domain.Repositories;
using CoreWebApplication = WebApplication.Domain.WebApplication.Entities.WebApplication;

namespace MicroCloud.Web.Services;

public class WebApplicationService : BaseService, IWebApplicationService
{
    private readonly IWebApplicationWebRepository _webApplicationRepository;
    private readonly IMapper _mapper;
    private readonly IWebApplicationRouteRepository _requestRepository;

    public WebApplicationService(IHttpContextAccessor contextAccessor,
        IWebApplicationWebRepository projectRepository,
        IWebApplicationRouteRepository requestRepository,
        IMapper mapper) : base(contextAccessor)
    {
        _requestRepository = requestRepository;
        _mapper = mapper;
        _webApplicationRepository = projectRepository;
    }

    public async Task Create(WebAppCreateViewModel app)
    {
        var existing = await _webApplicationRepository.FindByUserId(AuthUser.Id, app.Name);
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

    public async Task<WebAppModel> Get(int appId)
    {
        var app = await _webApplicationRepository.Get(appId);
        var mapped = _mapper.Map<WebAppModel>(app);
        return mapped;
    }

    public async Task<WebAppIndexViewModel> GetIndexViewModel(WebAppSearchModel searchModel)
    {
        var apps = await _webApplicationRepository.Search(AuthUser.Id, searchModel.Query, searchModel.Sort);
        var vm = new WebAppIndexViewModel
        {
            Applications = _mapper.Map<IEnumerable<WebAppIndexItem>>(apps)
        };
        return vm;
    }

    public async Task<WebApplicationViewModel> GetOverviewViewModel(int webApplicationId)
    {
        var app = await _webApplicationRepository.Get(webApplicationId);
        return _mapper.Map<WebApplicationViewModel>(app);
    }

    public async Task Rename(int appId, string name)
    {
        var app = await _webApplicationRepository.Get(appId);
        if (app != null)
        {
            app.Name = name;
            await _webApplicationRepository.Update(appId, app);
        }
    }
}