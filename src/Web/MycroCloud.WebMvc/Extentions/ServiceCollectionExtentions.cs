using Microsoft.AspNetCore.Identity;
using MycroCloud.WebMvc.Areas.Identity.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MycroCloud.WebMvc.Areas.Identity.Models;
using MycroCloud.WebMvc.Areas.Services.Services;

namespace MycroCloud.WebMvc.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddDbContext<IdentityDbContext>(options => {
                var provider = configuration.GetValue<string>("Database:Identity:Provider");
                var connectionString = configuration.GetValue<string>("Database:Identity:ConnectionString");
                if (provider == "PostgresSql")
                {
                    options.UseNpgsql(connectionString, b => b.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
                }
            });
            services.AddIdentityCore<IdentityUser>()
                .AddEntityFrameworkStores<IdentityDbContext>();
            //services.AddScoped<UserManager<MycroCloudUser>>();
            //services.AddScoped<SignInManager<MycroCloudUser>>();
            services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.AddScoped<IWebAppService, WebAppService>();
            services.AddScoped<IWebAppAuthenticationService, WebAppAuthenticationService>();
            services.AddScoped<IWebAppAuthorizationService, WebAppAuthorizationService>();
            services.AddScoped<IWebAppRouteService, WebAppRouteService>();
        }
    }
}