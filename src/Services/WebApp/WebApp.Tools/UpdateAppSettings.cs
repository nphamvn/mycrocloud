using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Repositories.EfCore;

namespace WebApp.Tools;

public static class UpdateAppSettings
{
    public static void DoWork()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>();
        options.UseNpgsql(Configuration.ConnectionString);
        using var context = new AppDbContext(options.Options);
        // context.Apps.ExecuteUpdate(a => a.SetProperty(app => app.Settings, app => new AppSettings()
        // {
        //     CheckFunctionExecutionLimitMemory = true,
        //     FunctionExecutionLimitMemoryBytes = 10 * 1024 * 1024,
        //     CheckFunctionExecutionTimeout = true,
        //     FunctionExecutionTimeoutSeconds = 15
        // }));
        var apps = context.Apps.ToList();
        foreach (var app in apps)
        {
            app.Settings = new AppSettings()
            {
                CheckFunctionExecutionLimitMemory = true,
                FunctionExecutionLimitMemoryBytes = 10 * 1024 * 1024,
                CheckFunctionExecutionTimeout = true,
                FunctionExecutionTimeoutSeconds = 15
            };
        }

        context.SaveChanges();
    }
}