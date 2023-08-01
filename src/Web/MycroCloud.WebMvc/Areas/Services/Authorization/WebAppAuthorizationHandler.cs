﻿using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using MycroCloud.WebMvc.Extentions;
using WebApp.Api.Grpc;
using static WebApp.Api.Grpc.WebApp;

namespace MycroCloud.WebMvc.Areas.Services.Authorization;

public class WebAppAuthorizationHandler(WebAppClient webAppClient, UserManager<IdentityUser> userManager) 
    : AuthorizationHandler<OperationAuthorizationRequirement, WebAppModel>
{
    private readonly WebAppClient _webAppClient = webAppClient;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, WebAppModel app)
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

    private async Task CheckViewRequirement(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, WebAppModel app)
    {
        try
        {
            var res = await _webAppClient.GetAsync(new GetWebAppRequest()
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