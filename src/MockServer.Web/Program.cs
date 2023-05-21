using Microsoft.AspNetCore.Identity;
using MockServer.Web.Extentions;
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
    .AddCookie(IdentityConstants.ApplicationScheme, options => {

    })
    .AddCookie(IdentityConstants.ExternalScheme)
    .AddCookie(IdentityConstants.TwoFactorUserIdScheme)
    .AddCookie(IdentityConstants.TwoFactorRememberMeScheme)
    .AddOpenIdConnect("Auth0", "Auth0", options => {
        options.Authority = $"https://{configuration["Auth0:Domain"]}";
        options.ClientId = configuration["Auth0:ClientId"];
        options.ClientSecret = configuration["Auth0:ClientSecret"];
        options.CallbackPath = new PathString("/auth0-callback");
        options.SignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddOpenIdConnect("Google", "Google", options => {
        options.Authority = "https://accounts.google.com";
        options.ClientId = configuration["Google:ClientId"];
        options.ClientSecret = configuration["Google:ClientSecret"];
        options.CallbackPath = new PathString("/signin-google");
        options.SignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddOpenIdConnect("Microsoft", "Microsoft", options => {
        options.Authority = $"https://login.microsoftonline.com/{configuration["Microsoft:TenantId"]}/v2.0";
        options.ClientId = configuration["Microsoft:ClientId"];
        options.ClientSecret = configuration["Microsoft:ClientSecret"];
        options.CallbackPath = new PathString("/signin-microsoft");
        options.SignInScheme = IdentityConstants.ExternalScheme;
        options.Scope.Add("email");
    })
    ;
var app = builder.Build();

app.Use(async (context, next) =>
{
    // Log the incoming request
    var request = context.Request;
    var message = $"{request.Method} {request.Scheme}://{request.Host}{request.Path}";
    app.Logger.LogInformation(message);
    // Call the next middleware in the pipeline
    await next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
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
