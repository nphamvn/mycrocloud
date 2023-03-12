using AutoMapper;
using MockServer.Web.Models.WebApplications;
using CoreWebApplication = MockServer.Core.WebApplications.WebApplication;
using CoreRoute = MockServer.Core.WebApplications.Route;
using CoreMockIntegrationResponseHeader = MockServer.Core.WebApplications.MockIntegrationResponseHeader;
using WebApplication = MockServer.Web.Models.WebApplications.WebApplication;
using Route = MockServer.Web.Models.WebApplications.Routes.Route;
using MockServer.Web.Models.WebApplications.Routes;
using MockServer.Web.Models.WebApplications.Routes.Integrations.MockIntegrations;
using CoreMockIntegration = MockServer.Core.WebApplications.MockIntegration;
using CoreAuthorizationPolicy = MockServer.Core.WebApplications.Security.Policy;
using MockServer.Web.Models.WebApplications.Routes.Authorizations;
using CoreAuthorization = MockServer.Core.WebApplications.Security.Authorization;
using CoreAuthenticationScheme = MockServer.Core.WebApplications.Security.AuthenticationScheme;
using MockServer.Web.Models.WebApplications.Authentications;
using CoreJwtBearerSchemeOptions = MockServer.Core.WebApplications.Security.JwtBearer.JwtBearerAuthenticationOptions;
using MockServer.Web.Models.WebApplications.Authentications.JwtBearer;
using MockServer.Web.Models.WebApplications.Authorizations;

namespace MockServer.Web.Profiles;

public class WebApplicationProfile : Profile
{
    public WebApplicationProfile()
    {
        CreateMap<CoreWebApplication, WebApplication>();
        CreateMap<CoreWebApplication, WebApplicationIndexItem>();
        CreateMap<CoreWebApplication, WebApplicationCreateModel>().ReverseMap();
        CreateMap<CoreWebApplication, WebApplicationViewModel>();
        
        CreateMap<CoreRoute, Route>();
        CreateMap<CoreRoute, RouteIndexItem>();
        CreateMap<CoreRoute,RouteSaveModel >().ReverseMap();
        CreateMap<CoreRoute, RouteViewModel>();

        CreateMap<CoreMockIntegrationResponseHeader, MockIntegrationResponseHeader>();
        CreateMap<CoreMockIntegrationResponseHeader, MockIntegrationResponseHeaderSaveModel>().ReverseMap();
        CreateMap<CoreMockIntegrationResponseHeader, MockIntegrationResponseHeaderViewModel>();

        CreateMap<CoreMockIntegration, MockIntegration>();
        CreateMap<CoreMockIntegration, MockIntegrationSaveModel>().ReverseMap();
        CreateMap<CoreMockIntegration, MockIntegrationViewModel>();

        CreateMap<CoreAuthorizationPolicy, Policy>();
        CreateMap<CoreAuthorizationPolicy, PolicyIndexItem>();
        CreateMap<CoreAuthorizationPolicy, PolicySaveModel>().ReverseMap();

        CreateMap<CoreAuthorization, Authorization>();
        CreateMap<CoreAuthorization, AuthorizationSaveModel>().ReverseMap();
        CreateMap<CoreAuthorization, AuthorizationViewModel>();

        CreateMap<CoreAuthenticationScheme, AuthenticationScheme>();
        CreateMap<CoreAuthenticationScheme, AuthenticationSchemeIndexItem>();
        CreateMap<CoreAuthenticationScheme, AuthenticationSchemeSaveModel>().ReverseMap();
        CreateMap<CoreAuthenticationScheme, AuthenticationSchemeViewModel>();

        CreateMap<CoreAuthorization, Authorization>();
        CreateMap<CoreAuthorization, AuthorizationSaveModel>().ReverseMap();
        CreateMap<CoreAuthorization, AuthorizationViewModel>();

        CreateMap<CoreJwtBearerSchemeOptions, JwtBearerSchemeOptions>();
        CreateMap<CoreJwtBearerSchemeOptions, JwtBearerSchemeOptionsSaveModel>().ReverseMap();
        CreateMap<CoreJwtBearerSchemeOptions, JwtBearerSchemeOptionsViewModel>();

        
    }
}
