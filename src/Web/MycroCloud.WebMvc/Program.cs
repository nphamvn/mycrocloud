using System.Security.Claims;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using MycroCloud.WebMvc.Areas.Services.Authorization;
using MycroCloud.WebMvc.Authentications;
using MycroCloud.WebMvc.Extentions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
//builder.Services.AddServerSideBlazor();
builder.Services
    .AddAuthentication(options =>
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
    .AddScheme<DevAuthenticationOptions, DevAuthenticationHandler>("DevAuthentication", o =>
    {
        o.Header = "X-Dev-Key";
        o.KeyClaims = new() {
            { "8D92604B-F47F-4C55-B9A4-D35EF8AE819F", new List<Claim> {
                new ( ClaimTypes.NameIdentifier, "2c2602c5-49bd-4afc-84b7-ceaa47a45d9a"),
                new ( ClaimTypes.Name, "Nam Pham"),
                new ( ClaimTypes.Email, "nam@mycrocloud.com"),
            } },
        };
    })
    ;
//builder.Services.AddTransient<IAuthorizationHandler, WebAppAuthorizationHandler>();

builder.Services.AddAuthorization(options => {
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme, "DevAuthentication")
            .Build();
});



Action<GrpcClientFactoryOptions> options = (o) =>
{
    o.Address = new Uri("https://localhost:5100");
};
builder.Services.AddGrpcClient<WebApp.Api.Grpc.WebApp.WebAppClient>(options);
builder.Services.AddGrpcClient<WebApp.Api.Grpc.WebAppRoute.WebAppRouteClient>(options);
builder.Services.AddGrpcClient<WebApp.Api.Grpc.WebAppAuthentication.WebAppAuthenticationClient>(options);

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
    app.UseHsts();
}
else
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
//app.MapBlazorHub();
app.Run();
