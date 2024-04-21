using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

public static class GetPeople
{
    public static void MapGetPeople(this WebApplication app)
    {
        app.MapGet("/people", async (
            ClaimsPrincipal user, 
            AppDbContext dbContext) =>
        {
            if (user.Identity is { IsAuthenticated: true })
            {
                var userId = user.GetUserId();
                
                var people = await dbContext.Users
                    .Where(u => u.Id != userId)
                    .ToListAsync();
                
                var contacts = await dbContext.Contacts
                    .Where(c => c.UserId == userId)
                    .ToListAsync();
                
                return Results.Ok(people.Select(p => new
                {
                    p.Id,
                    p.FullName,
                    p.Picture,
                    IsContact = contacts.Exists(c => c.Contact == p)
                }));
            }
            else
            {
                var people = await dbContext.Users.ToListAsync();
                return Results.Ok(people.Select(p => new
                {
                    p.Id,
                    p.FullName,
                    p.Picture
                }));
            }
        }).WithName("GetPeople");
    }
}