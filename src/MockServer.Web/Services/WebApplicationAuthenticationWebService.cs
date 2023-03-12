using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.Repositories;
using MockServer.Core.WebApplications.Security.JwtBearer;
using MockServer.Web.Models.WebApplications.Authentications;
using MockServer.Web.Models.WebApplications.Authentications.JwtBearer;

namespace MockServer.Web.Services;

public class WebApplicationAuthenticationWebService : BaseWebService, IWebApplicationAuthenticationWebService
{
    private readonly IWebApplicationAuthenticationSchemeRepository _webApplicationAuthenticationSchemeRepository;
    private readonly IWebApplicationWebService _webApplicationWebService;
    private readonly IMapper _mapper;

    public WebApplicationAuthenticationWebService(IHttpContextAccessor contextAccessor,
            IWebApplicationAuthenticationSchemeRepository webApplicationAuthenticationSchemeRepository,
            IWebApplicationWebService webApplicationRepository,
            IWebApplicationWebService webApplicationWebService,
            IMapper mapper)
            : base(contextAccessor)
    {
        _webApplicationAuthenticationSchemeRepository = webApplicationAuthenticationSchemeRepository;
        _webApplicationWebService = webApplicationRepository;
        _mapper = mapper;
    }

    public Task AddJwtBearerScheme(int appId, JwtBearerSchemeSaveModel scheme)
    {
        throw new NotImplementedException();
    }

    public Task EditJwtBearerScheme(int schemeId, JwtBearerSchemeSaveModel scheme)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthenticationIndexModel> GetIndexViewModel(int appId)
    {
        var schemes = await _webApplicationAuthenticationSchemeRepository.GetAll(appId);
        var vm = new AuthenticationIndexModel();
        vm.AuthenticationSchemes = _mapper.Map<IEnumerable<AuthenticationSchemeIndexItem>>(schemes);
        vm.SelectedAuthenticationSchemeIds = vm.AuthenticationSchemes.Where(s => s.Order > 0).Select(s => s.Id).ToList();
        vm.WebApplication = await _webApplicationWebService.Get(appId);
        return vm;
    }

    public async Task<JwtBearerSchemeSaveModel> GetCreateJwtBearerSchemeModel(int appId)
    {
        var vm = new JwtBearerSchemeSaveModel();
        vm.WebApplication = await _webApplicationWebService.Get(appId);
        return vm;
    }

    public async Task SaveSettings(int appId, AuthenticationSettingsModel model)
    {
        await _webApplicationAuthenticationSchemeRepository.Activate(appId, model.SelectedAuthenticationSchemeIds.ToList());
    }

    public async Task<JwtBearerSchemeSaveModel> GetEditJwtBearerSchemeModel(int appId, int schemeId)
    {
        var scheme = await _webApplicationAuthenticationSchemeRepository.Get<JwtBearerAuthenticationOptions>(schemeId);
        var mapped = _mapper.Map<JwtBearerSchemeSaveModel>(scheme);
        mapped.WebApplication = await _webApplicationWebService.Get(appId);
        return mapped;
    }

    public async Task<AuthenticationSettingsModel> GetAuthenticationSettingsModel(int appId)
    {
        var model = new AuthenticationSettingsModel();
        var schemes = await _webApplicationAuthenticationSchemeRepository.GetAll(appId);
        model.SelectedAuthenticationSchemeIds = schemes.Where(s => s.Order > 0)
                .Select(s => s.Id);
        model.AuthenticationSchemeSelectListItem = schemes.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString(),
            Selected = model.SelectedAuthenticationSchemeIds.Contains(s.Id)
        });
        model.WebApplication = await _webApplicationWebService.Get(appId);
        return model;
    }
}
