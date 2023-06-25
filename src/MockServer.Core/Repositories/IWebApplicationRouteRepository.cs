using MockServer.Core.WebApplications;

namespace MockServer.Core.Repositories;

public interface IWebApplicationRouteRepository
{
    Task<int> Create(int appId, Route route);
    Task<Route> Find(int appId, string method, string path);
    Task<Route> GetById(int id);
    Task<IEnumerable<Route>> GetByApplicationId(int appId, string searchTerm, string sort);
    Task Update(int id, Route route);
    Task Delete(int id);
    Task<MockResponse> GetMockResponse(int id);
}