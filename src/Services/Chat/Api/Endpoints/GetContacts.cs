using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

public static class GetContacts
{
    public static void MapGetContacts(this WebApplication app)
    {
        app.MapGet("/contacts", async (
                ClaimsPrincipal claimsPrincipal, 
                AppDbContext dbContext) =>
            {
                var userId = claimsPrincipal.GetUserId();
                var contacts = await dbContext.Contacts
                    .Include(c => c.Contact)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();
        
                return contacts.Select(c => new
                {
                    Id = c.ContactId,
                    c.Contact.FullName,
                    c.Contact.Picture,
                    c.CreatedAt,
                });
            })
            .WithName("GetContacts")
            .WithOpenApi()
            .RequireAuthorization();
    }
}