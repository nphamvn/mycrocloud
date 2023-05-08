using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MockServer.Core.Interfaces;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Core.Settings;
using MockServer.Infrastructure.Repositories;
using MockServer.Web.Services;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var ConnectionString = configuration.GetConnectionString("SQLite");
            var settings = new GlobalSettings();
            settings.Sqlite = new SqlSettings { ConnectionString = configuration.GetConnectionString("SQLite") };
            services.AddSingleton<GlobalSettings>(s => settings);
            services.AddTransient<IApiKeyService, ApiKeyService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddHttpContextAccessor();
            services.AddDbContext<IdentityDbContext>(options => {
                options.UseNpgsql("Host=npham.me;Database=MockServer_Identity;Username=postgres;Password=YJVeuWV5Mj7eDr",
                b => b.MigrationsAssembly("MockServer.Web"));
                //options.UseSqlServer("Data Source=localhost;Initial Catalog=MockServer_Identity;User Id=sa;Password=48QcSDDojw8Rug;");
            });
            // services.AddIdentity<IdentityUser, IdentityRole>(options => {
            // })
            // .AddEntityFrameworkStores<IdentityDbContext>()
            // .AddDefaultTokenProviders();
            services.AddIdentityCore<IdentityUser>()
                    .AddEntityFrameworkStores<IdentityDbContext>();
            //services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, UserClaimsPrincipalFactory<IdentityUser, IdentityRole>>();
            //services.AddScoped<IUserConfirmation<IdentityUser>, DefaultUserConfirmation<IdentityUser>>();
            services.AddScoped<UserManager<IdentityUser>>();

            services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IWebApplicationRepository, WebApplicationRepository>();
            services.AddScoped<IWebApplicationRouteRepository, WebApplicationRouteRepository>();
            services.AddScoped<IWebApplicationAuthenticationSchemeRepository, WebApplicationAuthenticationSchemeRepository>();
            services.AddScoped<IWebApplicationAuthorizationPolicyRepository, WebApplicationAuthorizationPolicyRepository>();
            services.AddScoped<IDatabaseRepository, DatabaseRespository>();
            services.AddScoped<IWebApplicationWebService, WebApplicationWebService>();
            services.AddScoped<IWebApplicationRouteWebService, WebApplicationRouteWebService>();
            services.AddScoped<IWebApplicationAuthenticationWebService, WebApplicationAuthenticationWebService>();
            services.AddScoped<IWebApplicationAuthorizationWebService, WebApplicationAuthorizationWebService>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IDatabaseWebService, DatabaseWebService>();
            services.AddScoped<IFunctionRepository, FunctionRepository>();
            services.AddScoped<IFunctionWebService, FunctionWebService>();
        }
    }
}