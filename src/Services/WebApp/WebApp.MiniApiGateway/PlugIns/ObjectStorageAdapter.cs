using WebApp.Domain.Entities;
using WebApp.Infrastructure;

namespace WebApp.MiniApiGateway.PlugIns;

internal class ObjectStorageAdapter(int appId, AppDbContext dbContext)
{
    public byte[] Read(string key)
    {
        var storage = dbContext.BucketObjects.Single(o => o.App.Id == appId && o.Key == key);
        return storage.Content;
    }

    public void Write(string key, byte[] content)
    {
        var obj = dbContext.BucketObjects.SingleOrDefault(o => o.App.Id == appId && o.Key == key);
        if (obj is null)
        {
            dbContext.BucketObjects.Add(new BucketObject
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