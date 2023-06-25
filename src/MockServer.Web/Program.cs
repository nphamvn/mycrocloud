using System.Data;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MockServer.Core.WebApplications;
using MockServer.Web.Extentions;
using MockServer.Web.Models.WebApplications.Routes;
using Serilog;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

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
builder.Services.AddControllersWithViews(
    options =>
    {
        var readerFactory = builder.Services.BuildServiceProvider().GetRequiredService<IHttpRequestStreamReaderFactory>();
        options.ModelBinderProviders.Insert(0, new RouteModelBinderProvider(options.InputFormatters, readerFactory));
    }
).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new RuleJsonConverter());
    //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
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
    ;
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ReadWebApplication", policy =>
    {
        policy.RequireClaim("scopes", "webapp:read");
    });
    options.AddPolicy("WriteWebApplication", policy =>
    {
        policy.RequireClaim("scopes", "webapp:write");
    });
    options.AddPolicy("DeleteWebApplication", policy =>
    {
        policy.RequireClaim("scopes", "webapp:delete");
    });
});
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
