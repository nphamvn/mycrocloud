using WebApp.Domain.Entities;
using WebApp.Infrastructure;
using Object = WebApp.Domain.Entities.Object;

namespace WebApp.MiniApiGateway.PlugIns;

internal class ObjectStorageAdapter(int appId, AppDbContext dbContext)
{
    public byte[] Read(string key)
    {
        var storage = dbContext.Objects.Single(o => o.App.Id == appId && o.Key == key);
        return storage.Content;
    }

    public void Write(string key, byte[] content)
    {
        var obj = dbContext.Objects.SingleOrDefault(o => o.App.Id == appId && o.Key == key);
        if (obj is null)
        {
            dbContext.Objects.Add(new Object
            {
                AppId = appId,
                Key = key,
                Content = content
            });
        }
        else
        {
            obj.Content = content;
        }

        dbContext.SaveChanges();
    }
}