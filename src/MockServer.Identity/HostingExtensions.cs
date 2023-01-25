using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using MockServer.IdentityServer.Services;
using Serilog;

namespace MockServer.IdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        // uncomment if you want to add a UI
        builder.Services.AddRazorPages();
        builder.Services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;

            })
            .AddProfileService<ProfileService>()
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddTestUsers(TestUsers.Users);

        builder.Services.AddAuthentication()
                        .AddGitHub("GitHub", options =>
                        {
                            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                            options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
                            options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
                            options.CallbackPath = new PathString("/github-oauth");
                            options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                            options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                            options.UserInformationEndpoint = "https://api.github.com/user";
                            options.SaveTokens = true;
                            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                            options.ClaimActions.MapJsonKey("urn:github:login", "login");
                            options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
                            options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");
                            options.Events = new OAuthEvents
                            {
                                OnCreatingTicket = async context =>
                                {
                                    var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                                    var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                                    response.EnsureSuccessStatusCode();
                                    var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                                    context.RunClaimActions(json.RootElement);
                                }
                            };
                        })
                        .AddGoogle(options =>
                        {
                            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                            options.ClientId = "1003248858371-s5bsd1mre1dvi635pflp0fm2tpcl8sj1.apps.googleusercontent.com";
                            options.ClientSecret = "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";

                            options.ClaimActions.Clear();
                        })
                        ;

        builder.Services.AddTransient<IProfileService, ProfileService>();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // uncomment if you want to add a UI
        app.UseStaticFiles();
        app.UseRouting();

        app.UseIdentityServer();

        // uncomment if you want to add a UI
        app.UseAuthorization();
        app.MapRazorPages().RequireAuthorization();

        return app;
    }
}
