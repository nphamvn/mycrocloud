using System.Text.Json;
using Jint;
using MockServer.Core.Repositories;
using MockServer.Core.Services;

namespace MockServer.Core.Databases;

public static class DatabaseAdapterUtilities
{
    public static IDatabaseAdapter CreateAdapter(
            Service service,
            IFactoryService factoryService,
            IDatabaseRepository databaseRepository,
            JsonSerializerOptions jsonSerializerOptions,
            int UserId, string databaseName,
            Engine engine,
            string[] codes)
    {
        var findTask = databaseRepository.Find(UserId, databaseName);
        findTask.Wait();
        var db = findTask.Result;
        if (db != null)
        {
            var getServiceTask = databaseRepository.GetDatabaseUsingService(db.Id);
            getServiceTask.Wait();
            var services = getServiceTask.Result;
            if (services.Any(s => s.Type == service.Type && s.Id == service.Id))
            {
                foreach (var code in codes)
                {
                    engine.Execute(code);
                }
                if (db.Adapter == nameof(JsonFileAdapter))
                {
                    var jsonFileAdapter = factoryService.Create<JsonFileAdapter>(db.JsonFilePath, jsonSerializerOptions);
                    return jsonFileAdapter;
                }
                else if (db.Adapter == nameof(NoSqlAdapter))
                {
                    var noSqlAdapter = factoryService.Create<NoSqlAdapter>(db.Id, databaseRepository, jsonSerializerOptions);
                    return noSqlAdapter;
                }
                else
                {
                    throw new DbException("Database provider not found");
                }
            }
            else
            {
                throw new DbException($"Application {service.Name} is not accessible to {service.Id}");
            }
        }
        else
        {
            throw new DbException("Database not found");
        }
    }
}
