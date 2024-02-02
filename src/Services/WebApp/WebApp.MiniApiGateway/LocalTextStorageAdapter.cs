using WebApp.Domain.Entities;
using WebApp.Infrastructure.Repositories.EfCore;

namespace WebApp.MiniApiGateway;

public class LocalTextStorageAdapter(App app, string name, AppDbContext appDbContext)
{
    public string Read()
    {
        var storage = appDbContext.TextStorages.SingleOrDefault(s => s.App == app && s.Name == name);
        return storage?.Content ?? "";
    }

    public void Write(string content)
    {
        var storage = appDbContext.TextStorages.SingleOrDefault(s => s.App == app && s.Name == name);
        if (storage is null)
        {
            return;
        }
        storage.Content = content;
        appDbContext.SaveChanges();
    }
}
