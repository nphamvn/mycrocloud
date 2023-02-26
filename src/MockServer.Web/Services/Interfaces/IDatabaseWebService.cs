using MockServer.Core.Services;
using MockServer.Web.Models.Database;

namespace MockServer.Web.Services.Interfaces;

public interface IDatabaseWebService
{
    Task<IndexViewModel> GetIndexViewModel();
    Task Create(SaveDatabaseViewModel db);
    Task Edit(int id, SaveDatabaseViewModel db);
    Task<SaveDatabaseViewModel> GetViewModel(int id);
    Task<byte[]> GetDataBinary(int id);
    Task ConfigureApplication(int id, Service service, bool allowed);
    Task Delete(int databaseId);
    Task SaveData(int id, string data);
    Task<string> GetData(int id);
}
