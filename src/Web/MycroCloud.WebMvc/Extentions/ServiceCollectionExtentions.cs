using MycroCloud.WebMvc.Areas.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
            services.AddScoped<UserManager<IdentityUser>>();
            services.AddScoped<SignInManager<IdentityUser>>();
            services.AddTransient<IEmailSender, SendGridEmailSender>();
        }
    }
}