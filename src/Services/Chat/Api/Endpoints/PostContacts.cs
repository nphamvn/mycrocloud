using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

public class Request
{
    [Required]
    public required string Id { get; set; }
}

public static class PostContacts
{
    public static void MapPostContacts(this WebApplication app)
    {
        app.MapPost("/contacts", async (
                ClaimsPrincipal claimsPrincipal, 
                AppDbContext dbContext, 
                Request request) =>
            {
                var userId = claimsPrincipal.GetUserId();
                var user = await dbContext.Users.SingleAsync(u => u.Id == userId);
                var other = await dbContext.Users.SingleAsync(u => u.Id == request.Id);
                
                await dbContext.Contacts.AddAsync(new UserContact
                {
                    User = user,
                    Contact = other,
                    CreatedAt = DateTime.UtcNow,
                });
                
                await dbContext.SaveChangesAsync();

                return Results.Created();
            })
            .WithName("CreateContact")
            .WithOpenApi()
            .RequireAuthorization();
    }
}