using MicroCloud.Web.Areas.Identity.Services;
using MicroCloud.Web.Repositories;
using MicroCloud.Web.Services;
using MicroCloud.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication.Domain.Repositories;
using WebApplication.Domain.Settings;
using WebApplication.Infrastructure.Repositories.PostgreSql;

namespace MicroCloud.Web.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new GlobalSettings();
            services.AddSingleton<GlobalSettings>(s => settings);
            services.Configure<PostgresSettings>(configuration.GetSection("Database:Application"));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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
            services.AddScoped<UserManager<IdentityUser>>();
            services.AddScoped<SignInManager<IdentityUser>>();
            services.AddTransient<IEmailSender, SendGridEmailSender>(); 
            
            services.AddScoped<IWebApplicationRepository, WebApplicationRepository>();
            services.AddScoped<IWebApplicationWebRepository, WebApplicationWebRepository>();
            services.AddScoped<IWebApplicationRouteRepository, WebApplicationRouteRepository>();
            services.AddScoped<IWebApplicationAuthenticationSchemeRepository, WebApplicationAuthenticationSchemeRepository>();
            services.AddScoped<IWebApplicationAuthorizationPolicyRepository, WebApplicationAuthorizationPolicyRepository>();
            services.AddScoped<IWebApplicationService, WebApplicationService>();
            services.AddScoped<IWebApplicationRouteService, WebApplicationRouteService>();
            services.AddScoped<IWebApplicationAuthenticationWebService, WebApplicationAuthenticationWebService>();
            services.AddScoped<IWebApplicationAuthorizationWebService, WebApplicationAuthorizationWebService>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IFunctionRepository, FunctionRepository>();
            services.AddScoped<IFunctionWebService, FunctionWebService>();
        }
    }
}