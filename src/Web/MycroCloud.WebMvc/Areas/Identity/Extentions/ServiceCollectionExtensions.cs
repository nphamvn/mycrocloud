using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MycroCloud.WebMvc.Areas.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureIdentityArea(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ExternalScheme;
            })
            .AddCookie(IdentityConstants.ApplicationScheme, options =>
            {

            })
            .AddCookie(IdentityConstants.ExternalScheme, options =>
            {
                options.LoginPath = "/identity/account/login";
            })
            .AddCookie(IdentityConstants.TwoFactorUserIdScheme)
            .AddCookie(IdentityConstants.TwoFactorRememberMeScheme)
            .AddOpenIdConnect("Auth0", "Auth0", options =>
            {
                options.Authority = $"https://{configuration["Auth0:Domain"]}";
                options.ClientId = configuration["Auth0:ClientId"];
                options.ClientSecret = configuration["Auth0:ClientSecret"];
                options.CallbackPath = new PathString("/auth0-callback");
                options.SignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddOpenIdConnect("Google", "Google", options =>
            {
                options.Authority = "https://accounts.google.com";
                options.ClientId = configuration["Google:ClientId"];
                options.ClientSecret = configuration["Google:ClientSecret"];
                options.CallbackPath = new PathString("/signin-google");
                options.SignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddOpenIdConnect("Microsoft", "Microsoft", options =>
            {
                options.Authority = $"https://login.microsoftonline.com/{configuration["Microsoft:TenantId"]}/v2.0";
                options.ClientId = configuration["Microsoft:ClientId"];
                options.ClientSecret = configuration["Microsoft:ClientSecret"];
                options.CallbackPath = new PathString("/signin-microsoft");
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.Scope.Add("email");
            })
            ;
        services.AddDbContext<IdentityDbContext>(options =>
        {
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
        return services;
    }
}
