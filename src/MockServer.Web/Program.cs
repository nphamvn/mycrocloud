using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using MockServer.Web.Authentication;
using MockServer.Web.Extentions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.AddSerilog(logger);
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddServerSideBlazor();
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
// builder.Services
//         .AddAuthentication()
//         // .AddCookie("LocalCookieScheme", "Local Cookie Scheme", options => {
//         //     options.Cookie.Name = "LocalCookieScheme";
//         // })
//         .AddScheme<MyAuthenticationOptions, MyAuthenticationHandler>("MyAuthenticationScheme", options => {
            
//         })
//         ;
// builder.Services.AddAuthentication(options =>
//     {
//         options.DefaultScheme = "Cookies";
//         options.DefaultChallengeScheme = "oidc";
//     })
//     .AddCookie("Cookies")
//     .AddOpenIdConnect("oidc", options =>
//     {
//         options.Authority = configuration["IdentityServer:Authority"];
//         options.ClientId = "web";
//         options.ClientSecret = "secret";
//         options.ResponseType = "code";

//         options.Scope.Clear();
//         options.Scope.Add("openid");
//         options.Scope.Add("profile");
//         options.GetClaimsFromUserInfoEndpoint = true;
//         options.SaveTokens = true;
//         options.RequireHttpsMetadata = false;
//     });
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ExternalScheme;
    })
    .AddCookie(IdentityConstants.ApplicationScheme, options => {

    })
    .AddCookie(IdentityConstants.ExternalScheme, options => {

    })
    .AddOpenIdConnect("Auth0", options => {
        options.Authority = $"https://{configuration["Auth0:Domain"]}";
        options.ClientId = configuration["Auth0:ClientId"];
        options.ClientSecret = configuration["Auth0:ClientSecret"];
        options.CallbackPath = new PathString("/auth0-callback");
        options.SignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options => {
        options.ClientId = "1003248858371-s5bsd1mre1dvi635pflp0fm2tpcl8sj1.apps.googleusercontent.com";
        options.ClientSecret = "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";
        options.CallbackPath = new PathString("/signin-google");
        options.SignInScheme = IdentityConstants.ExternalScheme;
        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
    })
    // .AddOpenIdConnect("Google", options => {
    //     options.Authority = "https://accounts.google.com";
    //     options.ClientId = "1003248858371-s5bsd1mre1dvi635pflp0fm2tpcl8sj1.apps.googleusercontent.com";
    //     options.ClientSecret = "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";
    //     options.CallbackPath = new PathString("/signin-google");
    //     options.ResponseType = "code";
    //     options.GetClaimsFromUserInfoEndpoint = true;
    // })
    // .AddOpenIdConnect("OAuth20Server", options => {
    //     options.Authority = "https://localhost:7275";
    //     options.ClientId = "1";
    //     options.ClientSecret = "123456789";
    //     options.CallbackPath = new PathString("/signin-oidc");
    //     options.ResponseType = "code";
    //     options.GetClaimsFromUserInfoEndpoint = true;
    // })
    ;
var app = builder.Build();

app.Use(async (context, next) =>
{
    // Log the incoming request
    var request = context.Request;
    var message = $"Request: {request.Method} {request.Path}";
    Console.WriteLine(message);

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
//app.MapBlazorHub();
app.Run();
