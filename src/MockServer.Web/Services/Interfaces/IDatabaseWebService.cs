using MockServer.Web.Models.Database;

namespace MockServer.Web.Services.Interfaces;

public interface IDatabaseWebService
{
    Task<IndexViewModel> GetIndexViewModel();
    Task Create(SaveDatabaseViewModel db);
    Task Edit(int id, SaveDatabaseViewModel db);
    Task<SaveDatabaseViewModel> GetViewModel(int id);
    Task<SaveDatabaseViewModel> GetViewModel(string name);
}
