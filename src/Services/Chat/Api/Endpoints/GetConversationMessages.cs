using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

public static class GetConversationMessages
{
    public static void MapGetConversationMessages(this WebApplication app)
    {
        app.MapGet("/conversations/{conversationId:int}/messages", async (
                int conversationId,
                ClaimsPrincipal user, 
                AppDbContext dbContext) =>
            {
                var userId = user.GetUserId();
    
                var conversation = await dbContext.Conversations
                    .Include(c => c.Users)
                    .SingleAsync(c => c.Id == conversationId && c.Users.Any(u => u.UserId == userId));
    
                var messages = await dbContext.Messages
                    .Where(m => m.Conversation == conversation)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(50)
                    .ToListAsync();

                return messages.Select(m => new
                {
                    m.Id,
                    m.SenderId,
                    m.ConversationId,
                    m.Text,
                    m.CreatedAt,
                    m.ClientId,
                    Mine = m.SenderId == userId
                });
            })
            .WithName("GetMessages")
            .WithOpenApi()
            .RequireAuthorization();
    }
}