using AutoMapper;
using MicroCloud.Web.Models.WebApplications.Authentications;
using MicroCloud.Web.Models.WebApplications.Authentications.JwtBearer;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.Domain.Repositories;
using WebApplication.Domain.WebApplication.Entities;

namespace MicroCloud.Web.Services;

public class WebApplicationAuthenticationWebService : BaseService, IWebApplicationAuthenticationWebService
{
    private readonly IWebApplicationAuthenticationSchemeRepository _webApplicationAuthenticationSchemeRepository;
    private readonly IWebApplicationService _webApplicationWebService;
    private readonly IMapper _mapper;

    public WebApplicationAuthenticationWebService(IHttpContextAccessor contextAccessor,
            IWebApplicationAuthenticationSchemeRepository webApplicationAuthenticationSchemeRepository,
            IWebApplicationService webApplicationRepository,
            IWebApplicationService webApplicationWebService,
            IMapper mapper)
            : base(contextAccessor)
    {
        _webApplicationAuthenticationSchemeRepository = webApplicationAuthenticationSchemeRepository;
        _webApplicationWebService = webApplicationRepository;
        _mapper = mapper;
    }

    public async Task AddJwtBearerScheme(int appId, JwtBearerSchemeSaveModel scheme)
    {
        await _webApplicationAuthenticationSchemeRepository.Add(appId, new() {
            WebApplicationId = appId,
            Type = WebApplication.Domain.WebApplication.Shared.AuthenticationSchemeType.JwtBearer,
            Name = scheme.Name,
            DisplayName = scheme.DisplayName,
            Description = scheme.Description,
            Options = new JwtBearerAuthenticationOptions() {
                Authority = scheme.Options.Authority,
                Audience = scheme.Options.Audience
            }
        });
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
