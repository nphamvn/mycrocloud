using MycroCloud.WebMvc.Areas.Identity.Services;
using MycroCloud.WebMvc.Identity;
using MycroCloud.WebMvc.Areas.Services;
using Microsoft.AspNetCore.Authorization;
using MycroCloud.WebMvc.Authorization;

namespace MycroCloud.WebMvc.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRouting(options => options.LowercaseUrls = true);
            // Add services to the container.
            services.AddControllersWithViews();
            services.AddRazorPages();
            //builder.Services.AddServerSideBlazor();
            services.AddHttpContextAccessor();
            services.AddTransient<IEmailSender, SendGridEmailSender>();
            
            services.ConfigureIdentityArea(configuration);
            services.ConfigureServicesArea(configuration);

            services.AddAuthorizationBuilder()
                .AddPolicy("read:messages", policy => policy.Requirements.Add(new 
                            HasScopeRequirement("read:messages", "issuer domain")));
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
        }
    }
}