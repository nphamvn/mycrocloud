using WebApp.Api.Models;

namespace WebApp.Api.Services;

public interface IAppService
{
    Task<int> Create(AppCreateRequest appCreateRequest);
}