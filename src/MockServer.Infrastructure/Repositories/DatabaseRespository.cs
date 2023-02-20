using MockServer.Core.Databases;
using MockServer.Core.Repositories;

namespace MockServer.Infrastructure.Repositories;

public class DatabaseRespository : IDatabaseRespository
{
    public Task Add(int userId, Database db)
    {
        throw new NotImplementedException();
    }

    public Task<Database> Find(int userId, string name)
    {
        return Task.FromResult(new Database
        {
            Name = name
        });
    }

    public Task<Database> Find(string username, string name)
    {
        return Task.FromResult(new Database
        {
        Name = name
        });
    }

    public Task<Database> Get(int id)
    {
        throw new NotImplementedException();
    }
}
