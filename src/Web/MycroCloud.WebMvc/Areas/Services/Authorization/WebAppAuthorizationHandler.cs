using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using MycroCloud.WebApp;
using MycroCloud.WebMvc.Extentions;
using WebMvcWebApp = MycroCloud.WebMvc.Areas.Services.Models.WebApps.WebAppModel;
namespace MycroCloud.WebMvc.Areas.Services.Authorization;

public class WebAppAuthorizationHandler(WebAppGrpcService.WebAppGrpcServiceClient webAppGrpcServiceClient, UserManager<IdentityUser> userManager)
    : AuthorizationHandler<OperationAuthorizationRequirement, WebMvcWebApp>
{
    private readonly WebAppGrpcService.WebAppGrpcServiceClient _webAppGrpcServiceClient = webAppGrpcServiceClient;
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, WebMvcWebApp app)
    {
        var user = context.User?.ToMycroCloudUser();

        if (requirement.Name == nameof(Operations.Index))
        {
            CheckIndexRequirement(context, requirement);
            return;
        }
        if (requirement.Name == nameof(Operations.Create))
        {
            CheckCreateRequirement(context, requirement);
            return;
        }
        if (requirement.Name == nameof(Operations.View))
        {
            await CheckViewRequirement(context, requirement, app);
            return;
        }
    }

    private async Task CheckViewRequirement(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, WebMvcWebApp app)
    {
        try
        {
            var res = await _webAppGrpcServiceClient.GetAsync(new GetWebAppRequest()
            {
                UserId = app.UserId,
                Name = app.WebAppName
            });
            context.Succeed(requirement);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            context.Fail();
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static void CheckCreateRequirement(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
    {
        context.Succeed(requirement);
    }

    private static void CheckIndexRequirement(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
    {
        context.Succeed(requirement);
    }
    public static class Operations
    {
        public static OperationAuthorizationRequirement Index =
            new() { Name = nameof(Index) };
        public static OperationAuthorizationRequirement Create =
            new() { Name = nameof(Create) };
        public static OperationAuthorizationRequirement View =
            new() { Name = nameof(View) };
        public static OperationAuthorizationRequirement Rename =
            new() { Name = nameof(Rename) };
        public static OperationAuthorizationRequirement Delete =
            new() { Name = nameof(Delete) };
    }
}