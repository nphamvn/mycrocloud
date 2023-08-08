using MycroCloud.WebMvc.Areas.Identity.Services;
using MycroCloud.WebMvc.Areas.Identity;
using MycroCloud.WebMvc.Areas.Services;

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
        }
    }
}