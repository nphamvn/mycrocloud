using System.Linq.Expressions;
using System.Security.Claims;
using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

public static class GetPrivateConversation
{
    public static void MapGetPrivateConversation(this WebApplication app)
    {
        app.MapGet("/conversations/private/{partnerId}", 
                async (
                    ClaimsPrincipal claimsPrincipal,
                    AppDbContext dbContext,
                    string partnerId) =>
            {
                var user = await dbContext.Users.SingleAsync(u => u.Id == claimsPrincipal.GetUserId());
                var partner = await dbContext.Users.SingleAsync(u => u.Id == partnerId);
                
                var conversation = await dbContext.Conversations
                    .Include(c => c.Users)
                    .SingleOrDefaultAsync(PrivateConversation(user, partner));

                if (conversation is not null)
                {
                    return Results.Ok(new
                    {
                        conversation.Id,
                    });
                }

                return Results.NotFound();
            })
            .WithName("GetPrivateConversationMessage")
            .RequireAuthorization()
            ;
    }

    private static Expression<Func<Conversation, bool>> PrivateConversation(User user, User partner)
    {
        return c => c.Users.Count == 2 
                    && c.Users.Any(u => u.UserId == user.Id) 
                    && c.Users.Any(u => u.UserId == partner.Id);
    }
}